using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.Models
{
    public class CustomerVendorModel
    {
        public string CompanyCode { get; set; }
        public string FType { get; set; }
        public string Form { get; set; }
        public string Mode { get; set; }
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerType { get; set; }
        public string Ratings { get; set; }
        public string Billadd1 { get; set; }
        public string Billadd2 { get; set; }
        public string Billadd3 { get; set; }
        public string Shipadd1 { get; set; }
        public string shipadd2 { get; set; }
        public string Shipadd3 { get; set; }
        public string Pincode { get; set; }
        public string ContactPerson { get; set; }
        public string Ph1 { get; set; }
        public string Ph2 { get; set; }
        public string Mob1 { get; set; }
        public string Mob2 { get; set; }
        public string Email { get; set; }
        public string PANNumber { get; set; }
        public string AadharNo { get; set; }
        public string DLNo20 { get; set; }
        public string DLNo21 { get; set; }
        public string FSSAINo { get; set; }
        public string StateID { get; set; }
        public string IfRegister { get; set; }
        public string GSTIN { get; set; }
        public string CreditTermID { get; set; }
        public string PaymentModeID { get; set; }
        public string TaxTypeID { get; set; }
        public string OSValue { get; set; }
        public string FAID { get; set; }
        public string OverDueValue { get; set; }
        public string OverDueInvCount { get; set; }
        public string CreditLimitValue { get; set; }
        public string CreditLimitCount { get; set; }
        public string CreditlimitOS { get; set; }
        public string PriceTypeID { get; set; }
        public string WeekCycle { get; set; }
        public string OwnerName { get; set; }
        public string DiscountPern { get; set; }
        public string TrackPoint { get; set; }
        public string ClosingTrackPoint { get; set; }
        public string TCSTax { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string Distance { get; set; }
        public string Remark { get; set; }
        public string Active { get; set; }
        public string UserID { get; set; }
        public string BeatID { get; set; }
        public string SalesmanID { get; set; }
        //public List<MapBeatSalesman> BSM { get; set; }
        //public List<ProductModel> lstProduct { get; set; }
        public string AIopen { get; set; }
        public string POopen { get; set; }
        public string RemoveCustomer { get; set; }
        //public List<clsCustomerRemarks> lstCustRemark { set; get; }
    }
    public class DailyActivity
    {
        public string ID { get; set; }
        public string TransMode { get; set; }
        public string ReturnTransType { get; set; }
        public string CurrentStatus { get; set; }
        public string CompanyCode { get; set; }
        public string BeatID { get; set; }
        public string BeatName { get; set; }
        public string SalesManID { get; set; }
        public string SalesManName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string BranchID { get; set; }
        public string AddnlDisc { get; set; }
        public string TrdDisc { get; set; }
        public string FeedBack { get; set; }
        public string ActivityID { get; set; }
        public List<DailyActivityDetails> lstProdDetails { get; set; }
        public int Allsalesman { get; set; }
        public string ot_OSValue { get; set; }
        public string Mode { get; set; }
        public string UserID { get; set; }
        public string CusLatitude { get; set; }
        public string CusLongtitude { get; set; }
        public string Pincode { get; set; }
        public string HasPincode { get; set; }
        public string BillAdd { get; set; }
        public string ShipAdd { get; set; }
        public string ContPerson { get; set; }
        public string MobileNo { get; set; }
        public string GSTIN { get; set; }
        public string ot_CustLocLink { get; set; }
        public string ot_MobnoLink { get; set; }
        public string ot_Remark { get; set; }
        public string TransType { get; set; }
        public string TransID { get; set; }
        public string LocGiven { get; set; }
        public string LastOTDate { get; set; }
        public bool EnableProdDiscAmt { get; set; }
        public string TranStartTime { get; set; }
        public string Narration { get; set; }
    }
    public class DailyActivityDetails
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Discount { get; set; }
        public string Qty { get; set; }
        public string Rate { get; set; }
        public string MRP { get; set; }
        public string AppPrice { get; set; }
        public string PriceDesc { get; set; }
        public string _DiscAmt { get; set; }
        public string Imagedata { get; set; }
    }
    public class CollectionModel
    {
        public string CompanyCode { get; set; }
        public string ID { get; set; }
        public string Date { get; set; }
         public string TransMode { get; set; }
        public string RefNo { get; set; }
        public string BeatID { get; set; }
        public string BeatName { get; set; }
        public string SalesManID { get; set; }
        public string SalesManName { get; set; }
        public string CustomerID { get; set; }
        public string BranchID { get; set; }
        public string CustomerName { get; set; }
        public string collectedamt { get; set; }
        public string PaymentmodeID { get; set; }
        public string Paymentmode { get; set; }
        public string Status { get; set; }
        public string Chequedate { get; set; }
        public string Chequeno { get; set; }
        public List<adjDocs> lstadjdocs { get; set; }
        public string AdvAmt { get; set; }
        public string Bankname { get; set; }
        public string BankID { get; set; }
        public string ifsc { get; set; }
        public string BankACno { get; set; }
        public string BankACID { get; set; }
        public int ot_Allsalesman { get; set; }
        public string UserID { get; set; }
        public string CusLatitude { get; set; }
        public string CusLongtitude { get; set; }
        public string OSAmt { get; set; }
        public string CashValue { get; set; }
        public string ChequeValue { get; set; }
        public string BankTransferValue { get; set; }
        public string Jsonlstadjdocs { get; set; }
        public string Remarks { get; set; }
        public string Narration { get; set; }
        public string CurrentStatus { get; set; }
    }
    public class adjDocs
    {
        public string ID { get; set; }
        public string Docprefix { get; set; }
        public string TransName { get; set; }
        public string Docdate { get; set; }
        public string Docid { get; set; }
        public string UDN { get; set; }
        public string Refno { get; set; }
        public string Balance { get; set; }
        public string NetAmt { get; set; }
        public string Amtadj { get; set; }
        public string DiscPern { get; set; }
        public string DiscAmt { get; set; }
        public string Ohtercharges { get; set; }
        public string totAdvAmount { get; set; }
        public string clsBalance { get; set; }
        public string AssignInvoiceID { get; set; }
    }
    public class SaveMessage
    {
        public string ID { get; set; }
        public string MsgID { get; set; }
        public string Message { get; set; }
        public string RowID { get; set; }
    }
    public class HomeDashBoardModel
    {
        public string TotCount { get; set; }
        public string OTCount { get; set; }
        public string CollAmt { get; set; }
        public string CashCollAmt { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class DraftInvoices
    {
        public string ID { get; set; }
        public string DocID { get; set; }
        public string DocDate { get; set; }
        public string Beat { get; set; }
        public string Salesman { get; set; }
        public string Customer { get; set; }
        public string Amount { get; set; }
        public string Branch { get; set; }
        public string CBy { get; set; }
        public string CDate { get; set; }
    }
    public class DeliveryModel
    {
        public string CompanyCode { get; set; }
        public string DeliveryType { get; set; }
        public string ID { get; set; }
        public string BranchID { get; set; }
        public string RecieverName { get; set; }
        public string UserID { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }

    }
}