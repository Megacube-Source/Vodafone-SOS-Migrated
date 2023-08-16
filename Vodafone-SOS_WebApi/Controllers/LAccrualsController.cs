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
    public class LAccrualsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

      

        public IHttpActionResult GetLAccrualById(int Id,string UserName,string Workflow)
        {
            var xx = (from aa in db.LAccruals.Where(p => p.Id == Id).Include(c => c.GCompany)
                      select new { aa.LaName, aa.LaDescription, aa.WFCompanyId, aa.Id,aa.WFComments,aa.LaCommissionPeriod,aa.LaAccrualAmount }).FirstOrDefault();
            return Ok(xx);
        }

        // PUT: api/LAccruals/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLAccrual(int id, LAccrual LAccrual, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ACCRUAL")));
            }

            if (id != LAccrual.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ACCRUAL")));
            }
            try
            {
                var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                db.Entry(LAccrual).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Entry in Audit Log
                Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Edit",
                       "Edit", LAccrual.LaCreatedById, DateTime.UtcNow, LAccrual.WFStatus, LAccrual.WFStatus,
                      WFDetails.RwfBaseTableName, LAccrual.Id, Convert.ToString(LAccrual.LaName), WFDetails.Id, LAccrual.WFCompanyId, string.Empty, LAccrual.WFRequesterRoleId,null);
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

        // POST: api/LAccruals
        [HttpPost]
        public async Task<IHttpActionResult> PostLAccrual(LAccrual LAccrual, string AttachedFiles, string AttachedFilePath, string SupportingDocuments, string LoggedInRoleId, string UserName, string Workflow)
        {
            //if (!ModelState.IsValid)## commented by shubham as it is failing while all values passed
            //{
            //    //return BadRequest(ModelState);
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ACCRUAL")));
            //}
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Note: Calculate Ordinal based on the Current Role who has created ACCRUALs based on RoleId and Opco and WorkflowName
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LAccrual.WFCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LAccrual.WFCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();

                    LAccrual.WFOrdinal = Ordinal;
                    LAccrual.WFStatus = "Saved";
                    LAccrual.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LAccrual.WFRequesterId : null;
                    LAccrual.WFType = WFDetails.RwfWFType;
                    db.LAccruals.Add(LAccrual);
                    await db.SaveChangesAsync();
                    //Add portfolio
                    if (!string.IsNullOrEmpty(LAccrual.ParameterCarrier))
                    {
                        var PFArray =LAccrual.ParameterCarrier.Split(',').ToList();
                        foreach (var PF in PFArray)
                        {
                            var Id = Convert.ToInt32(PF);
                            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LAccrual.Id, MepEntityType = "LAccruals" };
                            db.MEntityPortfolios.Add(MEntityPortfolio);
                            db.SaveChanges();
                        }
                    }

                    //add row in LAttachments for files are added while adding ACCRUALs
                    if (!string.IsNullOrEmpty(AttachedFiles))
                    {
                        var FilesArray = AttachedFiles.Split(',').ToList();
                        foreach (var file in FilesArray)
                        {
                            var LAttachments = new LAttachment { LaFileName = file, LaFilePath = AttachedFilePath, LaCreatedById = LAccrual.LaCreatedById, LaCreatedDateTime = DateTime.UtcNow, LaEntityId = LAccrual.Id, LaEntityType = "LAccruals", LaUpdatedById = LAccrual.LaUpdatedById, LaUpdatedDateTime = DateTime.UtcNow };
                            db.LAttachments.Add(LAttachments);
                            db.SaveChanges();
                        }
                    }

                    //add row in LSupporting Documents for files are added while adding ACCRUALs
                    if (!string.IsNullOrEmpty(SupportingDocuments))
                    {
                        var SupportingDocumentsArray = SupportingDocuments.Split(',').ToList();
                        foreach (var file in SupportingDocumentsArray)
                        {
                            var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LAccrual.LaCreatedById, LsdCreatedDateTime = DateTime.UtcNow, LsdEntityId = LAccrual.Id, LsdEntityType = "LAccruals", LsdFileName = file, LsdFilePath = AttachedFilePath + "/SupportingDocuments", LsdUpdatedById = LAccrual.LaUpdatedById, LsdUpdatedDateTime = DateTime.UtcNow };
                            db.LSupportingDocuments.Add(LSupportingDocuments);
                            db.SaveChanges();
                        }
                    }
                    //Add Entry in Audit Log
                    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                           "Create", LAccrual.LaCreatedById, DateTime.UtcNow, LAccrual.WFStatus, LAccrual.WFStatus,
                          WFDetails.RwfBaseTableName, LAccrual.Id, Convert.ToString(LAccrual.LaName), WFDetails.Id, LAccrual.WFCompanyId, string.Empty, LAccrual.WFRequesterRoleId, LAccrual.LaCommissionPeriod);
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
            return Ok(LAccrual);
        }

        // DELETE: api/LAccruals/5
        [ResponseType(typeof(LAccrual))]
        public async Task<IHttpActionResult> DeleteLAccrual(int id, string UserName, string Workflow)
        {
            LAccrual LAccrual = await db.LAccruals.FindAsync(id);
            if (LAccrual == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ACCRUAL")));
            }
            try
            {
                db.LAccruals.Remove(LAccrual);
                await db.SaveChangesAsync();
                return Ok(LAccrual);
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

        private bool LAccrualExists(int id)
        {
            return db.LAccruals.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Something else failed return original error message as retrieved from database
            //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
            //Add complete Url in description
            var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
            string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
            var ErrorDesc = "";
            //Get the list of parameters passed in api call
            var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            if (Desc.Count() > 0)
                ErrorDesc = string.Join(",", Desc);
            //This array will provide controller name at 2nd and action name at 3 rd index position
            string[] s = Request.RequestUri.AbsolutePath.Split('/');
            //add entry in GError Log table
           Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
            //Globals.LogError(SqEx.Message, ErrorDesc);
            return Globals.SomethingElseFailedInDBErrorMessage;
        }
    }
}
