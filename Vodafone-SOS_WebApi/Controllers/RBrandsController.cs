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
    public class RBrandsController : ApiController
    {
            private SOSEDMV10Entities db = new SOSEDMV10Entities();
           
            public IHttpActionResult GetRBrandsDropdownData(int CompanyId)
            {
                var data = db.RBrands.Where(p => p.RbCompanyId == CompanyId).Where(p => p.RbIsActive==true).Select(x => new { x.RbName, x.Id }).OrderBy(p => p.RbName).AsEnumerable();
                return Ok(data);
            }

            // GET: api/RBrands?CompanyId=5
            public IHttpActionResult GetRBrandsByCompanyId(int CompanyId)
            {
                var xx = (from aa in db.RBrands.Where(p => p.RbCompanyId == CompanyId).Include(c => c.GCompany)
                          select new {aa.RbIsActive, aa.RbName, aa.RbDescription, aa.RbCompanyId, aa.GCompany.GcCompanyName,aa.Id }).OrderBy(p => p.RbName);
                return Ok(xx);
            }

            // GET: api/RBrands/5
            [ResponseType(typeof(RBrand))]
            public async Task<IHttpActionResult> GetRBrand(int id)
            {
                var RBrand = db.RBrands.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RbIsActive, x.Id, x.RbCompanyId, x.RbName, x.RbDescription, x.GCompany.GcCompanyName }).FirstOrDefault();
                if (RBrand == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
                }
                return Ok(RBrand);
            }

            // PUT: api/RBrands/5
            [ResponseType(typeof(void))]
            public async Task<IHttpActionResult> PutRBrand(int id, RBrand RBrand)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BRAND")));
                }

                if (id != RBrand.Id)
                {
                    //return BadRequest();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BRAND")));
                }
                try
                {
                    db.Entry(RBrand).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!RBrandExists(id))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }

            // POST: api/RBrands
            [ResponseType(typeof(RBrand))]
            public async Task<IHttpActionResult> PostRBrand(RBrand RBrand)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "BRAND")));
                }
                try
                {
                    db.RBrands.Add(RBrand);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                return CreatedAtRoute("DefaultApi", new { id = RBrand.Id }, RBrand);
            }

            // DELETE: api/RBrands/5
            [ResponseType(typeof(RBrand))]
            public async Task<IHttpActionResult> DeleteRBrand(int id)
            {
                RBrand RBrand = await db.RBrands.FindAsync(id);
                if (RBrand == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
                }
                try
                {
                    db.RBrands.Remove(RBrand);
                    await db.SaveChangesAsync();
                    return Ok(RBrand);
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

            private bool RBrandExists(int id)
            {
                return db.RBrands.Count(e => e.Id == id) > 0;
            }

            private string GetCustomizedErrorMessage(Exception ex)
            {
                //Convert the exception to SqlException to get the error message returned by database.
                var SqEx = ex.GetBaseException() as SqlException;

                //Depending upon the constraint failed return appropriate error message
                if (SqEx.Message.IndexOf("UQ_RBrads_RbName_RbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "BRAND"));
                else if (SqEx.Message.IndexOf("FK_RBrands_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BRAND", "CLAIMS"));
                else if (SqEx.Message.IndexOf("VBrands", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BRAND", "VIEW(S)"));
            else
                    //Something else failed return original error message as retrieved from database
                    return SqEx.Message;
            }
    }
}
