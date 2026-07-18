using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShineWebMobileAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VanLoadingSlipController : ApiController
    {
        [HttpGet]
        [Route("api/dailyactivity/getdata")]
        public IHttpActionResult GetData(string CompanyCode, string Mode, string ID, string SalesmanID = "")
        {
            return Ok();
        }
    }
}
