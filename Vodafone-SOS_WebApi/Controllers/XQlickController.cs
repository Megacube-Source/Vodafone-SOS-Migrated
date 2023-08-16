using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class XQlickController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        public IHttpActionResult GetXQlickByRole(string LoggedInRoleName,int CompanyId)
        {
            var qry = "select XURL from {Schema}.XQlik where XRole='"+LoggedInRoleName+"'";
            var CommPeriod = db.LCommissionPeriods.Where(p => p.LcpCompanyId == CompanyId).Where(p => p.LcpDatabaseId.HasValue).Where(p => (DateTime.UtcNow >= p.LcpEffectiveStartDate) && (DateTime.UtcNow < p.LcpEffectiveEndDate)).FirstOrDefault();
            if (CommPeriod != null)
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommPeriod.LcpPeriodName);
                return Ok(xx);
            }
            return Ok();
        }
    }
}
