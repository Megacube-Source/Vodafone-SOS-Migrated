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
    public class LSchemesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
       
        public IHttpActionResult GetLSchemesDropdownData(int CompanyId, string UserName, string Workflow)
        {
            var data = db.LSchemes.Where(p => p.LsCompanyId == CompanyId).Select(x => new { x.LsName, x.Id }).OrderBy(p => p.LsName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/LSchemes?CompanyId=5
        public IHttpActionResult GetLSchemesByCompanyId(int CompanyId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LSchemes.Where(p => p.LsCompanyId == CompanyId).Include(c => c.GCompany)
                      select new { aa.LsName,aa.LsDescription, aa.GCompany.GcCompanyName, aa.Id }).OrderBy(p => p.LsName);
            return Ok(xx);
        }

        public IHttpActionResult AttachtestResults(int TransactionId,string AttachedFilePath,string AttachedFileList,string LoggedInUserId, string UserName, string Workflow)
        {
            if(!string.IsNullOrEmpty(AttachedFileList))
            {
                var FileArray = AttachedFileList.Split(',').ToList();
                foreach(var file in FileArray)
                {
                    var LAttachments = new LAttachment { LaCreatedById=LoggedInUserId,LaCreatedDateTime=DateTime.UtcNow,LaEntityId=TransactionId,LaEntityType="LSchemes",LaFileName=file,LaFilePath=AttachedFilePath,LaType="TestResults",LaUpdatedById=LoggedInUserId,LaUpdatedDateTime=DateTime.UtcNow};
                    db.LAttachments.Add(LAttachments);
                    db.SaveChanges();
                }
                //Update Scheme test Result Flag
                var Scheme = db.LSchemes.Find(TransactionId);
                Scheme.LsIsSchemeTested = true;
                db.Entry(Scheme).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Ok();
        }

        public IHttpActionResult GetLSchemeById(int Id, string UserName, string Workflow)
        {
            var xx = (from aa in db.LSchemes.Where(p => p.Id == Id).Include(c => c.GCompany)
                      select new { aa.LsName,aa.WFComments, aa.LsDescription, aa.GCompany.GcCompanyName, aa.Id }).FirstOrDefault();
            return Ok(xx);
        }

        // PUT: api/LSchemes/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutLScheme(int id, LScheme LScheme, string UserName, string Workflow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return BadRequest(ModelState);
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SCHEME")));
        //    }

        //    if (id != LScheme.Id)
        //    {
        //        //return BadRequest();
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SCHEME")));
        //    }
        //    try
        //    {
        //        db.Entry(LScheme).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!LSchemeExists(id))
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SCHEME")));
        //        }
        //        else
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //        }
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/LSchemes
        [HttpPost]
        public async Task<IHttpActionResult> PostLScheme(LScheme LScheme,string AttachedFiles,string AttachedFilePath,string SupportingDocuments,string LoggedInRoleId, string UserName, string Workflow)
        {
            //if (!ModelState.IsValid)## commented by shubham as it is failing while all values passed
            //{
            //    //return BadRequest(ModelState);
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "SCHEME")));
            //}
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Note: Calculate Ordinal based on the Current Role who has created Schemes based on RoleId and Opco and WorkflowName
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LScheme.LsCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LScheme.LsCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();

                    LScheme.WFOrdinal = Ordinal;
                    LScheme.WFStatus = "Saved";
                    LScheme.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LScheme.WFRequesterId : null;
                    LScheme.WFType = WFDetails.RwfWFType;
                    db.LSchemes.Add(LScheme);
                    await db.SaveChangesAsync();
                    //Add portfolio
                    if (!string.IsNullOrEmpty(LScheme.ParameterCarrier))
                    {
                        var PFArray = LScheme.ParameterCarrier.Split(',').ToList();
                        foreach (var PF in PFArray)
                        {
                            var Id = Convert.ToInt32(PF);
                            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LScheme.Id, MepEntityType = "LSchemes" };
                            db.MEntityPortfolios.Add(MEntityPortfolio);
                            db.SaveChanges();
                        }
                    }

                    //add row in LAttachments for files are added while adding Schemes
                    if (!string.IsNullOrEmpty(AttachedFiles))
                    {
                        var FilesArray = AttachedFiles.Split(',').ToList();
                        foreach (var file in FilesArray)
                        {
                            var LAttachments = new LAttachment { LaFileName = file, LaFilePath = AttachedFilePath, LaCreatedById = LScheme.LsCreatedById, LaCreatedDateTime = DateTime.UtcNow, LaEntityId = LScheme.Id, LaEntityType = "LSchemes", LaUpdatedById = LScheme.LsUpdatedById, LaUpdatedDateTime = DateTime.UtcNow };
                            db.LAttachments.Add(LAttachments);
                            db.SaveChanges();
                        }
                    }

                    //add row in LSupporting Documents for files are added while adding Schemes
                    if (!string.IsNullOrEmpty(SupportingDocuments))
                    {
                        var SupportingDocumentsArray = SupportingDocuments.Split(',').ToList();
                        foreach (var file in SupportingDocumentsArray)
                        {
                            var LSupportingDocuments = new LSupportingDocument { LsdCreatedById =LScheme.LsCreatedById, LsdCreatedDateTime = DateTime.UtcNow, LsdEntityId =LScheme.Id, LsdEntityType = "LSchemes", LsdFileName = file, LsdFilePath = AttachedFilePath + "/SupportingDocuments", LsdUpdatedById = LScheme.LsUpdatedById, LsdUpdatedDateTime = DateTime.UtcNow };
                            db.LSupportingDocuments.Add(LSupportingDocuments);
                            db.SaveChanges();
                        }
                    }
                    //Add Entry in Audit Log
                    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                           "Create", LScheme.LsCreatedById, DateTime.UtcNow, LScheme.WFStatus, LScheme.WFStatus,
                          WFDetails.RwfBaseTableName, LScheme.Id, Convert.ToString(LScheme.LsName), WFDetails.Id, LScheme.WFCompanyId, string.Empty, LScheme.WFRequesterRoleId, null);
                    transaction.Commit();
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
            }
            return Ok(LScheme);
        }

        // DELETE: api/LSchemes/5
        [ResponseType(typeof(LScheme))]
        public async Task<IHttpActionResult> DeleteLScheme(int id, string UserName, string Workflow)
        {
            LScheme LScheme = await db.LSchemes.FindAsync(id);
            if (LScheme == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SCHEME")));
            }
            try
            {
                db.LSchemes.Remove(LScheme);
                await db.SaveChangesAsync();
                return Ok(LScheme);
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

        private bool LSchemeExists(int id)
        {
            return db.LSchemes.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("UQ_RBrads_RbName_RbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return ("Can not insert duplicate SCHEME NAME");
            //else if (SqEx.Message.IndexOf("FK_LSchemes_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SCHEME", "RAW DATA"));
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
