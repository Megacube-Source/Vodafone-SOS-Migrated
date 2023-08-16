// commented XRawData_Bi_Prepaid_Activation_Report after database changes by vikas
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.Description;
//using System.Data.SqlClient; //being used in catch statement for identifying exception only.
//using Vodafone_SOS_WebApi.Models;
//using Vodafone_SOS_WebApi.Utilities;


//namespace Vodafone_SOS_WebApi.Controllers
//{
//    public class XRawData_BI_PREPAID_ACTIVATION_REPORTController : ApiController
//    {
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();

//        // GET: api/XRawData_BI_PREPAID_ACTIVATION_REPORT
//        public IHttpActionResult GetXRawData_BI_PREPAID_ACTIVATION_REPORT()
//        {
//            var xx = (from aa in db.XRawData_BI_PREPAID_ACTIVATION_REPORT
//                      select aa).OrderBy(p => p.DATE);
//            return Ok(xx);
//        }

      
       
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

      
//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;
//                //Something else failed return original error message as retrieved from database
//                return SqEx.Message;
//        }
//    }
//}
