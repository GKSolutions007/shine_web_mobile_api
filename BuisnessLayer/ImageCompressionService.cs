using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.BuisnessLayer
{
    /// <summary>
    /// Compresses images uploaded from any client (mobile browser, desktop browser,
    /// native app, WebView). Handles EXIF auto-rotation, downscaling, and
    /// progressive JPEG quality reduction to control output size.
    /// </summary>
    public class ImageCompressionService
    {
        // Tune these to your needs
        private const int MaxDimension = 1920;          // max width/height in px
        private const long TargetMaxBytes = 300 * 300;  // ~800 KB target output size
        private const long InitialJpegQuality = 75L;
        private const long MinJpegQuality = 30L;
        private const long QualityStep = 10L;

        /// <summary>
        /// Reads an image from the given stream, normalizes orientation,
        /// resizes if needed, and returns compressed JPEG bytes.
        /// </summary>
        public byte[] CompressImage(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            // Buffer the incoming stream into memory. Upload streams from
            // HttpContext.Request.Files aren't always seekable, and GDI+
            // needs a stable, seekable stream to decode from.
            byte[] originalBytes;
            using (var buffer = new MemoryStream())
            {
                if (inputStream.CanSeek)
                    inputStream.Position = 0;

                inputStream.CopyTo(buffer);
                originalBytes = buffer.ToArray();
            }

            if (originalBytes.Length == 0)
                throw new ArgumentException("Uploaded file is empty.", nameof(inputStream));

            using (var inputMs = new MemoryStream(originalBytes))
            using (var originalImage = Image.FromStream(inputMs, useEmbeddedColorManagement: true, validateImageData: true))
            {
                // Mobile photos (especially iOS) embed EXIF orientation instead of
                // physically rotating pixels. Without this, sideways/upside-down
                // images are a very common bug.
                NormalizeOrientation(originalImage);

                var (targetWidth, targetHeight) = CalculateTargetSize(originalImage.Width, originalImage.Height, MaxDimension);

                using (var resized = ResizeImage(originalImage, targetWidth, targetHeight))
                {
                    return EncodeToJpegWithSizeTarget(resized, InitialJpegQuality);
                }
            }
        }

        /// <summary>
        /// Rotates/flips the image in-place based on its EXIF orientation tag,
        /// then strips the tag so it isn't applied twice by downstream viewers.
        /// </summary>
        private void NormalizeOrientation(Image image)
        {
            const int OrientationPropertyId = 0x0112;

            if (!image.PropertyIdList.Contains(OrientationPropertyId))
                return;

            var prop = image.GetPropertyItem(OrientationPropertyId);
            int orientation = prop.Value[0];

            RotateFlipType rotateFlip;
            switch (orientation)
            {
                case 2: rotateFlip = RotateFlipType.RotateNoneFlipX; break;
                case 3: rotateFlip = RotateFlipType.Rotate180FlipNone; break;
                case 4: rotateFlip = RotateFlipType.Rotate180FlipX; break;
                case 5: rotateFlip = RotateFlipType.Rotate90FlipX; break;
                case 6: rotateFlip = RotateFlipType.Rotate90FlipNone; break;
                case 7: rotateFlip = RotateFlipType.Rotate270FlipX; break;
                case 8: rotateFlip = RotateFlipType.Rotate270FlipNone; break;
                default: rotateFlip = RotateFlipType.RotateNoneFlipNone; break;
            }

            if (rotateFlip != RotateFlipType.RotateNoneFlipNone)
            {
                image.RotateFlip(rotateFlip);
            }

            image.RemovePropertyItem(OrientationPropertyId);
        }

        /// <summary>
        /// Scales dimensions down (never up) so the longest side does not exceed maxDimension.
        /// </summary>
        private (int width, int height) CalculateTargetSize(int originalWidth, int originalHeight, int maxDimension)
        {
            if (originalWidth <= maxDimension && originalHeight <= maxDimension)
                return (originalWidth, originalHeight);

            double ratio = (double)originalWidth / originalHeight;

            return originalWidth >= originalHeight
                ? (maxDimension, Math.Max(1, (int)Math.Round(maxDimension / ratio)))
                : (Math.Max(1, (int)Math.Round(maxDimension * ratio)), maxDimension);
        }

        private Bitmap ResizeImage(Image source, int width, int height)
        {
            var dest = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            dest.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (var g = Graphics.FromImage(dest))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var attributes = new ImageAttributes())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(source, new Rectangle(0, 0, width, height),
                        0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return dest;
        }

        /// <summary>
        /// Encodes to JPEG (flattening any transparency onto white, since JPEG has
        /// no alpha channel), reducing quality step by step until under the target
        /// size or the minimum quality floor is hit.
        /// </summary>
        private byte[] EncodeToJpegWithSizeTarget(Bitmap source, long startingQuality)
        {
            var jpegCodec = GetEncoder(ImageFormat.Jpeg);
            if (jpegCodec == null)
                throw new InvalidOperationException("JPEG encoder not available on this system.");

            using (var flattened = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb))
            {
                using (var g = Graphics.FromImage(flattened))
                {
                    g.Clear(Color.White); // flatten transparency
                    g.DrawImage(source, 0, 0, source.Width, source.Height);
                }

                byte[] result = null;

                for (long quality = startingQuality; quality >= MinJpegQuality; quality -= QualityStep)
                {
                    using (var encoderParams = new EncoderParameters(1))
                    {
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                        using (var ms = new MemoryStream())
                        {
                            flattened.Save(ms, jpegCodec, encoderParams);
                            result = ms.ToArray();
                        }
                    }

                    if (result.Length <= TargetMaxBytes)
                        break;
                }

                return result;
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}