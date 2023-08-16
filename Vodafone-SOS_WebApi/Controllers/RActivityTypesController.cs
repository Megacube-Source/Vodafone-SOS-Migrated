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
    public class RActivityTypesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        
        //Method to get company specific data for dropdown in WebApplication
        public IHttpActionResult GetRActivityTypesDropdownData(int CompanyId)
        {
            var data = db.RActivityTypes.Where(p=>p.RatCompanyId==CompanyId).Where(p => p.RatIsActive==true).Select(x => new { x.RatName, x.Id }).OrderBy(p=>p.RatName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RActivityTypes?CompanyId=5
        public IHttpActionResult GetRActivityTypesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RActivityTypes.Where(p=>p.RatCompanyId==CompanyId).Include(c => c.GCompany)
                      select new {aa.RatIsActive, aa.RatName, aa.RatDescription, aa.RatCompanyId, aa.GCompany.GcCompanyName,aa.Id}).OrderByDescending(p => p.RatIsActive).ThenBy(p=>p.RatName);
            return Ok(xx); 
        }

        // GET: api/RActivityTypes/5
        [ResponseType(typeof(RActivityType))]
        public async  Task<IHttpActionResult> GetRActivityType(int id)
        {
            var RActivityType = db.RActivityTypes.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RatIsActive, x.Id, x.RatCompanyId, x.RatName, x.RatDescription, x.GCompany.GcCompanyName}).FirstOrDefault();
            if (RActivityType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ActivityType")));
            }
            return Ok(RActivityType);
        }

        // PUT: api/RActivityTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRActivityType(int id, RActivityType RActivityType)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ActivityType")));
            }

            if (id != RActivityType.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ActivityType")));
            }
            try
            {
                db.Entry(RActivityType).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!RActivityTypeExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ActivityTypE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RActivityTypes
        [ResponseType(typeof(RActivityType))]
        public async Task<IHttpActionResult> PostRActivityType(RActivityType RActivityType)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ActivityTypE")));
            }
            try
            {
                db.RActivityTypes.Add(RActivityType);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = RActivityType.Id }, RActivityType);
        }

        // DELETE: api/RActivityTypes/5
        [ResponseType(typeof(RActivityType))]
        public async Task<IHttpActionResult> DeleteRActivityType(int id)
        {
            RActivityType RActivityType = await db.RActivityTypes.FindAsync(id);
            if (RActivityType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ActivityTypE")));
            }
            try
            {
                db.RActivityTypes.Remove(RActivityType);
                await db.SaveChangesAsync();
                return Ok(RActivityType);
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

        private bool RActivityTypeExists(int id)
        {
            return db.RActivityTypes.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_RActivityTypes_RatActivityTypeName_RatCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "ACTIVITY TYPE"));
            else if (SqEx.Message.IndexOf("FK_RActivityTypes_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ACTIVITY TYPE", "CLAIMS"));
            //else if (SqEx.Message.IndexOf("FK_RActivityTypes_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ACTIVITY TYPE", "RAW DATA"));
            if(SqEx.Message.IndexOf("VActivityTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ACTIVITY TYPES", "VIEW(S)"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
