//Code Review Convert SQL to LINQ completed
using System;
using System.Collections.Generic;
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
using System.Data.Entity.Validation;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LDropDownsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LDropDowns?CompanyId=5
        public IHttpActionResult GetLDropDownsByCompanyId(int CompanyId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LDropDowns.Where(p => p.LdCompanyId == CompanyId).Include(c => c.GCompany)
                      select new { aa.Id, aa.LdDescription, aa.LdName, aa.LdCompanyId }).OrderByDescending(p => p.LdName);
            return Ok(xx);
        }

        // GET: api/LDropDowns?CompanyId=5
        public IHttpActionResult GetLDropDownCountsByCompanyId(int CompanyId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LDropDowns.Where(p => p.LdCompanyId == CompanyId).Include(c => c.GCompany)
                     select aa).Count();
            return Ok(xx);
        }

        // GET: api/LDropDowns/5
        [ResponseType(typeof(LDropDown))]
        public async Task<IHttpActionResult> GetLDropDown(int id, string UserName, string Workflow)
        {
            var LDropDown = db.LDropDowns.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new { x.Id, x.LdDescription, x.LdName, x.LdCompanyId }).FirstOrDefault();
            if (LDropDown == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DropDown")));
            }
            return Ok(LDropDown);
        }

        // PUT: api/LDropDowns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLDropDown(int id, LDropDown LDropDown, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown")));
            }

            if (id != LDropDown.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown")));
            }
          
                try
                {
                    db.Entry(LDropDown).State = EntityState.Modified;
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

        // POST: api/LDropDowns
        [ResponseType(typeof(LDropDown))]
        public async Task<IHttpActionResult> PostLDropDown(LDropDown LDropDown, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "DropDown")));
            }
          
                try
                {
                if (db.LDropDowns.Where(p => p.Id == LDropDown.Id).Where(p=>p.LdCompanyId==LDropDown.LdCompanyId).Count() > 0)
                {
                    db.Entry(LDropDown).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LDropDown.Id = 0;//To override the Id generated by grid
                    db.LDropDowns.Add(LDropDown);
                    await db.SaveChangesAsync();
                }

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

            return Ok();
            //return CreatedAtRoute("DefaultApi", new { id = LDropDown.Id }, LDropDown);
        }

        // DELETE: api/LDropDowns/5
        [ResponseType(typeof(LDropDown))]
        public async Task<IHttpActionResult> DeleteLDropDown(int id, string UserName, string Workflow,int CompanyId)
        {
            LDropDown LDropDown = db.LDropDowns.Where(p=>p.Id==id).Where(p=>p.LdCompanyId==CompanyId).FirstOrDefault();
            try
            {
                if (LDropDown != null)
                {
                    db.LDropDowns.Remove(LDropDown);
                    await db.SaveChangesAsync();
                }

                return Ok(LDropDown);
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

        private bool LDropDownExists(int id)
        {
            return db.LDropDowns.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_LDropDowns_LdName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "DropDown Name"));
            else if (SqEx.Message.IndexOf("FK_LDropDowns_LDropDownValues", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DropDown", "Values"));

            else
            {
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
    }
}
