using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.Models
{
    public class Sales
    {
        public class AssignInvoiceHeader
        {
            public string CompanyCode { get; set; }
            public string ID { get; set; }
            public string Mode { get; set; }
            public string Date { get; set; }
            public string BranchID { get; set; }
            public string DocID { get; set; }
            public string TransID { get; set; }
            public string TransMode { get; set; }
            public string TransName { get; set; }
            public string SalesmanID { get; set; }
            public string RefNo { get; set; }
            public string Status { get; set; }
            public string DetailStatusCount { get; set; }
            public string CollInvCount { get; set; }
            public string CurrentStatus { get; set; }
            public string TotalInvCount { get; set; }
            public string CBy { get; set; }
            public string CDate { get; set; }
            public string StatusID { get; set; }
            public string UDFId { get; set; }
            public string lstJsonAssignDetails { get; set; }
            public List<AssignInvoiceDetails> lstAssignDetails { get; set; }
            public string Remarks { get; set; }
            public string Narration { get; set; }
        }
        public class AssignInvoiceDetails
        {
            public string ID { get; set; }
            public string DocDate { get; set; }
            public string DocId { get; set; }
            public string Customer { get; set; }
            public string Beat { get; set; }
            public string Salesman { get; set; }
            public string RefNo { get; set; }
            public string NetAmt { get; set; }
            public string Balance { get; set; }
            public string Ageing { get; set; }
            public string AssignedInvoiceCount { get; set; }
            public string Status { get; set; }
        }
    }
}