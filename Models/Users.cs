using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.Models
{
    public class Users
    {
        public string Mode { get; set; }
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mobilenumber { get; set; }
        public string EMailID { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string PwdResetCount { get; set; }
        public string PwdResetTime { get; set; }
        public string LPin { get; set; }
        public string Active { get; set; }
        public string UserID { get; set; }
        public string BeatID { get; set; }
        public string SalesmanID { get; set; }
        public string BranchID { get; set; }

    }
}