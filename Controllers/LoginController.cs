using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using ShineWebMobileAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using static System.Net.WebRequestMethods;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        // Lazy-evaluated: decryption only runs in actions that open a raw SqlConnection.
        private string _connectionString;
        private string _companycode { get; set; }
        private string connectionString
        {
            get
            {
                if (_connectionString == null)
                    _connectionString = SecretConfig.ConnectionString(_companycode);
                return _connectionString;
            }
        }
        private DataTable dtUserInfo { get; set; }
        // Hash used when the user name does not exist, so a wrong user name takes as long as a
        // wrong password (otherwise response time reveals which user names exist).
        private static readonly Lazy<string> _dummyHash = new Lazy<string>(() => PasswordHasher.Hash(Guid.NewGuid().ToString()));

        /// <summary>
        /// Verifies a login password against tblUsers. Accepts PBKDF2 hashes and, for users not yet
        /// migrated, the legacy encrypted value; a legacy (or weaker) value is replaced with a new
        /// hash on success. Returns the stored value to pass to uspManageUsers mode 6, or null.
        /// </summary>
        private string VerifyLoginPassword(string Companycode,string userName, string password)
        {
            _companycode = Companycode;
            DataTable dt = bl.BL_ExecuteParamSP(Companycode,"uspGetUserCredential", userName);
            bool unused;
            if (dt.Rows.Count == 0)
            {
                PasswordHasher.Verify(password, _dummyHash.Value, out unused);
                return null;
            }
            else
            {
                dtUserInfo = dt;
            }
            foreach (DataRow row in dt.Rows)
            {
                string stored = row["Password"].ToString();
                bool needsRehash;
                if (!PasswordHasher.Verify(password, stored, out needsRehash)) continue;
                if (needsRehash)
                {
                    string newHash = PasswordHasher.Hash(password);
                    try
                    {
                        DataTable upd = bl.BL_ExecuteParamSP(Companycode, "uspSetUserPasswordHash", row["ID"], stored, newHash);
                        if (upd.Rows.Count > 0 && Convert.ToInt32(upd.Rows[0][0]) == 1)
                            stored = newHash;
                    }
                    catch (Exception ex)
                    {
                        // Login still succeeds with the legacy value; the upgrade is retried next time.
                        bl.BL_WriteErrorMsginLog(Companycode, "Login", "password hash upgrade", ex.Message);
                    }
                }
                return stored;
            }
            return null;
        }
        [HttpGet]
        [Route("api/login/get")]
        public IHttpActionResult GetloginData(string Companycode,string UserName, string Password)
        {
            _companycode = Companycode;
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                //DataTable DDT = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 6, 0, UserName, clsEncryptDecrypt.Encrypt(Password));
                string storedPassword = VerifyLoginPassword(Companycode, UserName, Password);
                DataTable DDT = storedPassword == null
                    ? new DataTable()
                    : bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 6, 0, UserName, storedPassword);
                List<Users> list = new List<Users>();
                if (DDT.Rows.Count > 0)
                {

                    list.Add(new Users
                    {
                        ID = DDT.Rows[0]["ID"].ToString(),
                        UserName = DDT.Rows[0]["UserName"].ToString(),
                        Active = DDT.Rows[0]["Active"].ToString(),
                        //Password = DDT.Rows[0]["Password"].ToString(),
                        Mobilenumber = DDT.Rows[0]["Mobilenumber"].ToString(),
                        EMailID = DDT.Rows[0]["EMailID"].ToString(),
                        RoleID = DDT.Rows[0]["RoleID"].ToString(),
                        PwdResetCount = DDT.Rows[0]["PwdResetCount"].ToString(),
                        PwdResetTime = DDT.Rows[0]["PwdResetTime"].ToString(),
                        LPin = DDT.Rows[0]["LPin"].ToString(),
                        UserID = DDT.Rows[0]["CBy"].ToString(),
                    });
                    //HttpContext.Current.Session["LoginUserID"] = DDT.Rows[0]["ID"].ToString();
                    //HttpContext.Current.Session.Add("LoginUserID", DDT.Rows[0]["ID"].ToString());// = DDT.Rows[0]["ID"].ToString();
                    //DataTable dtParent = bl.BL_ExecuteParamSP("uspMenuPermission", 1, null);
                    //DataTable dtPermission = bl.BL_ExecuteParamSP("uspMenuPermission", 2, DDT.Rows[0]["RoleID"].ToString(), DDT.Rows[0]["ID"].ToString());//Convert.ToInt32(Session["LoginUserID"])
                    //HttpContext.Current.Session["dtParent"] = dtParent;
                    //HttpContext.Current.Session["dtPermission"] = dtPermission;
                    //Session["dtParent"] = dtParent;
                    //Session["dtPermission"] = dtPermission;
                    //var authToken = TokenHelper.GenerateToken(DDT.Rows[0]["ID"].ToString());
                    //var refreshToken = TokenHelper.GenerateRefreshToken(DDT.Rows[0]["ID"].ToString());                    
                }
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/forgotpassword/validateuser")]
        public IHttpActionResult GetFGuserData(string Companycode, string UserName, string Email)
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 8, 0, UserName, Email);
                var list = new List<object>();
                if (DDT.Rows.Count > 0)
                {
                    int UserID = bl.BL_nValidation(DDT.Rows[0][0].ToString());
                    string LoginUserName = DDT.Rows[0][1].ToString();
                    DataTable dtCompData = bl.BL_ExecuteParamSP(Companycode,"uspValidateDevice", 4);
                    string ToEmail = dtCompData.Rows[0]["Email"].ToString();
                    string CompName = dtCompData.Rows[0]["CompanyName"].ToString();
                    string CompCode = dtCompData.Rows[0]["CompanyCode"].ToString();
                    string CCMail = dtCompData.Rows[0]["CCEmail"].ToString();
                    Random random = new Random();
                    string OTP = NewOtp();// random.Next(100000, 999999);
                    DataTable dtOtp = bl.BL_ExecuteParamSP(Companycode, "uspPasswordResetOtp", 1, UserName, Email, PasswordHasher.Hash(OTP));
                    if (dtOtp.Rows.Count > 0 && Convert.ToInt32(dtOtp.Rows[0]["Status"]) == 1)
                    {
                        bool Issend = bl.SendEmail("Reset Password OTP", "Hii " + LoginUserName + ", OTP for Reset password is <b>" + OTP + "</b>", Email, ToEmail);
                        if (Issend)
                        {
                            int OTPID = 0;
                            DataTable dtOTP = bl.BL_ExecuteParamSP(Companycode, "uspManageOTP", 1, 0, "MobileResetPassword", OTP, UserID);
                            if (dtOTP.Rows.Count > 0)
                            {
                                OTPID = Convert.ToInt32(dtOTP.Rows[0][0].ToString());
                            }

                            list.Add(new
                            {
                                Mode = "2",
                                ID = OTPID.ToString(),// DDT.Rows[0]["ID"].ToString(),
                                UserID = UserID,
                                EMailID = Email,
                                ResponseMessage = "OTP Send to your Email ID"
                            });
                            return Ok(list);
                        }
                        else
                        {
                            list.Add(new
                            {
                                Mode = "3",
                                ResponseMessage = "OTP E-Mail is not sending. Please check E-mail ID and try again"
                            });
                            return Ok(list);
                        }
                    }
                    else if (dtOtp.Rows.Count > 0 && Convert.ToInt32(dtOtp.Rows[0]["Status"]) == -1)
                    {
                        list.Add(new
                        {
                            Mode = "3",
                            ResponseMessage = "OTP request limit exceeded. You have reached the maximum number of OTP requests allowed per hour. Please try again later."
                        });
                        return Ok(list);
                    }
                }
                else
                {
                    list.Add(new
                    {
                        Mode = "3",
                        ResponseMessage = "Invalid User Name & Email"
                    });
                    return Ok(list);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode, "Login", "forgotpassword/validate", ex.Message);
            }
            return Ok();
        }
        private string NewOtp()
        {
            byte[] b = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(b);
            return (100000 + (BitConverter.ToUInt32(b, 0) % 900000)).ToString();
        }

        [HttpGet]
        [Route("api/otpverify_old")]
        public IHttpActionResult loginotpverify(string Companycode, string OTPID, string OTP)
        {
            try
            {
                var list = new List<object>();
                DataTable dtOTP = bl.BL_ExecuteParamSP(Companycode,"uspManageOTP", 2, OTPID, null, OTP);
                if (dtOTP.Columns.Count > 1)
                {
                    list.Add(new
                    {
                        MsgID = "1",
                        ID = dtOTP.Rows[0]["UserID"].ToString(),
                        Message = "OTP Validation success",
                    });
                    return Ok(list);
                }
                else
                {
                    list.Add(new
                    {
                        MsgID = "2",
                        Message = dtOTP.Rows[0][0].ToString()
                    });
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode,"Login", "login/otpverify", ex.Message);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/otpverify")]
        public IHttpActionResult VerifyFGOtp(string Companycode, string UserName, string Email, string OTP)
        {
            var fail = new List<SaveMessage> { new SaveMessage { MsgID = "1", Message = "OTP Mismatched" } };
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(OTP))
                    return Ok(fail);
                DataTable dt = bl.BL_ExecuteParamSP(Companycode, "uspPasswordResetOtp", 2, UserName, Email);
                if (dt.Rows.Count == 0)
                    return Ok(new List<SaveMessage> { new SaveMessage { MsgID = "2", Message = "OTP expired. Please request a new OTP." } });
                object otpId = dt.Rows[0]["OtpID"];
                bool unused;
                if (!PasswordHasher.Verify(OTP.Trim(), dt.Rows[0]["OtpHash"].ToString(), out unused))
                {
                    bl.BL_ExecuteParamSP(Companycode, "uspPasswordResetOtp", 3, null, null, null, otpId);   // counts the wrong attempt
                    return Ok(fail);
                }
                DataTable ok = bl.BL_ExecuteParamSP(Companycode, "uspPasswordResetOtp", 4, null, null, null, otpId);
                if (ok.Rows.Count > 0 && Convert.ToInt32(ok.Rows[0][0]) == 1)
                    return Ok(new List<SaveMessage> { new SaveMessage { MsgID = "0", Message = "OTP verified" } });
                return Ok(fail);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode, "Login", "otpverify", ex.Message);
            }
            return Ok(fail);
        }
        [HttpGet]
        [Route("api/forgotpassword/setpassword")]
        public IHttpActionResult GetFGsetpwd(string Companycode, string UserName, string Email, string Password)
        {
            try
            {

                //DataTable DDT = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 9, 0, UserName, clsEncryptDecrypt.Encrypt(Password),
                //    null, Email);
                //List<SaveMessage> list = new List<SaveMessage>();
                ////if (DDT.Rows.Count > 0)
                //{
                //    list.Add(new SaveMessage
                //    {
                //        MsgID = "0",
                //        Message = "Password chaged successfully"
                //    });
                //}
                //return Ok(list);
                if (string.IsNullOrEmpty(Password))
                    return Ok(new List<SaveMessage> { new SaveMessage { MsgID = "1", Message = "Password should not be empty" } });
                DataTable DDT = bl.BL_ExecuteParamSP(Companycode,"uspManageUsers", 9, 0, UserName, PasswordHasher.Hash(Password),
                    null, Email);
                bool updated = DDT.Rows.Count > 0 && DDT.Columns.Contains("Updated") && Convert.ToInt32(DDT.Rows[0]["Updated"]) == 1;
                List<SaveMessage> list = new List<SaveMessage>();
                list.Add(updated
                    ? new SaveMessage { MsgID = "0", Message = "Password chaged successfully" }
                    : new SaveMessage { MsgID = "1", Message = "OTP verification expired. Please request a new OTP." });
                return Ok(list);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(Companycode, "Login", "forgotpassword/setpassword", ex.Message);
            }
            return Ok();
        }
    }
}
