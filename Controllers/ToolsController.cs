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
    public class ToolsController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/todayroute/managetodayroute")]
        public IHttpActionResult GetSettodayroute(string CompanyCode,string Mode, string UserID, string BeatID = "0", string SalesmanID = "0", string BranchID = "0")
        {
            if (Mode == "1")
            {
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode,"uspManageTodayRoute", Mode);
                if (DDT.Rows.Count > 0)
                {
                    for (int i = 0; i < DDT.Rows.Count; i++)
                    {
                        list.Add(new CustomerVendorModel
                        {
                            FType = DDT.Rows[i][0].ToString(),
                            Form = DDT.Rows[i][1].ToString(),
                            ID = DDT.Rows[i][2].ToString(),
                            Name = DDT.Rows[i][3].ToString(),
                        });
                    }
                }
                return Ok(list);
            }
            else if (Mode == "2")
            {
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageTodayRoute", Mode, UserID);
                List<Users> list = new List<Users>();
                if (DDT.Rows.Count > 0)
                {
                    list.Add(new Users
                    {
                        BeatID = DDT.Rows[0][0].ToString(),
                        SalesmanID = DDT.Rows[0][1].ToString(),
                        BranchID = DDT.Rows[0][2].ToString(),
                    });
                }
                return Ok(list);
            }
            else if (Mode == "3")
            {
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageTodayRoute", Mode, UserID, BeatID, SalesmanID, BranchID);
                List<SaveMessage> list = new List<SaveMessage>();
                //if (DDT.Rows.Count > 0)
                {
                    list.Add(new SaveMessage
                    {
                        MsgID = "0",
                        Message = "Saved successfully"
                    });
                }
                return Ok(list);
            }
            return Ok();
        }
    }
}
