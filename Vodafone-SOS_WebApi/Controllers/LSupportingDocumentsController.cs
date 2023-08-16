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
    public class LSupportingDocumentsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LSupportingDocuments
        public IHttpActionResult GetLSupportingDocuments(string EntityType,int EntityId)
        {
            var xx = (from aa in db.LSupportingDocuments.Where(p=>p.LsdEntityId==EntityId).Where(p=>p.LsdEntityType==EntityType)
                      select new { aa.Id, aa.LsdFileName,aa.LsdFilePath }).OrderBy(p => p.LsdFileName);
            return Ok(xx);
        }

        //Delete Supporting Documents
        // DELETE: api/LSupportingDocuments/5
        [ResponseType(typeof(LSupportingDocument))]
        public async Task<IHttpActionResult> DeleteLSupportingDocument(int id)
        {
            LSupportingDocument LSupportingDocument = await db.LSupportingDocuments.FindAsync(id);
            if (LSupportingDocument == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE")));
            }
            try
            {
                db.LSupportingDocuments.Remove(LSupportingDocument);
                await db.SaveChangesAsync();
                return Ok(LSupportingDocument);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }

        // Post: api/LSupportingDocuments
        [HttpPost]
        public IHttpActionResult PostLSupportingDocuments(string EntityType, int EntityId,string FileNameList,string FilePath,string LoggedInUserId)
        {
            var FileLIst = FileNameList.Split(',').ToList();
            foreach (var file in FileLIst)
            {
                var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LoggedInUserId, LsdUpdatedById = LoggedInUserId, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = EntityType, LsdEntityId = EntityId, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                db.LSupportingDocuments.Add(LSupportingDocuments);
                db.SaveChanges();
            }
            return Ok();
        }


        [HttpGet]
        public IHttpActionResult UpdateAttachmentTicket(int id, string FileName, string FilePath, string CreatedBy, string entitytype)
        {
            try
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    var FilesArray = FileName.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        //var entitytype = "LUsers";
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = CreatedBy, LsdUpdatedById = CreatedBy, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = entitytype, LsdEntityId = id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                    }
                }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LSupportingDocumentExists(int id)
        {
            return db.LSupportingDocuments.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
           

            return SqEx.Message;
        }
    }
}
