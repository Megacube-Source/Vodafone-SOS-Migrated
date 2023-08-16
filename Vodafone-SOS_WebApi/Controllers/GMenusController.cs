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
    public class GMenusController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/GMenus
        public IHttpActionResult GetGMenus()
        {
            var xx = (from aa in db.GMenus
                      select new { aa.Id, aa.GmMenuName,aa.GmParentId,aa.GmMenuUrl,aa.GmOrdinalPosition }).OrderBy(p => p.GmMenuName);
            return Ok(xx);
        }

        // GET: api/GMenus/5
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> GetGMenu(int id)
        {
            var GMenu = db.GMenus.Where(p => p.Id == id).Select(x => new { x.Id, x.GmMenuName,x.GmParentId,x.GmMenuUrl,x.GmOrdinalPosition }).FirstOrDefault();
            if (GMenu == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            return Ok(GMenu);
        }

        // PUT: api/GMenus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGMenu(int id, GMenu GMenu)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }

            if (id != GMenu.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }
            try
            {
                db.Entry(GMenu).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!GMenuExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/GMenus
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> PostGMenu(GMenu GMenu)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "MENU")));
            }
            try
            {
                db.GMenus.Add(GMenu);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = GMenu.Id }, GMenu);
        }

        // DELETE: api/GMenus/5
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> DeleteGMenu(int id)
        {
            GMenu GMenu = await db.GMenus.FindAsync(id);
            if (GMenu == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            try
            {
                db.GMenus.Remove(GMenu);
                await db.SaveChangesAsync();
                return Ok(GMenu);
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

        private bool GMenuExists(int id)
        {
            return db.GMenus.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_GMenusGMenus_Id", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "MENU", "CHIELD MENU"));
            else if (SqEx.Message.IndexOf("FK_GMenus_MGMenusAspnetRoles", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "MENU", "MENU"));
       
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
