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
    public class RDeviceTypesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult GetRDeviceTypesDropdownData(int CompanyId)
        {
            var data = db.RDeviceTypes.Where(p=>p.RdtCompanyId==CompanyId).Where(p => p.RdtIsActive==true).Select(x => new { x.RdtName, x.Id }).OrderBy(p=>p.RdtName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RDeviceTypes?CompanyId=5
        public IHttpActionResult GetRDeviceTypesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RDeviceTypes.Where(p=>p.RdtCompanyId==CompanyId).Include(c => c.GCompany)
                      select new { aa.RdtIsActive,aa.RdtName, aa.RdtDescription, aa.RdtCompanyId, aa.GCompany.GcCompanyName,aa.Id}).OrderByDescending(p => p.RdtIsActive).ThenBy(p=>p.RdtName);
            return Ok(xx); 
        }

        // GET: api/RDeviceTypes/5
        [ResponseType(typeof(RDeviceType))]
        public async Task<IHttpActionResult> GetRDeviceType(int id)
        {
            var RDeviceType = db.RDeviceTypes.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RdtIsActive, x.Id, x.RdtCompanyId, x.RdtName, x.RdtDescription, x.GCompany.GcCompanyName }).FirstOrDefault();
            if (RDeviceType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DeviceType")));
            }
            return Ok(RDeviceType);
        }

        // PUT: api/RDeviceTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRDeviceType(int id, RDeviceType RDeviceType)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DeviceType")));
            }

            if (id != RDeviceType.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DeviceType")));
            }
            try
            {
                db.Entry(RDeviceType).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!RDeviceTypeExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DeviceType")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RDeviceTypes
        [ResponseType(typeof(RDeviceType))]
        public async Task<IHttpActionResult> PostRDeviceType(RDeviceType RDeviceType)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "DeviceType")));
            }
            try
            {
                db.RDeviceTypes.Add(RDeviceType);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
            // return CreatedAtRoute("DefaultApi", new { id = RDeviceType.Id }, RDeviceType);
        }

        // DELETE: api/RDeviceTypes/5
        [ResponseType(typeof(RDeviceType))]
        public async Task<IHttpActionResult> DeleteRDeviceType(int id)
        {
            RDeviceType RDeviceType = await db.RDeviceTypes.FindAsync(id);
            if (RDeviceType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DeviceType")));
            }
            try
            {
                db.RDeviceTypes.Remove(RDeviceType);
                await db.SaveChangesAsync();
                return Ok(RDeviceType);
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

        private bool RDeviceTypeExists(int id)
        {
            return db.RDeviceTypes.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_RDeviceTypes_RdtDeviceTypeName_RdtCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "DEVICE TYPE"));

            else if (SqEx.Message.IndexOf("FK_RDeviceTypes_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DEVICE TYPE", "CLAIMS"));

            else if (SqEx.Message.IndexOf("VDeviceTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DEVICE TYPE", "VIEW(S)"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
