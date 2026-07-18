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
    public class CommonController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/documentseries/docid")]
        public IHttpActionResult GetDocseriesid(string CompanyCode,string TransID, string BranchID, string DocDate)
        {
            var DocInfo = new List<object>();
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode,"uspgetDocumentID", DocDate, BranchID, TransID);
            if (DDT.Rows.Count > 0)
            {
                DocInfo.Add(new
                {
                    MsgID = "0",
                    Message = "Document ID fetched",
                    DocValue = DDT.Rows[0]["DocValue"].ToString(),
                    Prefix = DDT.Rows[0]["Prefix"].ToString(),
                    DocID = DDT.Rows[0]["DocID"].ToString()
                });
                string val = JsonConvert.SerializeObject(DDT);
                return Ok(DocInfo);
            }
            DocInfo.Add(new
            {
                MsgID = "1",
                Message = "Document ID not found"
            });
            return Ok(DocInfo);
        }
        [Route("api/colorsettings/get")]
        public IHttpActionResult Getcolors(string CompanyCode, string ThemeID)
        {
            DataTable DDT = new DataTable();
            DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageColorSettings", 1, ThemeID);
            return Ok(DDT);
        }
    }
}
