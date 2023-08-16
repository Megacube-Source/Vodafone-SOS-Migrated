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
    public class RCommissionTypesController : ApiController
    {
            private SOSEDMV10Entities db = new SOSEDMV10Entities();

            public IHttpActionResult GetRCommissionTypesDropdownData(int CompanyId)
            {
                var data = db.RCommissionTypes.Where(p => p.RctCompanyId == CompanyId).Where(p => p.RctIsActive==true).Select(x => new { x.RctName, x.Id }).OrderBy(p => p.RctName).AsEnumerable();
                return Ok(data);
            }

            // GET: api/RCommissionTypes?CompanyId=5
            public IHttpActionResult GetRCommissionTypesByCompanyId(int CompanyId)
            {
                var xx = (from aa in db.RCommissionTypes.Where(p => p.RctCompanyId == CompanyId).Include(c => c.GCompany)
                          select new {aa.RctIsActive, aa.RctName, aa.RctDescription, aa.RctCompanyId, aa.GCompany.GcCompanyName,aa.Id }).OrderByDescending(p => p.RctIsActive).ThenBy(p=>p.RctName);
                return Ok(xx);
            }

            // GET: api/RCommissionTypes/5
            [ResponseType(typeof(RCommissionType))]
            public async Task<IHttpActionResult> GetRCommissionType(int id)
            {
                var RCommissionType = db.RCommissionTypes.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RctIsActive, x.Id, x.RctCompanyId, x.RctName, x.RctDescription, x.GCompany.GcCompanyName }).FirstOrDefault();
                if (RCommissionType == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CommissionType")));
                }
                return Ok(RCommissionType);
            }

            // PUT: api/RCommissionTypes/5
            [ResponseType(typeof(void))]
            public async Task<IHttpActionResult> PutRCommissionType(int id, RCommissionType RCommissionType)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CommissionType")));
                }

                if (id != RCommissionType.Id)
                {
                    //return BadRequest();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CommissionType")));
                }
                try
                {
                    db.Entry(RCommissionType).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!RCommissionTypeExists(id))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CommissionType")));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }

            // POST: api/RCommissionTypes
            [ResponseType(typeof(RCommissionType))]
            public async Task<IHttpActionResult> PostRCommissionType(RCommissionType RCommissionType)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "CommissionType")));
                }
                try
                {
                    db.RCommissionTypes.Add(RCommissionType);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            return Ok();
            //return CreatedAtRoute("DefaultApi", new { id = RCommissionType.Id }, RCommissionType);
        }

            // DELETE: api/RCommissionTypes/5
            [ResponseType(typeof(RCommissionType))]
            public async Task<IHttpActionResult> DeleteRCommissionType(int id)
            {
                RCommissionType RCommissionType = await db.RCommissionTypes.FindAsync(id);
                if (RCommissionType == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CommissionType")));
                }
                try
                {
                    db.RCommissionTypes.Remove(RCommissionType);
                    await db.SaveChangesAsync();
                    return Ok(RCommissionType);
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

            private bool RCommissionTypeExists(int id)
            {
                return db.RCommissionTypes.Count(e => e.Id == id) > 0;
            }

            private string GetCustomizedErrorMessage(Exception ex)
            {
                //Convert the exception to SqlException to get the error message returned by database.
                var SqEx = ex.GetBaseException() as SqlException;

                //Depending upon the constraint failed return appropriate error message
                if (SqEx.Message.IndexOf("UQ_RCommissionTypes_RctCommissionTypeName_RctCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "COMMISSION TYPE"));
            else if (SqEx.Message.IndexOf("FK_RCommissionTypes_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMMISSION TYPE", "CLAIMS"));
            else if (SqEx.Message.IndexOf("FK_RCommissionTypes_LClaims_Payment", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMMISSION TYPE", "CLAIM PAYMENT"));
            else if (SqEx.Message.IndexOf("FK_RCommissionTypes_LClaims_Clawback", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMMISSION TYPE", "CLAIM CLAWBACK"));
            else if (SqEx.Message.IndexOf("VCommissionTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMMISSION TYPE", "VIEW(S)"));
            else
                    //Something else failed return original error message as retrieved from database
                    return SqEx.Message;
            }
        }
}
