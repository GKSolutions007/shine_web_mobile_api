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
    public class SalesReturnController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/mobilereturn/initialdata")]
        public IHttpActionResult Getinitialdata(string CompanyCode,string UserID)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspGetsetMobileSRDRData", 1, 0, UserID);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpGet]
        [Route("api/mobilereturn/customerlist")]
        public IHttpActionResult Getcustomerlist(string CompanyCode, string BeatID,string SalesmanID)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileSRDRData", 2, 0, BeatID, SalesmanID);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpGet]
        [Route("api/mobilereturn/customerdata")]
        public IHttpActionResult Getcustomerdata(string CompanyCode, string CustomerID)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspGetsetMobileSRDRData", 4, 0, CustomerID);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpGet]
        [Route("api/mobilereturn/documentlist")]
        public IHttpActionResult Getdocumentlist(string CompanyCode, string RetrunType,string UserID, string Showall)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileSRDRData", 5, RetrunType, UserID, Showall);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpGet]
        [Route("api/mobilereturn/itemfilter")]
        public IHttpActionResult Getproductdata(string CompanyCode, string TransType, string CustomerID, string BranchID, 
            string FilterType, string FilterValue)
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileSRDRData", 3, TransType, CustomerID, BranchID, FilterType, FilterValue);
            List<DailyActivityDetails> list = new List<DailyActivityDetails>();
            for (int i = 0; i < DDT.Rows.Count; i++)
            {
                list.Add(new DailyActivityDetails
                {
                    ID = DDT.Rows[i][0].ToString(),
                    Name = DDT.Rows[i][1].ToString(),
                    PriceDesc = DDT.Rows[i][2].ToString(),
                    Rate = DDT.Rows[i][3].ToString(),
                    Discount = DDT.Rows[i][4].ToString(),
                    MRP = DDT.Rows[i][5].ToString(),
                });
            }
            string str = "";
            var dtjsonData = new
            {
                data = from users in list
                       select
                           new
                           {
                               ID = users.ID,
                               Name = users.Name,
                               PriceDesc = users.PriceDesc,
                               Rate = users.Rate,
                               Discount = users.Discount,
                               MRP = users.MRP,
                           }
            };
            return Ok(dtjsonData);
        }
        [HttpGet]
        [Route("api/mobilereturn/documentdata")]
        public IHttpActionResult Getdocumentdata(string CompanyCode, string RetrunType, string ID)
        {
            DataSet DDT = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspGetsetMobileSRDRData", 6, RetrunType, ID);
            string dtjson = JsonConvert.SerializeObject(DDT);
            return Ok(dtjson);
        }
        [HttpPost]
        [Route("api/mobilereturn/save")]
        public IHttpActionResult Save(DailyActivity listTrans)
        {
            if (listTrans != null)
            {
                List<SaveMessage> list = new List<SaveMessage>();
                if (listTrans.lstProdDetails.Count == 0)
                {
                    list.Add(new SaveMessage()
                    {
                        ID = 0.ToString(),
                        MsgID = "1",
                        Message = "Items Data not found"
                    });
                    return Ok(list);
                }
                    
                
                    DataTable dtProd = new DataTable();
                if (dtProd.Columns.Count == 0)
                {
                    dtProd.Columns.Add("ProdId", typeof(int));
                    dtProd.Columns.Add("UomId", typeof(int));
                    dtProd.Columns.Add("Qty", typeof(decimal));
                    dtProd.Columns.Add("Price", typeof(decimal));
                    dtProd.Columns.Add("OrgPrice", typeof(decimal));
                    dtProd.Columns.Add("Amount", typeof(decimal), "(Qty*Price)");
                    dtProd.Columns.Add("MRP", typeof(decimal));
                    dtProd.Columns.Add("DiscPern", typeof(decimal));
                    dtProd.Columns.Add("DiscAmt", typeof(decimal), "(DiscPern*Amount)/100");
                    dtProd.Columns.Add("ConversionRate", typeof(decimal));
                    dtProd.Columns.Add("Serial", typeof(int));
                }
                DataTable dtProducts = bl.ConvertListToDataTable(listTrans.lstProdDetails);
                for (int j = 0; j < dtProducts.Rows.Count; j++)
                {
                    DataRow dtRow = dtProd.NewRow();
                    dtRow[0] = Convert.ToString(dtProducts.Rows[j]["ID"]);
                    dtRow[1] = 0;
                    dtRow[2] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["Qty"]));
                    dtRow[3] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["AppPrice"]));
                    dtRow[4] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["Rate"]));
                    dtRow[6] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["MRP"]));
                    dtRow[7] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["Discount"]));
                    dtRow[9] = 1;
                    dtRow[10] = (j + 1);
                    dtProd.Rows.Add(dtRow);
                }
                
                string formattedDate = DateTime.Today.ToString("yyyy-MM-dd");
                string dt = Convert.ToDateTime(formattedDate).ToString();//"yyyy-MM-dd"
                bl.bl_Transaction(listTrans.CompanyCode, 1);
                DataTable dtResult = bl.bl_ManageTrans(listTrans.CompanyCode, "uspManageOrderReturnSave", dtProd, bl.BL_nValidation(listTrans.TransMode), bl.BL_nValidation(listTrans.ReturnTransType), bl.BL_nValidation(listTrans.ID), 
                    dt, listTrans.BranchID,listTrans.CustomerID, bl.BL_nValidation(listTrans.BeatID), bl.BL_nValidation(listTrans.SalesManID), null,
                                           bl.BL_dValidation(listTrans.AddnlDisc),
                                           bl.BL_dValidation(listTrans.TrdDisc), bl.BL_nValidation(listTrans.CurrentStatus), 0, listTrans.Narration, bl.BL_nValidation(listTrans.UserID));
                if (dtResult.Rows.Count > 0)
                {
                    bl.bl_Transaction(listTrans.CompanyCode, 2);
                    int nBillScopeID = bl.BL_nValidation(dtResult.Rows[0][0]);
                    list.Add(new SaveMessage()
                    {
                        ID = nBillScopeID.ToString(),
                        MsgID = "0",
                        Message = "Saved Successfully"
                    });
                }
                else
                {
                    bl.bl_Transaction(listTrans.CompanyCode, 3);
                    list.Add(new SaveMessage()
                    {
                        ID = 0.ToString(),
                        MsgID = "1",
                        Message = "Data note Saved"
                    });
                }
                return Ok(list);
            }
            return Ok();
        }
    }
}
