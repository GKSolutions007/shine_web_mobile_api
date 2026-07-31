using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
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
    public class ReportsController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/customeros/initiatedata")]
        public IHttpActionResult COSinitiatedata(string CompanyCode)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspMobileCustomerOS", 1);
            string invjson = JsonConvert.SerializeObject(DDT);
            return Ok(invjson);
        }
        [HttpGet]
        [Route("api/customeros/generatedata")]
        public IHttpActionResult COSgeneratedata(string CompanyCode,int Mode, string BeatID, string SalesmanID, 
            string Party, string Period, string CustomerType, string Rating)
        {
            if(Mode == 2)
            {
                DataTable dtResult = bl.BL_ExecuteParamSP(CompanyCode, "uspMobileCustomerOS", Mode, BeatID, SalesmanID, Party, Period, CustomerType, Rating);
                string invjson = JsonConvert.SerializeObject(dtResult);
                return Ok(invjson);
            }
            else if (Mode == 3)
            {
                DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspMobileCustomerOS", Mode, BeatID);
                string invjson = JsonConvert.SerializeObject(DDT);
                return Ok(invjson);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/useractivity/initiatedata")]
        public IHttpActionResult Useractivityinitiatedata(string CompanyCode)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspMobileUserActivity", 1);
            string invjson = JsonConvert.SerializeObject(DDT);
            return Ok(invjson);
        }
        [HttpGet]
        [Route("api/useractivity/generatedata")]
        public IHttpActionResult Useractivitygeneratedata(string CompanyCode,string Users,string Date)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspMobileUserActivity", 2, Users, Date);
            string invjson = JsonConvert.SerializeObject(DDT);
            return Ok(invjson);
        }
        [HttpGet]
        [Route("api/mobiledevicelogin/Reportpermissions")]
        public IHttpActionResult validatepermissionsData(string Companycode, string RID, string UID)
        {
            DataSet ds = new DataSet();
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
        [Route("api/vls/generatedata")]
        public IHttpActionResult VLSgeneratedata(string CompanyCode, string BranchID, string SalesmanID)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspMobileVLSReport", BranchID, SalesmanID);
            return Ok(DDT);
        }
        [HttpGet]
        [Route("api/spotsales/generatedata")]
        public IHttpActionResult VLSgeneratedata(string CompanyCode,string Mode, string SalesmanID, string Date)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspMobileSpotinvoicesreport", Mode, SalesmanID, Date);
            return Ok(DDT);
        }
    }
}
