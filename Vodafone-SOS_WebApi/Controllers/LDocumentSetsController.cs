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
    public class LDocumentSetsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // POST: api/LDocumentSets
        [HttpPost]
        public async Task<IHttpActionResult> PostLDocumentSet(LDocumentSetsViewModel model, string AttachedFilePath, string LoggedInRoleId,  string UserName, string Workflow,string SupportingDocumentFilePath)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == model.WFCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == model.WFCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();
                    string ErrorMessage = string.Empty;
                    string id = string.Empty;


                    //Commentted by SG for procedure insertion
                    //////Note: Calculate Ordinal based on the Current Role who has created DocumentSets based on RoleId and Opco and WorkflowName
                    ////LDocumentSet LDocumentSet = new LDocumentSet();
                    ////LDocumentSet.WFOrdinal = Ordinal;
                    ////LDocumentSet.WFStatus = "Saved";
                    ////LDocumentSet.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LDocumentSet.WFRequesterId : null;
                    ////LDocumentSet.WFType = WFDetails.RwfWFType;
                    ////LDocumentSet.LdsCommissionPeriod = model.LdsCommissionPeriod;
                    ////LDocumentSet.LdsCreatedById = model.LdsCreatedById;
                    ////LDocumentSet.LdsCreatedDateTime = model.LdsCreatedDateTime;
                    ////LDocumentSet.LdsDescription = model.LdsDescription;
                    ////LDocumentSet.LdsDocumentList = model.LdsDocumentList;
                    ////LDocumentSet.LdsName = model.LdsName;
                    ////LDocumentSet.LdsPayeeList = model.LdsPayeeList;
                    ////LDocumentSet.LdsSendEmail = model.LdsSendEmail;
                    ////LDocumentSet.LdsUpdatedById = model.LdsUpdatedById;
                    ////LDocumentSet.LdsUpdatedDateTime = model.LdsUpdatedDateTime;
                    ////LDocumentSet.WFComments = model.WFComments;
                    ////LDocumentSet.WFCompanyId = model.WFCompanyId;
                    ////LDocumentSet.WFCurrentOwnerId = model.WFCurrentOwnerId;
                    ////LDocumentSet.WFManagerId = model.WFManagerId;
                    ////LDocumentSet.WFRequesterRoleId = model.WFRequesterRoleId;
                    ////LDocumentSet.WFRequesterId = model.WFRequesterId;
                    ////string DocumentSetId = "";
                    //////Map Document set with Payees if Payee List is supplied by the User
                    string AnalystIDNew = (ConfigData.LwfcActingAs == "Analyst") ? model.WFRequesterId : null;
                    if (!string.IsNullOrEmpty(model.PayeeListCarrier))
                    {
                        Globals.SaveSPPayeeDoucmentData(model.WFCompanyId, LoggedInRoleId, Workflow, model.LdsCommissionPeriod, model.LdsCreatedById, model.LdsDescription,
                        model.LdsDocumentList, model.LdsName, model.LdsPayeeList, model.LdsSendEmail, model.LdsUpdatedById, model.LdsUpdatedDateTime, model.WFComments,
                        model.WFCurrentOwnerId, model.WFManagerId, model.WFRequesterRoleId, model.WFRequesterId, model.AttachedFiles, "Saved", WFDetails.RwfWFType,
                        AttachedFilePath, model.ParameterCarrier, model.SupportingDocumentFiles, AnalystIDNew, Ordinal, SupportingDocumentFilePath,
                       "", model.PayeeSelection, model.PayeeListCarrier, out ErrorMessage, out id);

                        ////    //Get Payee Name List using dbo function and populate LdsPayeeList column
                        ////    var PayeeArray = model.PayeeListCarrier.Split(',').ToList();
                        ////    string PayeeNameList = "";
                        ////    //foreach (var Payee in PayeeArray)
                        ////    //{
                        ////    //    var PayeId = Convert.ToInt32(Payee);
                        ////    //    PayeeNameList += Globals.GetPayeeName(PayeId,true);
                        ////    //   // PayeeNameList += db.Database.SqlQuery<string>("select dbo.FnGetPayeeName({0})", PayeId).FirstOrDefault() + ",";
                        ////    //}
                        ////    if (PayeeArray.Count > 1)
                        ////    {
                        ////        PayeeNameList = "Multiple(" + PayeeArray.Count + ")";
                        ////    }
                        ////    else
                        ////    {
                        ////        PayeeNameList = Globals.GetPayeeName(Convert.ToInt32(PayeeArray[0]), true);
                        ////    }

                        ////    //for(int i =0; i <= 10; i++ )
                        ////    //{
                        ////    //    PayeeNameList += Globals.GetPayeeName(Convert.ToInt32(PayeeArray[i]), true);
                        ////    //}

                        ////    LDocumentSet.LdsPayeeList = PayeeNameList;
                        ////    db.LDocumentSets.Add(LDocumentSet);
                        ////    await db.SaveChangesAsync();
                        ////    DocumentSetId = LDocumentSet.Id.ToString();

                        ////    //foreach (var PF in PayeeArray)
                        ////    //{
                        ////    //    var Id = Convert.ToInt32(PF);
                        ////    //    var MPayeeDocumentSet = new MDocumentSetsPayee { MdspPayeeId = Id, MdspDocumentSetId = LDocumentSet.Id };
                        ////    //    db.MDocumentSetsPayees.Add(MPayeeDocumentSet);
                        ////    //    db.SaveChanges();
                        ////    //}


                        ////    Globals.ExecuteSPSaveMDocumentSetsPayees(model.PayeeListCarrier, LDocumentSet.Id);


                        ////    //add row in LAttachments for files are added while adding DocumentSets
                        ////    if (!string.IsNullOrEmpty(model.AttachedFiles))
                        ////    {

                        ////        var FilesArray = model.AttachedFiles.Split(',').ToList();

                        ////        foreach (var file in FilesArray)
                        ////        {
                        ////            var LAttachments = new LAttachment { LaFileName = file, LaFilePath = AttachedFilePath, LaCreatedById = LDocumentSet.LdsCreatedById, LaCreatedDateTime = DateTime.UtcNow, LaEntityId = LDocumentSet.Id, LaEntityType = "LDocumentSets", LaUpdatedById = LDocumentSet.LdsUpdatedById, LaUpdatedDateTime = DateTime.UtcNow };
                        ////            db.LAttachments.Add(LAttachments);
                        ////            db.SaveChanges();
                        ////        }

                        ////    }

                        ////    //Add portfolio
                        ////    if (!string.IsNullOrEmpty(LDocumentSet.ParameterCarrier))
                        ////    {
                        ////        var PFArray = LDocumentSet.ParameterCarrier.Split(',').ToList();
                        ////        foreach (var PF in PFArray)
                        ////        {
                        ////            var Id = Convert.ToInt32(PF);
                        ////            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LDocumentSet.Id, MepEntityType = "LDocumentSets" };
                        ////            db.MEntityPortfolios.Add(MEntityPortfolio);
                        ////            db.SaveChanges();
                        ////        }
                        ////    }



                        ////    //add row in LSupporting Documents for files are added while adding DocumentSets
                        ////    if (!string.IsNullOrEmpty(model.SupportingDocumentFiles))
                        ////    {
                        ////        var SupportingDocumentsArray = model.SupportingDocumentFiles.Split(',').ToList();
                        ////        foreach (var file in SupportingDocumentsArray)
                        ////        {
                        ////            var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LDocumentSet.LdsCreatedById, LsdCreatedDateTime = DateTime.UtcNow, LsdEntityId = LDocumentSet.Id, LsdEntityType = "LDocumentSets", LsdFileName = file, LsdFilePath = SupportingDocumentFilePath, LsdUpdatedById = LDocumentSet.LdsUpdatedById, LsdUpdatedDateTime = DateTime.UtcNow };
                        ////            db.LSupportingDocuments.Add(LSupportingDocuments);
                        ////            db.SaveChanges();
                        ////        }
                        ////    }
                        //if (id != 0)
                        //{
                        //    //Add Entry in Audit Log
                        //    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                        //           "Create", model.LdsCreatedById, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                        //          WFDetails.RwfBaseTableName, id, Convert.ToString(model.LdsName), WFDetails.Id, model.WFCompanyId, string.Empty, model.WFRequesterRoleId);
                        //}
                        //else
                        //{
                        //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));

                        //}
                        if (id == "0")
                        {
                            //Add Entry in Audit Log
                            //Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                            //       "Create", model.LdsCreatedById, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                            //      WFDetails.RwfBaseTableName, id, Convert.ToString(model.LdsName), WFDetails.Id, model.WFCompanyId, string.Empty, model.WFRequesterRoleId);

                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
                        }
                        ////Add Entry in Audit Log
                        //Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                        //       "Create", LDocumentSet.LdsCreatedById, DateTime.UtcNow, LDocumentSet.WFStatus, LDocumentSet.WFStatus,
                        //      WFDetails.RwfBaseTableName, LDocumentSet.Id, Convert.ToString(LDocumentSet.LdsName), WFDetails.Id, LDocumentSet.WFCompanyId, string.Empty, LDocumentSet.WFRequesterRoleId);

                    }
                    else
                    {
                       
                        Globals.SaveSPPayeeDoucmentData(model.WFCompanyId, LoggedInRoleId, Workflow, model.LdsCommissionPeriod, model.LdsCreatedById, model.LdsDescription,
                       model.LdsDocumentList, model.LdsName, model.LdsPayeeList, model.LdsSendEmail, model.LdsUpdatedById, model.LdsUpdatedDateTime, model.WFComments,
                       model.WFCurrentOwnerId, model.WFManagerId, model.WFRequesterRoleId, model.WFRequesterId, model.AttachedFiles, "Saved", WFDetails.RwfWFType,
                       AttachedFilePath, model.ParameterCarrier, model.SupportingDocumentFiles, model.WFAnalystId, Ordinal, SupportingDocumentFilePath,
                      "", model.PayeeSelection, model.PayeeListCarrier, out ErrorMessage, out id);
                        if (id == "0")
                        {
                            //Add Entry in Audit Log
                            //Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                            //       "Create", model.LdsCreatedById, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                            //      WFDetails.RwfBaseTableName, id, Convert.ToString(model.LdsName), WFDetails.Id, model.WFCompanyId, string.Empty, model.WFRequesterRoleId);

                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
                        }
                        //else
                        //{
                        //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));

                        //}



                        //code to get  Payee from FileName and add entry in LDocument set and Mapping table of Document Set with Payees
                        /*create as many document set entries in LDocimentSet as many files are uploaded
                         Also, there will be as many entries made in mapping table based on the Payee code provided in file name. */
                        //Globals.ExecuteFilesName(model.AttachedFiles);
                        //var FilesArray = model.AttachedFiles.Split(',').ToList();
                        //foreach (var file in FilesArray)
                        //{
                        //    var PayeeCode = file.Split('_').ElementAt(0);
                        //    var PayeeDetails = db.LPayees.Where(p => p.LpPayeeCode == PayeeCode).Where(p => p.WFCompanyId == LDocumentSet.WFCompanyId).FirstOrDefault();
                        //    var PayeeName = string.Empty;
                        //    //Check if Payee Exist for that Payee code
                        //    if (PayeeDetails != null)
                        //    {
                        //        //SG- Commented due to get the decripted payeename
                        //        //PayeeName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName + " (" + PayeeDetails.LpPayeeCode + ")";
                        //        PayeeName = Globals.GetPayeeName(Convert.ToInt32(PayeeDetails.Id), true);
                        //        var DocumentSet = new LDocumentSet
                        //        {
                        //            LdsCommissionPeriod = LDocumentSet.LdsCommissionPeriod,
                        //            LdsCreatedById = LDocumentSet.LdsCreatedById,
                        //            LdsCreatedDateTime = LDocumentSet.LdsCreatedDateTime,
                        //            LdsDescription = LDocumentSet.LdsDescription,
                        //            LdsName = LDocumentSet.LdsName,
                        //            LdsSendEmail = LDocumentSet.LdsSendEmail,
                        //            LdsUpdatedById = LDocumentSet.LdsUpdatedById,
                        //            LdsUpdatedDateTime = LDocumentSet.LdsUpdatedDateTime,
                        //            WFAnalystId = LDocumentSet.WFAnalystId,
                        //            WFComments = LDocumentSet.WFComments,
                        //            WFCompanyId = LDocumentSet.WFCompanyId,
                        //            WFCurrentOwnerId = LDocumentSet.WFCurrentOwnerId,
                        //            WFManagerId = LDocumentSet.WFManagerId,
                        //            WFOrdinal = LDocumentSet.WFOrdinal,
                        //            WFRequesterId = LDocumentSet.WFRequesterId,
                        //            WFRequesterRoleId = LDocumentSet.WFRequesterRoleId,
                        //            WFStatus = LDocumentSet.WFStatus,
                        //            WFType = LDocumentSet.WFType,
                        //            LdsDocumentList = file,
                        //            LdsPayeeList = PayeeName
                        //        };

                        //        db.LDocumentSets.Add(DocumentSet);
                        //        db.SaveChanges();
                        //        DocumentSetId += DocumentSet.Id + ",";
                        //        //add file
                        //        var LAttachments = new LAttachment { LaFileName = file, LaFilePath = AttachedFilePath, LaCreatedById = DocumentSet.LdsCreatedById, LaCreatedDateTime = DateTime.UtcNow, LaEntityId = DocumentSet.Id, LaEntityType = "LDocumentSets", LaUpdatedById = DocumentSet.LdsUpdatedById, LaUpdatedDateTime = DateTime.UtcNow };
                        //        db.LAttachments.Add(LAttachments);
                        //        db.SaveChanges();
                        //        //add mapping of payee and ldocument sets
                        //        var MPayeeDocumentSet = new MDocumentSetsPayee { MdspPayeeId = PayeeDetails.Id, MdspDocumentSetId = DocumentSet.Id };
                        //        db.MDocumentSetsPayees.Add(MPayeeDocumentSet);
                        //        db.SaveChanges();
                        //        //Add Entry in Audit Log
                        //        Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                        //               "Create", DocumentSet.LdsCreatedById, DateTime.UtcNow, DocumentSet.WFStatus, DocumentSet.WFStatus,
                        //              WFDetails.RwfBaseTableName, DocumentSet.Id, Convert.ToString(DocumentSet.LdsName), WFDetails.Id, DocumentSet.WFCompanyId, string.Empty, DocumentSet.WFRequesterRoleId);


                        //        //Add portfolio
                        //        if (!string.IsNullOrEmpty(LDocumentSet.ParameterCarrier))
                        //        {
                        //            var PFArray1 = LDocumentSet.ParameterCarrier.Split(',').ToList();
                        //            foreach (var PF1 in PFArray1)
                        //            {
                        //                var Id = Convert.ToInt32(PF1);
                        //                var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = DocumentSet.Id, MepEntityType = "LDocumentSets" };
                        //                db.MEntityPortfolios.Add(MEntityPortfolio);
                        //                db.SaveChanges();
                        //            }
                        //        }



                        //        //add row in LSupporting Documents for files are added while adding DocumentSets
                        //        if (!string.IsNullOrEmpty(model.SupportingDocumentFiles))
                        //        {
                        //            var SupportingDocumentsArray = model.SupportingDocumentFiles.Split(',').ToList();
                        //            foreach (var file1 in SupportingDocumentsArray)
                        //            {
                        //                var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = DocumentSet.LdsCreatedById, LsdCreatedDateTime = DateTime.UtcNow, LsdEntityId = LDocumentSet.Id, LsdEntityType = "LDocumentSets", LsdFileName = file1, LsdFilePath = SupportingDocumentFilePath, LsdUpdatedById = LDocumentSet.LdsUpdatedById, LsdUpdatedDateTime = DateTime.UtcNow };
                        //                db.LSupportingDocuments.Add(LSupportingDocuments);
                        //                db.SaveChanges();
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Invalid PayeeCode in FIle"));
                        //    }

                        //}

                        if (!string.IsNullOrEmpty(id))
                        {
                            //Trim and remove the last comma
                            id = id.Trim();
                            id = id.Remove(id.Length - 1, 1);

                        }
                    }

                    //transaction.Commit();
                    return Ok(id);
                }
                //catch (DbEntityValidationException dbex)
                //{
                //    transaction.Rollback();
                //    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                //    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                //}
                catch (Exception ex)
                {
                   // Globals.SendEmail("ssharma@megacube.com.au", "shubhamshrm97@yahoo.com", "Document Set Create", ex.Message+ex.StackTrace+ex.InnerException.Message, "QA");
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
         
        }
        //private int GetDataSaved(List<string> PayeeArray, int IdNew)
        //{
        //    foreach (var PF in PayeeArray)
        //    {
        //        var Id = Convert.ToInt32(PF);
        //        var MPayeeDocumentSet = new MDocumentSetsPayee { MdspPayeeId = Id, MdspDocumentSetId = IdNew };
        //        db.MDocumentSetsPayees.Add(MPayeeDocumentSet);
        //        db.SaveChanges();
        //    }
        //    return 0;
        //}

        public IHttpActionResult GetLDocumentSetById(int Id, string UserName, string Workflow)
        {
            var xx = (from aa in db.LDocumentSets.Where(p => p.Id == Id).Include(c => c.GCompany)
                      select new {aa.LdsPayeeList,aa.LdsSendEmail,aa.LdsDocumentList,aa.LdsCommissionPeriod, aa.LdsName, aa.LdsDescription, aa.GCompany.GcCompanyName, aa.Id,aa.WFComments }).FirstOrDefault();
            return Ok(xx);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LDocumentSetExists(int id)
        {
            return db.LDocumentSets.Count(e => e.Id == id) > 0;
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
