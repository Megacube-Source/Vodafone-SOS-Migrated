//Code Review Convert SQL to LINQ completed

using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;


namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class RRejectionReasonsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult GetRRejectionReasonsDropdownData(int CompanyId)
        {
            var data = db.RRejectionReasons.Where(p => p.RrrCompanyId == CompanyId).Where(p=>p.RrrIsActive==true).Select(x => new { x.RrrReason, x.Id }).OrderBy(p => p.RrrReason).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RRejectionReasons?CompanyId=5
        public IHttpActionResult GetRRejectionReasonsByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RRejectionReasons.Where(p => p.RrrCompanyId == CompanyId).Include(c => c.GCompany)
                      select new {aa.RrrIsActive, aa.Id,aa.RrrCompanyId,aa.RrrReason,aa.GCompany.GcCompanyName ,aa.RrrDescription}).OrderByDescending(p => p.RrrIsActive).ThenBy(p=>p.RrrReason);
            return Ok(xx);
        }

        // GET: api/RRejectionReasons/5
        [ResponseType(typeof(RRejectionReason))]
        public async Task<IHttpActionResult> GetRRejectionReason(int id)
        {
            var RRejectionReason = db.RRejectionReasons.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RrrIsActive, x.Id, x.GCompany.GcCompanyName,x.RrrCompanyId,x.RrrReason,x.RrrDescription}).FirstOrDefault();
            if (RRejectionReason == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RejectionReason")));
            }
            return Ok(RRejectionReason);
        }

        // PUT: api/RRejectionReasons/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRRejectionReason(int id, RRejectionReason RRejectionReason)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RejectionReason")));
            }

            if (id != RRejectionReason.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RejectionReason")));
            }
            try
            {
                db.Entry(RRejectionReason).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!RRejectionReasonExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RejectionReason")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RRejectionReasons
        [ResponseType(typeof(RRejectionReason))]
        public async Task<IHttpActionResult> PostRRejectionReason(RRejectionReason RRejectionReason)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RejectionReason")));
            }
            try
            {
                db.RRejectionReasons.Add(RRejectionReason);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
            // return CreatedAtRoute("DefaultApi", new { id = RRejectionReason.Id }, RRejectionReason);
        }

        // DELETE: api/RRejectionReasons/5
        [ResponseType(typeof(RRejectionReason))]
        public async Task<IHttpActionResult> DeleteRRejectionReason(int id)
        {
            RRejectionReason RRejectionReason = await db.RRejectionReasons.FindAsync(id);
            if (RRejectionReason == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RejectionReason")));
            }
            try
            {
                db.RRejectionReasons.Remove(RRejectionReason);
                await db.SaveChangesAsync();
                return Ok(RRejectionReason);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RRejectionReasonExists(int id)
        {
            return db.RRejectionReasons.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_RRejectionReasons_CompanyId_RejectionREason", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "REJECTION REASON"));
            else if (SqEx.Message.IndexOf("FK_RRejectionReasons_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "REJECTION REASON", "CLAIMS"));

            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
