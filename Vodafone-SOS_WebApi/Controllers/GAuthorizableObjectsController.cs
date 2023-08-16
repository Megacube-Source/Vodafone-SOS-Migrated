using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient;
using Vodafone_SOS_WebApi.Models;
using System.Threading.Tasks;
using Vodafone_SOS_WebApi.Utilities;


namespace Vodafone_SOS_WebApi.Controllers
{
    public class GAuthorizableObjectsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

          [HttpGet]
        

        [ResponseType(typeof(GAuthorizableObject))]
        public async Task<IHttpActionResult>  GetGAuthorizableObjects()
        {
            //  var GAuthorizableObject= db.GAuthorizableObjects.Select(x => new { x.Id, x.GaoControllerName, x.GaoControllerMethodName, x.GaoDescription }).FirstOrDefault();
            var GAuthorizableObject = (from x in db.GAuthorizableObjects
                                       select new { x.Id, x.GaoControllerName, x.GaoControllerMethodName, x.GaoDescription });
            
            if (GAuthorizableObject == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "GAuthorizableObject")));

            }
            return Ok(GAuthorizableObject);
        }


    }
}
