using Newtonsoft.Json;
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
    public class DeliveryController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/delivery/initialdata")]
        public IHttpActionResult Getinitialdata(string CompanyCode,string ID)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspgetsetMobileDeliverydata", 1, ID);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpGet]
        [Route("api/delivery/invoicedata")]
        public IHttpActionResult GetInvoicedata(string CompanyCode, string BranchID, string Docvalue,string DeliveryType)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspgetsetMobileDeliverydata", 2, BranchID, Docvalue, DeliveryType);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpPost]
        [Route("api/delivery/save")]
        public IHttpActionResult SaveBankAccont(DeliveryModel deliverydata)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (deliverydata != null)
                {
                    
                    DataTable DDT = bl.BL_ExecuteParamSP(deliverydata.CompanyCode, "uspSaveMobileDelivery", deliverydata.DeliveryType, deliverydata.ID, deliverydata.BranchID,
                        deliverydata.UserID, deliverydata.RecieverName, deliverydata.Latitude, deliverydata.Longtitude);
                    if (DDT.Columns.Count == 0)
                    {                       
                        //Success message
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "0",
                            Message = "Saved Successfully"
                        });
                    }
                    else
                    {
                        //Error message
                        list.Add(new SaveMessage()
                        {
                            ID = "0",
                            MsgID = "1",
                            Message = DDT.Rows[0][0].ToString()
                        });
                    }
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage()
                {
                    ID = "1",
                    MsgID = "1",
                    Message = ex.Message
                });
            }
            return Ok(list);
        }
    }
}
