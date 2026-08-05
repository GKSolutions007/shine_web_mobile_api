using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.Models
{
    public class VLSModel
    {
        public string CompanyCode { get; set; }

        public int ID { get; set; }

        public int SalesmanID { get; set; }

        public int CustomerID { get; set; }
        public int BranchID { get; set; }
        public int SpotSalePriceMode { get; set; }
        public int UserID { get; set; }
        public int Status { get; set; }

        public List<VLSSelectedProducts> ItemData { get; set; } = new List<VLSSelectedProducts>();
    }
    public class VLSSelectedProducts
    {
        public int IdentID { get; set; }
        public int ProdID { get; set; }

        public string ProductName { get; set; }

        public decimal MRP { get; set; }

        public decimal AvailableVLS { get; set; }

        public decimal AdjustQty { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal OrgPrice { get; set; }

        public decimal UOMCR { get; set; }
        public decimal TaxPern { get; set; }
        public decimal TaxID { get; set; }
        public decimal UOMID { get; set; }
        public decimal BaseUOMPrice { get; set; }
    }
}