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
//    public class RStatusesController : ApiController
//    {
      
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();

//        // GET: api/RStatuses
//        public IHttpActionResult GetRStatuses()
//        {
//            var xx = (from aa in db.RStatuses.Include(c => c.RStatusOwner)
//                      select new {aa.Id, aa.RsStatus, aa.RsDescription, aa.RsOwnerId, aa.RStatusOwner.RsoStatusOwner }).OrderBy(p=>p.RsStatus);
//            return Ok(xx);
//        }

//        //Method to get company specific data for dropdown in WebApplication
//        public IHttpActionResult GetRStatusesDropdownData(int OwnerId)
//        {
//            var data = db.RStatuses.Where(p=>p.RsOwnerId==OwnerId).Select(x => new { x.RsStatus, x.Id }).OrderBy(p=>p.RsStatus).AsEnumerable();
//            return Ok(data);
//        }

//        // GET: api/RStatuses?OwnerId=5
//        public IHttpActionResult GetRStatusesByOwnerId(int OwnerId)
//        {
//            var xx = (from aa in db.RStatuses.Where(p=>p.RsOwnerId==OwnerId).Include(c => c.RStatusOwner)
//                      select new { aa.Id, aa.RsStatus, aa.RsDescription, aa.RsOwnerId, aa.RStatusOwner.RsoStatusOwner}).OrderBy(p => p.RsStatus);
//            return Ok(xx); 
//        }


//        // GET: api/RStatuses?OwnerId=5
//        public IHttpActionResult GetRStatusesByOwnerId(string OwnerName)
//        {
//            var xx = (from aa in db.RStatuses.Where(p => p.RStatusOwner.RsoStatusOwner == OwnerName).Include(c => c.RStatusOwner)
//                      select new { aa.Id, aa.RsStatus, aa.RsDescription, aa.RsOwnerId, aa.RStatusOwner.RsoStatusOwner }).OrderBy(p => p.RsStatus);
//            return Ok(xx);
//        }

//        public IHttpActionResult GetRStatusesByName(string Name)
//        {
//            var xx = (from aa in db.RStatuses.Where(p => p.RsStatus==Name).Include(c => c.RStatusOwner)
//                      select new { aa.Id, aa.RsStatus, aa.RsDescription, aa.RsOwnerId, aa.RStatusOwner.RsoStatusOwner }).OrderBy(p => p.RsStatus);
//            return Ok(xx);
//        }
//        // GET: api/RStatuses/5
//        [ResponseType(typeof(RStatus))]
//        public async Task<IHttpActionResult> GetRStatus(int id)
//        {
//            var RStatus = db.RStatuses.Where(p => p.Id == id).Include(c => c.RStatusOwner).Select(x => new { x.Id, x.RsOwnerId, x.RsStatus, x.RsDescription, x.RStatusOwner.RsoStatusOwner}).FirstOrDefault();
//            if (RStatus == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Status")));
//            }
//            return Ok(RStatus);
//        }

//        // PUT: api/RStatuses/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutRStatus(int id, RStatus RStatus)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Status")));
//            }

//            if (id != RStatus.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Status")));
//            }
//            try
//            {
//                db.Entry(RStatus).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!RStatusExists(id))
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Status")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/RStatuses
//        [ResponseType(typeof(RStatus))]
//        public async Task<IHttpActionResult> PostRStatus(RStatus RStatus)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Status")));
//            }
//            try
//            {
//                db.RStatuses.Add(RStatus);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = RStatus.Id }, RStatus);
//        }

//        // DELETE: api/RStatuses/5
//        [ResponseType(typeof(RStatus))]
//        public async Task<IHttpActionResult> DeleteRStatus(int id)
//        {
//            RStatus RStatus = await db.RStatuses.FindAsync(id);
//            if (RStatus == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Status")));
//            }
//            try
//            {
//                db.RStatuses.Remove(RStatus);
//                await db.SaveChangesAsync();
//                return Ok(RStatus);
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

//        private bool RStatusExists(int id)
//        {
//            return db.RStatuses.Count(e => e.Id == id) > 0;
//        }

//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;

//            //Depending upon the constraint failed return appropriate error message
//            if (SqEx.Message.IndexOf("UQ_RStatuses_RsStatus_RsOwnerId", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "STATUS"));

//            else if (SqEx.Message.IndexOf("FK_RStatuses_LChangeRequests", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "STATUS", "CHANGE REQUEST"));
//            else
//                //Something else failed return original error message as retrieved from database
//                return SqEx.Message;
//        }
//    }
//}
