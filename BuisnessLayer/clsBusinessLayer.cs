using ShineWebMobileAPI.DALHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ShineWebMobileAPI.BuisnessLayer
{
    public class clsBusinessLayer
    {
        clsDAL ObjDL = new clsDAL();
        public DataTable BL_ExecuteParamSP(string CompanyCode, string strProcedure, params object[] objParams)
        {
            return ObjDL.dl_ExecuteParamSP(CompanyCode,strProcedure, objParams);
        }
        public DataTable BL_ExecuteSqlQuery(string CompanyCode, string strquery)
        {
            return ObjDL.dl_ExecuteSqlQuery(CompanyCode, strquery);
        }
        public DataSet BL_ExecuteParamSPDataset(string CompanyCode, string strProcedure, params object[] objParams)
        {
            return ObjDL.dl_ExecuteParamSPDataset(CompanyCode,strProcedure, objParams);
        }
        public string BL_AppConfigValuebyID(string CompanyCode, int AppID)
        {
            string AppValue = "";
            DataTable dtApps = ObjDL.dl_ExecuteSqlQuery(CompanyCode, "SELECT * FROM tblAppConfig WHERE AppId = " + AppID);
            if (dtApps.Rows.Count > 0)
            {
                AppValue = dtApps.Rows[0][2].ToString();
            }
            return AppValue;
        }
        public void bl_Transaction(string CompanyCode, int action)
        {
            ObjDL.dl_Transaction(CompanyCode, action);
        }
        public DataTable bl_ManageTrans(string CompanyCode, string strStoredProc, params object[] obj)
        {
            return ObjDL.dl_ManageTrans(CompanyCode, strStoredProc, obj);
        }
        public string CheckString(string str, string DefaultValue = "0")
        {
            if (string.IsNullOrEmpty(str))
            {
                return DefaultValue;
            }
            return str;
        }
        public string dValidationExp(string strExpression)
        {
            return "CONVERT(( " + strExpression + "    ) * 1000000, System.Int64) / 1000000";
        }
        public Int32 BL_nValidation(object obj)
        {
            Int32 dstrValue;
            if (!Int32.TryParse(Convert.ToString(obj), out dstrValue))
            {
                dstrValue = 0;
            }
            return dstrValue;
        }
        public decimal BL_dValidation(object obj)
        {
            decimal dstrValue;
            if (!decimal.TryParse(Convert.ToString(obj), out dstrValue))
            {
                dstrValue = 0;
            }
            string number = Convert.ToString(dstrValue);
            if (number.Contains('.'))
            {
                if (number.Substring(number.IndexOf(".")).Length > 6)
                {
                    dstrValue = Math.Round(dstrValue, 6);
                    //dstrValue = Convert.ToDecimal(dstrValue.ToString().Substring(0, (dstrValue.ToString().IndexOf('.') + 7)));
                }
            }
            return dstrValue;
        }
        public bool SendEmail(string Subject, string Body, string ToMailID)
        {
            bool MailSend = false;
            try
            {
                string HostName = "", EMail = "", Pwd = "";
                //DataTable dtNames = BL_ExecuteParamSP("uspApplicationConfigValue");
                //if (dtNames.Rows.Count > 0)
                //{
                //    HostName = dtNames.Rows[0]["HostName"].ToString();
                //    EMail = dtNames.Rows[0]["EMailID"].ToString();
                //    Pwd = dtNames.Rows[0]["Password"].ToString();
                //}
                HostName = "smtp.gmail.com";
                EMail = "gksolutions.work007@gmail.com";
                Pwd = "ujrx zkfl kpfo dehr";
                if (!string.IsNullOrEmpty(HostName) && !string.IsNullOrEmpty(EMail) && !string.IsNullOrEmpty(Pwd))
                {
                    //
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    //message.From = new MailAddress("gks.helpdesk@gmail.com");//gks.helpdesk@gmail.com
                    message.From = new MailAddress(EMail);//gks.helpdesk@gmail.com//"vipassana.pveasllp@gmail.com"
                    message.To.Add(new MailAddress(ToMailID));
                    message.Subject = Subject;
                    message.IsBodyHtml = true; //to make message body as html  
                    message.Body = Body;
                    smtp.Port = 587;
                    smtp.Host = HostName;// "smtp.gmail.com"; //for gmail host  
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    //smtp.Credentials = new NetworkCredential("gks.helpdesk@gmail.com", "mkolocmylhzuuvdk");
                    //smtp.Credentials = new NetworkCredential("vipassana.pveasllp@gmail.com", "eyilfwmebqgydnjg");//pveasllp@2021
                    smtp.Credentials = new NetworkCredential(EMail, Pwd);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(message);
                    MailSend = true;
                }
                else
                {
                    MailSend = false;
                    //BL_WriteErrorMsginLog("BL", "Mail Send", "E-Mail config details are empty in App Config. You should give the details there.");
                }
            }
            catch (Exception ex)
            {
                MailSend = false;
                //BL_WriteErrorMsginLog("BL", "Mail Send - Exception", ex.Message);
                throw ex;
            }
            finally
            {

            }
            return MailSend;
        }
        public DataTable ConvertListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public void BL_AddCollectionData(DataTable dtHeader, DataTable dtDetail, DataTable dtMop)
        {
            try
            {
                dtHeader.Columns.Add("Date", typeof(DateTime));
                dtHeader.Columns.Add("CoLLPYType", typeof(int));
                dtHeader.Columns.Add("DocRefNo", typeof(string));
                dtHeader.Columns.Add("AccID", typeof(int));
                dtHeader.Columns.Add("ColAmt", typeof(decimal));
                dtHeader.Columns.Add("Balance", typeof(decimal));
                dtHeader.Columns.Add("ColMode", typeof(int));
                dtHeader.Columns.Add("Status", typeof(int));
                dtHeader.Columns.Add("ExAccId", typeof(int));
                dtHeader.Columns.Add("UID", typeof(int));
                dtHeader.Columns.Add("Type", typeof(int));
                dtHeader.Columns.Add("SerialNo", typeof(int));
                dtHeader.Columns.Add("VisaPern", typeof(decimal));
                dtHeader.Columns.Add("VisaAmt", typeof(decimal));

                dtDetail.Columns.Add("AccID", typeof(int));
                dtDetail.Columns.Add("DocPrefix", typeof(int));
                dtDetail.Columns.Add("DocValue", typeof(int));
                dtDetail.Columns.Add("DocID", typeof(int));
                dtDetail.Columns.Add("DocDate", typeof(DateTime));
                dtDetail.Columns.Add("Balance", typeof(decimal));
                dtDetail.Columns.Add("ColValue", typeof(decimal));
                dtDetail.Columns.Add("AdjAmt", typeof(decimal));
                dtDetail.Columns.Add("DiscPer", typeof(string));
                dtDetail.Columns.Add("DiscAmt", typeof(decimal));
                dtDetail.Columns.Add("FullyAdj", typeof(int));
                dtDetail.Columns.Add("FullyAdjAmt", typeof(decimal));
                dtDetail.Columns.Add("TotalAmtAdj", typeof(decimal));
                dtDetail.Columns.Add("TranType", typeof(int));
                dtDetail.Columns.Add("SerialNo", typeof(int));

                dtMop.Columns.Add("AccID", typeof(int));
                dtMop.Columns.Add("Mode", typeof(int));
                dtMop.Columns.Add("[Cheque/DD Number]", typeof(string));
                dtMop.Columns.Add("Date", typeof(DateTime));
                dtMop.Columns.Add("BankAccId", typeof(string));
                dtMop.Columns.Add("Neft", typeof(string));
                dtMop.Columns.Add("Amt", typeof(decimal));
                dtMop.Columns.Add("IFSC", typeof(string));
                dtMop.Columns.Add("Bank", typeof(string));
                dtMop.Columns.Add("Branch", typeof(string));
                dtMop.Columns.Add("PayAt", typeof(string));
                dtMop.Columns.Add("BankAccNo", typeof(string));
                dtMop.Columns.Add("ChequeBkRefNo", typeof(string));
                dtMop.Columns.Add("ChequeBookID", typeof(int));
                dtMop.Columns.Add("SerialNo", typeof(int));
                dtMop.Columns.Add("RecdAmt", typeof(decimal));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable listConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public void BL_WriteErrorMsginLog(string Companycode,string strfrmName, string strmsg, string errors, string LogType = "Error")
        {
            //int Error = System.Runtime.InteropServices.Marshal.GetExceptionCode();            
            BL_LogFileWrite(Companycode + "|" + LogType + "|" + strfrmName + "|" + strmsg + "|" + errors);
        }
        //LOG SYSTEM ERROR MESSAGE
        public static void BL_LogFileWrite(string message)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory;// System.Configuration.ConfigurationManager.AppSettings["SupportFilePath"];
                DirectoryInfo parentDir = Directory.GetParent(logFilePath.EndsWith("\\") ? logFilePath : string.Concat(logFilePath, "\\"));
                var myParentDir = parentDir.Parent.FullName;
                string strFol = myParentDir + "\\Log File Errors\\";

                strFol = strFol + "Log System Error" + "-" + DateTime.Today.ToString("ddMMyyyy") + "." + "txt";
                string[] Msgs = message.Split(new char[] { '|' });
                var obj = new List<object>();
                obj.Add(new
                {
                    Date = DateTime.Now.ToString("dd-MMM-yyyy"),
                    LogType = Msgs[1],
                    DataBase = Msgs[0],
                    FormName = Msgs[2],
                    FunctionName = Msgs[3],
                    Error = Msgs[4],
                    Timestamp = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt")
                });
                if (strFol.Equals("")) return;
                #region Create the Log file directory if it does not exists
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo = new FileInfo(strFol);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                #endregion Create the Log file directory if it does not exists
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(strFol, FileMode.Append);
                }
                streamWriter = new StreamWriter(fileStream);
                message = JsonConvert.SerializeObject(obj[0]);
                streamWriter.WriteLine(message + ",");//
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }
        }
        public bool BL_Alpha(string txtvalue)
        {
            if (txtvalue.ToString().All(char.IsLetter))
                return (true);
            else
                //MessageBox.Show("Enter Alphanumeric(A – Z)", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
        }
        public bool BL_AlphaNumeric(string txtvalue)
        {
            if (txtvalue.ToString().All(char.IsLetterOrDigit))
                return (true);
            else
                return (false);
        }
        public bool BL_NumericValidation(string txtvalue)
        {
            if (txtvalue.ToString().All(char.IsDigit))
                return (true);
            else
                return (false);
        }
        public bool BL_AlphaNumericSpl(string txtvalue)
        {
            // @ % - _ | , / \ .
            string strRegex = @"^[a-zA-Z0-9_@. ,%|*~&()\/-]+$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
                return (true);
            else
                //MessageBox.Show("Enter AlphaNumeric With Spacial characters(A – Z or 0 – 9 or _ @ . , % /|)", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);

        }
        public bool BL_DateformatDMY(string txtvalue)
        {
            // @ % - _ | , / \ .
            string strRegex = @"[0-9]{2}/[0-9]{2}/[0-9]{4}";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
                return (true);
            else
                return (false);

        }
        public bool BL_AlphaNumericSlashMinus(string txtvalue)
        {
            // @ % - _ | , / \ .
            string strRegex = @"^[a-zA-Z0-9/-]+$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
                return (true);
            else
                //MessageBox.Show("Enter AlphaNumeric With Spacial characters(A – Z or 0 – 9 or _ @ . , % /|)", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);

        }
        public bool BL_Numeric(object sender, KeyPressEventArgs e)
        {
            return e.Handled = ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_Numeric(string txtvalue)
        {
            if (txtvalue.ToString().All(char.IsDigit))
                return (true);
            else
                // MessageBox.Show("Enter Numbers(0 - 9)", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
        }
        public bool BL_PANValidation(string strrange)
        {
            try
            {
                if (strrange.Length >= 10 && strrange.Length <= 10)
                {
                    Regex rg = new Regex(@"[A-Z]{5}\d{4}[A-Z]{1}");
                    if (rg.IsMatch(strrange))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public bool BL_HSNSACValidation(string strrange)
        {
            try
            {
                Regex rg = new Regex(@"[0-9]{0,8}");
                if (rg.IsMatch(strrange))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        //^[0-9]+(\.[0-9]{1,2})?$
        public bool BL_NumericWithDecimal(string txtvalue)
        {
            //string strRegex = (@"(?<=^| )\d+(\.\d{1,2})?(?=$| )|(?<=^| )\.\d+(?=$| )"); //@"^\d+(\.\d{1,2})?$";
            string strRegex = (@"^\d+(\.\d{1,})?$");
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
            {
                return (true);
            }
            else if (txtvalue.All(char.IsNumber))
            {
                return (true);
            }
            else
                // MessageBox.Show("Enter Numeric or decimal with 2 Digit", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
        }
        public bool BL_NumericWithDecimalMinus(string txtvalue)
        {
            //string strRegex = (@"(?<=^| )\d+(\.\d{1,2})?(?=$| )|(?<=^| )\.\d+(?=$| )"); //@"^\d+(\.\d{1,2})?$";
            string strRegex = (@"^-\d+(\.\d{1,})?$");
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
            {
                return (true);
            }
            else if (txtvalue.All(char.IsNumber))
            {
                return (true);
            }
            else
                // MessageBox.Show("Enter Numeric or decimal with 2 Digit", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
        }
        public bool BL_AlphaNumeric(object sender, KeyPressEventArgs e)
        {
            return e.Handled = (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != ' ' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_Alpha(object sender, KeyPressEventArgs e)
        {
            return e.Handled = (!char.IsLetter(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != ' ' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_AlphaWithoutSpace(object sender, KeyPressEventArgs e)
        {
            return e.Handled = (!char.IsLetter(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_AlphaNumerWoSpaceWSlash(object sender, KeyPressEventArgs e)
        {
            return e.Handled = ((!char.IsLetter(e.KeyChar) && !char.IsNumber(e.KeyChar)) && e.KeyChar != '/' && e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_AlphaNumerWoSpaceWHyphonSlashComma(object sender, KeyPressEventArgs e)
        {
            return e.Handled = ((!char.IsLetter(e.KeyChar) && !char.IsNumber(e.KeyChar)) && e.KeyChar != '/' && e.KeyChar != '-' && e.KeyChar != ',' && e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != '\u001b');
        }
        public bool BL_Dateonly(string txtvalue)
        {
            string strRegex = @"^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[- /.](19|20)\d\d$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
                return (true);
            else
                // MessageBox.Show("Enter Format for date (dd(. or - or /)mm(. or - or /)yyyy)", " Validation Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
        }
        public bool BL_Email(string txtvalue)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
        public bool BL_MobileNumberValidate(string nLength)
        {
            bool bResult = false;

            if (nLength.Length >= 10 && nLength.Length <= 12)
            {
                Regex rg = new Regex("[0-9]{10,12}");
                if (rg.IsMatch(nLength))
                {
                    bResult = true;
                }

            }
            return bResult;
        }
        public bool BL_AadhaarValidate(string nLength)
        {
            bool bResult = false;

            if (nLength.Length == 12)
            {
                Regex rg = new Regex("[0-9]{12}");
                if (rg.IsMatch(nLength))
                {
                    bResult = true;
                }

            }
            return bResult;
        }
        public bool BL_isValidGSTIN(string txtvalue)
        {
            string strRegex = @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{3}$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(txtvalue))
                return (true);
            else
                return (false);

        }
        public bool BL_FSSAIValidate(string nLength)
        {
            bool bResult = false;

            if (nLength.Length == 14)
            {
                Regex rg = new Regex("[0-9]{14}");
                if (rg.IsMatch(nLength))
                {
                    bResult = true;
                }

            }
            return bResult;
        }
        public bool BL_PinNumberValidate(string nLength)
        {
            bool bResult = false;
            Regex rg = new Regex("[0-9]{6}");
            if (rg.IsMatch(nLength))
            {
                bResult = true;
            }
            return bResult;
        }
        public bool NumberValidate(string nLength)
        {
            bool bResult = false;
            Regex rg = new Regex("[0-9.]");
            if (rg.IsMatch(nLength))
            {
                bResult = true;
            }
            return bResult;
        }
        public static CultureInfo SetSysCulture = new CultureInfo("es-ES");
        public bool BL_IsValidDate(string strdate)
        {
            bool bIsDate = false;
            try
            {
                if (this.BL_Dateonly(strdate) == true)
                {
                    DateTime dtTemp;
                    //SetSysCulture.DateTimeFormat
                    if (DateTime.TryParse(strdate, SetSysCulture.DateTimeFormat, DateTimeStyles.None, out dtTemp))
                    {
                        bIsDate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bIsDate;
        }
        public decimal BL_GetFormulaValue(DataTable dtValue, string strFormula, int nRowIndex)
        {
            try
            {
                string[] strArray = new string[dtValue.Columns.Count - 1];
                for (int i = 0; i < dtValue.Columns.Count - 1; i++)
                {
                    strArray[i] = "[" + dtValue.Columns[i].ColumnName + "]";
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (strFormula.Contains(strArray[i]))
                    {
                        string str = strArray[i];
                        str = str.Replace("[", "");
                        str = str.Replace("]", "");
                        DataTable dt = dtValue.DefaultView.ToTable(false, dtValue.Columns[str].ColumnName);
                        if (dt.Rows.Count > 0)
                        {
                            strFormula = strFormula.Replace(strArray[i], dt.Rows[nRowIndex][0].ToString());
                        }
                        dt.Dispose();
                    }
                }
                object objValue = new DataTable().Compute(strFormula, null);
                return Convert.ToDecimal(objValue);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public DateTime ConvertToDate(string date)
        {
            string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };

            return DateTime.ParseExact(
                date,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None
            );
        }
        public byte[] CompressImage_old(Stream inputStream)
        {
            if (inputStream == null || inputStream.Length == 0)
                return null;

            inputStream.Position = 0;

            using (var image = Image.Load(inputStream))
            {
                image.Mutate(x =>
                {
                    x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(300, 300)
                    });
                    x.BackgroundColor(SixLabors.ImageSharp.Color.White);
                });

                using (var ms = new MemoryStream())
                {
                    var encoder = new JpegEncoder { Quality = 75 };
                    image.Save(ms, encoder);
                    return ms.ToArray();
                }
            }
        }
        public byte[] CompressImage(Stream inputStream)
        {
            if (inputStream == null || inputStream.Length == 0)
                return null;

            inputStream.Position = 0;

            try
            {
                IImageFormat format;

                using (var image = Image.Load(inputStream, out format))
                {
                    image.Mutate(x =>
                    {
                        x.AutoOrient();

                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(300, 300)
                        });
                    });

                    image.Metadata.ExifProfile = null;

                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, new JpegEncoder
                        {
                            Quality = 75
                        });

                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                string inner = ex.InnerException?.ToString();
                BL_WriteErrorMsginLog("BL", "BL", "CompressImage", ex.Message+"\nInner Error :"+ inner);
                throw new Exception("Image compression failed.", ex);
            }
        }
    }
}