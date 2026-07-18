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
    public class HomeDashBoardController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        DataTable dtNames = new DataTable();
        [HttpGet]
        [Route("api/homedashboard/getdata")]
        public IHttpActionResult GetDatatoLoad(string CompanyCode, int Flag,  string UID)
        {
            if (Flag == 1)//Credit note ,Debit Note
            {
                List<HomeDashBoardModel> objNames = new List<HomeDashBoardModel>();
                dtNames = bl.BL_ExecuteParamSP(CompanyCode,"uspGetMobileHomeDashboard", UID);
                if (dtNames.Rows.Count > 0)
                {
                    for (int nCount = 0; nCount < dtNames.Rows.Count; nCount++)
                    {
                        objNames.Add(new HomeDashBoardModel()
                        {
                            TotCount = Convert.ToString(dtNames.Rows[0][0]),
                            OTCount = Convert.ToString(dtNames.Rows[1][0]),
                            CollAmt = Convert.ToString(dtNames.Rows[2][0]),
                            CashCollAmt = Convert.ToString(dtNames.Rows[3][0]),
                            StartTime = dtNames.Rows.Count == 6 ? Convert.ToString(dtNames.Rows[4][0]) : "Not Yet started",
                            EndTime = dtNames.Rows.Count == 6 ? Convert.ToString(dtNames.Rows[5][0]) : "Not Yet started",
                        });
                        break;
                    }
                }
                return Ok(objNames);
            }
            return Ok();
        }
    }
}
