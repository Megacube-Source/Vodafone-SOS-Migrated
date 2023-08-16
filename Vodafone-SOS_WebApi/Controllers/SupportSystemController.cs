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
    public class SupportSystemController : ApiController
    {
        // GET: SupportSystem
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        #region Category
        [ResponseType(typeof(RSupportCategory))]
        public async Task<IHttpActionResult> PostSupportCategory(RSupportCategory RSupportCategory)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "BRAND")));
            }
            try
            {
                db.RSupportCategories.Add(RSupportCategory);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = RSupportCategory.Id }, RSupportCategory);
        }

        [ResponseType(typeof(RSupportCategory))]
        public async Task<IHttpActionResult> GetSupportcategory(int id)
        {
            var Scat = db.RSupportCategories.Where(p => p.Id == id).Select(x => new { x.Id, x.RscName, x.RscDescription,x.RscTicketDescription }).FirstOrDefault();
            if (Scat == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SUPPORT CATEGORY")));
            }
            return Ok(Scat);
        }
        public IHttpActionResult GetAllSupportCategories()
        {
            var xx = (from aa in db.RSupportCategories 
                      select new { aa.Id, aa.RscName, aa.RscDescription, aa.RscTicketDescription }).OrderBy(p => p.RscName);
            
            return Ok(xx);
        }
        public IHttpActionResult GetSupportCategoriesDropdownData(int CompanyId)
        {
            var data = db.RSupportCategories.Where(p=>p.RscCompanyId==CompanyId).Select(x => new { x.RscName, x.Id, x.RscDescription, x.RscTicketDescription }).OrderBy(p => p.RscName).AsEnumerable();
            return Ok(data);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateCategory(int id, RSupportCategory Rsc)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Category")));
            }

            if (id != Rsc.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Category")));
            }
            try
            {
                db.Entry(Rsc).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!CategoryExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Category")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [ResponseType(typeof(RSupportCategory))]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            RSupportCategory rsc = await db.RSupportCategories.FindAsync(id);
            if (rsc == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Category")));
            }
            try
            {
                db.RSupportCategories.Remove(rsc);
                await db.SaveChangesAsync();
                return Ok(rsc);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }
        private bool CategoryExists(int id)
        {
            return db.RSupportCategories.Count(e => e.Id == id) > 0;
        }
        #endregion
        #region QuickTicket
        [ResponseType(typeof(RSupportQuickTicket))]
        public async Task<IHttpActionResult> PostSupportQuickTicket(RSupportQuickTicket RSupportQuickTicket)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "BRAND")));
            }
            try
            {
                db.RSupportQuickTickets.Add(RSupportQuickTicket);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.StackTrace.ToString()));//GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = RSupportQuickTicket.Id }, RSupportQuickTicket);
        }
        [ResponseType(typeof(RSupportCategory))]
        public async Task<IHttpActionResult> GetSupportQuickTicket(int id)
        {
            var Qtkt = db.RSupportQuickTickets.Where(p => p.Id == id).Select(x => new { x.Id,x.RsqtCategoryId, x.RsqtUILabel, x.RsqtSummary,x.RsqtTicketDescription }).FirstOrDefault();
            if (Qtkt == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "QUICK TICKET")));
            }
            return Ok(Qtkt);
        }
        public IHttpActionResult GetAllSupportQuickTickets()
        {
            var xx = (from aa in db.RSupportQuickTickets.Include(c => c.RSupportCategory)
                      select new { aa.Id,aa.RSupportCategory,aa.RSupportCategory.RscName, aa.RsqtUILabel, aa.RsqtSummary,aa.RsqtTicketDescription }).OrderBy(p => p.RsqtUILabel);
            
            return Ok(xx);
        }
        public IHttpActionResult GetQuickTicketDropdownData()
        {
            var data = db.RSupportQuickTickets.Select(x => new { x.RsqtUILabel, x.Id }).OrderBy(p => p.RsqtUILabel).AsEnumerable();
            return Ok(data);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateQuickTicket(int id, RSupportQuickTicket Rsqt)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "QuickTicket")));
            }

            if (id != Rsqt.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "QuickTicket")));
            }
            try
            {
                db.Entry(Rsqt).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!QuickTicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "QuickTicket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [ResponseType(typeof(RSupportQuickTicket))]
        public async Task<IHttpActionResult> DeleteQuickTicket(int id)
        {
            RSupportQuickTicket rsqt = await db.RSupportQuickTickets.FindAsync(id);
            if (rsqt == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "QuickTicket")));
            }
            try
            {
                db.RSupportQuickTickets.Remove(rsqt);
                await db.SaveChangesAsync();
                return Ok(rsqt);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }
        private bool QuickTicketExists(int id)
        {
            return db.RSupportQuickTickets.Count(e => e.Id == id) > 0;
        }

        #endregion

        #region TicketStages

        [ResponseType(typeof(RTicketStage))]
        public async Task<IHttpActionResult> PostTicketStages(RTicketStage RTicketStage)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Support Ticket Stage")));
            }
            try
            {
                db.RTicketStages.Add(RTicketStage);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = RTicketStage.Id }, RTicketStage);
        }

        [ResponseType(typeof(RTicketStage))]
        public async Task<IHttpActionResult> GetRTicketStageDetails(int id)
        {
            var STckStg = db.RTicketStages.Where(p => p.Id == id).Select(x => new { x.Id, x.RtsName, x.RtsDescription, x.RtsIsActive}).FirstOrDefault();
            if (STckStg == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket Stage")));
            }
            return Ok(STckStg);
        }
        public IHttpActionResult GetAllRTicketStagesList()
        {
            var xx = (from aa in db.RTicketStages
                      select new { aa.Id, aa.RtsName, aa.RtsDescription,aa.RtsIsActive }).OrderBy(p => p.RtsName).AsEnumerable();

            return Ok(xx);
        }
        public IHttpActionResult GetSupportTicketStagesDropdownData()
        {
            //var data = db.RTicketStages.Select(x => new { x.Id,x.RtsName }).OrderBy(p => p.RtsName).AsEnumerable();

            var data = db.RTicketStages.Where(p => p.RtsIsActive == true).Select(x => new { x.RtsName, x.Id, x.RtsDescription }).OrderBy(p => p.RtsName).AsEnumerable();
            return Ok(data);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateTicketStage(int id, RTicketStage Rtktsg)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Support Ticket Stage")));
            }

            if (id != Rtktsg.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Support Ticket Stage")));
            }
            try
            {
                db.Entry(Rtktsg).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!TicketStageExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket Stage")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(RTicketStage))]
        public async Task<IHttpActionResult> DeleteRTktStage(int id)
        {
            RTicketStage rsqt = await db.RTicketStages.FindAsync(id);
            if (rsqt == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Stage")));
            }
            try
            {
                db.RTicketStages.Remove(rsqt);
                await db.SaveChangesAsync();
                return Ok(rsqt);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }
        private bool TicketStageExists(int id)
        {
            return db.RTicketStages.Count(e => e.Id == id) > 0;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("UQ_RSubChannels_ReasonText_CompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return ("Can not insert duplicate Sub Channel");
            //else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}