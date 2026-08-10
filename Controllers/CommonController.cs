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
using System.Web.Security;

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
        [HttpGet]
        [Route("api/BranchMapping/getByRole")]
        public IHttpActionResult GetBranchesByRole(string CompanyCode, int RoleID)
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode,"uspGetBranchByRoles", RoleID);
                return Ok(DDT);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(CompanyCode,"Common", "BranchMapping/getByRole", ex.Message);
            }
            return Ok();
        }
        [Route("api/partyinfos")]
        public IHttpActionResult Getpartyinfo(string CompanyCode, string PartyID,string BranchID)
        {
            try
            {
                var objNames = new List<object>();
                DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspPartyInfo", PartyID, BranchID);
                if (DDT.Tables.Count > 0)
                {
                    DataTable dtCustomerActivity = DDT.Tables[0];
                    DataTable dtCustomer = DDT.Tables[1];
                    DataTable dtCustomerImages = DDT.Tables[2];
                    string customerjson = JsonConvert.SerializeObject(DDT);
                    var customerimagedata = new List<object>();
                    foreach (DataRow dr in dtCustomerImages.Rows)
                    {
                        string imgdata = null;
                        if (!string.IsNullOrEmpty(dr["Imagedata"].ToString()))
                        {
                            byte[] photoBytes = (byte[])dr["Imagedata"];
                            imgdata = Convert.ToBase64String(photoBytes);
                        }
                        customerimagedata.Add(new
                        {
                            ImageData = imgdata
                        });
                    }
                    objNames.Add(new
                    {
                        Mode = 0,
                        Message = "Customer Data fetched",
                        CustomerActivity = dtCustomerActivity,
                        Customerdata = dtCustomer,
                        Imagedata = customerimagedata
                    });

                    return Ok(objNames);
                }
                return Ok(DDT);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(CompanyCode, "Common", "partyinfos", ex.Message);
            }
            return Ok();
        }
    }
}
