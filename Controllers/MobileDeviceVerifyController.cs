using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ShineWebMobileAPI.Controllers
{
    public class MobileDeviceVerifyController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/mobiledeviceverify/verify")]
        public IHttpActionResult GetData(string Companycode, string Mode, string DeviceName, string DeviceID, string UID, string DBName, string Active, string Ident)
        {
            DataTable dt = bl.BL_ExecuteParamSP(Companycode,"uspVerifyMobileDeviceInfo", Mode, DeviceName, DeviceID, UID, DBName, Active, DateTime.Now, Ident);
            string JSONCONV = JsonConvert.SerializeObject(dt);
            return Ok(JSONCONV);
        }
        [HttpGet]
        [Route("api/mobiledevicelogin/loginverify")]
        public IHttpActionResult Getloginverify(string Companycode, string Mode, string TokenValue, string UID, string DivIDent)
        {
            DataTable dt = bl.BL_ExecuteParamSP(Companycode,"uspLoginInfoRecieve", Mode, TokenValue, UID, DivIDent);
            string JSONCONV = JsonConvert.SerializeObject(dt);
            return Ok(JSONCONV);
        }
        [HttpGet]
        [Route("api/mobiledevicelogin/getpermissions")]
        public IHttpActionResult validatepermissionsData(string Companycode, string UID)
        {
            DataSet ds = new DataSet();
            DataTable dt = bl.BL_ExecuteSqlQuery(Companycode, "select *,DB_NAME() DBName from tblCompanyRegistration");
            dt.TableName = "CompanyReg";
            ds.Tables.Add(dt);
            DataTable dtAppconfig = bl.BL_ExecuteParamSP(Companycode, "uspManageApplicationConfig", 1);
            dtAppconfig.TableName = "AppConfig";
            ds.Tables.Add(dtAppconfig);
            DataTable dtRes = bl.BL_ExecuteParamSP(Companycode, "uspManageUsers", 4, UID);
            dtRes.TableName = "UserData";
            ds.Tables.Add(dtRes);
            string RID = dtRes.Rows[0]["RoleID"].ToString();
            DataTable dtParent = bl.BL_ExecuteParamSP(Companycode, "uspMenuPermission", 1, null, 0, 1);
            dtParent.TableName = "ParentMenu";
            ds.Tables.Add(dtParent);
            DataTable dtPermission = bl.BL_ExecuteParamSP(Companycode, "uspMenuPermission", 2, RID, UID, 1);
            dtPermission.TableName = "UserMenus";
            ds.Tables.Add(dtPermission);
            DataTable dtReportParent = bl.BL_ExecuteParamSP(Companycode, "uspReportPermission", 1, RID, 0, 1);
            dtReportParent.TableName = "ParentRepMenu";
            ds.Tables.Add(dtReportParent);
            DataTable dtReportPermission = bl.BL_ExecuteParamSP(Companycode, "uspReportPermission", 2, RID, UID, 1);
            dtReportPermission.TableName = "UserRepMenus";
            ds.Tables.Add(dtReportPermission);
            string dtjson = JsonConvert.SerializeObject(ds);
            return Ok(dtjson);
        }
        
        [HttpGet]
        [Route("api/mobiledevicelogin/savelogininfo")]
        public IHttpActionResult savelogininfo(string Companycode, int LoginMode, string IpAddress, string DeviceName, string TokenValue, int SesStatus, int UserID = 0, string DBName = "", int DeviceID = 0)
        {
            DataTable dt = bl.BL_ExecuteParamSP(Companycode, "uspMobileDeviceSaveLoginInfo", LoginMode, IpAddress, DeviceName, TokenValue, SesStatus, UserID, DBName, DeviceID);
            string JSONCONV = JsonConvert.SerializeObject(dt);
            return Ok(JSONCONV);
        }
    }
}
