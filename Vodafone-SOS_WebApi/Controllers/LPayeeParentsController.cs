using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class LPayeeParentsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LPayeeParents
        public IHttpActionResult GetPayeeParents()
        {
            var xx = (from aa in db.LPayeeParents
                      select new { aa.Id,aa.LppEffectiveEndDate,aa.LppEffectiveStartDate,aa.LppParentPayeeId,aa.LppPayeeId});
            return Ok(xx);
        }

        public IHttpActionResult GetCurrentPayeeParentByPayeeId(int PayeeId)
        {
            var CurrentParent = db.LPayeeParents.Where(p => p.LppPayeeId == PayeeId).Where(p => p.LppEffectiveEndDate == null).Select(p=>new {p.Id,p.LppEffectiveStartDate,p.LppEffectiveEndDate,p.LppParentPayeeId,p.LppPayeeId }).FirstOrDefault();
            return Ok(CurrentParent);
        }
        public IHttpActionResult GetPayeeParentByPayeeId(int PayeeId)
        {
            var CurrentParent = db.LPayeeParents.Where(p => p.LppPayeeId == PayeeId).Where(p=>p.LppEffectiveEndDate!=null).Select(p => new { p.Id, p.LppEffectiveStartDate, p.LppEffectiveEndDate, p.LppParentPayeeId, p.LppPayeeId });
            return Ok(CurrentParent);
        }
        // GET: api/LPayeeParents/5
        [ResponseType(typeof(LPayeeParent))]
        public async Task<IHttpActionResult> GetLPayeeParent(int id)
        {
            var LPayeeParent = db.LPayeeParents.Where(p => p.Id == id).Select(x => new { x.Id, x.LppPayeeId,x.LppParentPayeeId,x.LppEffectiveStartDate,x.LppEffectiveEndDate }).FirstOrDefault();
            if (LPayeeParent == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PAYEE PARENT")));
            }
            return Ok(LPayeeParent);
        }

        // PUT: api/LPayeeParents/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLPayeeParent(int id, LPayeeParent LPayeeParent)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PAYEE PARENT")));
            }

            if (id != LPayeeParent.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PAYEE PARENT")));
            }
            try
            {
                db.Entry(LPayeeParent).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LPayeeParentExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PAYEE PARENT")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LPayeeParents
        [ResponseType(typeof(LPayeeParent))]
        public async Task<IHttpActionResult> PostLPayeeParent(LPayeeParent LPayeeParent)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "PAYEE PARENT")));
            }
            try
            {
                //As soon as PAYEE PARENT is created a database trigger will automatically Create a sequence for this PAYEE PARENT which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.LPayeeParents.Add(LPayeeParent);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LPayeeParent.Id }, LPayeeParent);
        }

        // DELETE: api/LPayeeParents/5
        [ResponseType(typeof(LPayeeParent))]
        public async Task<IHttpActionResult> DeleteLPayeeParent(int id)
        {
            LPayeeParent LPayeeParent = await db.LPayeeParents.FindAsync(id);
            if (LPayeeParent == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PAYEE PARENT")));
            }
            try
            {
                db.LPayeeParents.Remove(LPayeeParent);
                await db.SaveChangesAsync();
                return Ok(LPayeeParent);
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

        private bool LPayeeParentExists(int id)
        {
            return db.LPayeeParents.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("GetPayeeParentId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEE PARENT", "DATABASE OBJECTS"));
           else if (SqEx.Message.IndexOf("SpUpdateChangeRequestData", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEE PARENT", "DATABASE OBJECTS"));
            else if (SqEx.Message.IndexOf("VPayeeParents", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEE PARENT", "VIEW(S)"));
             else
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }
    }
}
