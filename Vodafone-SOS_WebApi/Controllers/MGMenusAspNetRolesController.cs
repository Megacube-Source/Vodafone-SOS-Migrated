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
    public class MGMenusAspnetRolesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/MMGMenusAspnetRolesAspnetRoles
        //public IHttpActionResult GetMGMenusAspnetRoles()
        //{
        //    var xx = (from aa in db.MGMenusAspnetRoles.Include(p=>p.GMenu)
        //              select new { aa.Id, aa.MgmarMenuId,aa.MgmarRoleId,aa.GMenu.GmMenuName,aa.AspNetRole.Name,aa.GMenu.GmParentId,aa.GMenu.GmMenuUrl,aa.GMenu.GmOrdinalPosition }).OrderBy(p => p.GmMenuName);
        //    return Ok(xx);
        //}

        //Get Menus as per role passed as parameter
        public IHttpActionResult GetMGMenusAspnetRolesByUserRole(string UserRole,string CompanyCode)
        {
            var xx = (from aa in db.MGMenusAspnetRoles.Include(p => p.GMenu).Include(p=>p.AspNetRole).Where(p=>p.AspNetRole.Name.Equals(UserRole)).Where(p=>p.AspNetRole.CompanyCode==CompanyCode)
                      select new { aa.Id, aa.MgmarMenuId, aa.MgmarRoleId, aa.GMenu.GmMenuName, aa.AspNetRole.Name, aa.GMenu.GmParentId, aa.GMenu.GmMenuUrl, aa.GMenu.GmOrdinalPosition }).OrderBy(p => p.GmOrdinalPosition);
            return Ok(xx);
        }

        // GET: api/MMGMenusAspnetRolesAspnetRoles/5
        [ResponseType(typeof(MGMenusAspnetRole))]
        public async Task<IHttpActionResult> GetMGMenusAspnetRole(int id)
        {
            var MGMenusAspnetRole = db.MGMenusAspnetRoles.Include(p=>p.GMenu).Where(p => p.Id == id).Select(x => new {x.GMenu.GmOrdinalPosition, x.Id,x.GMenu.GmMenuUrl, x.GMenu.GmMenuName,x.MgmarMenuId,x.GMenu.GmParentId,x.MgmarRoleId,x.AspNetRole.Name }).FirstOrDefault();
            if (MGMenusAspnetRole == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            return Ok(MGMenusAspnetRole);
        }

        // PUT: api/MMGMenusAspnetRolesAspnetRoles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMGMenusAspnetRole(int id, MGMenusAspnetRole MGMenusAspnetRole)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }

            if (id != MGMenusAspnetRole.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }
            try
            {
                db.Entry(MGMenusAspnetRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!MGMenusAspnetRoleExists(id))
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

        // POST: api/MMGMenusAspnetRolesAspnetRoles
        [ResponseType(typeof(MGMenusAspnetRole))]
        public async Task<IHttpActionResult> PostMGMenusAspnetRole(MGMenusAspnetRole MGMenusAspnetRole)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "MENU")));
            }
            try
            {
                db.MGMenusAspnetRoles.Add(MGMenusAspnetRole);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = MGMenusAspnetRole.Id }, MGMenusAspnetRole);
        }

        // DELETE: api/MMGMenusAspnetRolesAspnetRoles/5
        [ResponseType(typeof(MGMenusAspnetRole))]
        public async Task<IHttpActionResult> DeleteMGMenusAspnetRole(int id)
        {
            MGMenusAspnetRole MGMenusAspnetRole = await db.MGMenusAspnetRoles.FindAsync(id);
            if (MGMenusAspnetRole == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            try
            {
                db.MGMenusAspnetRoles.Remove(MGMenusAspnetRole);
                await db.SaveChangesAsync();
                return Ok(MGMenusAspnetRole);
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

        private bool MGMenusAspnetRoleExists(int id)
        {
            return db.MGMenusAspnetRoles.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_GMenus_MGMenusAspnetRoles", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "MENU", "ROLES"));
            if (SqEx.Message.IndexOf("UQ_MGMenusAspnetRoles_RoleId_MenuId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "MENUS"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
