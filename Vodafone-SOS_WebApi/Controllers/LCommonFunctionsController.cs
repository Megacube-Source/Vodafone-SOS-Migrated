using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class LCommonFunctionsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        public bool DoesUserHaveRole(string UserId, string Role)
        {
            //AspNetUserRoles table is not available in entity model and hence wrote this SQL query. 
            //Once table is available in Entity framework (probably on next update) below mechanism should be replaced with appropriate linq query. 
            string Qry = "select count(*) ";
            Qry = Qry + "from AspnetRoles R Inner Join AspNetUserRoles UR ON R.Id = UR.RoleId ";
            Qry = Qry + "where R.Name = '" + Role + "' AND UR.UserId = '" + UserId + "' ";

            var xx = db.Database.SqlQuery<int>(Qry).First();
            return (xx > 0);
        }
    }
}
