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
    public class LRefFilesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LRefFiles
        //public IHttpActionResult GetLRefFilesByUserId(int UserId)
        //{
        //    var xx = (from aa in db.LRefFiles.Where(p=>p.LufUserId==UserId).Include(c => c.LRefFileType)
        //              select new { aa.Id,aa.LufComments,aa.LufCreatedById,aa.LufCreatedDateTime,aa.LufDescription,aa.LufFileName,aa.LufUploadType,aa.LufPayeeId,
        //              aa.LufMonth,aa.LufRefFileTypeId,aa.LufStatus,aa.LufUPdatedById,aa.LufUpdatedDateTime,
        //              aa.LufYear}).OrderBy(p => p.LufFileName);
        //    return Ok(xx);
        //}

        //Method to get company specific data for dropdown in WebApplication
        //public IHttpActionResult GetLRefFilesDropdownData(int RefFileTypeId)
        //{
        //    var data = db.LRefFiles.Where(p =>p.LrfRefFileTypeId==RefFileTypeId).Select(x => new { x., x.Id }).OrderBy(p => p.LufFileName).AsEnumerable();
        //    return Ok(data);
        //}

        // GET: api/LRefFiles?CompanyId=5
        //public IHttpActionResult GetLRefFilesByCompanyId(int RefFileTypeId)
        //{
        //    var xx = (from aa in db.LRefFiles.Where(p =>p.LaUploadFileId==RefFileTypeId).Include(c =>c.LRefFileType).Include(p=>p.GCompany)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LufComments,
        //                  aa.LufCreatedById,
        //                  aa.LufCreatedDateTime,
        //                  aa.LufDescription,
        //                  aa.LufFileName,
        //                  aa.LufUploadType,
        //                  aa.LufPayeeId,aa.GCompany.GcCompanyName,aa.LRefFileType.LrftName,
        //              aa.LufMonth,aa.LufRefFileTypeId,aa.LufStatus,aa.LufUPdatedById,aa.LufUpdatedDateTime,
        //              aa.LufYear}).OrderBy(p => p.LufFileName);
        //    return Ok(xx);
        //}

        //public IHttpActionResult GetLRefFilesByStatusName(string StatusName, string UploadType, string ReportsToId)
        //{
        //    var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LRefFiles.Where(p => p.LufStatus == StatusName).Where(p => p.LufUploadType == UploadType).Include(c => c.LRefFileType).Include(p => p.GCompany)
        //              join tt in db.LUsers on aa.LufUPdatedById equals tt.LuUserId
        //              into grp
        //              from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
        //              where c.LuReportsToId==ReportsToId ||aa.LufCreatedById==ReportsToId
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LufComments,
        //                  aa.LufCreatedById,
        //                  aa.LufCreatedDateTime,
        //                  aa.LufDescription,
        //                  aa.LufFileName,
        //                  aa.LufMonth,
        //                  aa.LRefFileType.LrftName,
        //                  aa.LufRefFileTypeId,
        //                  aa.LufStatus,
        //                  aa.LufUPdatedById,
        //                  aa.LufUpdatedDateTime,aa.LufIsSchemeTested,aa.LufSchemeTestResults,
        //                  aa.LufYear,aa.LufUploadType,aa.GCompany.GcCompanyName,
        //                  CreatedBy = (aa.LufCreatedById != null) ? UserList.Where(p => p.Id == aa.LufCreatedById).FirstOrDefault().UserName : "",
        //                  UpdatedBy = (aa.LufUPdatedById != null) ? UserList.Where(p => p.Id == aa.LufUPdatedById).FirstOrDefault().UserName : "",
        //                  aa.LufPayeeId
        //              }).OrderBy(p => p.LufFileName);
        //    return Ok(xx);
        //}

        //public IHttpActionResult GetLRefFilesByStatusNameAndUserId(string StatusName,string UserId,string UploadType)
        //{
        //    var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LRefFiles.Where(p => p.LufStatus == StatusName).Where(p=>p.LufCreatedById==UserId).Where(p=>p.LufUploadType==UploadType).Include(c =>c.LRefFileType).Include(p=>p.LPayee)
        //              select new
        //              {
        //                  aa.Id,aa.LPayee.LpPayeeCode,aa.LufPayeeId,
        //                  aa.LufComments,
        //                  aa.LufCreatedById,
        //                  aa.LufCreatedDateTime,
        //                  aa.LufDescription,
        //                  aa.LufFileName,
        //                  aa.LufMonth,aa.LRefFileType.LrftName,
        //                  aa.LufRefFileTypeId,
        //                  aa.LufStatus,
        //                  aa.LufUPdatedById,
        //                  aa.LufUpdatedDateTime,
        //                  aa.LufIsSchemeTested,
        //                  aa.LufSchemeTestResults,
        //                  aa.LufYear,aa.GCompany.GcCompanyName,
        //                  CreatedBy = (aa.LufCreatedById != null) ? UserList.Where(p => p.Id == aa.LufCreatedById).FirstOrDefault().UserName : "",
        //                  UpdatedBy = (aa.LufUPdatedById != null) ? UserList.Where(p => p.Id == aa.LufUPdatedById).FirstOrDefault().UserName : "",
        //                  aa.LufUploadType
        //              }).OrderBy(p => p.LufFileName);
        //    return Ok(xx);
        //}

        // GET: api/LRefFiles/5
        [ResponseType(typeof(LRefFile))]
        public async Task<IHttpActionResult> GetLRefFile(int id, string UserName, string Workflow)
        {
            var LRefFile = (from x in db.LRefFiles.Where(p => p.Id == id)
                            join y in db.LRefFileTypes on x.LrfRefFileTypeId equals y.Id
                            select new
                            {
                                x.Id,
                                x.LrfDescription,
                               x.LrfRefFileTypeId,
                               x.LrfYear,x.LrfMonth,x.WFComments,
                                y.LrftName,
                                x.LrfRefFileName
                            }).FirstOrDefault();
            if (LRefFile == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE")));
            }
            return Ok(LRefFile);
        }

        //method to update LUploded files status and related data
        [ResponseType(typeof(void))]
        [HttpPut]
        public async Task<IHttpActionResult> PutUpdateStatus(LRefFile model,string Comments,string UpdatedBy, string UserName, string Workflow)
        {
            
            try
            {
                var LRefFile = db.LRefFiles.Find(model.Id);
                if(!string.IsNullOrEmpty(Comments))
                {
                    LRefFile.WFComments = "[" + UpdatedBy + "] [" + DateTime.UtcNow.Date.ToString("dd-MM-yyyy HH:mm") + "] " + Comments + Environment.NewLine + LRefFile.WFComments;
                }
                LRefFile.WFStatus = model.WFStatus;
                LRefFile.LrfUPdatedById = model.LrfUPdatedById;
                LRefFile.LrfUpdatedDateTime = model.LrfUpdatedDateTime;
                
                db.Entry(LRefFile).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LRefFileExists(model.Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/LRefFiles/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutLRefFile(int id, LRefFile LRefFile)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return BadRequest(ModelState);
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "REF FILE")));
        //    }

        //    if (id != LRefFile.Id)
        //    {
        //        //return BadRequest();
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "REF FILE")));
        //    }
        //    try
        //    {
        //        db.Entry(LRefFile).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!LRefFileExists(id))
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE")));
        //        }
        //        else
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //        }
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/LRefFiles

        public IHttpActionResult PostLRefFile(LRefFile LRefFile, string LoggedInRoleId,string AttachmentFilePath,string SupportingFiles, string UserName, string Workflow,string SupportingDocFilePath )
        {
           
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string AttachmentFiles = LRefFile.AttachedFilesName;

                       
                    //Note: Calculate Ordinal based on the Current Role who has created Ref Files based on RoleId and Opco and WorkflowName
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LRefFile.LrfCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LRefFile.LrfCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();

                    LRefFile.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LRefFile.WFRequesterId : null;
                    LRefFile.WFOrdinal = Ordinal;
                    LRefFile.WFType = WFDetails.RwfWFType;
                    LRefFile.WFStatus = "Saved";
                    db.LRefFiles.Add(LRefFile);
                    
                    db.SaveChanges();
                    
                    //Add portfolio
                    if (!string.IsNullOrEmpty(LRefFile.ParameterCarrier))
                    {
                        var PFArray = LRefFile.ParameterCarrier.Split(',').ToList();
                        foreach (var PF in PFArray)
                        {
                            var Id = Convert.ToInt32(PF);
                            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LRefFile.Id, MepEntityType = "LRefFiles" };
                            db.MEntityPortfolios.Add(MEntityPortfolio);
                            db.SaveChanges();
                        }
                    }

                    //add row in LAttachments for files are added while adding RefFiles
                    if (!string.IsNullOrEmpty(AttachmentFiles))
                    {
                        var FilesArray = AttachmentFiles.Split(',').ToList();
                        foreach (var file in FilesArray)
                        {
                            var LAttachments = new LAttachment { LaFileName = file, LaFilePath = AttachmentFilePath, LaCreatedById = LRefFile.LrfCreatedById, LaCreatedDateTime = DateTime.UtcNow, LaEntityId = LRefFile.Id, LaEntityType = "LRefFiles", LaUpdatedById = LRefFile.LrfUPdatedById, LaUpdatedDateTime = DateTime.UtcNow };
                            db.LAttachments.Add(LAttachments);
                            db.SaveChanges();
                        }
                    }

                    //add row in LSupporting Documents for files are added while adding RefFiles
                    if (!string.IsNullOrEmpty(SupportingFiles))
                    {
                        var SupportingFilesArray = SupportingFiles.Split(',').ToList();
                        foreach (var file in SupportingFilesArray)
                        {
                            var SupportingDocuments = new LSupportingDocument {LsdCreatedById=LRefFile.LrfCreatedById,LsdCreatedDateTime=DateTime.UtcNow,LsdEntityId=LRefFile.Id,LsdEntityType="LRefFiles",LsdFileName=file,LsdFilePath= SupportingDocFilePath, LsdUpdatedById=LRefFile.LrfCreatedById,LsdUpdatedDateTime=DateTime.UtcNow };
                            db.LSupportingDocuments.Add(SupportingDocuments);
                            db.SaveChanges();
                        }
                    }
                  //Added Trim for R2.4 Issue
                    if (LRefFile.LrfDescription.Length > 200)
                        LRefFile.LrfDescription = LRefFile.LrfDescription.Substring(0, 200);
                    //Add Entry in Audit Log
                    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                           "Create", LRefFile.LrfCreatedById, DateTime.UtcNow, LRefFile.WFStatus, LRefFile.WFStatus,
                          WFDetails.RwfBaseTableName, LRefFile.Id, Convert.ToString(LRefFile.LrfDescription), WFDetails.Id, LRefFile.WFCompanyId, string.Empty, LRefFile.WFRequesterRoleId,null);
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
                transaction.Commit();
            }
            return Ok(LRefFile.Id);
            //return CreatedAtRoute("DefaultApi", new { id = LRefFile.Id }, LRefFile);
        }

        // DELETE: api/LRefFiles/5
        [ResponseType(typeof(LRefFile))]
        public async Task<IHttpActionResult> DeleteLRefFile(int id, string UserName, string Workflow)
        {
            LRefFile LRefFile = await db.LRefFiles.FindAsync(id);
            if (LRefFile == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE")));
            }
            try
            {
                db.LRefFiles.Remove(LRefFile);
                await db.SaveChangesAsync();
                return Ok(LRefFile);
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

        private bool LRefFileExists(int id)
        {
            return db.LRefFiles.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("VUpdatedFiles", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "REF FILES", "VIEW(S)"));
            }
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
    }
}
