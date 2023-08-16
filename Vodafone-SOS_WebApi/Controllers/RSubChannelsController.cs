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
    public class RSubChannelsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult GetRSubChannelsDropdownData(int CompanyId)
        {
            var data = db.RSubChannels.Where(p => p.RscChannelId == CompanyId).Where(p=>p.RscIsActive==true).Select(x => new {x.RscIsActive, x.RscName, x.Id }).OrderBy(p => p.RscName).AsEnumerable();
            return Ok(data);
        }

        public IHttpActionResult GetRSubChannelsDropdownDataByChannelId(int ChannelId)
        {
            var data = db.RSubChannels.Where(p => p.RscChannelId==ChannelId).Where(p => p.RscIsActive==true).Select(x => new { x.RscName, x.Id }).OrderBy(p => p.RscName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RSubChannels?CompanyId=5
        public IHttpActionResult GetRSubChannelsByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RSubChannels.Include(c => c.RChannel.GCompany).Where(p => p.RChannel.RcCompanyId == CompanyId)
                      select new { aa.RscIsActive, aa.Id, aa.RscChannelId, aa.RscDescription,aa.RscName, aa.RChannel.RcName,aa.RChannel.GCompany.GcCompanyName }).OrderByDescending(p =>p.RscIsActive).ThenBy(p=>p.RcName).ThenBy(p=>p.RscName);
            return Ok(xx);
        }

        // GET: api/RSubChannels/5
        [ResponseType(typeof(RSubChannel))]
        public async Task<IHttpActionResult> GetRSubChannel(int id)
        {
            var RSubChannel = db.RSubChannels.Where(p => p.Id == id).Include(c => c.RChannel).Select(x => new {x.RscIsActive, x.Id, x.RChannel.RcName, x.RscChannelId, x.RscDescription,x.RscName }).FirstOrDefault();
            if (RSubChannel == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SubChannelName")));
            }
            return Ok(RSubChannel);
        }

        // PUT: api/RSubChannels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRSubChannel(int id, RSubChannel RSubChannel)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SubChannelName")));
            }

            if (id != RSubChannel.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SubChannelName")));
            }
            try
            {
                db.Entry(RSubChannel).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!RSubChannelExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SubChannelName")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RSubChannels
        [ResponseType(typeof(RSubChannel))]
        public async Task<IHttpActionResult> PostRSubChannel(RSubChannel RSubChannel)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "SubChannelName")));
            }
            try
            {
                db.RSubChannels.Add(RSubChannel);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
           // return CreatedAtRoute("DefaultApi", new { id = RSubChannel.Id }, RSubChannel);
        }

        // DELETE: api/RSubChannels/5
        [ResponseType(typeof(RSubChannel))]
        public async Task<IHttpActionResult> DeleteRSubChannel(int id)
        {
            RSubChannel RSubChannel = await db.RSubChannels.FindAsync(id);
            if (RSubChannel == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
            }
            try
            {
                db.RSubChannels.Remove(RSubChannel);
                await db.SaveChangesAsync();
                return Ok(RSubChannel);
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

        private bool RSubChannelExists(int id)
        {
            return db.RSubChannels.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("UQ_RSubChannels_ReasonText_CompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return ("Can not insert duplicate Sub Channel");
            if (SqEx.Message.IndexOf("FK_RSubChannels_LPayees", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SUB CHANNEL", "PAYEE"));
           else if (SqEx.Message.IndexOf("VSubChannels", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SUB CHANNEL", "VIEW(S)"));

            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
