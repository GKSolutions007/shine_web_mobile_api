using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    }
}
