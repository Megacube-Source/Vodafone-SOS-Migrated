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
using System.Data.Entity.Validation;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class GCompaniesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/GCompanies
        public IHttpActionResult GetGCompanies(string UserName,string Workflow)
        {
           // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            var xx = (from aa in db.GCompanies
                      select new { aa.Id, aa.GcCompanyName, aa.GcCode}).OrderBy(p => p.GcCompanyName);
            return Ok(xx);
        }

        // GET: api/GCompanies/5
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> GetGCompany(int id,string UserName, string Workflow)
        {
           // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            var GCompany = db.GCompanies.Where(p => p.Id == id).Select(x => new { x.Id, x.GcCompanyName,x.GcCode}).FirstOrDefault();
            if (GCompany == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COMPANY")));
            }
            return Ok(GCompany);
        }

        // PUT: api/GCompanies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGCompany(int id, GCompany GCompany,string UserName, string Workflow)
        {
            //System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COMPANY")));
            }

            if (id != GCompany.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COMPANY")));
            }
            try
            {
                db.Entry(GCompany).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/GCompanies
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> PostGCompany(GCompany GCompany,string UserName, string Workflow)
        {
           // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "COMPANY")));
            }
            try
            {
                //As soon as company is created a database trigger will automatically Create a sequence for this company which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.GCompanies.Add(GCompany);
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                   
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = GCompany.Id }, GCompany);
        }

        // DELETE: api/GCompanies/5
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> DeleteGCompany(int id,string UserName, string Workflow)
        {
           // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            GCompany GCompany = await db.GCompanies.FindAsync(id);
            if (GCompany == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COMPANY")));
            }
            try
            {
                db.GCompanies.Remove(GCompany);
                await db.SaveChangesAsync();
                return Ok(GCompany);
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
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

        private bool GCompanyExists(int id)
        {
            return db.GCompanies.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_GCompanies_AspnetUsers", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "USERS"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RActiveTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "ACTIVE TYPES"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RBrands", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "BRANDS"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RCommissionTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "COMMISSION TYPES"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RDeviceTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "DEVICE TYPES"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_GKeyValues", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "KEY VALUES"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_LCompanyspecificLabels", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "Form Labels"));
            else if (SqEx.Message.IndexOf("FK_LCompanies_LRawDataTypes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "RAW DATA TYPES"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RChannels", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "CHANNELS"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RProductCodes", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "PRODUCT CODE"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RRejectionReasons", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "REJECTION REASON"));
            else if (SqEx.Message.IndexOf("FK_GCompanies_RSupportCategories_RscCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "SUPPORT CATEGORIES"));
            else if (SqEx.Message.IndexOf("UQ_GCompanies_GCCompanyName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "COMPANY", "COMPANY NAME"));

            else
            {
                //Something else failed return original error message as retrieved from database
                //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //Globals.LogError(SqEx.Message, ErrorDesc);
                return Globals.SomethingElseFailedInDBErrorMessage;
            }
        }

        public IHttpActionResult GetGCompaniesForAuditor(string UserName, string Workflow)
        {
            // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            //var xx = db.GCompanies.Where(x=>x.GcCode)
            var usersdate = db.LUsers.Where(x => x.LuEmail == UserName).FirstOrDefault();
            var xx = ( from aa in db.AspNetRoles
                        join bb in db.AspNetUserRoles on aa.Id equals bb.RoleId
                        join  cc in db.GCompanies    on aa.CompanyCode equals cc.GcCode
                        where aa.Name == "Auditor" && bb.UserId == usersdate.LuUserId
                       select new { cc.Id, cc.GcCompanyName, cc.GcCode }).OrderBy(p => p.GcCompanyName);
            return Ok(xx);
        }

        public IHttpActionResult GetGCompaniesForAuditor(string UserName, int Companyid)
        {
            // System.Web.HttpContext.Current.Session["UserName"] = UserName;//set UserName in Session
            //var xx = db.GCompanies.Where(x=>x.GcCode)
            var usersdate = db.LUsers.Where(x => x.LuEmail == UserName).FirstOrDefault();
            var compantdetails = db.GCompanies.Where(x => x.Id == Companyid).FirstOrDefault();
            var xx = (from aa in db.AspNetRoles
                      join bb in db.AspNetUserRoles on aa.Id equals bb.RoleId
                      join cc in db.GCompanies on aa.CompanyCode equals cc.GcCode
                      where aa.Name == "Auditor" && bb.UserId == usersdate.LuUserId && aa.CompanyCode == compantdetails.GcCode
                      select new { cc.GcCompanyName, cc.GcCode, aa.Id }).OrderBy(p => p.GcCompanyName).FirstOrDefault();
            return Ok(xx);
        }
    }

}
