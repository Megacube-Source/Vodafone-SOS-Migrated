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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;


namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class AspNetRolesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

       public IHttpActionResult GetAspnetFinOpsRoles(string CompanyCode)
        {
            //List of roles provided by JS which are  shown in FinOps
            var xx = (from aa in db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode).Where(p =>p.Name.Equals("Channel Manager",StringComparison.OrdinalIgnoreCase))
                      select new { aa.Id, aa.Name }).OrderBy(p => p.Name).AsEnumerable();
            return Ok(xx);
        }


        //Update the Roles For Which MFA will be enabled
        [HttpGet ]
        public async Task<IHttpActionResult> UpdateMFAForRoles(string RoleList,string CompanyCode)
        {
            //check if RoleList s empty
            var RoleArray = new List<string>();
            if (!string.IsNullOrEmpty(RoleList))
            {
                //Split the string to List
                RoleArray = RoleList.Split(',').ToList();

            }
                var RolesInOpco = db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode).ToList();
                foreach(var role in RolesInOpco)
                {
                    if(RoleArray.Contains(role.Id))
                    {
                        role.MFAEnabled = true;
                    }
                    else
                    {
                        role.MFAEnabled = false;
                    }
                    db.Entry(role).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            return Ok();
        }

        public IHttpActionResult GetMFARoles(string CompanyCode)
        {
            //List of roles provided by JS which are  shown in FinOps
            var xx = (from aa in db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode)
                      select new { aa.Id, aa.Name , Selected =aa.MFAEnabled}).OrderBy(p => p.Name).AsEnumerable();
            return Ok(xx);
        }

        // Method to Display Listbox of Roles on Users Create Page. Edit Page List box is in LUsers controller.
        public IHttpActionResult GetAspNetRoles(string CompanyCode)
        {
            //List of roles provided by JS which are not shown in Listbox
            var ExcludedRoles =new string[] { "BI", "Finance Decision Support", "Group Admin", "L2 Admin", "MDM Analyst", "OpCoAdmin","Payee" };
            var xx = (from aa in db.AspNetRoles.Where(p=>p.CompanyCode==CompanyCode).Where(p=>ExcludedRoles.Contains(p.Name)==false)
                      select new {aa.Id,aa.Name }).OrderBy(p=>p.Name).AsEnumerable();
            return Ok(xx);
        }


        // GET: api/GetAspnetRoleId
        public IHttpActionResult GetAspnetRoleId(string CompanyCode,string UserRole)
        {
            var xx = db.AspNetRoles.Where(p => p.Name == UserRole).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault().Id;
            int UserRoleId = Convert.ToInt32(xx);
            return Ok(UserRoleId);
        }

        public IHttpActionResult GetClaimsAnalystByCompanyId(int CompanyId)
        {
            var xx = db.Database.SqlQuery<AspnetUserViewModel>("select AU.UserName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where AR.Name='Claims Analyst'  and AU.GcCompanyId="+CompanyId).ToList();
            return Ok(xx);
        }
       
        public IHttpActionResult GetManagerByCompanyId(int CompanyId)
        {
            var xx = db.Database.SqlQuery<AspnetUserViewModel>("select AU.UserName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where AR.Name='Manager'  and AU.GcCompanyId=" + CompanyId).ToList();
            return Ok(xx);
        }
        //select list of users based on a role named as claims analyst to display dropdown of claims analyst user
        public IHttpActionResult GetAspNetUsers()
        {
            var xx = (from aa in db.AspNetUsers select new {aa.Id,aa.UserName }).OrderBy(p=>p.UserName);
            //var RawQuery = db.Database.SqlQuery<long>("SELECT Id,UserName from AspNetUsers." + Claim.GkvValue);
            //    var Task = RawQuery.SingleAsync();
            //    LClaim.LcClaimId = Task.Result;
            return Ok(xx);
        }

        // GET: api/GCompanies/5
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> GetAspNetRole(string id)
        {
            var Role = db.AspNetRoles.Where(p => p.Id == id).Select(x => new { x.Id, x.Name }).FirstOrDefault();
            if (Role == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
            }
            return Ok(Role);
        
        }

        public IHttpActionResult GetRolesByCompanyId(int CompanyId)
        {
            var xx = (from ar in db.AspNetRoles
                      join gc in db.GCompanies.Where(p => p.Id == CompanyId)
                      on ar.CompanyCode equals gc.GcCode
                      select new
                      {
                          ar.Name,
                          ar.Id
                      });

            return Ok(xx);
        }

        // PUT: api/GCompanies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAspNetRole(string id, AspNetRole AspNetRole)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
            }

            if (id != AspNetRole.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
            }
            try
            {
                //db.Database.ExecuteSqlCommand("update AspNetRoles set Name="+AspNetRole.Name +"Where Id="+id);
                db.Entry(AspNetRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                //var result = RoleManager.Update(new IdentityRole(AspNetRole.Name));
            }
            catch (Exception ex)
            {
               // Globals.SendEmail("ssharma@megacube.com.au",null, "Vodafone-SOS WebApi", ex.Message + " " + ex.StackTrace);
                if (!AspNetRoleExists(Convert.ToInt32(id)))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/GCompanies
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> PostAspNetRole(AspNetRole AspNetRole)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ROLE")));
            }
            var Role=new IdentityRole();
            try
            {
                var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                var result = RoleManager.Create(new IdentityRole(AspNetRole.Name));
                Role = RoleManager.FindByName(AspNetRole.Name);
                //db.AspNetRoles.Add(AspNetRole);
                //await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               // Globals.SendEmail("ssharma@megacube.com.au",null, "Vodafone-SOS WebApi", ex.Message + " " + ex.StackTrace);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = Role.Id }, AspNetRole);
        }

        // DELETE: api/GCompanies/5
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> DeleteAspNetRole(string id)
        {
            AspNetRole AspNetRole = await db.AspNetRoles.FindAsync(id);
            if (AspNetRole == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
            }
            try
            {
                //db.Database.ExecuteSqlCommand("delete from AspNetRoles where Id="+id);
                db.AspNetRoles.Remove(AspNetRole);
                await db.SaveChangesAsync();
                //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                //var result =await RoleManager.DeleteAsync(new IdentityRole(AspNetRole.Name));
                return Ok(AspNetRole);
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

        private bool AspNetRoleExists(int id)
        {
            return db.GCompanies.Count(e => e.Id == id) > 0;
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
           
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LBatches", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "BATCHES"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LChangeRequests", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "CHANGE REQUESTS"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LClaims", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "CLAIMS"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LPayees", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "PAYEES"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LRefFiles_RequesterRoleId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "REFERENCE FILES"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LSchemes_RequesterRoleId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "SCHEMES"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LUsers", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "USERS"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_LWorkFlowConfig", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "WORK FLOWS"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_MAspnetRolesGAuthorizableObjects", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "AUTHORIZABLE OBJECTS"));
           
            if (SqEx.Message.IndexOf("FK_ASpnetRoles_MGMenusAspnetRoles", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "MENUS"));
            if (SqEx.Message.IndexOf("FK_AspnetRoles_RSupportTeams", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ROLES", "SUPPORT TEAM"));
            //UNIQUE CONSTRAINT WILL ALSO COME
            else
            return SqEx.Message;
        }
    }
}
