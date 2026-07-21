using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using ShineWebMobileAPI.Printing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PrintingController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        public IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        [HttpGet]
        [Route("api/invoice/PDFGenerate")]
        public IHttpActionResult PDFGenerate(string Companycode,string DocID, string TransID = "", string ConfigID = "", string PrinterID = "",
            string TransName = "", string Copies = "1")
        {
            try
            {
                string pdfFilePath = AppDomain.CurrentDomain.BaseDirectory + "PDF\\";// System.Configuration.ConfigurationManager.AppSettings["SupportFilePath"] + "PDF\\";
                string FileLocationwithname = "";

                if (!string.IsNullOrEmpty(pdfFilePath))
                {
                    //DataTable dtTName = bl.BL_ExecuteSqlQuery("select TransName from tblTransName where Id = " + TransID);
                    PrintBase PB = new PrintBase { GKS_BL = bl };
                    if (Convert.ToInt32(DocID) > 0)
                    {
                        if (!string.IsNullOrEmpty(ConfigID.ToString()))
                        {
                            DataTable dtDecimal = bl.BL_ExecuteSqlQuery(Companycode,"select AppValue from tblAppConfig where AppName in ('DecimalValues')");
                            int strDigits = Convert.ToInt32(dtDecimal.Rows[0][0].ToString());
                            string CT = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            //SendEmail(int nTranType, int nTranId, string strMachineName, string strMailID, int ConfigID)
                            //FileLocationwithname = PB.SaveAsPDF(Convert.ToInt32(TransID), Convert.ToInt32(DocID), Dns.GetHostName(), "", Convert.ToInt32(ConfigID));
                            bl.strDigits = strDigits;
                            PB.CurrentCompanycode = Companycode;
                            PB.GroupPDFPB(Companycode,Convert.ToInt32(TransID), Convert.ToInt32(DocID), Convert.ToInt32(ConfigID), true, bl.BL_nValidation(Copies), CT);
                            FileLocationwithname = PB.GroupPDFoutputPath;
                        }
                    }
                }
                string pathwithFileName = FileLocationwithname;
                string exts = Path.GetExtension(pathwithFileName);
                string ctype = GetMimeType(exts);
                string fileName = Path.GetFileName(pathwithFileName);
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode,"PDFGenerate", "SaveFileinLocation", ex.Message);
            }
            return null;
        }

        [HttpGet]
        [Route("api/invoice/MailGenerate")]
        public IHttpActionResult MailGenerate(string Companycode, string DocID, string TransID = "", string ConfigID = "", string Copies = "1")
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {

                DataTable dtMailData = bl.BL_ExecuteParamSP("uspGetMailId", TransID, DocID);
                if (dtMailData.Rows.Count > 0)
                {
                    string PartyEmail = dtMailData.Rows[6][0].ToString();
                    if (!string.IsNullOrEmpty(PartyEmail))
                    {
                        PrintBase PB = new PrintBase { GKS_BL = bl };
                        PB.CurrentCompanycode = Companycode;
                        PB.SendEmail(Companycode, Convert.ToInt32(TransID), Convert.ToInt32(DocID), PartyEmail, Convert.ToInt32(ConfigID));
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "0",
                            Message = "Mail Send Successfully"
                        });
                    }
                    else
                    {
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "1",
                            Message = "Party Mail ID not found"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage()
                {
                    ID = 0.ToString(),
                    MsgID = "1",
                    Message = ex.Message
                });
                bl.BL_WriteErrorMsginLog(Companycode, "MailGenerate", "MailGenerate", ex.Message);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/invoice/AutomaticMailGenerate")]
        public IHttpActionResult AutomaticMailGenerate(string Companycode, string DocID, string TransID = "", string Copies = "1")
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                DataTable dtPrinterConfig = bl.BL_ExecuteParamSP("uspGetPrinterConfigByTransID", TransID);

                if (dtPrinterConfig.Rows.Count > 0)
                {
                    string configID = dtPrinterConfig.Rows[0][0].ToString();

                    DataTable dtMailData = bl.BL_ExecuteParamSP("uspGetMailId", TransID, DocID);
                    if (dtMailData.Rows.Count > 0)
                    {
                        string PartyEmail = dtMailData.Rows[6][0].ToString();
                        if (!string.IsNullOrEmpty(PartyEmail))
                        {
                            PrintBase PB = new PrintBase { GKS_BL = bl };
                            PB.CurrentCompanycode = Companycode;
                            PB.SendEmail(Companycode, Convert.ToInt32(TransID), Convert.ToInt32(DocID), PartyEmail, Convert.ToInt32(configID));

                            list.Add(new SaveMessage()
                            {
                                ID = 0.ToString(),
                                MsgID = "0",
                                Message = "Mail Send Successfully"
                            });
                        }
                        else
                        {
                            list.Add(new SaveMessage()
                            {
                                ID = 0.ToString(),
                                MsgID = "1",
                                Message = "Party Mail ID not found"
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage()
                {
                    ID = 0.ToString(),
                    MsgID = "1",
                    Message = ex.Message
                });
                bl.BL_WriteErrorMsginLog(Companycode, "MailGenerate", "MailGenerate", ex.Message);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/invoice/WhatsappGenerate")]
        public IHttpActionResult WhatsappGenerate(string Companycode, string DocID, string TransID = "", string ConfigID = "",
            string Copies = "1", string APIURL = "")
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {

                DataTable dtWhatsappData = bl.BL_ExecuteParamSP("uspWhatsappmessagecontent", TransID, DocID, APIURL);
                if (dtWhatsappData.Rows.Count > 0)
                {
                    string PartyMobile = dtWhatsappData.Rows[0][0].ToString();
                    if (!string.IsNullOrEmpty(PartyMobile))
                    {
                        PartyMobile = PartyMobile.Length == 10 ? "91" + PartyMobile : PartyMobile;
                        string WAMessage = dtWhatsappData.Rows[0][2].ToString();
                        string DocValue = dtWhatsappData.Rows[0][1].ToString();
                        var sb = new StringBuilder();
                        string encDocID = BuisnessLayer.clsEncryptDecrypt.Encrypt(DocValue);
                        string encTransID = BuisnessLayer.clsEncryptDecrypt.Encrypt(TransID);
                        string encConfigID = BuisnessLayer.clsEncryptDecrypt.Encrypt(ConfigID);
                        WAMessage += "Document Link : \n" + APIURL + "invoice/viewmydocument?DocID=" + HttpUtility.UrlEncode(encDocID) + "&TransID=" + HttpUtility.UrlEncode(encTransID) + "&ConfigID=" + HttpUtility.UrlEncode(encConfigID);
                        WAMessage += "\n\nThank You!🙏";
                        //sb.AppendFormat(WAMessage, APIURL, encDocID, encTransID, encConfigID);
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "0",
                            Message = WAMessage,
                            RowID = PartyMobile
                        });
                    }
                    else
                    {
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "1",
                            Message = "Party Mobile No not found"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage()
                {
                    ID = 0.ToString(),
                    MsgID = "1",
                    Message = ex.Message
                });
                bl.BL_WriteErrorMsginLog(Companycode,"WhatsappGenerate", "WhatsappGenerate", ex.Message);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/transactionprint/generateprint")]
        public IHttpActionResult TransprintPDFGenerate(string Companycode, int TransID = 0, int ConfigID = 0, string DocValue = "",
            string Copies = "1", string Branch = "0", string FromDate = null, string ToDate = null)
        {
            try
            {
                DataView dtView = new DataView(bl.BL_StringSplitCommaHyphen(Companycode, DocValue.Trim()));
                DataTable dtDocIDs = dtView.ToTable(true, "SerialNo");
                int nTransrange = 0;
                if (dtDocIDs.Rows.Count > 0)
                {
                    if (!Convert.ToString(dtDocIDs.Rows[0][0]).Contains("Range Should be"))
                    {
                        Stopwatch STPWT = new Stopwatch();
                        STPWT.Start();
                        string Outputfile = "";
                        string CT = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        DataTable dtDecimal = bl.BL_ExecuteSqlQuery(Companycode,"select AppValue from tblAppConfig where AppName in ('DecimalValues')");
                        int strDigits = Convert.ToInt32(dtDecimal.Rows[0][0].ToString());
                        for (int nCount = 0; nCount < dtDocIDs.Rows.Count; nCount++)
                        {
                            int Ident = 0;
                            nTransrange = bl.BL_nValidation(dtDocIDs.Rows[nCount][0]);
                            DataTable dtID = bl.BL_ExecuteParamSP(Companycode,"uspGetTransIdentforPrint", TransID, nTransrange,
                                Branch, FromDate, ToDate);
                            if (dtID.Rows.Count > 0)
                            {
                                Ident = bl.BL_nValidation(dtID.Rows[0][0]);
                            }
                            PrintBase Print = new PrintBase
                            {
                                GKS_BL = bl
                            };
                            bl.strDigits = strDigits;
                            Print.CurrentCompanycode = Companycode;
                            Print.GroupPDFPB(Companycode,TransID, Ident, ConfigID, (nCount + 1) == dtDocIDs.Rows.Count, bl.BL_nValidation(Copies), CT);
                            if ((nCount + 1) == dtDocIDs.Rows.Count)
                                Outputfile = Print.GroupPDFoutputPath;
                        }
                        STPWT.Start();
                        string pathwithFileName = Outputfile;
                        //byte[] bytes = System.IO.File.ReadAllBytes(pathwithFileName);
                        string exts = Path.GetExtension(pathwithFileName);
                        string fileName = Path.GetFileName(pathwithFileName);
                        return Ok(fileName);
                    }
                    else
                    {
                        //obj_mdi.ShowMessage(Convert.ToString(dtDocIDs.Rows[0][0]), GKSShineBL.ToolStripErrorMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode, "Invoice", "transactionprint/generateprint", ex.Message);
            }
            return Ok();
        }
        public string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/print/downloadprint")]
        public HttpResponseMessage downloadprintData(string Companycode,string FName)
        {
            try
            {
                DataTable dt = new DataTable();
                string FPath = AppDomain.CurrentDomain.BaseDirectory + "PDF\\" + FName;
                string fileName = FName;
                if (!File.Exists(FPath))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(FPath, FileMode.Open, FileAccess.Read);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };
                return result;
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode,"Printing", "print/downloadprint", ex.Message);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
