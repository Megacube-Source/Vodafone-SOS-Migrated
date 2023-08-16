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
//    [CustomExceptionFilter]
//    public class RStatusOwnersController : ApiController
//    {
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();

//        // GET: api/RStatusOwners
//        public IHttpActionResult GetRStatusOwners()
//        {
//            var xx = (from aa in db.RStatusOwners
//                      select new {aa.Id, aa.RsoStatusOwner}).OrderBy(p=>p.RsoStatusOwner);
//            return Ok(xx);
//        }

//        // GET: api/RStatusOwners/5
//        [ResponseType(typeof(RStatusOwner))]
//        public async Task<IHttpActionResult> GetRStatusOwner(int id)
//        {
//            var RStatusOwner = db.RStatusOwners.Where(p => p.Id == id).Select(x => new { x.Id, x.RsoStatusOwner}).FirstOrDefault();
//            if (RStatusOwner == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "StatusOwner")));
//            }
//            return Ok(RStatusOwner);
//        }

//        // PUT: api/RStatusOwners/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutRStatusOwner(int id, RStatusOwner RStatusOwner)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "StatusOwner")));
//            }

//            if (id != RStatusOwner.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "StatusOwner")));
//            }
//            try
//            {
//                db.Entry(RStatusOwner).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!RStatusOwnerExists(id))
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "StatusOwner")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/RStatusOwners
//        [ResponseType(typeof(RStatusOwner))]
//        public async Task<IHttpActionResult> PostRStatusOwner(RStatusOwner RStatusOwner)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "StatusOwner")));
//            }
//            try
//            {
//                db.RStatusOwners.Add(RStatusOwner);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = RStatusOwner.Id }, RStatusOwner);
//        }

//        // DELETE: api/RStatusOwners/5
//        [ResponseType(typeof(RStatusOwner))]
//        public async Task<IHttpActionResult> DeleteRStatusOwner(int id)
//        {
//            RStatusOwner RStatusOwner = await db.RStatusOwners.FindAsync(id);
//            if (RStatusOwner == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "StatusOwner")));
//            }
//            try
//            {
//                db.RStatusOwners.Remove(RStatusOwner);
//                await db.SaveChangesAsync();
//                return Ok(RStatusOwner);
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        private bool RStatusOwnerExists(int id)
//        {
//            return db.RStatusOwners.Count(e => e.Id == id) > 0;
//        }

//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;

//            //Depending upon the constraint failed return appropriate error message
//            if (SqEx.Message.IndexOf("UQ_RStatusOwners_RsoStatusOwner", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "STATUS OWNER"));

//            else if (SqEx.Message.IndexOf("FK_RStatusOwners_RStatuses", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "STATUS OWNER", "STATUS"));
//            else
//                //Something else failed return original error message as retrieved from database
//                return SqEx.Message;
//        }
//    }
//}
