using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MasterController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/customer/customerlist")]
        public IHttpActionResult Getcustomerlist(string CompanyCode,string UserID)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 12, UserID);
            DataTable dtCustomerImg = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 11, 0);
            var listcustomerimages = new List<object>();
            for (int k = 0; k < dtCustomerImg.Rows.Count; k++)
            {
                string imgdata = null;
                if (!string.IsNullOrEmpty(dtCustomerImg.Rows[k][5].ToString()))
                {
                    byte[] photoBytes = (byte[])dtCustomerImg.Rows[k][5];
                    imgdata = Convert.ToBase64String(photoBytes);
                }
                listcustomerimages.Add(new 
                {
                    FileName = dtCustomerImg.Rows[k][3].ToString(),
                    FileSize = dtCustomerImg.Rows[k][4].ToString(),
                    CompressImageData = imgdata,
                });
            }
            string dtjson = JsonConvert.SerializeObject(DDT);
            object Customerdata = new
            {
                CustomerData = dtjson,
                //Imagedata = listcustomerimages
            };
            return Ok(Customerdata);
        }
        [HttpGet]
        [Route("api/customer/customerdata")]
        public IHttpActionResult Getcustomerdata(string CompanyCode,int ID)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 5, ID);
            DataTable ddtImages = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 10, ID);            
            DataTable DDTBTSM = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 7, ID);

            var listproductimages = new List<object>();
            for (int k = 0; k < ddtImages.Rows.Count; k++)
            {
                string imgdata = null;
                if (!string.IsNullOrEmpty(ddtImages.Rows[k][5].ToString()))
                {
                    byte[] photoBytes = (byte[])ddtImages.Rows[k][5];
                    imgdata = Convert.ToBase64String(photoBytes);
                }
                listproductimages.Add(new
                {
                    FileName = ddtImages.Rows[k][3].ToString(),
                    FileSize = ddtImages.Rows[k][4].ToString(),
                    CompressImageData = imgdata,
                });
            }
            string dt1 = JsonConvert.SerializeObject(DDT);
            string dt2 = JsonConvert.SerializeObject(DDTBTSM);
            var objMain = new List<object>();
            objMain.Add(new
            {
                Table1 = dt1,
                Beatsalesman = dt2,
                CompressImageData = listproductimages,
            });
            return Ok(objMain);
        }
        [HttpPost]
        [Route("api/customermaster/save_old")]
        public IHttpActionResult SaveBankAccont_old(CustomerVendorModel lstMaster)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (lstMaster != null)
                {
                    string ID = !string.IsNullOrEmpty(lstMaster.ID) ? lstMaster.ID : "0";
                    DataTable DDT = bl.BL_ExecuteParamSP(lstMaster.CompanyCode, "uspSaveCustomerfromMobile", lstMaster.Mode, ID, lstMaster.Code, lstMaster.Name,
                        lstMaster.Mob1, lstMaster.GSTIN.ToUpper(), lstMaster.Pincode, lstMaster.Billadd1, lstMaster.Latitude, lstMaster.Longtitude,
                         lstMaster.UserID);
                    if (DDT.Columns.Count == 1)
                    {
                        int IdentID = Convert.ToInt32(DDT.Rows[0][0].ToString());
                       
                        //Success message
                        list.Add(new SaveMessage()
                        {
                            ID = IdentID.ToString(),
                            MsgID = "0",
                            Message = "Saved Successfully"
                        });
                    }
                    else
                    {
                        //Error message
                        list.Add(new SaveMessage()
                        {
                            ID = "0",
                            MsgID = "1",
                            Message = DDT.Rows[0][0].ToString()
                        });
                    }
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage()
                {
                    ID = "1",
                    MsgID = "1",
                    Message = ex.Message
                });
            }
            return Ok(list);
        }
        private readonly ImageCompressionService _compressionService = new ImageCompressionService();
        [HttpPost]
        [Route("api/customermaster/save")]
        public async Task<IHttpActionResult> SaveCustomer()
        {
            List<SaveMessage> list = new List<SaveMessage>();
            string Companycode = "";
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    list.Add(new SaveMessage() { ID = "1", MsgID = "1", Message = "Expected multipart/form-data request." });
                    return Ok(list);
                }
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CustomerVendorModel lstMaster = null;
                List<string> savedImageNames = new List<string>();

                // Folder to store uploaded product images — adjust to your structure
                //string uploadFolder = HostingEnvironment.MapPath("~/Uploads/ProductImages/");
                //if (!Directory.Exists(uploadFolder))
                //    Directory.CreateDirectory(uploadFolder);
                Companycode = "";
                List<byte[]> compressimagedata = new List<byte[]>();
                var compimgdata = new List<object>();
                foreach (var content in provider.Contents)
                {
                    var fieldName = content.Headers.ContentDisposition.Name?.Trim('"');
                    var fileName = content.Headers.ContentDisposition.FileName?.Trim('"');
                    //content.Headers
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // Read the multipart part into a stream
                        byte[] rawBytes = await content.ReadAsByteArrayAsync();
                        using (var fileStream = new MemoryStream(rawBytes))
                        {
                            // Compress
                            //byte[] compressedBytes = bl.CompressImage(fileStream);
                            byte[] compressedBytes = _compressionService.CompressImage(fileStream);

                            if (compressedBytes != null && compressedBytes.Length > 0)
                            {
                                compimgdata.Add(new
                                {
                                    FileName = fileName,
                                    FileSize = (compressedBytes.Length / 1024) + " kb",
                                    compressimagedata = compressedBytes
                                });
                                compressimagedata.Add(compressedBytes);
                                // CompressImage always encodes to JPEG, so force the extension to match
                                //string safeName = Guid.NewGuid().ToString("N") + ".jpg";
                                //string fullPath = Path.Combine(uploadFolder, safeName);
                                //File.WriteAllBytes(fullPath, compressedBytes);
                                //savedImageNames.Add(safeName);
                            }
                        }
                    }
                    else if (fieldName == "data")
                    {
                        // This part is the JSON payload
                        string json = await content.ReadAsStringAsync();
                        lstMaster = JsonConvert.DeserializeObject<CustomerVendorModel>(json);
                        Companycode = lstMaster.CompanyCode;
                    }
                }

                if (lstMaster != null)
                {
                    string ID = !string.IsNullOrEmpty(lstMaster.ID) ? lstMaster.ID : "0";
                    DataTable DDT = bl.BL_ExecuteParamSP(lstMaster.CompanyCode, "uspSaveCustomerfromMobile", lstMaster.Mode, ID, lstMaster.Code, lstMaster.Name,
                        lstMaster.Mob1, lstMaster.GSTIN.ToUpper(), lstMaster.Pincode, lstMaster.Billadd1, lstMaster.Latitude, lstMaster.Longtitude,
                         lstMaster.UserID);

                    if (DDT.Columns.Count == 1)
                    {
                        int IdentID = Convert.ToInt32(DDT.Rows[0][0].ToString());
                        if (lstMaster.RemoveCustomer != null)
                        {
                            if (!string.IsNullOrEmpty(lstMaster.RemoveCustomer))
                                bl.BL_ExecuteParamSP(lstMaster.CompanyCode, "uspSaveImagedata", 2, 2, "Customer",
                                    IdentID, null, lstMaster.RemoveCustomer, null);
                        }
                        // Save image file names against the product — adjust SP/table to your schema
                        foreach (object imgdata in compimgdata)
                        {
                            var type = imgdata.GetType();
                            string fileName = type.GetProperty("FileName")?.GetValue(imgdata)?.ToString();
                            string fileSize = type.GetProperty("FileSize")?.GetValue(imgdata)?.ToString();
                            byte[] imageData = (byte[])type.GetProperty("compressimagedata")?.GetValue(imgdata);

                            //bl.BL_ExecuteParamSP("uspAddProductImage", IdentID, imgName);
                            bl.BL_ExecuteParamSP(lstMaster.CompanyCode, "uspSaveImagedata", 1, 2, "Customer", IdentID, imageData, fileName, fileSize);
                        }

                        list.Add(new SaveMessage()
                        {
                            ID = IdentID.ToString(),
                            MsgID = "0",
                            Message = "Saved Successfully"
                        });
                    }
                    else
                    {
                        list.Add(new SaveMessage()
                        {
                            ID = "0",
                            MsgID = "1",
                            Message = DDT.Rows[0][0].ToString()
                        });
                    }
                }
                else
                {
                    list.Add(new SaveMessage() { ID = "1", MsgID = "1", Message = "Invalid product data received." });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode, "Customer", "Save", ex.Message);
                list.Add(new SaveMessage()
                {
                    ID = "1",
                    MsgID = "1",
                    Message = ex.Message
                });

            }
            return Ok(list);
        }
    }
}
