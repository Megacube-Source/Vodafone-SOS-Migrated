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
    public class LEmailTemplatesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LEmailTemplates
        public IHttpActionResult GetLEmailTemplates()
        {
            var xx = (from aa in db.LEmailTemplates
                      select new { aa.Id,aa.LetTemplateName, aa.LetEmailSubject,aa.LetEmailBody,aa.LetSignature,aa.LetCompanyId }).OrderBy(p => p.LetTemplateName);
            return Ok(xx);
        }

        // GET: api/LEmailTemplates/5
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> GetLEmailTemplate(int id)
        {
            var Template = db.LEmailTemplates.Where(p => p.Id == id).Select(x => new { x.Id, x.LetTemplateName,x.LetEmailSubject,x.LetEmailBody,x.LetSignature,x.LetCompanyId }).FirstOrDefault();
            if (Template == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Template")));
            }
            return Ok(Template);
        }

        // PUT: api/LEmailTemplates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLEmailTemplate(int id, LEmailTemplate EmailTemplate)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COMPANY")));
            }

            if (id != EmailTemplate.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COMPANY")));
            }
            try
            {
                db.Entry(EmailTemplate).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!EmailTemplateExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COMPANY")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LEmailTemplates
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> PostLEmailTemplate(LEmailTemplate EmailTemplate)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "EmailTemplate")));
            }
            try
            {
                //As soon as company is created a database trigger will automatically Create a sequence for this company which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.LEmailTemplates.Add(EmailTemplate);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = EmailTemplate.Id }, EmailTemplate);
        }

        // DELETE: api/LEmailTemplates/5
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> DeleteLEmailTemplate(int id)
        {
            LEmailTemplate EmailTemplate = await db.LEmailTemplates.FindAsync(id);
            if (EmailTemplate == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "EmailTemplate")));
            }
            try
            {
                db.LEmailTemplates.Remove(EmailTemplate);
                await db.SaveChangesAsync();
                return Ok(EmailTemplate);
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

        private bool EmailTemplateExists(int id)
        {
            return db.LEmailTemplates.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
             return SqEx.Message;
        }
    }

}
