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

        [HttpGet]
        [Route("api/customerlocation")]
        public IHttpActionResult Getcustomerlocation(string CompanyCode, string Type, string BeatID = "0", string SalesmanID = "0", 
            string Customer = "0",string Latitude = "",string Longitude="")
        {
            DataTable dtLocations = bl.BL_ExecuteParamSP(CompanyCode, "uspCustomerLocations", Type, BeatID, SalesmanID, Customer, Latitude, Longitude);
            if(dtLocations.Rows.Count > 0)
            {
                var Customerdata = new List<object>();
                for (int i = 0; i < dtLocations.Rows.Count; i++)
                {
                    string imgdata = null;
                    if (!string.IsNullOrEmpty(dtLocations.Rows[i]["Imagedata"].ToString()))
                    {
                        byte[] photoBytes = (byte[])dtLocations.Rows[i]["Imagedata"];
                        imgdata = "data:image/jpeg;base64," + Convert.ToBase64String(photoBytes);
                        //ProdID ProductName UomQty UomPrice    InclPrice MRP AdjustQty AvlVLS  Imagedata
                    }
                    Customerdata.Add(new
                    {
                        ID = dtLocations.Rows[i]["ID"],
                        Code = dtLocations.Rows[i]["Code"],
                        Name = dtLocations.Rows[i]["Name"],
                        Mob1 = dtLocations.Rows[i]["Mob1"],
                        Latitude = dtLocations.Rows[i]["Latitude"],
                        Longtitude = dtLocations.Rows[i]["Longtitude"],
                        Distance = dtLocations.Rows[i]["Distance"],
                        Balance = dtLocations.Rows[i]["Balance"],
                        InvCount = dtLocations.Rows[i]["InvCount"],
                        Aging = dtLocations.Rows[i]["Aging"],
                        Imagedata = imgdata
                    });
                }
                return Ok(Customerdata);
            }
            return Ok();
        }
        }
}
