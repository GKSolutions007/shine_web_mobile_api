using Newtonsoft.Json;
using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml.Linq;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DailyActivityController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        [HttpGet]
        [Route("api/dailyactivity/getdata")]
        public IHttpActionResult GetData(string CompanyCode, string Mode, string ID, string SalesmanID = "")
        {
            DataTable DDT = new DataTable();
            if (Mode == "1" || Mode == "2")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetDailyactivityData", Mode, ID, SalesmanID);
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CustomerVendorModel
                    {
                        FType = DDT.Rows[i][0].ToString(),
                        Form = DDT.Rows[i][1].ToString(),
                        ID = DDT.Rows[i][2].ToString(),
                        Code = DDT.Rows[i][3].ToString(),
                        Name = DDT.Rows[i][4].ToString(),
                    });
                }
                return Ok(list);
            }
            else if(Mode == "5")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetDailyactivityData", Mode, ID);
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CustomerVendorModel
                    {
                        ID = DDT.Rows[i][2].ToString(),
                        Code = DDT.Rows[i][0].ToString(),
                        Name = DDT.Rows[i][1].ToString(),
                        Billadd1 = DDT.Rows[i][3].ToString(),
                        Billadd2 = DDT.Rows[i][4].ToString(),
                    });
                }
                return Ok(list);
            }
            else if (Mode == "6")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetDailyactivityData", Mode, ID, SalesmanID);
                string MobCollData = JsonConvert.SerializeObject(DDT);
                return Ok(MobCollData);
            }
            else if (Mode == "7" || Mode == "8")
            {
                DataSet dsCollData = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspGetsetDailyactivityData", Mode, ID);
                string MobCollData = JsonConvert.SerializeObject(dsCollData);
                return Ok(MobCollData);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/customerinfo")]
        public IHttpActionResult Getsurroundingcustomers(string CompanyCode, string CustomerID)
        {
            var objNames = new List<object>();
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 5, CustomerID);
            DataTable dtCustomerImages = bl.BL_ExecuteParamSP(CompanyCode, "uspManageCustomerMaster", 10, CustomerID);
            if (DDT.Rows.Count > 0)
            {
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
                    Customerdata = customerjson,
                    Imagedata= customerimagedata
                });

                return Ok(objNames);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/surroundingcustomers")]
        public IHttpActionResult Getsurroundingcustomers(string CompanyCode, string UserID, string Latitude = "", string Longitude = "")
        {
            var objNames = new List<object>();
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetSurroundingCustomer", Latitude, Longitude, UserID);
            if (DDT.Rows.Count > 0)
            {
                if (DDT.Columns.Count == 3)
                {
                    objNames.Add(new
                    {
                        Mode = 1,
                        Message = Convert.ToString(DDT.Rows[0][0]),
                        Customerdata = ""
                    });
                }
                else
                {
                    //string customerjson = JsonConvert.SerializeObject(DDT);       
                    var customerjson = new List<object>();
                    foreach (DataRow dr in DDT.Rows)
                    {
                        string imgdata = null;
                        if (!string.IsNullOrEmpty(dr["Imagedata"].ToString()))
                        {
                            byte[] photoBytes = (byte[])dr["Imagedata"];
                            imgdata = Convert.ToBase64String(photoBytes);
                        }                        
                        customerjson.Add(new
                        {
                            ID = dr["ID"],
                            Customercode = dr["Customer Code"],
                            Customername = dr["Customer Name"],
                            Radius = dr["Radius"],
                            Address = dr["Address"],
                            ImageData = imgdata
                        });
                    }
                    objNames.Add(new
                    {
                        Mode = 0,
                        Message = "Customer Data fetched",
                        Customerdata = customerjson
                    });
                }
                return Ok(objNames);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/dailyactivity/itemfilter")]
        public IHttpActionResult GetProductFilter(string CompanyCode, string Mode, string CustomerID, string BranchID, string FilterType, string FilterValue = "")
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetDailyactivityData", Mode, CustomerID, BranchID, FilterType, FilterValue);
            List<DailyActivityDetails> list = new List<DailyActivityDetails>();
            for (int i = 0; i < DDT.Rows.Count; i++)
            {
                string imgdata = null;
                if (!string.IsNullOrEmpty(DDT.Rows[i]["Imagedata"].ToString()))
                {
                    byte[] photoBytes = (byte[])DDT.Rows[i]["Imagedata"];
                    imgdata = Convert.ToBase64String(photoBytes);
                }
                list.Add(new DailyActivityDetails
                {
                    ID = DDT.Rows[i][0].ToString(),
                    Name = DDT.Rows[i][1].ToString(),
                    PriceDesc = DDT.Rows[i][2].ToString(),
                    Rate = DDT.Rows[i][3].ToString(),
                    Discount = DDT.Rows[i][4].ToString(),
                    MRP = DDT.Rows[i][5].ToString(),
                    Imagedata= imgdata
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
                               Imagedata = users.Imagedata
                           }
            };
            return Ok(dtjsonData);
        }
        [HttpPost]
        [Route("api/dailyactivity/save")]
        public IHttpActionResult Save(DailyActivity listTrans)
        {
            if (listTrans != null)
            {
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
                    dtRow[6] = 0;
                    dtRow[7] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[j]["Discount"]));
                    dtRow[9] = 1;
                    dtRow[10] = (j + 1);
                    dtProd.Rows.Add(dtRow);
                }
                List<SaveMessage> list = new List<SaveMessage>();
                string formattedDate = DateTime.Today.ToString("yyyy-MM-dd");
                string dt = Convert.ToDateTime(formattedDate).ToString();//"yyyy-MM-dd"
                bl.bl_Transaction(listTrans.CompanyCode, 1);
                DataTable dtResult = bl.bl_ManageTrans(listTrans.CompanyCode, "uspManageOrderTakenImport", dtProd, bl.BL_nValidation(listTrans.TransMode), bl.BL_nValidation(listTrans.ID), dt, listTrans.BranchID,
                                           listTrans.CustomerID, bl.BL_nValidation(listTrans.BeatID), bl.BL_nValidation(listTrans.SalesManID), null,
                                           bl.BL_dValidation(listTrans.AddnlDisc),
                                           bl.BL_dValidation(listTrans.TrdDisc), bl.BL_nValidation(listTrans.CurrentStatus), 0, listTrans.Narration, bl.BL_nValidation(listTrans.UserID), 2, listTrans.ActivityID, listTrans.FeedBack);
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
        [HttpGet]
        [Route("api/mobilecollection/getdata")]
        public IHttpActionResult GetMOBCOLLData(string CompanyCode, string Mode, string ID, string SalesmanID = "", 
            string FilterType = "",string BranchID = "0")
        {
            DataTable DDT = new DataTable();
            if (Mode == "1" || Mode == "2")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileCollectionData", Mode, ID, SalesmanID, FilterType);
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CustomerVendorModel
                    {
                        FType = DDT.Rows[i][0].ToString(),
                        Form = DDT.Rows[i][1].ToString(),
                        ID = DDT.Rows[i][2].ToString(),
                        Code = DDT.Rows[i][3].ToString(),
                        Name = DDT.Rows[i][4].ToString(),
                    });
                }
                return Ok(list);
            }
            else if (Mode == "4")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileCollectionData", Mode, ID, SalesmanID, FilterType,BranchID);
                List<adjDocs> list = new List<adjDocs>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new adjDocs
                    {
                        ID = DDT.Rows[i][0].ToString(),
                        Docprefix = DDT.Rows[i][1].ToString(),
                        Docid = DDT.Rows[i][2].ToString(),
                        Docdate = DDT.Rows[i][3].ToString(),
                        Refno = DDT.Rows[i][4].ToString(),
                        NetAmt = DDT.Rows[i][5].ToString(),
                        Balance = DDT.Rows[i][6].ToString(),
                        AssignInvoiceID = DDT.Rows[i][7].ToString(),
                    });
                }
                var data = from users in list
                           select
                               new
                               {
                                   ID = users.ID,
                                   Docprefix = users.Docprefix,
                                   Docid = users.Docid,
                                   Docdate = users.Docdate,
                                   Refno = users.Refno,
                                   NetAmt = users.NetAmt,
                                   Balance = users.Balance,
                                   AssignInvoiceID = users.AssignInvoiceID,
                               };
                return Ok(data);
            }
            else if (Mode == "5")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileCollectionData", Mode, ID);
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CustomerVendorModel
                    {
                        ID = DDT.Rows[i][2].ToString(),
                        Code = DDT.Rows[i][0].ToString(),
                        Name = DDT.Rows[i][1].ToString(),
                        Billadd1 = DDT.Rows[i][3].ToString(),
                        Billadd2 = DDT.Rows[i][4].ToString(),
                    });
                }
                return Ok(list);
            }
            else if (Mode == "7")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileCollectionData", Mode, ID, SalesmanID, FilterType);
                string MobCollData = JsonConvert.SerializeObject(DDT);
                return Ok(MobCollData);
            }
            else if (Mode == "8" || Mode == "9")
            {
                DataSet dsCollData = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspGetsetMobileCollectionData", Mode, ID);
                string MobCollData = JsonConvert.SerializeObject(dsCollData);
                return Ok(MobCollData);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/mobilecollection/validateswitchsm")]
        public IHttpActionResult validateswitchsm(string CompanyCode, string Mode, string CustomerID = "0", string SalesmanID = "0")
        {
            DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetsetMobileCollectionData", Mode, CustomerID, SalesmanID);
            string nCounts = DDT.Rows[0][0].ToString();
            return Ok(nCounts);
        }
            [HttpPost]
        [Route("api/mobilecollection/save")]
        public IHttpActionResult Savemobcoll(CollectionModel listTrans)
        {
            try
            {
                if (listTrans != null)
                {
                    DataTable dtDetail = new DataTable();
                    dtDetail.Columns.Add("AccID", typeof(int));
                    dtDetail.Columns.Add("DocPrefix", typeof(int));
                    dtDetail.Columns.Add("DocValue", typeof(int));
                    dtDetail.Columns.Add("DocID", typeof(int));
                    dtDetail.Columns.Add("DocDate", typeof(DateTime));
                    dtDetail.Columns.Add("Balance", typeof(decimal));
                    dtDetail.Columns.Add("ColValue", typeof(decimal));
                    dtDetail.Columns.Add("AdjAmt", typeof(decimal));
                    dtDetail.Columns.Add("DiscPer", typeof(string));
                    dtDetail.Columns.Add("DiscAmt", typeof(decimal));
                    dtDetail.Columns.Add("FullyAdj", typeof(int));
                    dtDetail.Columns.Add("FullyAdjAmt", typeof(decimal));
                    dtDetail.Columns.Add("TotalAmtAdj", typeof(decimal));
                    dtDetail.Columns.Add("TranType", typeof(int));
                    dtDetail.Columns.Add("SerialNo", typeof(int));
                    dtDetail.Columns.Add("ReasonID", typeof(int));
                    DataTable dtInvoices = bl.ConvertListToDataTable(listTrans.lstadjdocs);
                    for (int j = 0; j < dtInvoices.Rows.Count; j++)
                    {
                        string DT = Convert.ToString(dtInvoices.Rows[j]["Docdate"]);
                        string ID = Convert.ToString(dtInvoices.Rows[j]["ID"]);
                        DataRow dtRow = dtDetail.NewRow();
                        dtRow["Docid"] = Convert.ToString(dtInvoices.Rows[j]["ID"]);
                        dtRow["DocPrefix"] =Convert.ToString(dtInvoices.Rows[j]["DocPrefix"]);
                        dtRow["DocDate"] = bl.ConvertToDate(DT);
                        dtRow["Balance"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["Balance"]));
                        dtRow["ColValue"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["NetAmt"]));
                        dtRow["AdjAmt"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["AmtAdj"]));
                        dtRow["FullyAdj"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["Ohtercharges"]));
                        dtRow["DiscPer"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["DiscPern"]));
                        dtRow["DiscAmt"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["DiscAmt"]));
                        dtRow["TranType"] = bl.BL_dValidation(Convert.ToString(dtInvoices.Rows[j]["AssignInvoiceID"]));
                        dtRow["SerialNo"] = (j + 1);
                        dtRow["ReasonID"] = 0;
                        dtDetail.Rows.Add(dtRow);
                    }
                    List<SaveMessage> list = new List<SaveMessage>();
                    string formattedDate = DateTime.Today.ToString("yyyy-MM-dd");
                    string dt = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");//"yyyy-MM-dd"                
                    string chqdate = !string.IsNullOrEmpty(listTrans.Chequedate) ? Convert.ToDateTime(listTrans.Chequedate).ToString("yyyy-MM-dd") : null;
                    try
                    {
                        bl.bl_Transaction(listTrans.CompanyCode, 1);
                        DataTable dtResult = bl.bl_ManageTrans(listTrans.CompanyCode, "uspManageMobileCollection", dtDetail, bl.BL_nValidation(listTrans.TransMode), bl.BL_nValidation(listTrans.ID), dt,
                                                   listTrans.CustomerID, bl.BL_nValidation(listTrans.BeatID), bl.BL_nValidation(listTrans.SalesManID),
                                                   bl.BL_nValidation(listTrans.PaymentmodeID),
                                                   bl.BL_dValidation(listTrans.collectedamt), bl.BL_dValidation(listTrans.AdvAmt),
                                                   listTrans.Chequeno, chqdate,
                                                   listTrans.BankACno, listTrans.BankID, listTrans.ifsc, bl.BL_nValidation(listTrans.CurrentStatus),
                                                   bl.BL_nValidation(listTrans.UserID), listTrans.Remarks, listTrans.Narration, listTrans.BranchID);
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
                            string errmsg = dtResult.Rows[0][0].ToString();
                            list.Add(new SaveMessage()
                            {
                                ID = 0.ToString(),
                                MsgID = "1",
                                Message = errmsg
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        bl.bl_Transaction(listTrans.CompanyCode, 3);
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "1",
                            Message = ex.Message
                        });
                        bl.BL_WriteErrorMsginLog(listTrans.CompanyCode, "Web Collection 1", "Save", ex.Message); return Ok();
                    }
                    return Ok(list);
                }
                return Ok();
            }
            catch(Exception ex)
            {
                bl.BL_WriteErrorMsginLog(listTrans.CompanyCode, "Web Collection 2", "Save", ex.Message); return Ok();
            }
        }
        [HttpGet]
        [Route("api/webcollection/getdata")]
        public IHttpActionResult GetWEBCOLLECTIONData(string CompanyCode, string Mode, string ID, string UserID = "")
        {
            DataTable DDT = new DataTable();
            if (Mode == "1")
            {
                DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetSetWebCollectionData", Mode);
                List<CustomerVendorModel> list = new List<CustomerVendorModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CustomerVendorModel
                    {
                        FType = DDT.Rows[i][0].ToString(),
                        Form = DDT.Rows[i][1].ToString(),
                        ID = DDT.Rows[i][2].ToString(),
                        Code = DDT.Rows[i][3].ToString(),
                        Name = DDT.Rows[i][4].ToString(),
                    });
                }
                return Ok(list);
            }
            if (Mode == "4")
            {
                string[] Ids = ID.Split(',');
                for (int i = 0; i < Ids.Length; i++)
                {
                    DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspGetSetWebCollectionData", Mode, Ids[i]);
                }
            }
            if (Mode == "5")
            {
                List<SaveMessage> list = new List<SaveMessage>();
                DataTable dtDenominationPMDetail = new DataTable(); DataTable dtMopDetails = new DataTable("MOP"), dtDetail = new DataTable("CollectionDetail"), dtHeader = new DataTable("CollectionHeader");
                dtDenominationPMDetail.Columns.Add("ColDetailDid", typeof(int));
                dtDenominationPMDetail.Columns.Add("ColDetailDenomination", typeof(int));
                dtDenominationPMDetail.Columns.Add("ColtotCoupons", typeof(int));
                dtDenominationPMDetail.Columns.Add("ColDetailCount", typeof(string));
                dtDenominationPMDetail.Columns.Add("ColDetailAmount", typeof(decimal));
                bl.BL_AddCollectionData(dtHeader, dtDetail, dtMopDetails);
                DataTable dtAdjRefId = new DataTable(), dtTVPTable = new DataTable();
                List<int> CollectionIDs = ID.Split(',').Select(int.Parse).OrderBy(n => n).ToList();
                for (int i = 0; i < CollectionIDs.Count; i++)
                {
                    int CollectionID = Convert.ToInt32(CollectionIDs[i]);
                    DataTable dtColHeader = bl.BL_ExecuteParamSP(CompanyCode, "uspGetSetWebCollectionData", 5, CollectionID);

                    //DDT = bl.BL_ExecuteParamSP("uspGetSetWebCollectionData", Mode, CollectionIDs[i]);
                    int nPaymentMode = bl.BL_nValidation(dtColHeader.Rows[0]["PaymentID"]);
                    decimal dAmt = bl.BL_dValidation(dtColHeader.Rows[0]["Amt"]);
                    dtHeader.Rows.Clear();
                    DataRow CustRow = dtHeader.NewRow();
                    CustRow["Date"] = dtColHeader.Rows[0]["DocDate"];
                    CustRow["CoLLPYType"] = 0;
                    CustRow["AccID"] = dtColHeader.Rows[0]["FAID"];
                    CustRow["ColAmt"] = dAmt;
                    CustRow["Balance"] = bl.BL_dValidation(0);
                    CustRow["DocRefNo"] = "Web Collection";
                    CustRow["ColMode"] = nPaymentMode;
                    CustRow["Status"] = 1;
                    CustRow["ExAccId"] = 0;
                    CustRow["UID"] = UserID;
                    CustRow["Type"] = 0;
                    CustRow["SerialNo"] = 1;
                    CustRow["VisaPern"] = bl.BL_dValidation(0);
                    CustRow["VisaAmt"] = bl.BL_dValidation(0);
                    dtHeader.Rows.Add(CustRow);

                    dtMopDetails.Rows.Clear();

                    DataRow MopRow = dtMopDetails.NewRow();
                    MopRow["AccID"] = dtColHeader.Rows[0]["FAID"];

                    MopRow["Mode"] = nPaymentMode;

                    if (nPaymentMode == 2 || nPaymentMode == 3)
                    {
                        MopRow["[Cheque/DD Number]"] = (dtColHeader.Rows[0]["ChequeNo"]);
                    }
                    if (nPaymentMode == 4 || nPaymentMode == 5)
                    {
                        MopRow["Neft"] = (dtColHeader.Rows[0]["ChequeNo"]);
                    }
                    if (nPaymentMode == 2 || nPaymentMode == 3 || nPaymentMode == 4)
                    {
                        MopRow["Date"] = dtColHeader.Rows[0]["ChequeDate"];
                    }
                    else
                    {
                        MopRow["Date"] = dtColHeader.Rows[0]["DocDate"];
                    }
                    MopRow["BankAccId"] = bl.BL_nValidation(dtColHeader.Rows[0]["BankAccountID"]);
                    MopRow["Amt"] = dAmt;
                    MopRow["IFSC"] = dtColHeader.Rows[0]["IFSCcode"];
                    MopRow["Bank"] = bl.BL_nValidation(dtColHeader.Rows[0]["BankID"]);
                    MopRow["Branch"] = dtColHeader.Rows[0]["BranchName"];
                    MopRow["PayAt"] = null;
                    MopRow["BankAccNo"] = dtColHeader.Rows[0]["BankAcNo"];
                    MopRow["ChequeBkRefNo"] = "";
                    MopRow["ChequeBookID"] = 0;
                    dtMopDetails.Rows.Add(MopRow);
                    dtDetail.Rows.Clear();
                    DataTable dtColDetails = bl.BL_ExecuteParamSP(CompanyCode, "uspGetSetWebCollectionData", 6, CollectionID);
                    for (int J = 0; J < dtColDetails.Rows.Count; J++)
                    {
                        DataRow InvRow = dtDetail.NewRow();
                        InvRow["AccID"] = bl.BL_nValidation(dtColHeader.Rows[0]["FAID"]);
                        InvRow["DocPrefix"] = bl.BL_nValidation(dtColDetails.Rows[J]["InvDocPrefix"]);
                        InvRow["DocValue"] = bl.BL_nValidation(dtColDetails.Rows[J]["DocValue"]);
                        InvRow["DocID"] = bl.BL_nValidation(dtColDetails.Rows[J]["InvoiceID"]);
                        InvRow["DocDate"] = Convert.ToString(dtColDetails.Rows[J]["InvDate"]);// DateTime.ParseExact(Convert.ToString(dtColDetails.Rows[J]["InvDate"]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        InvRow["Balance"] = bl.BL_dValidation(Convert.ToString(dtColDetails.Rows[J]["InvBalance"]));
                        InvRow["ColValue"] = bl.BL_dValidation(Convert.ToString(dtColDetails.Rows[J]["AdjAmt"]));
                        InvRow["AdjAmt"] = bl.BL_dValidation(Convert.ToString(dtColDetails.Rows[J]["WriteOff"]));
                        InvRow["DiscPer"] = 0;
                        InvRow["DiscAmt"] = 0;
                        decimal dbal = bl.BL_dValidation(dtColDetails.Rows[J]["InvBalance"]);
                        decimal dCollValue = bl.BL_dValidation(dtColDetails.Rows[J]["AdjAmt"]);
                        decimal dAdjAmt = bl.BL_dValidation(dtColDetails.Rows[J]["WriteOff"]);
                        int nFullyAdj = (dbal == (dCollValue + (dAdjAmt < 0 ? 0 : dAdjAmt))) ? 1 : 0;
                        InvRow["FullyAdj"] = nFullyAdj;
                        InvRow["FullyAdjAmt"] = 0;
                        InvRow["TotalAmtAdj"] = bl.BL_dValidation(dtColDetails.Rows[J]["TotAdjAmt"]);
                        InvRow["TranType"] = 1;
                        InvRow["SerialNo"] = 1;
                        dtDetail.Rows.Add(InvRow);
                    }
                    int nBeatID = bl.BL_nValidation(dtColHeader.Rows[0]["BeatID"]);
                    int nSMID = bl.BL_nValidation(dtColHeader.Rows[0]["SalesmanID"]);
                    bl.bl_Transaction(CompanyCode, 1);
                    DataTable dtResult = new DataTable();
                    dtResult = bl.bl_ManageTrans(CompanyCode, "uspManageFullColl",
                        19, bl.BL_nValidation(0), dtHeader, dtDetail, dtMopDetails,
                        0,
                        nBeatID,
                        nSMID,
                        0,
                        dtDenominationPMDetail, 1, 0,
                        1,
                        0, "Web Collection", null);
                    if (dtResult.Columns.Count == 1)
                    {
                        int nScopeInvID = bl.BL_nValidation(dtResult.Rows[0][0].ToString());

                        bl.bl_Transaction(CompanyCode,2);
                        bl.BL_ExecuteParamSP(CompanyCode, "uspGetSetWebCollectionData", 7, CollectionID, nScopeInvID);
                        list.Add(new SaveMessage()
                        {
                            ID = nScopeInvID.ToString(),
                            MsgID = "0",
                            Message = "Saved Successfully"
                        });
                        return Ok(list);
                    }
                    else
                    {
                        bl.bl_Transaction(CompanyCode,3);
                        string ErrMsg = "";
                        string[] strErrorList = dtResult.Rows[0][0].ToString().Split('$');
                        if (strErrorList.Length == 1)
                        {
                            if (strErrorList[0].Trim().ToUpper() == "PAYMENTSTATUS")
                            {
                                ErrMsg = "Payment mode status changed";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "ACC")
                            {
                                ErrMsg = "Account name already deactivated";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "CASH")
                            {
                                ErrMsg = "You don't have enough amount in account";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "BANKACC")
                            {
                                ErrMsg = "Bank Account already deactivated";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "BALANCE")
                            {
                                ErrMsg = "You don't have enough amount in account";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "CHEQUE")
                            {
                                ErrMsg = "Cheque book permission changed";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "CHEQUESTATUS")
                            {
                                ErrMsg = "Cheque book status already changed";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "DOCUMENTSTATUS")
                            {
                                ErrMsg = "This document already processed";
                            }
                        }
                        else
                        {
                            int nDocPrefix = bl.BL_nValidation(strErrorList[1]);
                            int nDocIdent = bl.BL_nValidation(strErrorList[2]);
                            if (strErrorList[0].Trim().ToUpper() == "DOCUMENTAMOUNT")
                            {
                                ErrMsg = "Document amount was changed";
                            }
                            if (strErrorList[0].Trim().ToUpper() == "DOCUMENTSTATUS")
                            {
                                ErrMsg = "This document already processed";
                            }
                        }
                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "0",
                            Message = ErrMsg
                        });
                        return Ok(list);
                    }
                }
            }
            if (Mode == "8")
            {
                List<adjDocs> list = new List<adjDocs>();
                DDT = bl.BL_ExecuteParamSP("uspGetSetWebCollectionData", Mode, ID);
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new adjDocs
                    {
                        Docid = DDT.Rows[i][0].ToString(),
                        Docdate = DDT.Rows[i][1].ToString(),
                        Refno = DDT.Rows[i][2].ToString(),
                        NetAmt = DDT.Rows[i][3].ToString(),
                        Amtadj = DDT.Rows[i][4].ToString(),
                        Ohtercharges = DDT.Rows[i][5].ToString(),
                    });
                }
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/webcollection/filterdata")]
        public IHttpActionResult GetwebcollectionFilter(string CompanyCode, string Mode, string ID, string SalesmanID, string CustomerID, string PayModeID, string ChequeDate, string AllowDate)
        {
            if (Mode == "2")
            {
                DataTable DDT = bl.BL_ExecuteParamSP("uspGetSetWebCollectionData", Mode, ID, SalesmanID, CustomerID, PayModeID, ChequeDate, AllowDate);
                List<CollectionModel> list = new List<CollectionModel>();
                string Cash = "0.00 / 0", Cheque = "0.00 / 0", Bank = "0.00 / 0";
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    if (DDT.Rows[i][0].ToString() == "1")
                    {
                        Cash = DDT.Rows[i][2].ToString() + " / " + DDT.Rows[i][1].ToString();
                    }
                    if (DDT.Rows[i][0].ToString() == "2")
                    {
                        Cheque = DDT.Rows[i][2].ToString() + " / " + DDT.Rows[i][1].ToString();
                    }
                    if (DDT.Rows[i][0].ToString() == "4")
                    {
                        Bank = DDT.Rows[i][2].ToString() + " / " + DDT.Rows[i][1].ToString();
                    }
                }
                list.Add(new CollectionModel
                {
                    CashValue = Cash,
                    ChequeValue = Cheque,
                    BankTransferValue = Bank
                });
                return Ok(list);
            }
            if (Mode == "3")
            {
                DataTable DDT = bl.BL_ExecuteParamSP("uspGetSetWebCollectionData", Mode, ID, SalesmanID, CustomerID, PayModeID, ChequeDate, AllowDate);
                List<CollectionModel> list = new List<CollectionModel>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new CollectionModel
                    {
                        ID = DDT.Rows[i][0].ToString(),
                        Date = DDT.Rows[i][1].ToString(),
                        BeatName = DDT.Rows[i][2].ToString(),
                        SalesManName = DDT.Rows[i][3].ToString(),
                        CustomerName = DDT.Rows[i][4].ToString(),
                        Paymentmode = DDT.Rows[i][5].ToString(),
                        collectedamt = DDT.Rows[i][6].ToString(),
                        Chequedate = DDT.Rows[i][7].ToString(),
                        Chequeno = DDT.Rows[i][8].ToString(),
                    });
                }
                string str = "";

                var data = from users in list
                           select
                               new
                               {
                                   ID = users.ID,
                                   Date = users.Date,
                                   BeatName = users.BeatName,
                                   SalesManName = users.SalesManName,
                                   CustomerName = users.CustomerName,
                                   Paymentmode = users.Paymentmode,
                                   collectedamt = users.collectedamt,
                                   Chequedate = users.Chequedate,
                                   Chequeno = users.Chequeno,
                               };

                return Ok(data);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/draftinvoice/filterdata")]
        public IHttpActionResult GetdraftinvoiceFilter(string CompanyCode, string Mode,string ID = "0")
        {            
            if (Mode == "1")
            {
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode,"uspgetsetMobileDraftInvoice", Mode);
                List<DraftInvoices> list = new List<DraftInvoices>();
                for (int i = 0; i < DDT.Rows.Count; i++)
                {
                    list.Add(new DraftInvoices
                    {
                        ID = DDT.Rows[i][0].ToString(),
                        DocID = DDT.Rows[i][1].ToString(),
                        DocDate = DDT.Rows[i][2].ToString(),
                        Beat = DDT.Rows[i][3].ToString(),
                        Salesman = DDT.Rows[i][4].ToString(),
                        Customer = DDT.Rows[i][5].ToString(),
                        Amount = DDT.Rows[i][6].ToString(),
                        Branch = DDT.Rows[i][7].ToString(),
                        CBy = DDT.Rows[i][8].ToString(),
                        CDate = DDT.Rows[i][9].ToString(),
                    });
                }
                string str = "";

                var data = from users in list
                           select
                               new
                               {
                                   ID = users.ID,
                                   DocID = users.DocID,
                                   DocDate = users.DocDate,
                                   Beat = users.Beat,
                                   Salesman = users.Salesman,
                                   Customer = users.Customer,
                                   Amount = users.Amount,
                                   Branch = users.Branch,
                                   CBy = users.CBy,
                                   CDate = users.CDate,
                               };
                return Ok(data);
            }
            if(Mode == "2")
            {
                List<DailyActivity> list = new List<DailyActivity>();
                List<DailyActivityDetails> listDetail = new List<DailyActivityDetails>();
                DataTable DDT = bl.BL_ExecuteParamSP(CompanyCode, "uspgetsetMobileDraftInvoice", Mode,ID);
                if(DDT.Rows.Count > 0)
                {
                    DataTable dtDetail = bl.BL_ExecuteParamSP(CompanyCode, "uspgetsetMobileDraftInvoice", 3, ID);
                    for (int i = 0; i < dtDetail.Rows.Count; i++)
                    {
                        listDetail.Add(new DailyActivityDetails
                        {
                            ID = dtDetail.Rows[i]["ProdID"].ToString(),
                            Name = dtDetail.Rows[i]["ProductName"].ToString(),
                            PriceDesc = dtDetail.Rows[i]["PriceInfo"].ToString(),
                            Rate = dtDetail.Rows[i]["OrgPrice"].ToString(),
                            Discount = dtDetail.Rows[i]["DisPern"].ToString(),
                            MRP = dtDetail.Rows[i]["MRP"].ToString(),
                            AppPrice = dtDetail.Rows[i]["Price"].ToString(),
                            Qty = dtDetail.Rows[i]["Qty"].ToString(),
                        });
                    }

                    list.Add(new DailyActivity
                    {
                        ID = DDT.Rows[0][0].ToString(),
                        BranchID = DDT.Rows[0][1].ToString(),
                        BeatID = DDT.Rows[0][2].ToString(),
                        SalesManID = DDT.Rows[0][3].ToString(),
                        CustomerID = DDT.Rows[0][4].ToString(),
                        CustomerName = DDT.Rows[0][5].ToString(),
                        AddnlDisc = DDT.Rows[0][6].ToString(),
                        TrdDisc = DDT.Rows[0][7].ToString(),
                        Narration = DDT.Rows[0]["Narration"].ToString(),
                        lstProdDetails = listDetail
                    });
                }
                return Ok(list);
            }
            return Ok();
        }
        [HttpPost]
        [Route("api/draftinvoice/save")]
        public IHttpActionResult Savedraftinvoice(DailyActivity listTrans)
        {
            if (listTrans != null)
            {
                DataTable dtProd = new DataTable();
                if (dtProd.Columns.Count == 0)
                {
                    dtProd.Columns.Add("ProdId", typeof(int));
                    dtProd.Columns.Add("InventoryYesNo", typeof(int));
                    dtProd.Columns.Add("BatchYesNo", typeof(int));
                    dtProd.Columns.Add("PKDYesNo", typeof(int));
                    dtProd.Columns.Add("SerialYesNo", typeof(int));
                    dtProd.Columns.Add("BaseUomPrice", typeof(decimal));
                    dtProd.Columns.Add("UomId", typeof(int));
                    dtProd.Columns.Add("UomQty", typeof(decimal));
                    dtProd.Columns.Add("UomPrice", typeof(decimal));
                    dtProd.Columns.Add("GoodsAmt", typeof(decimal), "(UomQty*UomPrice)");
                    dtProd.Columns.Add("UserDisc", typeof(decimal));
                    dtProd.Columns.Add("UserDiscAmt", typeof(decimal));
                    dtProd.Columns.Add("ProdDisc", typeof(decimal));
                    dtProd.Columns.Add("ProdDiscAmt", typeof(decimal), "(ProdDisc * (UomQty*UomPrice)) / 100");
                    dtProd.Columns.Add("TradeDiscPern", typeof(decimal));
                    dtProd.Columns.Add("TradeDisc", typeof(decimal), "(TradeDiscPern * ((UomQty*UomPrice) - ProdDiscAmt) / 100)");
                    dtProd.Columns.Add("AddnlDiscPern", typeof(decimal));
                    dtProd.Columns.Add("AddnlDisc", typeof(decimal), "(AddnlDiscPern * ((UomQty*UomPrice) - ProdDiscAmt) / 100)");
                    dtProd.Columns.Add("GrossAmt", typeof(decimal), "(UomQty*UomPrice) - (ProdDiscAmt + TradeDisc + AddnlDisc)");
                    dtProd.Columns.Add("TaxId", typeof(int));
                    dtProd.Columns.Add("TaxPercentage", typeof(decimal));
                    dtProd.Columns.Add("TaxAmt", typeof(decimal), "(GrossAmt * TaxPercentage) / 100");
                    dtProd.Columns.Add("NetAmt", typeof(decimal), "GrossAmt + TaxAmt");
                    dtProd.Columns.Add("ReasonId", typeof(int));
                    dtProd.Columns.Add("Serial", typeof(int)); 
                    dtProd.Columns.Add("BatchNumber", typeof(string));
                    dtProd.Columns.Add("PkgDate", typeof(string));
                    dtProd.Columns.Add("ExpiryDate", typeof(string));
                    dtProd.Columns.Add("InventoryPrice", typeof(decimal));
                    dtProd.Columns.Add("MRP", typeof(decimal));
                    dtProd.Columns.Add("InvQtyType", typeof(int));
                    dtProd.Columns.Add("TempBatchInvId", typeof(int));
                    dtProd.Columns.Add("UomCR", typeof(decimal));
                    dtProd.Columns.Add("DiffAmt", typeof(decimal));
                }
                DataTable dtTempBachInfo = new DataTable();
                DataColumn column = new DataColumn("Serial");
                column.DataType = System.Type.GetType("System.Int32");
                column.AutoIncrement = true;
                column.AutoIncrementSeed = 1;
                column.AutoIncrementStep = 1;
                dtTempBachInfo.Columns.Add(column);
                dtTempBachInfo.Columns.Add("ProdId", typeof(int));
                dtTempBachInfo.Columns.Add("Batch", typeof(string));
                dtTempBachInfo.Columns.Add("PKD", typeof(string));
                dtTempBachInfo.Columns.Add("Expiry", typeof(string));
                dtTempBachInfo.Columns.Add("PPrice", typeof(decimal));
                dtTempBachInfo.Columns.Add("SPrice", typeof(decimal));
                dtTempBachInfo.Columns.Add("ECP", typeof(decimal));
                dtTempBachInfo.Columns.Add("MRP", typeof(decimal));
                dtTempBachInfo.Columns.Add("SPLPrice", typeof(decimal));
                dtTempBachInfo.Columns.Add("ReturnPrice", typeof(decimal));
                dtTempBachInfo.Columns.Add("TaxId", typeof(int));
                dtTempBachInfo.Columns.Add("TaxTypeId", typeof(int));
                dtTempBachInfo.Columns.Add("InclusiveYesNo", typeof(int));
                dtTempBachInfo.Columns.Add("BatchType", typeof(int));
                dtTempBachInfo.Columns.Add("HiddenRowID", typeof(int));
                DataTable dtSerialInfo = new DataTable();
                dtSerialInfo.Columns.Add("Index", typeof(int));
                dtSerialInfo.Columns.Add("ProdId", typeof(int));
                dtSerialInfo.Columns.Add("Serial", typeof(string));
                DataTable dtDocument = new DataTable();
                dtDocument.Columns.Add("TransName");
                dtDocument.Columns.Add("Status", typeof(int)).DefaultValue = 0;
                dtDocument.Columns.Add("DocumentId", typeof(int)).DefaultValue = 0;
                DataTable dtProducts = bl.ConvertListToDataTable(listTrans.lstProdDetails);
                int nSerial = 1;
                string formattedDate = DateTime.Today.ToString("yyyy-MM-dd");
                string dt = formattedDate;// Convert.ToDateTime(formattedDate).ToString();//"yyyy-MM-dd"
                for (int i = 0; i < dtProducts.Rows.Count; i++)
                {
                    int nProdID = bl.BL_nValidation(Convert.ToString(dtProducts.Rows[i]["ID"]));
                    if (nProdID > 0)
                    {
                        DataTable dtItem = bl.BL_ExecuteParamSP(listTrans.CompanyCode, "uspgetsetMobileDraftInvoice", 4, nProdID);
                        DataTable dtItemTransPrices = bl.BL_ExecuteParamSP(listTrans.CompanyCode, "uspgetsetMobileDraftInvoice", 7, nProdID);
                        //DataTable getConvFact = bl.BL_ExecuteSqlQuery("select dbo.fnGetConvertionFact(" + bl.BL_nValidation(dgvProd.Rows[DetailCount].Cells[UomGrpID.Name].Value) + "," + bl.BL_nValidation(dgvProd.Rows[DetailCount].Cells[UomID.Name].Value) + ")");
                        decimal dUomTax = 0;// bl.GetUOMTaxValue(bl.BL_nValidation(iRow["TaxID"]), bl.BL_nValidation(txtTaxType.Tag),
                                            //(bl.BL_dValidation(iRow["Qty"]) + bl.BL_dValidation(iRow["DmgQty"])) * (getConvFact.Rows.Count > 0 ? bl.BL_dValidation(getConvFact.Rows[0][0].ToString()) : 0.00M));// bl.BL_dValidation(dgvProd.Rows[DetailCount].Cells[SelectedUomCF.Name].Value));
                        decimal dDiffPrice = dtItemTransPrices.Rows.Count > 0 ?
       bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Rate"])) - bl.BL_dValidation(dtItemTransPrices.Rows[0]["InvoicePrice"]) : 0;

                        #region Discount Schemme
                        DataTable dtDiscScheme = bl.BL_ExecuteParamSP(listTrans.CompanyCode, "uspGetCustWiseProdDisc", dt, listTrans.CustomerID, 0);
                        string SchemeApply = dtDiscScheme.Rows.Count > 0 ? "1" : "0";
                        decimal dConvFact = bl.BL_dValidation(dtItem.Rows[0]["BaseCR"]);
                        decimal ApplyPrice = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["AppPrice"]));
                        dtDiscScheme = new DataTable();
                        if (SchemeApply == "1")//Go when scheme applied for selected Customer
                            dtDiscScheme = bl.BL_ExecuteParamSP(listTrans.CompanyCode, "uspGetCustWiseProdDisc", dt, listTrans.CustomerID, nProdID);
                        decimal OrgDiscPern = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Discount"]));
                        decimal OrgTradeDiscPern = bl.BL_dValidation(listTrans.TrdDisc);
                        decimal OldDiscPern = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Discount"]));
                        decimal DSProdDiscPern = 0, DSProdDiscAmt = 0, DSTradeDiscPern = 0, DSTradeDiscAmt = 0;
                        if (dtDiscScheme.Rows.Count > 0)
                        {
                            DSProdDiscPern = bl.BL_dValidation(dtDiscScheme.Rows[0][2]);
                            DSProdDiscAmt = bl.BL_dValidation(dtDiscScheme.Rows[0][3]) * dConvFact;
                            DSTradeDiscPern = bl.BL_dValidation(dtDiscScheme.Rows[0][4]);
                            DSTradeDiscAmt = bl.BL_dValidation(dtDiscScheme.Rows[0][5]) * dConvFact;
                            int ReplaceExists = bl.BL_nValidation(dtDiscScheme.Rows[0][1]);

                            decimal PDiscAmt = 0, dTradPernfromAmt = 0, dProdPernfromAmt = 0;
                            if (ReplaceExists == 1)//Replay exists
                            {
                                PDiscAmt = (ApplyPrice * DSProdDiscPern) / 100;
                            }
                            else
                            {
                                PDiscAmt = (ApplyPrice * (DSProdDiscPern + OldDiscPern)) / 100;
                            }
                            if (DSTradeDiscAmt > 0)
                            {
                                if (ApplyPrice > 0)
                                    dTradPernfromAmt = bl.BL_dValidation((DSTradeDiscAmt / (ApplyPrice - PDiscAmt - DSProdDiscAmt)) * 100);
                                else
                                    dTradPernfromAmt = 0;
                            }
                            if (DSProdDiscAmt > 0)
                            {
                                if (ApplyPrice > 0)
                                    dProdPernfromAmt = bl.BL_dValidation((DSProdDiscAmt / ApplyPrice) * 100);
                                else
                                    dProdPernfromAmt = 0;
                            }
                            if (ReplaceExists == 1)//Replay exists
                            {
                                OrgDiscPern = DSProdDiscPern;
                                OrgTradeDiscPern = DSTradeDiscPern + dTradPernfromAmt;
                            }
                            else
                            {
                                OrgDiscPern = dProdPernfromAmt + DSProdDiscPern + OldDiscPern;
                                OrgTradeDiscPern = DSTradeDiscPern + dTradPernfromAmt;
                            }
                        }
                        #endregion

                        DataRow dtRow = dtProd.NewRow();
                        dtRow["ProdId"] = nProdID;
                        dtRow["InventoryYesNo"] = dtItem.Rows[0]["TrackInventory"];
                        dtRow["BatchYesNo"] = dtItem.Rows[0]["TrackBatch"];
                        dtRow["PKDYesNo"] = dtItem.Rows[0]["TrackPDK"];
                        dtRow["SerialYesNo"] = dtItem.Rows[0]["TrackSerial"];
                        dtRow["BaseUomPrice"] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Rate"]));
                        dtRow["UomId"] = bl.BL_nValidation(Convert.ToString(dtItem.Rows[0]["BaseUomID"]));
                        dtRow["UomQty"] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Qty"]));
                        dtRow["UomPrice"] = ApplyPrice;
                        dtRow["UserDisc"] = 0;
                        dtRow["UserDiscAmt"] = 0;
                        dtRow["ProdDisc"] = OrgDiscPern;// bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Discount"]));                        
                        dtRow["TradeDiscPern"] = OrgTradeDiscPern;// bl.BL_dValidation(listTrans.TrdDisc);                        
                        dtRow["AddnlDiscPern"] = bl.BL_dValidation(listTrans.AddnlDisc);                        
                        dtRow["TaxId"] = bl.BL_nValidation(dtItem.Rows[0]["SalesTaxID"]);
                        dtRow["TaxPercentage"] = bl.BL_dValidation(Convert.ToString(dtItem.Rows[0]["GST"]));
                        //dtRow["TaxAmt"] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["TaxAmt"]));
                        //dtRow["NetAmt"] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["NetAmt"]));
                        dtRow["ReasonId"] = 0;
                        dtRow["Serial"] = nSerial;
                        dtRow["BatchNumber"] = null;
                        dtRow["PkgDate"] = null;
                        dtRow["ExpiryDate"] = null;
                        dtRow["InventoryPrice"] = bl.BL_dValidation(Convert.ToString(dtProducts.Rows[i]["Rate"]));
                        dtRow["MRP"] = bl.BL_dValidation(Convert.ToString(dtItem.Rows[0]["MRP"]));
                        dtRow["UomCR"] = bl.BL_dValidation(dtItem.Rows[0]["BaseCR"]);
                        dtRow["InvQtyType"] = 1;
                        dtRow["TempBatchInvId"] = 0;
                        dtRow["DiffAmt"] = dDiffPrice;
                        dtProd.Rows.Add(dtRow);
                        nSerial++;
                    }
                }
                DataTable dtParty = bl.BL_ExecuteParamSP(listTrans.CompanyCode, "uspgetsetMobileDraftInvoice", 5, listTrans.CustomerID);
                List<SaveMessage> list = new List<SaveMessage>();
               
                decimal totalProdDiscAmt = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("ProdDiscAmt"));
                decimal totalAddnlDiscAmt = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("TradeDisc"));
                decimal totalTrdDiscAmt = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("AddnlDisc"));
                decimal TotalDiscount = totalProdDiscAmt + totalAddnlDiscAmt + totalTrdDiscAmt;
                decimal Gross = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("GrossAmt")); 
                decimal Tax = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("TaxAmt"));
                decimal Net = dtProd.AsEnumerable().Sum(row => row.Field<decimal>("NetAmt"));
                decimal totalNetAmt = Math.Round(Net, 0, MidpointRounding.AwayFromZero); 
                decimal Roundoff = totalNetAmt - Net;
                bl.bl_Transaction(listTrans.CompanyCode, 1);
                DataTable dtResult = bl.bl_ManageTrans(listTrans.CompanyCode, "uspManageSalesDraftHeader", 1, bl.BL_nValidation(listTrans.UserID),
                                 15, 0, dt, dt, listTrans.BeatID, listTrans.SalesManID,
                                 listTrans.BranchID, listTrans.CustomerID, bl.BL_nValidation(dtParty.Rows[0]["PriceTypeID"]), 
                                 bl.BL_nValidation(dtParty.Rows[0]["TaxTypeID"]), bl.BL_nValidation(dtParty.Rows[0]["PaymentModeID"]), 
                                 bl.BL_nValidation(dtParty.Rows[0]["CreditTermID"]),
                                 0, null, 0, 0, 0,
                                 bl.BL_dValidation(Roundoff), 0, 0, bl.BL_dValidation(listTrans.TrdDisc), bl.BL_dValidation(totalTrdDiscAmt),
                                 bl.BL_dValidation(totalProdDiscAmt), bl.BL_dValidation(listTrans.AddnlDisc), bl.BL_dValidation(totalAddnlDiscAmt),
                                 bl.BL_dValidation(Gross), bl.BL_dValidation(Tax), bl.BL_dValidation(TotalDiscount),
                                 bl.BL_dValidation(totalNetAmt), 0, dtDocument, dtProd, dtSerialInfo, dtTempBachInfo, 1, 1, null,
                                 0, 0, 0, 0,null, listTrans.Narration, 0);
                if (dtResult.Rows.Count > 0)
                {
                    if (dtResult.Columns.Count == 1)
                    {
                        int nBillScopeID = bl.BL_nValidation(dtResult.Rows[0][0]);
                        bl.bl_ManageTrans(listTrans.CompanyCode, "uspgetsetMobileDraftInvoice", 6, listTrans.ID, nBillScopeID);
                        bl.bl_Transaction(listTrans.CompanyCode, 2);

                        list.Add(new SaveMessage()
                        {
                            ID = nBillScopeID.ToString(),
                            MsgID = "0",
                            Message = "Saved Successfully"
                        });
                    }
                    else
                    {
                        //int nBillScopeID = bl.BL_nValidation(dtResult.Rows[0][0]);
                        //bl.bl_ManageTrans(listTrans.CompanyCode, "uspgetsetMobileDraftInvoice", 6, listTrans.ID, nBillScopeID);
                        bl.bl_Transaction(listTrans.CompanyCode, 2);

                        list.Add(new SaveMessage()
                        {
                            ID = 0.ToString(),
                            MsgID = "1",
                            Message = dtResult.Rows[0][0].ToString()
                        });
                    }
                }
                else
                {
                    bl.bl_Transaction(listTrans.CompanyCode, 3);
                    list.Add(new SaveMessage()
                    {
                        ID = 0.ToString(),
                        MsgID = "1",
                        Message = "Data not Saved"
                    });
                }
                return Ok(list);
            }
            return Ok();
        }

        [HttpGet]
        [Route("api/salestransdocuments")]
        public IHttpActionResult salestransdocuments(string CompanyCode, string TransID, string UserID, string FromDate,
             string ToDate, string ShowAll, string TypeID)
        {
            try
            {
                DataTable dtMSTdetail = bl.BL_ExecuteParamSP(CompanyCode, "uspMobileDACollSRFilterdata", TransID, UserID, FromDate,
                    ToDate, ShowAll, TypeID);
                return Ok(dtMSTdetail);
            }
            catch (Exception ex)
            {
                bl.BL_WriteErrorMsginLog(CompanyCode, "Common", "masterlist", ex.Message); return Ok();

            }
            return Ok();
        }
    }
}
