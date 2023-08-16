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
//using System.Globalization;

//namespace Vodafone_SOS_WebApi.Controllers
//{
//    [CustomExceptionFilter]
//    public class LPayeeAuditLogsController : ApiController
//    {
//        private SOSEDMV10Entities db = new SOSEDMV10Entities();

//        //This method is commented as we will not get data from api without companyid filter
//        // GET: api/LPayeeAuditLogs
//        //public IHttpActionResult GetLPayeeAuditLogs()
//        //{
//        //    var xx = (from aa in db.LPayeeAuditLogs.Include(p => p.LPayee.GCompany) select new {aa.LPayee.LpFirstName,aa.LPayee.LpLastName,aa.LPayee.LpPayeeCode,
//        //        aa.LPayee.GCompany.GcCompanyName,aa.LpalUpdatedDateTime,aa.LpalUpdatedById,aa.LpalPayeeId,aa.LpalAction,aa.Id }).OrderByDescending(p=>p.LpalUpdatedDateTime);
//        //    return Ok(xx);
//        //}

//        public IHttpActionResult GetLPayeeAuditLogsByCompanyId(int CompanyId)
//        {
//             var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
//            var xx = (from aa in db.LPayeeAuditLogs.Include(p => p.LPayee.GCompany).Where(p=>p.LPayee.LpCompanyId==CompanyId) select new { aa.LPayee.LpFirstName,aa.LPayee.LpLastName, 
//                aa.LPayee.LpPayeeCode, aa.LPayee.GCompany.GcCompanyName, aa.LpalUpdatedDateTime, aa.LpalUpdatedById, aa.LpalPayeeId, aa.LpalAction, aa.Id,
//                              CreatedBy=(aa.LPayee.LpCreatedById!=null)?UserList.Where(p=>p.Id==aa.LPayee.LpCreatedById).FirstOrDefault().UserName:"",
//                          UpdatedBy = (aa.LPayee.LpUpdatedById!=null)?UserList.Where(p => p.Id == aa.LPayee.LpUpdatedById).FirstOrDefault().UserName:"",aa.LPayee.LpCreatedDateTime
//            }).OrderByDescending(p => p.LpalUpdatedDateTime);
//            return Ok(xx);
//        }

//        public IHttpActionResult GetAuditLogUnderStartDateEndDate(int CompanyId,string StartDate,string EndDate)
//        {
//            try
//            {
//                //These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
//                var StartDateTime = DateTime.ParseExact(StartDate,"yyyy-MM-dd",CultureInfo.InvariantCulture);
//                var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
//                var xx = (from p in db.LPayeeAuditLogs.Include(p => p.LPayee.GCompany).Where(p => p.LpalUpdatedDateTime >= StartDateTime && p.LpalUpdatedDateTime < EndDateTime).Where(p => p.LPayee.LpCompanyId == CompanyId)
//                          join bb in db.AspNetUsers on p.LPayee.LpCreatedById equals bb.Id
//                          join tt in db.AspNetUsers on p.LPayee.LpUpdatedById equals tt.Id
//                          select new
//                          {
//                              p.Id,
//                              p.LpalAction,
//                              p.LpalPayeeId,
//                              p.LpalUpdatedById,
//                              p.LpalUpdatedDateTime,
//                              FullName = p.LPayee.LpFirstName + " " + p.LPayee.LpLastName,
//                              p.LPayee.LpPayeeCode,
//                              p.LPayee.GCompany.GcCompanyName,
//                              CreatedBy = bb.UserName,
//                              UpdatedBy = tt.UserName,
//                              p.LPayee.LpCreatedDateTime
//                          });
//                return Ok(xx);
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
//            }
//        }

//        // GET: api/LPayeeAuditLogs/5
//        [ResponseType(typeof(LPayeeAuditLog))]
//        public async Task<IHttpActionResult> GetLPayeeAuditLog(int id)
//        {
//            LPayeeAuditLog LPayeeAuditLog = await db.LPayeeAuditLogs.FindAsync(id);
//            if (LPayeeAuditLog == null)
//            {
//                //return NotFound();
//                //AUDIT to be displayed could not be found. Send appropriate response to the request.
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AUDIT")));
//            }
//            return Ok(LPayeeAuditLog);
//        }

//        // PUT: api/LPayeeAuditLogs/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutLPayeeAuditLog(int id,LPayeeAuditLog LPayeeAuditLog)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "AUDIT")));
//            }

//            if (id != LPayeeAuditLog.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "AUDIT")));
//            }
//            try
//            {
//                db.Entry(LPayeeAuditLog).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!LPayeeAuditLogExists(id))
//                {
//                    //return NotFound();
//                    //CITY/POST CODE could not be found. Send appropriate response to the request.
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AUDIT")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/LPayeeAuditLogs
//        [ResponseType(typeof(LPayeeAuditLog))]
//        public async Task<IHttpActionResult> PostLPayeeAuditLog(LPayeeAuditLog LPayeeAuditLog)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "AUDIT")));
//            }
//            try
//            {
//                db.LPayeeAuditLogs.Add(LPayeeAuditLog);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = LPayeeAuditLog.Id }, LPayeeAuditLog);
//        }

//        // DELETE: api/LPayeeAuditLogs/5
//        [ResponseType(typeof(LPayeeAuditLog))]
//        public async Task<IHttpActionResult> DeleteLPayeeAuditLog(int id)
//        {
//            LPayeeAuditLog LPayeeAuditLog = await db.LPayeeAuditLogs.FindAsync(id);
//            if (LPayeeAuditLog == null)
//            {
//                //return NotFound();
//                //CITY/POST CODE could not be found. Send appropriate response to the request.
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AUDIT")));
//            }
//            try
//            {
//                db.LPayeeAuditLogs.Remove(LPayeeAuditLog);
//                await db.SaveChangesAsync();
//                return Ok(LPayeeAuditLog);
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

//        private bool LPayeeAuditLogExists(int id)
//        {
//            return db.LPayeeAuditLogs.Count(e => e.Id == id) > 0;
//        }

//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;
//            if (SqEx.Message.IndexOf("SpUpdateChangeRequestData", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEE AUDIT LOGS", "DATABASE OBJECTS"));
//            else
//            //Depending upon the constraint failed return appropriate error message
//            //Something else failed return original error message as retrieved from database
//            return SqEx.Message;
//        }
//    }
//}
