using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.DALHelper
{
    public class Connection
    {
        public static string GetConnectionString(string CompanyCode)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[CompanyCode].ConnectionString;
            return connectionString;
        }
    }
}