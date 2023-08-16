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
    public class RProductCodesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult GetRProductCodesDropdownData(int CompanyId)
        {
            var data = db.RProductCodes.Where(p => p.RpcCompanyId == CompanyId).Where(p => p.RpcIsActive==true).Select(x => new { x.RpcProductCode, x.Id }).OrderBy(p => p.RpcProductCode).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RProductCodes?CompanyId=5
        public IHttpActionResult GetRProductCodesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RProductCodes.Where(p => p.RpcCompanyId == CompanyId).Include(c => c.GCompany)
                      select new {aa.RpcIsActive, aa.RpcProductCode,aa.RpcDescription, aa.GCompany.GcCompanyName, aa.Id,aa.RpcCompanyId }).OrderByDescending(p => p.RpcIsActive).ThenBy(p=>p.RpcProductCode);
            return Ok(xx);
        }

        // GET: api/RProductCodes/5
        [ResponseType(typeof(RProductCode))]
        public async Task<IHttpActionResult> GetRProductCode(int id)
        {
            var RProductCode = db.RProductCodes.Where(p => p.Id == id).Include(c => c.GCompany).Select(aa => new {aa.RpcIsActive, aa.RpcProductCode, aa.RpcDescription, aa.GCompany.GcCompanyName, aa.Id, aa.RpcCompanyId }).FirstOrDefault();
            if (RProductCode == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT CODE")));
            }
            return Ok(RProductCode);
        }

        // PUT: api/RProductCodes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRProductCode(int id, RProductCode RProductCode)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCT CODE")));
            }

            if (id != RProductCode.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCT CODE")));
            }
            try
            {
                db.Entry(RProductCode).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!RProductCodeExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT CODE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RProductCodes
        [ResponseType(typeof(RProductCode))]
        public async Task<IHttpActionResult> PostRProductCode(RProductCode RProductCode)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "PRODUCT CODE")));
            }
            try
            {
                db.RProductCodes.Add(RProductCode);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
            // return CreatedAtRoute("DefaultApi", new { id = RProductCode.Id }, RProductCode);
        }

        // DELETE: api/RProductCodes/5
        [ResponseType(typeof(RProductCode))]
        public async Task<IHttpActionResult> DeleteRProductCode(int id)
        {
            RProductCode RProductCode = await db.RProductCodes.FindAsync(id);
            if (RProductCode == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT CODE")));
            }
            try
            {
                db.RProductCodes.Remove(RProductCode);
                await db.SaveChangesAsync();
                return Ok(RProductCode);
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

        private bool RProductCodeExists(int id)
        {
            return db.RProductCodes.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("VProductCodes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCT CODE", "VIEW(S)"));
            else
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }
    }
}
