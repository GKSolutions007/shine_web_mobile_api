using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/login/get")]
        public IHttpActionResult GetloginData(string Companycode,string UserName, string Password)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                DataTable DDT = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 6, 0, UserName, clsEncryptDecrypt.Encrypt(Password));
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
                    int OTP = random.Next(100000, 999999);
                    bool Issend = bl.SendEmail("Reset Password OTP", "Hii "+ LoginUserName + ", OTP for Reset password is <b>" + OTP.ToString() + "</b>", Email, ToEmail);
                    if (Issend)
                    {
                        int OTPID = 0;
                        DataTable dtOTP = bl.BL_ExecuteParamSP(Companycode,"uspManageOTP", 1, 0, "MobileResetPassword", OTP, UserID);
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


        [HttpGet]
        [Route("api/otpverify")]
        public IHttpActionResult loginotpverify(string Companycode, string OTPID, string OTP)
        {
            try
            {
                var list = new List<object>();
                DataTable dtOTP = bl.BL_ExecuteParamSP(Companycode,"uspManageOTP", 2, OTPID, null, OTP);
                if (dtOTP.Rows.Count > 0)
                {
                    string UserID = dtOTP.Rows[0]["UserID"].ToString();
                    list.Add(new
                    {
                        MsgID = "1",
                        ID = UserID.ToString(),
                        Message = "OTP Validation success",
                    });
                    return Ok(list);
                }
                else
                {
                    list.Add(new
                    {
                        MsgID = "2",
                        Message = "Invalid OTP"
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
        [Route("api/forgotpassword/setpassword")]
        public IHttpActionResult GetFGsetpwd(string Companycode, string UserName, string Email, string Password)
        {
            try
            {

                DataTable DDT = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 9, 0, UserName, clsEncryptDecrypt.Encrypt(Password),
                    null, Email);
                List<SaveMessage> list = new List<SaveMessage>();
                //if (DDT.Rows.Count > 0)
                {
                    list.Add(new SaveMessage
                    {
                        MsgID = "0",
                        Message = "Password chaged successfully"
                    });
                }
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
