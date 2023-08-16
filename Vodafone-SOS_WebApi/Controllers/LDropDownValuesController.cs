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
    public class LDropDownValuesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LDropDownValues?DropDownId=5
        public IHttpActionResult GetLDropDownValuesByDropDownId(int DropDownId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LDropDownValues.Where(p => p.LdvDropDownId == DropDownId).Include(c => c.LDropDown)
                      select new { aa.Id, aa.LDropDown.LdName,aa.LdvDescription,aa.LdvDropDownId,aa.LdvValue }).OrderBy(p => p.LdvValue);
            return Ok(xx);
        }

        // GET: api/LDropDownValues?DropDownId=5
        public IHttpActionResult GetLDropDownValueCountsByDropDownId(int DropDownId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LDropDownValues.Where(p => p.LdvDropDownId == DropDownId).Include(c => c.LDropDown)
                      select aa).Count();
            return Ok(xx);
        }

        // GET: api/LDropDownValues/5
        [ResponseType(typeof(LDropDownValue))]
        public async Task<IHttpActionResult> GetLDropDownValue(int id, string UserName, string Workflow)
        {
            var LDropDownValue = db.LDropDownValues.Where(p => p.Id == id).Select(x => new { x.Id, x.LdvDropDownId,x.LDropDown.LdName,x.LdvValue,x.LdvDescription }).FirstOrDefault();
            if (LDropDownValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DropDown Value")));
            }
            return Ok(LDropDownValue);
        }

        // PUT: api/LDropDownValues/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLDropDownValue(int id, LDropDownValue LDropDownValue, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown Value")));
            }

            if (id != LDropDownValue.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown Value")));
            }

            try
            {
                db.Entry(LDropDownValue).State = EntityState.Modified;
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

        // POST: api/LDropDownValues
        [ResponseType(typeof(LDropDownValue))]
        public async Task<IHttpActionResult> PostLDropDownValue(LDropDownValue LDropDownValue, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "DropDown Value")));
            }

            try
            {
                if (db.LDropDownValues.Where(p => p.Id == LDropDownValue.Id).Where(p=>p.LdvDropDownId==LDropDownValue.LdvDropDownId).Count() > 0)
                {
                    db.Entry(LDropDownValue).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LDropDownValue.Id = 0;//Reset the dropdown value id coming from model
                    db.LDropDownValues.Add(LDropDownValue);
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
            //return CreatedAtRoute("DefaultApi", new { id = LDropDownValue.Id }, LDropDownValue);
        }

        // DELETE: api/LDropDownValues/5
        [ResponseType(typeof(LDropDownValue))]
        public async Task<IHttpActionResult> DeleteLDropDownValue(int id, string UserName, string Workflow,int DropdownId)
        {
            LDropDownValue LDropDownValue =  db.LDropDownValues.Where(p=>p.Id==id).Where(p=>p.LdvDropDownId==DropdownId).FirstOrDefault();
           
            try
            {
                if (LDropDownValue != null)
                {
                    db.LDropDownValues.Remove(LDropDownValue);
                    await db.SaveChangesAsync();
                }

                return Ok(LDropDownValue);
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

        private bool LDropDownValueExists(int id)
        {
            return db.LDropDownValues.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
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
