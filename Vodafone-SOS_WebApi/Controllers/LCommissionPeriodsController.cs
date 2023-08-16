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
    public class LCommissionPeriodsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        private String BaseQry = @"Select 	CP.[Id], CP.[LcpCompanyId], CP.[LcpPeriodName], 
            CP.[LcpCreatedById] , CP.[LcpCreatedDateTime], 
            CP.[LcpUpdatedById], CP.[LcpUpdatedDateTime] , CP.[LcpStatus] 
            From [dbo].[LCommissionPeriods] CP 
            inner join [dbo].[GCompanies] C on CP.[LcpCompanyId] = C.Id 
            inner join [dbo].[AspNetUsers] UC on CP.[LcpCreatedById] = UC.Id 
            inner join [dbo].[AspNetUsers] UU on CP.[LcpUpdatedById] = UU.Id ";
        //method to get concatenated string for Payee and status --for PayeeCalc(sg)
        public IHttpActionResult GetLCommissionPeriodsStatusByCompanyId(int CompanyId)
        {
            /*- Shows LcommissionPeriods.LcpPeriodName for that OpCo where PeriodType NOT = 'Daily' order by LcpPeriodCode desc*/
            //Commented for 2.8.1
            //string tempQry = @"Select 	CP.[Id], CP.[LcpCompanyId],CP.[LcpPeriodName], (CP.[LcpPeriodName] + '('+ CP.[LcpStatus] + ')') as LcpStatus, 
            string tempQry = @"Select 	CP.[Id], CP.[LcpCompanyId],CP.[LcpPeriodName], CP.[LcpPeriodName] as LcpStatus, 
            CP.[LcpCreatedById] , CP.[LcpCreatedDateTime], 
            CP.[LcpUpdatedById], CP.[LcpUpdatedDateTime] , CP.[LcpStatus] 
            From [dbo].[LCommissionPeriods] CP 
            inner join [dbo].[GCompanies] C on CP.[LcpCompanyId] = C.Id 
            inner join [dbo].[AspNetUsers] UC on CP.[LcpCreatedById] = UC.Id 
            inner join [dbo].[AspNetUsers] UU on CP.[LcpUpdatedById] = UU.Id ";
            //JS suggested condition for displaying Periosd in Global DropDown and add End Date with three month later than GetDate
            string Qry = tempQry + "Where CP.LcpCompanyId = " + CompanyId + " and CP.LcpPeriodType!='Daily' and CP.LcpEffectiveStartDate<DATEADD(month, 3, GetDate()) ";
            Qry = Qry + "Order By LcpPeriodCode desc ";
            var xx = db.Database.SqlQuery<LCommissionPeriodsViewModel>(Qry);
            return Ok(xx);
        }


        // GET: api/LCommissionPeriods?CompanyId=5
        public IHttpActionResult GetLCommissionPeriodsByCompanyId(int CompanyId)
        {
            string Qry = BaseQry + "Where CP.LcpCompanyId = " + CompanyId + " ";
            Qry = Qry + "Order By LcpPeriodName ";
            var xx = db.Database.SqlQuery<LCommissionPeriodsViewModel>(Qry);
            return Ok(xx);
        }

        public IHttpActionResult GetLCommissionPeriodsByCompanyIdStatus(int CompanyId, string Status)
        {
            string Qry = BaseQry + "Where CP.LcpCompanyId = " + CompanyId + " ";
            Qry = Qry + "AND LcpStatus = '" + Status + "' ";
            Qry = Qry + "Order By LcpCreatedDateTime desc ";
            var xx = db.Database.SqlQuery<LCommissionPeriodsViewModel>(Qry);
            return Ok(xx);
        }



        // GET: api/LCommissionPeriods/5
        [ResponseType(typeof(LCommissionPeriodsViewModel))]
        public async Task<IHttpActionResult> GetLCommissionPeriod(int id)
        {
            string Qry = BaseQry + "Where CP.Id = " + id;
            var LCommissionPeriod = db.Database.SqlQuery<LCommissionPeriodsViewModel>(Qry).FirstOrDefault();

            if (LCommissionPeriod == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Commission Period")));
            }
            return Ok(LCommissionPeriod);
        }

        // PUT: api/LCommissionPeriods/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLCommissionPeriod(int Id, LCommissionPeriod LCommissionPeriod)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Commission Period")));
            }

            if (Id != LCommissionPeriod.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Commissio Period")));
            }
            try
            {
                db.Entry(LCommissionPeriod).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LCommissionPeriodExists(Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Commission Period")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LCommissionPeriods
        [ResponseType(typeof(LCommissionPeriod))]
        public async Task<IHttpActionResult> PostLCommissionPeriod(LCommissionPeriod LCommissionPeriod)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Commission Period")));
            }
            try
            {
                db.LCommissionPeriods.Add(LCommissionPeriod);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LCommissionPeriod.Id }, LCommissionPeriod);
        }

        // DELETE: api/LCommissionPeriods/5
        [ResponseType(typeof(LCommissionPeriod))]
        public async Task<IHttpActionResult> DeleteLCommissionPeriod(int id)
        {
            LCommissionPeriod LCommissionPeriod = await db.LCommissionPeriods.FindAsync(id);
            if (LCommissionPeriod == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Commission Period")));
            }
            try
            {
                db.LCommissionPeriods.Remove(LCommissionPeriod);
                await db.SaveChangesAsync();
                return Ok(LCommissionPeriod);
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

        private bool LCommissionPeriodExists(int id)
        {
            return db.LCommissionPeriods.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_LCommissionPeriods_CompanyId_PeriodName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "COMMISSION PERIOD"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
