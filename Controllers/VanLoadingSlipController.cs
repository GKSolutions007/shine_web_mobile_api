using ShineWebMobileAPI.BuisnessLayer;
using ShineWebMobileAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VanLoadingSlipController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();

        [HttpGet]
        [Route("api/vls/initialdata")]
        public IHttpActionResult GetData(string CompanyCode, string ID)
        {
            try
            {
                DataSet dt = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspgetVLSData", 1, ID);
                return Ok(dt);
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
        [HttpGet]
        [Route("api/vls/documentdata")]
        public IHttpActionResult GetData(string CompanyCode, string SalesmanID, string DocNo)
        {
            try
            {
                var Responsedata = new List<object>();
                DataSet dt = bl.BL_ExecuteParamSPDataset(CompanyCode, "uspgetVLSData", 2, SalesmanID, DocNo);
                DataTable dtHeader = dt.Tables[0];
                if (dtHeader.Rows.Count > 0)
                {
                    if (dtHeader.Columns.Count == 3)
                    {
                        Responsedata.Add(new
                        {
                            MessageID = "1",
                            Message = dtHeader.Rows[0][0].ToString()
                        });
                    }
                    else
                    {
                        DataTable dtProduct = dt.Tables[1];

                        var Productdata = new List<object>();
                        for (int i = 0; i < dtProduct.Rows.Count; i++)
                        {
                            string imgdata = null;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[i]["Imagedata"].ToString()))
                            {
                                byte[] photoBytes = (byte[])dtProduct.Rows[i]["Imagedata"];
                                imgdata = "data:image/jpeg;base64," + Convert.ToBase64String(photoBytes);
                                //ProdID ProductName UomQty UomPrice    InclPrice MRP AdjustQty AvlVLS  Imagedata
                            }
                            Productdata.Add(new
                            {
                                IdentID = dtProduct.Rows[i]["IdentID"],
                                ProdID = dtProduct.Rows[i]["ProdID"],
                                ProductName = dtProduct.Rows[i]["ProductName"],
                                UomQty = dtProduct.Rows[i]["UomQty"],
                                UomPrice = dtProduct.Rows[i]["UomPrice"],
                                InclPrice = dtProduct.Rows[i]["InclPrice"],
                                MRP = dtProduct.Rows[i]["MRP"],
                                AdjustQty = dtProduct.Rows[i]["AdjustQty"],
                                AvlVLS = dtProduct.Rows[i]["AvlVLS"],
                                Imagedata = imgdata
                            });

                        }
                        Responsedata.Add(new
                        {
                            MessageID = "0",
                            Message = "Data Fetched",
                            DocData = dtHeader,
                            ItemData = Productdata
                        });
                    }
                }
                else
                {
                    Responsedata.Add(new
                    {
                        MessageID = "1",
                        Message = "No records found for this document"
                    });
                }
                return Ok(Responsedata);
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
        [HttpPost]
        [Route("api/vls/save")]
        public IHttpActionResult Save(VLSModel vlsdata)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (vlsdata != null)
                {
                    DataTable dtItemDetails = new DataTable();

                    dtItemDetails.Columns.Add("IdentID", typeof(int));
                    dtItemDetails.Columns.Add("ProdID", typeof(int));
                    dtItemDetails.Columns.Add("AvailableQty", typeof(decimal));
                    dtItemDetails.Columns.Add("AdjustQty", typeof(decimal));
                    foreach (VLSSelectedProducts item in vlsdata.ItemData)
                    {
                        DataRow dr = dtItemDetails.NewRow();

                        dr["IdentID"] = item.IdentID;
                        dr["ProdID"] = item.ProdID;
                        dr["AvailableQty"] = item.AvailableVLS;
                        dr["AdjustQty"] = item.AdjustQty;

                        dtItemDetails.Rows.Add(dr);
                    }
                    bl.bl_Transaction(vlsdata.CompanyCode, 1);
                    DataTable DDT = bl.bl_ManageTrans(vlsdata.CompanyCode, "uspSaveMobileVLSData", vlsdata.ID,
                            vlsdata.Status, vlsdata.CustomerID, vlsdata.SalesmanID, vlsdata.UserID, dtItemDetails);
                    if (DDT.Columns.Count > 5)
                    {
                        #region GST Posting
                        DataTable dtGSTInfo = new DataTable();
                        dtGSTInfo.Columns.Add("TransID", typeof(int));
                        dtGSTInfo.Columns.Add("TransIdentID", typeof(int));
                        dtGSTInfo.Columns.Add("ProdID", typeof(int));
                        dtGSTInfo.Columns.Add("TaxID", typeof(int));
                        dtGSTInfo.Columns.Add("GSTTaxTypeID", typeof(int));
                        dtGSTInfo.Columns.Add("TaxTypeID", typeof(int));
                        dtGSTInfo.Columns.Add("TaxCompID", typeof(int));
                        dtGSTInfo.Columns.Add("TaxCompPern", typeof(decimal));
                        dtGSTInfo.Columns.Add("TaxCompAmount", typeof(decimal));
                        dtGSTInfo.Columns.Add("GrossAmount", typeof(decimal));
                        dtGSTInfo.Columns.Add("TransSerial", typeof(int));
                        dtGSTInfo.Columns.Add("SerialNo", typeof(int));
                        int nBillScopeID = bl.BL_nValidation(DDT.Rows[0]["InvoiceScopeID"]);
                        if (DDT.Rows.Count > 0)
                        {
                            int nProdID = 0, nTaxID = 0, nTaxTypeID = 0, SRSerial = 1, nTranSerial = 1;
                            decimal dQtnGrossAmount = 0.00M, dQtys = 0.00M;
                            dtGSTInfo.Rows.Clear();
                            for (int nCount = 0; nCount < DDT.Rows.Count; nCount++)
                            {
                                //if (bl.BL_dValidation(DDT.Rows[nCount]["Qty"]) > 0)
                                //{
                                nProdID = bl.BL_nValidation(DDT.Rows[nCount]["ProdId"]);
                                nTaxID = bl.BL_nValidation(DDT.Rows[nCount]["TaxID"]);
                                nTaxTypeID =  bl.BL_nValidation(DDT.Rows[nCount]["TaxTypeID"]);
                                decimal dMRP = bl.BL_dValidation(DDT.Rows[nCount]["MRP"].ToString());
                                DataTable dtMTdetail = bl.bl_ManageTrans(vlsdata.CompanyCode, "uspGetTaxCumulative", nTaxID, nTaxTypeID, 1);
                                decimal dApponMRPCum = dtMTdetail.Select("AppOn = -1")
                              .Select(r => Convert.ToDecimal(r["CumulativeTax"]))
                              .DefaultIfEmpty(0)
                              .Sum();
                                dQtnGrossAmount = bl.BL_dValidation(DDT.Rows[nCount]["GrossAmt"]);

                                //DataTable getConvFact = bl.BL_ExecuteSqlQuery("select dbo.fnGetConvertionFact(" + bl.BL_nValidation(DDT.Rows[nCount]["UomGrpID"]) + "," + bl.BL_nValidation(DDT.Rows[nCount]["UomId"]) + ")");

                                dQtys = (bl.BL_dValidation(DDT.Rows[nCount]["UomQty"])) * 1;// bl.BL_dValidation(dtResult.Rows[0][0]);
                                decimal newgrossamt = dApponMRPCum == 0 ? dQtnGrossAmount : bl.ReturnGrossorMRPTaxAmt(vlsdata.CompanyCode,1, nTaxID, nTaxTypeID, dQtnGrossAmount,
                                      dMRP * dQtys, true);
                                DataTable DDTaxCompInfo = bl.bl_ManageTrans(vlsdata.CompanyCode, "uspGetTaxCompInfo", nTaxID, nTaxTypeID);
                                if (DDTaxCompInfo.Rows.Count > 0)
                                {
                                    bool ValidtoCalc = false;

                                    for (int nTaxComp = 0; nTaxComp < DDTaxCompInfo.Rows.Count; nTaxComp++)
                                    {
                                        ValidtoCalc = true;
                                        //nTaxTypeID == 1 && bl.BL_nValidation(DDTaxCompInfo.Rows[nTaxComp][1]) == 1 ||
                                        //       nTaxTypeID == 2 && bl.BL_nValidation(DDTaxCompInfo.Rows[nTaxComp][1]) == 2 ? false : true;
                                        DataRow dr = dtGSTInfo.NewRow();
                                        dr["TransID"] = 15;
                                        dr["TransIdentID"] = nBillScopeID;
                                        dr["ProdID"] = nProdID;
                                        dr["TaxID"] = nTaxID;
                                        dr["GSTTaxTypeID"] = bl.BL_nValidation(DDTaxCompInfo.Rows[nTaxComp][1]);
                                        dr["TaxTypeID"] = nTaxTypeID;
                                        dr["TaxCompID"] = bl.BL_nValidation(DDTaxCompInfo.Rows[nTaxComp][0]);
                                        dr["TaxCompPern"] = bl.BL_dValidation(DDTaxCompInfo.Rows[nTaxComp][2]);
                                        dr["TaxCompAmount"] = ValidtoCalc ? ((newgrossamt * bl.BL_dValidation(DDTaxCompInfo.Rows[nTaxComp][2])) / 100) :
                                                bl.BL_dValidation(DDTaxCompInfo.Rows[nTaxComp][2]) * dQtys;//dQtnGrossAmount
                                        dr["GrossAmount"] = newgrossamt;// dQtnGrossAmount;
                                                                        //dr["TransSerial"] = nTranSerial;
                                        dr["TransSerial"] = (nCount + 1);
                                        dr["SerialNo"] = SRSerial;
                                        dtGSTInfo.Rows.Add(dr);
                                        SRSerial++;
                                    }
                                    nTranSerial++;
                                }
                                //}
                            }
                            if (dtGSTInfo.Rows.Count > 0)
                            {
                                bl.bl_ManageTrans(vlsdata.CompanyCode, "uspSaveTranGSTInfo", dtGSTInfo);
                            }
                        }
                        #endregion
                        bl.bl_Transaction(vlsdata.CompanyCode, 2);
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
                        bl.bl_Transaction(vlsdata.CompanyCode, 3);
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
