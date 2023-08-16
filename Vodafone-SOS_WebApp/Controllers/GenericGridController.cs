
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Vodafone_SOS_WebApp.ViewModels;
using Vodafone_SOS_WebApp.Helper;
using System.Reflection;
using CsvHelper;
using System.Configuration;
using System.IO;
using Vodafone_SOS_WebApp.Utilities;
using Ionic.Zip;
using System.Text;
using System.Threading;
using NPOI.SS.UserModel;
using Amazon.S3;
using Amazon.S3.Model;
using NPOI.XSSF.UserModel;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class GenericGridController : PrimaryController
    {
        ILWorkflowGridColumnsRestClient RestClient = new LWorkflowGridColumnsRestClient();
        ILBatchesRestClient LBRC = new LBatchesRestClient();
        ILClaimsRestClient LCLRC = new LClaimsRestClient();
        ILPayRestClient LPRC = new LPayRestClient();
        ILSchemesRestClient LLSRC = new LSchemesRestClient();
        ILAttachmentsRestClient LARC = new LAttachmentsRestClient();
        ILCalcRestClient LCRC = new LCalcRestClient();
        ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
        ILRawDataRestClient LRRC = new LRawDataRestClient();
        ILBatchFilesRestClient LBFRC = new LBatchFilesRestClient();

        string FirstName = Convert.ToString(System.Web.HttpContext.Current.Session["FirstName"]);
        string LastName = Convert.ToString(System.Web.HttpContext.Current.Session["LastName"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string LoggedInRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();

        // GET: GenericGrid
        //[ControllerActionFilter]
        public ActionResult Index(string WorkFlow,string PortfolioList)
        {
            TempData.Remove("FilterQuery");
            string WfConfigId = string.Empty;
            if (string.IsNullOrEmpty(WorkFlow))
            {
                WorkFlow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            }

            System.Web.HttpContext.Current.Session["Title"] = "Manage " + WorkFlow;

            ILPayeesRestClient LPARC = new LPayeesRestClient();
            ViewBag.CanRaiseClaims = LPARC.CanRaiseClaims(LoggedInUserId);
            ViewBag.BottomButtons = GGRC.GetGridBottomButtons(WorkFlow, CompanyId, LoggedInRoleId, LoggedInUserId);

            //Passing Hardcoded WorkFlowName which has been obtained from menu
            System.Web.HttpContext.Current.Session["Workflow"] = WorkFlow;
            //Used to get Columns of Workflow Grid
            var ColumnDetails = RestClient.GetByWorkFlow(WorkFlow, CompanyId, LoggedInRoleId);//.OrderBy(p=>p.LwfcOrdinalNumber).ThenBy(p=>p.LwfgcOrdinal);
            ViewBag.GridPageTitle = ColumnDetails.FirstOrDefault().RwfUILabel.ToUpper();

            //Used to Load Tabs in Workflow
            var xx = RestClient.GetRolesByWorkflow(WorkFlow, CompanyId).ToList();
            ViewBag.Roles = xx.Select(p => p.Name).ToList(); //xx.Select(p => p.UserRole).ToList();
            ViewBag.ConfigIds = xx.ToList();
            ViewBag.DownloadGlymph = true; //will use session variable but confirm from JS for this visibility
                                          /* ViewBag.GenericGridTabDetailsList = GenericGridTabDetailsList;*/
            ViewBag.GenericGridGridColumnDetails = ColumnDetails;
            ViewBag.PortfolioList = PortfolioList;
            //Get Tab Count List
            ViewBag.CountTable = GGRC.WorkflowGridCounts(LoggedInRoleId, LoggedInUserId, WorkFlow, CompanyId, null);
            for (int j = 0; j < xx.Count; ++j)//look for config Id based on tab Name and assign it to WfConfigId variable
            {
                if(xx[j].Name == LoggedInRole)
                {
                    WfConfigId = Convert.ToString(xx[j].Id);
                }
            }
            

           // ViewBag.UserPreference = JsonConvert.SerializeObject(GGRC.GetUserPreferenceData(LoggedInUserId, "", "", "", WfConfigId, ObjSession._UserSessionID));
           // ViewBag.UserPreference = GGRC.GetUserPreferenceData(LoggedInUserId, "", "", "", WfConfigId, ObjSession._UserSessionID);
            return View();
        }

        public JsonResult GetUserPreference(string WfConfigId, string TabName)
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            var ApiData = GGRC.GetUserPreferenceData(LoggedInUserId, WorkflowName, TabName, "", WfConfigId, ObjSession._UserSessionID);
            return Json(ApiData.AsEnumerable(), JsonRequestBehavior.AllowGet);
        }

        //This method will be called when User clicks on self assign link in Generic Grid
        //[ControllerActionFilter]
        public ActionResult SelfAssign(int TransactionId)
        {
            var LoggedInUserName = FirstName + " " + LastName;
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            GGRC.SelfAssign(WorkflowName, TransactionId, CompanyId, LoggedInUserName, LoggedInUserId, LoggedInRoleId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        //[ControllerActionFilter]
        public ActionResult AssignTo(string TransactionId)
        {
            string[] values = TransactionId.Split(',');
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            List<AssigneeListViewModel> AssigneeList = GGRC.GetAssigneeList(WorkflowName,Convert.ToInt32(values[0]), CompanyId).ToList();
            if (AssigneeList.Count > 0)
            {
                //RK R2.8.5 Before removing the item, check if the item exist or not
                if (AssigneeList.Where(r => r.Id == LoggedInUserId).Count() > 0)
                {
                    var itemToRemove = AssigneeList.Single(r => r.Id == LoggedInUserId);
                    AssigneeList.Remove(itemToRemove);
                }
            }
            ViewBag.Assignees = new SelectList(AssigneeList, "Id", "FullName");
            ViewBag.TransactionId = TransactionId;
            TempData["AssigneeList"] = AssigneeList;
            return View();
        }

        //This method will make the user as current owner whose id is passed as parameter
        [HttpPost]
        //[ControllerActionFilter]
        public ActionResult AssignTo(string Assignees, string TransactionId)
        {
            if (!string.IsNullOrEmpty(Assignees))
            {
                var dd = 0;
                string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                var AssigneeList = TempData["AssigneeList"] as List<AssigneeListViewModel>;
                var AssineeId = Assignees;
                // var AssigneeName = AssigneeList.Where(p => p.Id == Assignees).FirstOrDefault().FullName;
                GGRC.UpdateActionStatus(WorkflowName, TransactionId, CompanyId, "AssignTo", LoggedInUserId, string.Empty, LoggedInRoleId, AssineeId);
                //GGRC.AssignTo(WorkflowName, TransactionId, CompanyId, AssigneeName, AssineeId, LoggedInRoleId);
            }
            return RedirectToAction("Index");
        }

        //This method will update Status of TransactionId based on parameters passed
        //UpdateBaseTableWfStatus
        //[ControllerActionFilter]//
        public ActionResult UpdateBaseTableWfStatus(string TransactionId, string ActionName, string Comments)
        {
            try
            {
                if(String.IsNullOrEmpty(TransactionId))
                    return RedirectToAction("Index");
                //var TransactionArray = TransactionId.Split(',').ToList();
                //foreach (var TranId in TransactionArray)
                //{
                    if (!string.IsNullOrEmpty(TransactionId))
                    {

                        if (!string.IsNullOrEmpty(Comments))
                        {
                            Comments = "[" + System.Web.HttpContext.Current.Session["UserName"] + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + Comments;
                        }
                        else
                        {
                            Comments = "";
                        }
                       // var Id = Convert.ToInt32(TranId);
                        string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                        GGRC.UpdateActionStatus(WorkflowName, TransactionId, CompanyId, ActionName, LoggedInUserId, Comments, LoggedInRoleId, string.Empty);
                    //}
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return RedirectToAction("Index");
            }
        }

        public JsonResult UpdateBaseTableWfStatusMass(string[] TransactionIdData, string ActionName, string Comments)
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            // string TransactionId = string.Empty;
            try
            {
                //if (TransactionIdData == null)
                //    return RedirectToAction("Index");
                //int counter = 1;
                //foreach (string s in TransactionIdData)
                //{
                //    if (counter == 1)
                //    {
                //        TransactionId = s;
                //    }
                //    else
                //    {
                //        TransactionId = TransactionId + ',' + s;
                //    }
                //    counter = counter + 1;
                //}

                //var TransactionArray = TransactionId.Split(',').ToList();
                //foreach (var TranId in TransactionArray)
                //{
                if (TransactionIdData != null)
                {

                    if (!string.IsNullOrEmpty(Comments))
                    {
                        Comments = "[" + System.Web.HttpContext.Current.Session["UserName"] + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + Comments;
                    }
                    else
                    {
                        Comments = "";
                    }
                    // var Id = Convert.ToInt32(TranId);
                    
                    GGRC.UpdateActionStatusNew(WorkflowName, TransactionIdData, CompanyId, ActionName, LoggedInUserId, Comments, LoggedInRoleId, string.Empty);
                    //}
                }
               // return  WorkflowName;
                return Json(WorkflowName, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //return WorkflowName;
                return Json(WorkflowName, JsonRequestBehavior.AllowGet);
            }
        }
        //
        //
        //This is a generic method for edit action which will be called from all workflows
        /*NOTE Currently Using Switch Case But Secondary Form can be converted to flex Grid as well*/
        public ActionResult Edit(int TransactionId, int WFConfigId)
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            switch (WorkflowName)
            {
                case "Users":
                    return RedirectToAction("Edit", "LUsers", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Payees":
                    return RedirectToAction("Create", "LPayees", new { FormType = "EditDetails", TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Claims":
                    return RedirectToAction("Edit", "LClaims", new { TransactionId = TransactionId, WFConfigId = WFConfigId });

                default:
                    return RedirectToAction("Index");
            }
        }

        //This is a generic method for review action which will be called from all workflows
        /*NOTE Currently Using Switch Case But Secondary Form can be converted to flex Grid as well*/
        public ActionResult Review(int TransactionId, int WFConfigId)
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            switch (WorkflowName)
            {
                case "Users":
                    return RedirectToAction("Edit", "LUsers", new { FormType = "Review", TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Payees":
                    return RedirectToAction("ViewPayeeDetails", "LPayees", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Claims":
                    return RedirectToAction("Edit", "LClaims", new { FormType = "disabled", TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Calc":
                    ILCalcRestClient LRDRC = new LCalcRestClient();

                    string result = LRDRC.CheckCompanySpecificMappedorNot(CompanyId, "XCalc");
                    if (result != "")
                    {
                        return RedirectToAction("Index", "LCalc", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                    }else
                    {
                        TempData["Message"] = "Column mapping is missing. Please contact System Analyst to configure Calc Columns in Menu Config-> Form Labels.";
                        return RedirectToAction("Index");
                    }
                case "RawData":
                    return RedirectToAction("Index", "LRawData", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Pay":
                    return RedirectToAction("Index", "LPay", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "ManualAdjustments":
                    return RedirectToAction("Review", "ManualAdjustments", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "RefFiles":
                    return RedirectToAction("Review", "LUploadedFiles", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Schemes":
                    return RedirectToAction("Review", "LSchemes", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "Accruals":
                    return RedirectToAction("Review", "LAccruals", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                case "DocumentSets":
                    return RedirectToAction("Review", "LDocumentSets", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
                default:
                    return RedirectToAction("Index");
            }
        }
        
        //Implemented by RS for Review page of PayeeCR and UserCR Workflow
        public ActionResult ReviewForCR(int TransactionId)
        {
            ILPayeesRestClient payeerest = new LPayeesRestClient();
            ILUsersRestClient luser = new LUsersRestClient();
            var ApiData = GGRC.GetChangeRequestDetailbyId(TransactionId);

            var field = ApiData.FirstOrDefault().LcrColumnLabel;
            var oldvalue = ApiData.FirstOrDefault().LcrOldValue;
            var newvalue = ApiData.FirstOrDefault().LcrNewValue;
            string payeeCode="";
            string payeeEmail ="";
            string userEmail ="";
            int rowid = Convert.ToInt32(ApiData.FirstOrDefault().LcrRowId);

            string EntityName = ApiData.FirstOrDefault().LcrEntityName;
            if (EntityName == "LPayees")
            {
                var payeedetail = payeerest.GetById(rowid);
                 payeeCode = payeedetail.LpPayeeCode;
                payeeEmail = payeedetail.LpEmail;
            }
            else if (EntityName == "LUsers")
            {
                var userdetail = luser.GetById(rowid);
                 userEmail = userdetail.LuEmail;
            }
            ViewBag.field = field;
            ViewBag.oldvalue = oldvalue;
            ViewBag.newvalue = newvalue;
            ViewBag.payeeCode = payeeCode;
            ViewBag.payeeEmail = payeeEmail;
            ViewBag.userEmail = userEmail;
            ViewBag.TranctionId = TransactionId;
            ViewBag.EntityName = EntityName;
            return View();
        }        


        //This method will be called when User clicks on Download button in Generic Grid to download the file attached to it in case of excel upload
        // [ControllerActionFilter]
        public ActionResult DownloadFile(int TransactionId)
        {
            string result = "";
            var CompleteFileName = "";
            var FileName = "";
            var FileType = "";
            var extension = "";
            var IsSingleFile = true;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;

            //SG commented on 23/03/2020 - for Download File issue testing
            //Delete unnecessary files of temp folder
            //if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
            //{
            //    System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");//Now Delete all files
            //    foreach (FileInfo file in di.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //}
            //delete files under forzip
            //Delete functionality of S3 SDK is used
            if (Globals.FileExistsInS3("forzip", LoggedInUserName, CompanyCode))
             Globals.DeleteMultipleFilesFromS3(CompanyCode + "/" + LoggedInUserName + "/forzip/");
            
            Encoding utf8 = Encoding.UTF8;
            using (ZipFile zip = new ZipFile(utf8))
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;//??Why this statement is used
                zip.AddDirectoryByName("Supporting Documents");//Create this folder only when Supporting documents are present
                switch (Workflow)
                {
                    case "ManualAdjustments":
                        var BatchFiles = LBFRC.GetBatchFilesByBatchId(TransactionId).FirstOrDefault();
                        var BatchFilesData = LBRC.GetById(BatchFiles.LbfBatchId);
                        if (!string.IsNullOrEmpty(BatchFiles.LbfFileName))
                        {
                            var filePath = CompanyCode + "/ManualAdj/" + BatchFilesData.LbCommissionPeriod + "/Upload/";
                            FileName = BatchFiles.LbfFileName;
                            CompleteFileName = ConfigurationManager.AppSettings["ManualAdjustmentDocumentPath"] + "/" + filePath + BatchFiles.LbfFileName;
                            var xx = CompleteFileName.Split('.');
                            extension = xx.LastOrDefault();
                            FileType = Globals.GetFileContentType(extension);

                            var FileData = Globals.DownloadFromS3(FileName, filePath);
                            var TempLocalFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + FileName;
                            System.IO.File.WriteAllBytes(TempLocalFilePath, FileData); // Requires System.IO                                                      //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                            zip.AddFile(TempLocalFilePath, "");


                            //Get Attachments associated with the Manual Adjustment Batch
                            var Attachments = LSDRC.GetByEntityType("LBatches", TransactionId).ToList();
                            foreach (var Attachment in Attachments)
                            {
                                IsSingleFile = false;

                                var FileSupportData = Globals.DownloadFromS3(Attachment.LsdFileName, Attachment.LsdFilePath + "/");
                                var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Attachment.LsdFileName;
                                System.IO.File.WriteAllBytes(TempFilePath, FileSupportData); // Requires System.IO
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                zip.AddFile(TempFilePath, "Supporting Documents");
                            }
                        }
                        break;



                    case "Users":
                        //Get Attachments associated with the Manual Adjustment Batch
                        var UserAttachments = LSDRC.GetByEntityType("LUsers", TransactionId).ToList();
                        if (UserAttachments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in UserAttachments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                if (UserAttachments.Count() == 1)
                                {
                                    var filePath = Attachment.LsdFilePath;
                                    CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    FileName = Attachment.LsdFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                    var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    zip.AddFile(yy, "Supporting Documents");
                                }
                            }
                        }
                        break;
                    case "Payees":
                        //Get Attachments associated with the Payees which are saved in LSupporting Douments table
                        var PayeesAttachments = LSDRC.GetByEntityType("LPayees", TransactionId).ToList();

                        if (PayeesAttachments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                       
                            foreach (var Attachment in PayeesAttachments)
                            {
                                if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                                {
                                    if (PayeesAttachments.Count() == 1)
                                    {
                                        var filePath = Attachment.LsdFilePath;
                                        FileName = Attachment.LsdFileName;
                                        CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                        var xx = CompleteFileName.Split('.');
                                        extension = xx.LastOrDefault();
                                        FileType = Globals.GetFileContentType(extension);
                                    }
                                    else
                                    {
                                        IsSingleFile = false;
                                        var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                        zip.AddFile(yy, "Supporting Documents");
                                    }
                                }
                            }
                       
                        
                        break;
                    case "Claims":
                        //Get SupportingDocuments associated with the Claims
                        var ClaimsAttachments = LSDRC.GetByEntityType("LClaims", TransactionId).ToList();

                        if(ClaimsAttachments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in ClaimsAttachments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                if (ClaimsAttachments.Count() == 1)
                                {
                                    var filePath = Attachment.LsdFilePath;
                                    FileName = Attachment.LsdFileName;
                                    CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                    var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    zip.AddFile(yy, "Supporting Documents");
                                }
                            }
                        }
                        break;
                    case "Calc":
                        //get all LCalc records based on batch number and save the file in S drive in api method
                        var Batch = LBRC.GetById(TransactionId);
                        var query = string.Empty;
                        //SG : Commented as per senario mention by Louiza
                        //if (TempData["FilterQuery"] != null)
                        //{
                        //    query = Convert.ToString(TempData["FilterQuery"]);
                        //    TempData.Keep("FilterQuery");
                        //}
                        result = LCRC.DownloadLCalc(Batch.LbBatchNumber, CompanyCode, LoggedInUserName, CompanyId, query);
                        //Thread.Sleep(3000);
                        FileName = result;//"ExportCalcFile.xlsx";
                        if (FileName == "No Mapping columns found")
                        {
                            CompleteFileName = "";
                        }
                        else
                        {
                            CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + result;
                        }

                        FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"text/csv";
                        extension = "zip";
                        //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
                        var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                        DirectoryInfo dir = new DirectoryInfo(FilePath);
                        dir.Refresh();
                        var SupportingDocuments = LSDRC.GetByEntityType("Calc", TransactionId).ToList();
                        if (SupportingDocuments.Count() > 0)
                        {
                            if (System.IO.File.Exists(CompleteFileName))
                                zip.AddFile(CompleteFileName, "");
                        }
                        foreach (var Attachment in SupportingDocuments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                IsSingleFile = false;
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                zip.AddFile(yy, "Supporting Documents");
                            }
                        }
                        //using (StreamReader r = new StreamReader(CompleteFileName, Encoding.GetEncoding("iso-8859-1")))
                        //{
                        //    var encoding = Encoding.GetEncoding("iso-8859-1");
                        //    var CompleteFileNameInBytes = encoding.GetBytes(r.ReadToEnd());
                        //    return File(CompleteFileNameInBytes, "application/csv;charset=ISO-8859-1", "ExportCalcFile.csv");
                        //}

                        break;
                    case "Pay":
                        //get all LPay records based on batch number and save the file in S drive in api method
                        result = LPRC.DownloadLPayForGrid(TransactionId, CompanyCode, LoggedInUserName);
                       
                        FileName = result;//"ExportPayFile.xlsx";
                        if (FileName == "No Mapping columns found")
                        {
                            CompleteFileName = "";
                        }
                        else
                        {
                            CompleteFileName = ConfigurationManager.AppSettings["PayDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + result;
                        }
                        
                        FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"text/csv";
                        extension = "zip";
                        //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
                        FilePath = ConfigurationManager.AppSettings["PayDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName;
                        DirectoryInfo dir1 = new DirectoryInfo(FilePath);
                        dir1.Refresh();
                        //using (StreamReader r = new StreamReader(CompleteFileName, Encoding.GetEncoding("iso-8859-1")))
                        //{
                        //    var encoding = Encoding.GetEncoding("iso-8859-1");
                        //    var CompleteFileNameInBytes = encoding.GetBytes(r.ReadToEnd());
                        //    return File(CompleteFileNameInBytes, "application/csv;charset=ISO-8859-1", "ExportPayFile.csv");
                        //}

                        break;
                    case "RawData":
                        // var TempEmailBody = "";
                        //get all Raw Data records based on batch number and save the file in S drive in api method
                        var RawDataBatch = LBRC.GetById(TransactionId);
                        result = LRRC.DownloadRawData(CompanyId, RawDataBatch.LbBatchNumber, RawDataBatch.LbRawDataTableId, LoggedInUserName);
                        //Thread.Sleep(3000);//Adding temporary delay to investigate download error issue on 22JUN2017 
                        var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";

                        FileName = result;//"ExportRawDataFile.xlsx";

                        FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"application/csv;charset=UTF-8";
                        extension = "zip";
                        if (FileName == "No Mapping columns found")
                        {
                            CompleteFileName = "";
                        } else if(FileName == "Table is missing for this batch !!")
                        {
                            CompleteFileName = "";
                        }
                        else
                        {
                            CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + FileName;
                        }
                        
                        //TempEmailBody += CompleteFileName+Environment.NewLine;
                        DirectoryInfo dir2 = new DirectoryInfo(TempPath);
                        dir2.Refresh();
                        var RawDataSupportingDocuments = LSDRC.GetByEntityType("RawData", TransactionId).ToList();
                        if (RawDataSupportingDocuments.Count() > 0)
                        {
                            if (System.IO.File.Exists(CompleteFileName))
                                zip.AddFile(CompleteFileName, "");
                        }
                        foreach (var Attachment in RawDataSupportingDocuments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                IsSingleFile = false;
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                zip.AddFile(yy, "Supporting Documents");
                            }
                        }
                        // System.IO.File.Copy(CompleteFileName, "C:\\/SOSTemp/"+FileName);
                        // CompleteFileName = "C:\\/SOSTemp/" + FileName;
                        // var FileList = Directory.GetFiles(TempPath, "ExportRawDataFile*.*", SearchOption.AllDirectories).ToList();
                        // var TempFileList= string.Join(",",FileList);
                        //TempEmailBody += TempFileList;
                        // Globals.SendEmail(TempEmailBody, "Testing Download Issue", "ssharma@megacube.com.au", "catvikas@gmail.com", null);

                        //    Response.Clear();
                        //    Response.WriteFile(CompleteFileName);//.Write(exportData);
                        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Tmp.xlsx"));
                        //    Response.End();

                        //return View("AttachTestResults");
                        // var RawData =System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ExportCalcDocumentPath"] + "/" + CompanyCode + "/ExportRawDataFile.csv");
                        //using (StreamReader r = new StreamReader(CompleteFileName, Encoding.UTF8))
                        //{
                        //    var encoding = Encoding.UTF8;
                        //    var CompleteFileNameInBytes = encoding.GetBytes(r.ReadToEnd());
                        //    return File(CompleteFileNameInBytes, "application/csv;charset=UTF-8", "ExportRawDataFile.csv");
                        //}
                        //IsSingleFile = false;
                        // zip.AddFile(data, "");
                        break;
                    //case "Schemes":
                    //    //Get Attachments associated with the Schemes which are saved in LSupporting Douments table
                    //    var SchemesDocuments = LSDRC.GetByEntityType("LSchemes", TransactionId).ToList();

                    //    //Get Attached Files from LAttachments table
                    //    var AttachedFiles = LARC.GetByEntityType("LSchemes", TransactionId);

                    //    foreach (var Attachment in SchemesDocuments)
                    //    {

                    //        if (SchemesDocuments.Count() == 1 && AttachedFiles.Count() == 0)
                    //        {

                    //            var filePath = Attachment.LsdFilePath;
                    //            CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                    //            FileName = Attachment.LsdFileName;
                    //            var xx = CompleteFileName.Split('.');
                    //            extension = xx.LastOrDefault();
                    //            FileType = Globals.GetFileContentType(extension);
                    //            var ByteData = Globals.DownloadFromS3(FileName, filePath);
                    //            return File(ByteData, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently
                    //        }
                    //        else
                    //        {
                    //            IsSingleFile = false;
                    //        }
                    //        //Adding in zip for the case if attachments are found ahead then files will be exported as zip

                    //        var FileData = Globals.DownloadFromS3(Attachment.LsdFileName, Attachment.LsdFilePath);
                    //        var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Attachment.LsdFileName;
                    //        System.IO.File.WriteAllBytes(TempFilePath, FileData); // Requires System.IO                                                      //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                    //        zip.AddFile(TempFilePath, "Supporting Documents");


                    //        //}
                    //    }

                    //    //Get Attached Files from LAttachments table
                    //    //var AttachedFiles = LARC.GetByEntityType("LSchemes", TransactionId);
                    //    foreach (var Attachment in AttachedFiles)
                    //    {
                    //        //if (System.IO.Directory.Exists(Attachment.LaFilePath))
                    //        // {
                    //        if (AttachedFiles.Count() == 1 && SchemesDocuments.Count() == 0)
                    //        {
                    //            var filePath = Attachment.LaFilePath;
                    //            CompleteFileName = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                    //            FileName = Attachment.LaFileName;
                    //            var xx = CompleteFileName.Split('.');
                    //            extension = xx.LastOrDefault();
                    //            FileType = Globals.GetFileContentType(extension);
                    //            var ByteData = Globals.DownloadFromS3(FileName, filePath);
                    //            return File(ByteData, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently

                    //        }
                    //        else
                    //        {
                    //            IsSingleFile = false;
                    //            //var yy = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                    //            //zip.AddFile(yy, "");
                    //            var FileData = Globals.DownloadFromS3(Attachment.LaFileName, Attachment.LaFilePath);
                    //            var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Attachment.LaFileName;
                    //            System.IO.File.WriteAllBytes(TempFilePath, FileData); // Requires System.IO
                    //            zip.AddFile(TempFilePath, "");
                    //        }
                    //        // }
                    //    }

                    //    break;
                    case "Schemes":
                        //Get Attachments associated with the Schemes which are saved in LSupporting Douments table
                        var SchemesDocuments = LSDRC.GetByEntityType("LSchemes", TransactionId).ToList();
                        if (SchemesDocuments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in SchemesDocuments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                if (SchemesDocuments.Count() == 1)
                                {
                                    var filePath = Attachment.LsdFilePath;
                                    CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    FileName = Attachment.LsdFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                }
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                zip.AddFile(yy, "Supporting Documents");
                            }
                        }

                        //Get Attached Files from LAttachments table
                        var AttachedFiles = LARC.GetByEntityType("LSchemes", TransactionId);
                        foreach (var Attachment in AttachedFiles)
                        {
                            if (System.IO.Directory.Exists(Attachment.LaFilePath))
                            {
                                if (AttachedFiles.Count() == 1 && SchemesDocuments.Count() == 0)
                                {
                                    var filePath = Attachment.LaFilePath;
                                    CompleteFileName = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                                    FileName = Attachment.LaFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                    var yy = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                                    zip.AddFile(yy, "");
                                }
                            }
                        }

                        break;
                    case "DocumentSets":
                        //Get Attachments associated with the Schemes which are saved in LSupporting Douments table
                        var DSDocuments = LSDRC.GetByEntityType("LDocumentSets", TransactionId).ToList();
                        if (DSDocuments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        //Get Attached Files from LAttachments table
                        var DSAttachedFiles = LARC.GetByEntityType("LDocumentSets", TransactionId);
                        if (DSDocuments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in DSDocuments)
                        {
                            if (DSDocuments.Count() == 1 && DSAttachedFiles.Count() == 0)
                            {
                                //var filePath = Attachment.LsdFilePath;
                                //CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                //FileName = Attachment.LsdFileName;
                                //var xx = CompleteFileName.Split('.');
                                //extension = xx.LastOrDefault();
                                //FileType = Globals.GetFileContentType(extension);
                                var filePath = Attachment.LsdFilePath;
                                CompleteFileName = Attachment.LsdFilePath + Attachment.LsdFileName;
                                FileName = Attachment.LsdFileName;
                                var xx = CompleteFileName.Split('.');
                                extension = xx.LastOrDefault();
                                FileType = Globals.GetFileContentType(extension);
                                var ByteData = Globals.DownloadFromA2S(FileName, filePath);//check for the file in A2S bucket
                                if (ByteData == null)//now try downloading from sos bucket
                                {
                                    ByteData = Globals.DownloadFromS3(FileName, filePath);
                                }
                                //var ByteData = Globals.DownloadFromS3(FileName, filePath);
                                return File(ByteData, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently
                            }
                            else
                            {
                                IsSingleFile = false;
                            }
                            // var FileData = Globals.DownloadFromS3(Attachment.LsdFileName, Attachment.LsdFilePath);
                            var ByteData1 = Globals.DownloadFromA2S(Attachment.LsdFileName, Attachment.LsdFilePath);//check for the file in A2S bucket
                            if (ByteData1 == null)//now try downloading from sos bucket
                            {
                                ByteData1 = Globals.DownloadFromS3(Attachment.LsdFileName, Attachment.LsdFilePath);
                            }
                            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                            var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Attachment.LsdFileName;
                            System.IO.File.WriteAllBytes(TempFilePath, ByteData1); // Requires System.IO
                                                                                   //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                            zip.AddFile(TempFilePath, "Supporting Documents");
                        }


                        foreach (var Attachment in DSAttachedFiles)
                        {

                            if (DSAttachedFiles.Count() == 1 && DSDocuments.Count() == 0)
                            {
                                var filePath = Attachment.LaFilePath;
                                CompleteFileName = Attachment.LaFilePath + Attachment.LaFileName;
                                FileName = Attachment.LaFileName;
                                var xx = CompleteFileName.Split('.');
                                extension = xx.LastOrDefault();
                                FileType = Globals.GetFileContentType(extension);
                                //var ByteData = Globals.DownloadFromS3(FileName,filePath);
                                var ByteData1 = Globals.DownloadFromA2S(FileName, filePath);//check for the file in A2S bucket
                                if (ByteData1 == null)//now try downloading from sos bucket
                                {
                                    ByteData1 = Globals.DownloadFromS3(FileName, filePath);
                                }
                                return File(ByteData1, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently
                            }
                            else
                            {
                                IsSingleFile = false;
                                // var FileData = Globals.DownloadFromS3(Attachment.LaFileName, Attachment.LaFilePath);
                                var ByteData1 = Globals.DownloadFromA2S(Attachment.LaFileName, Attachment.LaFilePath);//check for the file in A2S bucket
                                if (ByteData1 == null)//now try downloading from sos bucket
                                {
                                    ByteData1 = Globals.DownloadFromS3(Attachment.LaFileName, Attachment.LaFilePath);
                                }
                                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                                    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                                var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Attachment.LaFileName;
                                System.IO.File.WriteAllBytes(TempFilePath, ByteData1); // Requires System.IO
                                zip.AddFile(TempFilePath, "");
                            }
                        }

                        break;
                    case "Accruals":
                        //Get Attachments associated with the Schemes which are saved in LSupporting Douments table
                        var AccrualsDocuments = LSDRC.GetByEntityType("LAccruals", TransactionId).ToList();
                        if (AccrualsDocuments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in AccrualsDocuments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                if (AccrualsDocuments.Count() == 1)
                                {
                                    var filePath = Attachment.LsdFilePath;
                                    CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                    FileName = Attachment.LsdFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                }
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                zip.AddFile(yy, "Supporting Documents");
                            }
                        }

                        //Get Attached Files from LAttachments table
                        var AccrualsAttachedFiles = LARC.GetByEntityType("LAccruals", TransactionId);
                        foreach (var Attachment in AccrualsAttachedFiles)
                        {
                            if (System.IO.Directory.Exists(Attachment.LaFilePath))
                            {
                                if (AccrualsAttachedFiles.Count() == 1 && AccrualsDocuments.Count() == 0)
                                {
                                    var filePath = Attachment.LaFilePath;
                                    CompleteFileName = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                                    FileName = Attachment.LaFileName;
                                    var xx = CompleteFileName.Split('.');
                                    extension = xx.LastOrDefault();
                                    FileType = Globals.GetFileContentType(extension);
                                }
                                else
                                {
                                    IsSingleFile = false;
                                    var yy = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                                    zip.AddFile(yy, "");
                                }
                            }
                        }

                        break;
                    case "RefFiles":
                        //Get Attachments associated with the RefFiles which are saved in LSupporting Douments table
                        var RefFilesDocuments = LSDRC.GetByEntityType("LRefFiles", TransactionId).ToList();
                        //Get Attached Files from LAttachments table
                        var RefAttachedFiles = LARC.GetByEntityType("LRefFiles", TransactionId);
                        if (RefFilesDocuments.Count == 0)
                        {
                            CompleteFileName = "No File Found";
                        }
                        if (RefAttachedFiles == null)
                        {
                            CompleteFileName = "No File Found";
                        }
                        foreach (var Attachment in RefFilesDocuments)
                        {
                            if (RefFilesDocuments.Count() == 1 && RefAttachedFiles.Count() == 0)
                            {
                                //var filePath = Attachment.LsdFilePath;
                                //CompleteFileName = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                //FileName = Attachment.LsdFileName;
                                //var xx = CompleteFileName.Split('.');
                                //extension = xx.LastOrDefault();
                                //FileType = Globals.GetFileContentType(extension);
                                var filePath = Attachment.LsdFilePath;
                                CompleteFileName = Attachment.LsdFilePath + Attachment.LsdFileName;
                                FileName = Attachment.LsdFileName;
                                var xx = CompleteFileName.Split('.');
                                extension = xx.LastOrDefault();
                                FileType = Globals.GetFileContentType(extension);
                                var ByteData = Globals.DownloadFromA2S(FileName, filePath);
                                return File(ByteData, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently
                            }

                            else
                            {
                                IsSingleFile = false;

                            }
                            //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                            //var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                            var FileData = Globals.DownloadFromA2S(Attachment.LsdFileName, Attachment.LsdFilePath);
                            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                            var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Attachment.LsdFileName;
                            //SG testing Copy file
                            //Globals.CopyFileFromS3toS3(Attachment.LsdFilePath, Attachment.LsdFileName, ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/", "");
                            System.IO.File.WriteAllBytes(TempFilePath, FileData); // Requires System.IO
                            zip.AddFile(TempFilePath, "Supporting Documents");
                        }

                        foreach (var Attachment in RefAttachedFiles)
                        {
                            //if (System.IO.Directory.Exists(Attachment.LaFilePath))
                            //{
                            //if (RefAttachedFiles.Count() == 1 && zip.Count() == 0)
                            //{
                            //    var filePath = Attachment.LaFilePath;
                            //    FileName = Attachment.LaFileName;
                            //    CompleteFileName = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                            //    var xx = CompleteFileName.Split('.');
                            //    extension = xx.LastOrDefault();
                            //    FileType = Globals.GetFileContentType(extension);
                            //}
                            if (RefAttachedFiles.Count() == 1 && RefFilesDocuments.Count() == 0)
                            {
                                var filePath = Attachment.LaFilePath;
                                CompleteFileName = Attachment.LaFilePath + Attachment.LaFileName;
                                FileName = Attachment.LaFileName;
                                var xx = CompleteFileName.Split('.');
                                extension = xx.LastOrDefault();
                                FileType = Globals.GetFileContentType(extension);
                                var ByteData = Globals.DownloadFromA2S(FileName, filePath);
                                return File(ByteData, "application/unknown", FileName);//This line has been added by SS because I do not want the generic function to get disturbed which does not use SDK for download currently
                            }
                            else
                            {
                                IsSingleFile = false;
                                //var yy = Attachment.LaFilePath + "/" + Attachment.LaFileName;
                                var FileData = Globals.DownloadFromA2S(Attachment.LaFileName, Attachment.LaFilePath);
                                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                                    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                                var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Attachment.LaFileName;
                                System.IO.File.WriteAllBytes(TempFilePath, FileData); // Requires System.IO
                                zip.AddFile(TempFilePath, "");
                            }
                            //}
                        }
                        break;
                }
                /*NOTE: If there is white space in the file path then download will fail giving below error
                 The handle is invalid. (Exception from HRESULT: 0x80070006 (E_HANDLE))
                */
                //Download file if not attachments are present and make a zip if multiple files are present
                if (IsSingleFile)
                {
                    if (CompleteFileName == "")
                    {
                        TempData["Message"] = "Column mapping is missing. Please contact System Analyst to configure Calc Columns in Menu Config -> Form Labels.";
                        return RedirectToAction("Index");
                    }
                    else if (!System.IO.File.Exists(CompleteFileName))//if no file found
                    {
                        TempData["Message"] = (string.IsNullOrEmpty(result)) ? "No Data or Supporting Files are available" : result;
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        return File(CompleteFileName, FileType, FileName);
                    }
                   
                }
                else
                {
                    string path = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip"))
                    {
                        System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }
                    

                    try
                    {
                        zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }catch(Exception ex)
                    {
                        TempData["Message"] = "Your personal ZIP folder could not be created.Please contact Support@vodafonelite.com and request them to create your forzip folder";
                        return RedirectToAction("Index");
                    }
                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip"))
                    {
                        return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip", "application/zip", "Export" + Workflow + ".zip");
                    }else
                    {
                        TempData["Message"] = "Your personal ZIP folder could not be created.Please contact Support@vodafonelite.com and request them to create your forzip folder";
                        return RedirectToAction("Index");
                    }
                }
            }
        }

        public ActionResult DownloadFileRawData(int TransactionId, string status)
        {
            string result = "";
            var CompleteFileName = "";
            var FileName = "";
            var FileType = "";
            var extension = "";
            var IsSingleFile = true;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;

            //Delete unnecessary files of temp folder
            if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");//Now Delete all files
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;//??Why this statement is used
                zip.AddDirectoryByName("Supporting Documents");//Create this folder only when Supporting documents are present
                var query = string.Empty;
                if (TempData["FilterQuery"] != null)
                {
                    query = Convert.ToString(TempData["FilterQuery"]);
                    TempData.Keep("FilterQuery");
                }
                // var TempEmailBody = "";
                //get all Raw Data records based on batch number and save the file in S drive in api method
                var RawDataBatch = LBRC.GetById(TransactionId);
                        result = LRRC.DownloadRawDataForStatuswise(CompanyId, RawDataBatch.LbBatchNumber, RawDataBatch.LbRawDataTableId, LoggedInUserName, query, status);
                        // Thread.Sleep(10000);//Adding temporary delay to investigate download error issue on 22JUN2017 
                        var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";

                        FileName = result;//"ExportRawDataFile.xlsx";
                        FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"application/csv;charset=UTF-8";
                        extension = "zip";
                        CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + FileName;
                        //TempEmailBody += CompleteFileName+Environment.NewLine;
                        DirectoryInfo dir2 = new DirectoryInfo(TempPath);
                        dir2.Refresh();
                        var RawDataSupportingDocuments = LSDRC.GetByEntityType("RawData", TransactionId).ToList();
                        if (RawDataSupportingDocuments.Count() > 0)
                        {
                            if (System.IO.File.Exists(CompleteFileName))
                                zip.AddFile(CompleteFileName, "");
                        }
                        foreach (var Attachment in RawDataSupportingDocuments)
                        {
                            if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                            {
                                IsSingleFile = false;
                                //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                                zip.AddFile(yy, "Supporting Documents");
                            }
                        }
                        
                        
                /*NOTE: If there is white space in the file path then download will fail giving below error
                 The handle is invalid. (Exception from HRESULT: 0x80070006 (E_HANDLE))
                */
                //Download file if not attachments are present and make a zip if multiple files are present
                if (IsSingleFile)
                {
                    if (!System.IO.File.Exists(CompleteFileName))//if no file found
                    {
                        TempData["Message"] = (string.IsNullOrEmpty(result)) ? "No Data or Supporting Files are available" : result;
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        return File(CompleteFileName, FileType, FileName);
                    }
                }
                else
                {
                    string path = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip"))
                    {
                        System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }

                    try
                    {
                        zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Your personal ZIP folder could not be created.Please contact Support@vodafonelite.com and request them to create your forzip folder";
                        return RedirectToAction("Index");
                    }
                    

                    return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip", "application/zip", "Export" + Workflow + ".zip");
                }
            }
        }

        public ActionResult DownloadFileCalcData(int TransactionId, string status)
        {
            string result = "";
            var CompleteFileName = "";
            var FileName = "";
            var FileType = "";
            var extension = "";
            var IsSingleFile = true;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;

            //Delete unnecessary files of temp folder
            if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");//Now Delete all files
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;//??Why this statement is used
                zip.AddDirectoryByName("Supporting Documents");//Create this folder only when Supporting documents are present
                var query = string.Empty;
                if (TempData["FilterQuery"] != null)
                {
                    query = Convert.ToString(TempData["FilterQuery"]);
                    TempData.Keep("FilterQuery");
                }
                var Batch = LBRC.GetById(TransactionId);

                result = LCRC.DownloadLCalcData(Batch.LbBatchNumber, CompanyCode, LoggedInUserName, CompanyId, query);
                FileName = result;//"ExportCalcFile.xlsx";
                CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + result;
                FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"text/csv";
                extension = "zip";
                //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
                var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                DirectoryInfo dir = new DirectoryInfo(FilePath);
                dir.Refresh();
                var SupportingDocuments = LSDRC.GetByEntityType("Calc", TransactionId).ToList();
                if (SupportingDocuments.Count() > 0)
                {
                    if (System.IO.File.Exists(CompleteFileName))
                        zip.AddFile(CompleteFileName, "");
                }
                foreach (var Attachment in SupportingDocuments)
                {
                    if (System.IO.Directory.Exists(Attachment.LsdFilePath))
                    {
                        IsSingleFile = false;
                        //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                        var yy = Attachment.LsdFilePath + "/" + Attachment.LsdFileName;
                        zip.AddFile(yy, "Supporting Documents");
                    }
                }
                if (IsSingleFile)
                {
                    if (!System.IO.File.Exists(CompleteFileName))//if no file found
                    {
                        TempData["Message"] = (string.IsNullOrEmpty(result)) ? "No Data or Supporting Files are available" : result;
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        return File(CompleteFileName, FileType, FileName);
                    }
                }
                else
                {
                    string path = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip"))
                    {
                        System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }

                    try
                    {
                        zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Your personal ZIP folder could not be created.Please contact Support@vodafonelite.com and request them to create your forzip folder";
                        return RedirectToAction("Index");
                    }

                    

                    return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + Workflow + ".zip", "application/zip", "Export" + Workflow + ".zip");
                }
            }
        }


        //The following method will export the Grid in csv 
        // [ControllerActionFilter]
        public ActionResult ExportGenericGrid(Nullable<int> SelectedTabIndex,string TabName,string PortfolioList)
        {
            string query = string.Empty;
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            //Get column list
            //Passing Hardcoded WorkFlowId for Manual Adjustment which is 1
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            System.Web.HttpContext.Current.Session["Workflow"] = WFName;
            var xx = RestClient.GetRolesByWorkflow(WFName, CompanyId).ToList();
            var ConfigIds = xx.ToList();
            //Get WfConfigId
            var WFConfigId = 0;
            if (ConfigIds.Count() > SelectedTabIndex.Value)
            {
                WFConfigId = ConfigIds.ElementAt(SelectedTabIndex.Value).Id;
            }
            if (TempData["FilterQuery"] != null)
            {
                query =Convert.ToString(TempData["FilterQuery"]);
                TempData.Keep("FilterQuery");
            }

            var FileName = GGRC.ExportGenericGrid(WFConfigId,LoggedInRoleId,LoggedInUserId,WFName,CompanyId,CompanyCode,TabName,PortfolioList, query);
            //Thread.Sleep(5000);//5sec delay added to avoiad error of file not found
            //var FilePath = Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName);
            ////refresh the directory
            //DirectoryInfo dir = new DirectoryInfo(FilePath);
            //dir.Refresh();
            // using (var CTextWriter = new StreamWriter(CfileLocation))
            //using (var Csv = new CsvWriter(CTextWriter))
            //{

            //    for (var j = 0; j < 1; j++)//loop one time to get column headings
            //    {
            //        foreach (var yy in (dynamic)ApiData.ElementAt(0))
            //        {
            //            if (yy.Key != "Actions" && yy.Key != "WFAnalystId" && yy.Key != "Id" && yy.Key != "WFRequesterId" && yy.Key != "WFCurrentOwnerId" && yy.Key != "WFStatus" && yy.Key != "WFRequesterId1" && yy.Key != "WFComments" && yy.Key != "row")//skipping hardcode columns which are added in data but not displayed in grid
            //            {
            //                var ColumnLabel = ColumnDetails.Where(p => p.LwfgcColumnName.Equals(yy.Key)).FirstOrDefault().LwfgcUILabel;
            //                Csv.WriteField(ColumnLabel);
            //            }
            //        }
            //    }
            //    Csv.NextRecord();

            //    for (var i = 0; i < ApiData.Count(); i++)//loop one time to get data rows
            //    {
            //        foreach (var yy in (dynamic)ApiData.ElementAt(i))
            //        {
            //            if (yy.Key != "Actions" && yy.Key != "WFAnalystId" && yy.Key != "Id" && yy.Key != "WFRequesterId" && yy.Key != "WFCurrentOwnerId" && yy.Key != "WFStatus" && yy.Key != "WFRequesterId1" && yy.Key != "WFComments" && yy.Key != "row")//Exclude the columns from The Grid Data 
            //            {
            //                Csv.WriteField(yy.Value);
            //            }
            //        }
            //        Csv.NextRecord();
            //    }

            //}
          //  if (System.IO.File.Exists(FileName))
           // {
               byte[] FileData= DownloadFromS3(FileName);
            // return File(FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportData(" + WFName + ").xlsx");
            // }
            if (FileData == null)
            {
                TempData["Error"] = "No file/Data found";
            }
            else
            {
                return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",FileName);
            }
            return RedirectToAction("Index");
        }
        //this method has been created to allow dashboard download with BatchNumber
        public ActionResult DownloadDashboardByBatchNumber(int BatchNumber)
        {
            var BatchDetails = LBRC.GetByBatchNumber(BatchNumber);
            return RedirectToAction("DownloadDashboardFile",new { TransactionId=BatchDetails.Id });
        }

        //This is a Generic Method to Download Dashboard as User Clicks on Dashboard Link
        // [ControllerActionFilter]
        public ActionResult DownloadDashboardFile(int TransactionId)
        {
            string FileName = "";
            string FileExtension = "";
            string FileType = "";
            var BatchDetails = LBRC.GetById(TransactionId);
            string DashboardFilePath = DashboardFilePath = System.Configuration.ConfigurationManager.AppSettings["S3_A2SCommomFilePath"].ToString() + "/" + CompanyCode + "/" + BatchDetails.LbBatchType + "/"; ;
            string DashboardFileName = BatchDetails.LbBatchNumber + "_*.*";
            var DashboardFileList = Directory.GetFiles(DashboardFilePath, DashboardFileName, SearchOption.AllDirectories).ToList();//BatchDetails.LbBatchNumber.ToString() + "*";

            var IsSingleFile = true;
          //  using (ZipFile zip = new ZipFile())
           // {
              //  zip.AlternateEncodingUsage = ZipOption.AsNecessary;

                for (var i = 0; i < DashboardFileList.Count(); i++)
                {
                    if (DashboardFileList.Count() == 1)
                    {

                        var xx = DashboardFileList[i].Split('.');
                        FileExtension = xx.LastOrDefault();
                        FileType = Globals.GetFileContentType(FileExtension);
                        FileName = DashboardFileList[i];
                        IsSingleFile = true;
                    }
                    else
                    {
                        IsSingleFile = false;
                        //zip.AddFile(DashboardFileList[i], "");
                    }

                }

               // Download file if not attachments are present and make a zip if multiple files are present
                if (IsSingleFile)
                {

                    if (System.IO.File.Exists(FileName))//if file exists then export it
                    {
                        var FileNameWithoutPath = FileName.Split('/');
                        var zz = FileNameWithoutPath.LastOrDefault();
                        return File(FileName, FileType, zz);
                    }
                }
                else
                {
                    List<GenericModelFilesViewModel> objFilesData = new List<GenericModelFilesViewModel>();

                    for (var i = 0; i < DashboardFileList.Count(); i++)
                    {
                        GenericModelFilesViewModel objfile = new GenericModelFilesViewModel();
                        objfile.FileName = DashboardFileList[i];
                        objfile.FileName = DashboardFileList[i].Split('/').Last();
                        objfile.DisplayFileName = DashboardFileList[i].Split('/').Last();
                        objfile.FileName = objfile.FileName.Replace(" ", "$");
                        objfile.FileName = objfile.FileName.Replace("&", "~");

                        DashboardFileList[i] = DashboardFileList[i].Replace("U:", "transfer/a2s");
                        DashboardFileList[i] = DashboardFileList[i].Replace(" ", "$");
                        DashboardFileList[i] = DashboardFileList[i].Replace("&", "~");

                    objfile.FilePath = "<i class='fa fa-download'  style='color:#e60000;font-size:20px ;cursor: pointer; !important;' aria-hidden='true' onclick=DownloadfileTemp('" + objfile.FileName + "','" + DashboardFileList[i] + "')></i>";

                        objFilesData.Add(objfile);
                    }

                    TempData["GridSectionItems"] = objFilesData;
                    return RedirectToAction("GlobalDownloadGrid","GenericGrid");

                    //Commented By SG for Creating new common download page for Dashboard.
                    //zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip");
                    //return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip", "application/zip", BatchDetails.LbBatchType + "_Dashboard_" + BatchDetails.LbBatchNumber + ".zip");
                }
           // }

            TempData["Message"] = "There is no Dashboard available";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            // return RedirectToAction("Index");
        }

        
        public ActionResult GlobalDownloadGrid()
        {
            var objFilesData = TempData["GridSectionItems"];
            return View(objFilesData);
        }

        public JsonResult GetGenericGridCounts(Nullable<int> WFConfigId,string TabName,string PortfolioList)
        {
            if (string.IsNullOrEmpty(PortfolioList))
                PortfolioList = string.Empty;
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            var GridCounts = GGRC.GetGenericGridCounts(WFConfigId.Value, LoggedInRoleId, LoggedInUserId, WFName, CompanyId,TabName, PortfolioList);
            return Json(GridCounts, JsonRequestBehavior.AllowGet);
        }
        //Generic method to get grid data
        //[ControllerActionFilter]
        //The variable names a pagesize and pagenum are default for generic grid
        public JsonResult GetGridData(Nullable<int> WFConfigId, string sortdatafield, string sortorder, int pagesize, int pagenum, bool IsGridReloading,string TabName,string PortfolioList)//resolve with vg
        {
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(PortfolioList))
                PortfolioList = string.Empty;
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);

            if (TempData["FilterQuery"] != null )
            {
                TempData["FilterQuery"] = FilterQuery;
                if(Session["WFConfigId"] == null)
                {
                    Session["WFConfigId"] = WFConfigId;
                } else {
                    if (FilterQuery == "" && Convert.ToInt32(Session["WFConfigId"]) == WFConfigId)
                    {
                        SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
                        GGRC.DeleteUserPreferenceData(LoggedInUserId, WFName, TabName, "Filters", Convert.ToString(WFConfigId), ObjSession._UserSessionID);
                    }else
                    {
                        Session["WFConfigId"] = WFConfigId;

                    }
                }
            }
            else
            {
                TempData["FilterQuery"] = FilterQuery;
                TempData["TabName"] = TabName;
                Session["WFConfigId"] = WFConfigId;
            }
            //NOTE: As save state and load state are not working to retain the Grid Filter condition. Storing the filter query in session
            //if(IsGridReloading)
            //{
            //    var QueryList =new List<string>();

            //}
            if(FilterQuery != null && FilterQuery != "")
            {
                List<FilterDataViewModel> objData = new List<FilterDataViewModel>();
                string query = qry.ToString();
                int filterCount = 0;
                //int count = query.Trim().Split('filtervalue').Count();
                string[] data = query.Split('&');
                foreach(string data1 in data)
                {
                    string[] data2 = data1.Split('=');
                    //foreach(string data3 in data2)
                    //{
                        if(data2[0] == "filterscount")
                        {
                            filterCount =Convert.ToInt32(data2[1]);
                        }
                    //}
                }
                for (var i = 0; i < filterCount; i += 1)
                {
                    FilterDataViewModel obhFilter = new FilterDataViewModel();
                    obhFilter.filtervalue =Convert.ToString(qry.GetValues("filtervalue" + i)[0]);
                    obhFilter.filtercondition = Convert.ToString(qry.GetValues("filtercondition" + i)[0]);
                    obhFilter.filterdatafield = Convert.ToString(qry.GetValues("filterdatafield" + i)[0]);
                    obhFilter.filteroperator = Convert.ToString(qry.GetValues("filteroperator" + i)[0]);
                    obhFilter.IsUsed = "false";
                    objData.Add(obhFilter);
                }
                // for (int i = 0; i < filterCount; i++)
                //{
                //    FilterDataViewModel obhFilter = new FilterDataViewModel();
                //    foreach (string data1 in data)
                //    {
                //        string[] data2 = data1.Split('=');
                //        if("filtervalue"+i == data2[0])
                //        {
                //            obhFilter.filtervalue = data2[1].Replace("+", " "); ;
                //        }
                //        if ("filtercondition" + i == data2[0])
                //        {
                //            obhFilter.filtercondition = data2[1];
                //        }
                //        if ("filteroperator" + i == data2[0])
                //        {
                //            obhFilter.filteroperator = data2[1];
                //        }
                //        if ("filterdatafield" + i == data2[0])
                //        {
                //            obhFilter.filterdatafield = data2[1];
                //        }
                //        obhFilter.IsUsed = "false";
                //    }
                //    objData.Add(obhFilter);
                //}
                string WFCon = Convert.ToString(WFConfigId);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(objData);
                SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
                GGRC.SaveUserPreferenceData(LoggedInUserId, WFName, TabName, "Filters", WFCon, jsonSerialiser.Serialize(objData), ObjSession._UserSessionID);
            }
            if(pagesize != 200)
            {
                string WFCon = Convert.ToString(WFConfigId);
                SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
                GGRC.SaveUserPreferenceData(LoggedInUserId, WFName, TabName, "PageSize", WFCon, pagesize.ToString(), ObjSession._UserSessionID);
            }
            
            var ApiData = GGRC.GetByWorkflowId(WFConfigId.Value, LoggedInRoleId, LoggedInUserId, WFName, CompanyId, pagesize, pagenum, sortdatafield, sortorder, FilterQuery,TabName, PortfolioList);
            return Json(ApiData.AsEnumerable(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult SaveUserPreferenceData(string WFConfigId, string[] Config,string TabName)
        {
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            var jsonSerialiser = new JavaScriptSerializer();
            GGRC.SaveUserPreferenceData(LoggedInUserId, WFName, TabName, "GridSettings", WFConfigId, jsonSerialiser.Serialize(Config), ObjSession._UserSessionID);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteUserPreferenceData(string WFConfigId, string TabName)
        {
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            //GGRC.DeleteUserPreferenceData(LoggedInUserId, "", "", "GridSettings", WFConfigId, ObjSession._UserSessionID);
            //GGRC.DeleteUserPreferenceData(LoggedInUserId, "", "", "PageSize", WFConfigId, ObjSession._UserSessionID);
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            GGRC.DeleteUserPreferenceData(LoggedInUserId, WFName, TabName, "Filters", WFConfigId, ObjSession._UserSessionID);
            
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        

        //This method will be called when user clicks on delete icon to remove 
        // [ControllerActionFilter]
        public ActionResult DeleteSupportingDocument(int id, int EntityId, string EntityType, string FormType)
        {
            LSDRC.Delete(id);
            switch (EntityType)
            {
                case "LUsers":
                    return RedirectToAction("Edit", "LUsers", new { TransactionId = EntityId, FormType = FormType });
                case "LPayees":
                    return RedirectToAction("Create", "LPayees", new { TransactionId = EntityId, FormType = FormType });
                case "LClaims":
                    return RedirectToAction("Edit", "LClaims", new { TransactionId = EntityId });
                case "LSupportTickets":
                    return RedirectToAction("Edit", "LSupportTickets", new { Id = EntityId });
            }
            return RedirectToAction("Index");//If Form Type Not found redirect to Index page
        }

        //This method will call a form to attach test results in the LAttachments of type Base table 
        public ActionResult AttachTestResults(int TransactionId)
        {
            ViewBag.TransactionId = TransactionId;
            return View();
        }

        [HttpPost]
        public ActionResult AttachTestResults(int TransactionId, HttpPostedFileBase[] File1)
        {
            var AttachedFiles = "";
            var AttachedfilePath = "";
            foreach (HttpPostedFileBase file in File1)
            {
                if (file.ContentLength > 0)
                {

                    var fileLocation = "";
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;

                    var filePath = "";
                    filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/RefFiles/Schemes");
                    AttachedfilePath = filePath;
                    fileLocation = filePath + "/" + FileNames;
                    //check if directory exists or not. iIf notcreate that directory
                    bool exists = System.IO.Directory.Exists(filePath);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    file.SaveAs(fileLocation);

                    //made a comma seperated list of  file names
                    if (string.IsNullOrEmpty(AttachedFiles))
                    {
                        AttachedFiles = FileNames;
                    }
                    else
                    {
                        AttachedFiles = AttachedFiles + "," + FileNames;
                    }

                }
            }

            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            switch (WFName)
            {
                case "Schemes":
                    LLSRC.AttachTestResults(LoggedInUserId, AttachedFiles, AttachedfilePath, TransactionId);
                    break;
            }
            return RedirectToAction("Index");
        }
        //Method to create payee temmplate
        [ControllerActionFilter]
        public ActionResult DownloadPayeeTemplate()
        {
            //CreateDownloadableTemplate("Payee");
            //var payeetemplatepath = ConfigurationManager.AppSettings["UploadTemplatePath"] + "\\Payee Upload\\" + System.Web.HttpContext.Current.Session["CompanyCode"] + "_PayeesUpload.xlsx";
            ////var payeetemplatepath = ConfigurationManager.AppSettings["TempDocumentPath"] +"\\"+ System.Web.HttpContext.Current.Session["CompanyCode"] + "_PayeesUpload.xlsx";
            //if (System.IO.File.Exists(payeetemplatepath))
            //{
            //    return File(payeetemplatepath, "application/pdf", System.Web.HttpContext.Current.Session["CompanyCode"] + "_PayeesUpload.xlsx");
            //}
            //TempData["Error"] = "No File found";
            //return Redirect(System.Web.HttpContext.Current.Session["from"] as string);

            //NOW PayeeUpload Template is generated from API and file name is returned same as RawData
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var result = LPRC.MyPayeeReport(CompanyId, LoggedInUserId, LoggedInRole, string.Empty, LoggedInUserName, false);
            Thread.Sleep(3000);
            //Added below code for this issue :- My Payees report (Change download mechanism to be same as raw data)
            if (!string.IsNullOrEmpty(result))
            {
                byte[] FileData = DownloadFromS3(result);
                if (FileData == null)
                {
                    TempData["Error"] = "No file/Data found";
                }
                else
                {
                    return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.Web.HttpContext.Current.Session["CompanyCode"] + "_PayeesUpload.xlsx");
                }
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        [ControllerActionFilter]
        public ActionResult DownloadClaimTemplate()
        {
            CreateDownloadableTemplate("Claims");
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var result = LPRC.MyPayeeReport(CompanyId, LoggedInUserId, LoggedInRole, string.Empty, LoggedInUserName, false);
            var claimtemplatepath = ConfigurationManager.AppSettings["UploadTemplatePath"] + "\\Claims Upload\\" + System.Web.HttpContext.Current.Session["CompanyCode"] + "_ClaimsUpload.xlsx";
            //var claimtemplatepath = ConfigurationManager.AppSettings["TempDocumentPath"] + "\\" + System.Web.HttpContext.Current.Session["CompanyCode"] + "_ClaimsUpload.xlsx";
            if (System.IO.File.Exists(claimtemplatepath))
            {
                return File(claimtemplatepath, "application/pdf", System.Web.HttpContext.Current.Session["CompanyCode"] + "_ClaimsUpload.xlsx");
            }
            TempData["Error"] = "No File found";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        public ActionResult DownloadUserTemplate()
        {
            
            string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
            string strFileName = strUploadBasePath + "\\Users Upload\\" + System.Web.HttpContext.Current.Session["CompanyCode"] + "_UsersUpload.xlsx";
            //Create Directory if Not Present
            if (!System.IO.Directory.Exists(strUploadBasePath + "\\Users Upload\\" + CompanyCode))
            {
                System.IO.Directory.CreateDirectory(strUploadBasePath + "\\Users Upload\\" + CompanyCode);
            }
            //Delete previous template
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }
            IWorkbook workbook = new XSSFWorkbook();
            ICell cell;
            #region UserSheet


            //ICell cell2;
            //ISheet sheet1 = workbook.CreateSheet("Users");
            //IRow row1 = sheet1.CreateRow(0);
            //row1.ZeroHeight = true;
            //IRow row2 = sheet1.CreateRow(1);
            //#region toprow
            //cell = row1.CreateCell(0); cell.SetCellValue("LuFirstName");
            //cell = row1.CreateCell(1); cell.SetCellValue("LuLastName");
            //cell = row1.CreateCell(2); cell.SetCellValue("LuEmail");
            //cell = row1.CreateCell(3); cell.SetCellValue("LuPhone");
            //cell = row1.CreateCell(4); cell.SetCellValue("LuBlockNotification");
            //cell = row1.CreateCell(5); cell.SetCellValue("WFComments");
            //cell = row1.CreateCell(6); cell.SetCellValue("LuIsManager");
            //cell = row1.CreateCell(7); cell.SetCellValue("LuBand");
            //cell = row1.CreateCell(8); cell.SetCellValue("LuStatus");
            //cell = row1.CreateCell(9); cell.SetCellValue("A01");
            //cell = row1.CreateCell(10); cell.SetCellValue("A02");
            //cell = row1.CreateCell(11); cell.SetCellValue("A03");
            //cell = row1.CreateCell(12); cell.SetCellValue("A04");
            //cell = row1.CreateCell(13); cell.SetCellValue("A05");
            //cell = row1.CreateCell(14); cell.SetCellValue("AN01");
            //cell = row1.CreateCell(15); cell.SetCellValue("AN02");
            //cell = row1.CreateCell(16); cell.SetCellValue("AN03");
            //cell = row1.CreateCell(17); cell.SetCellValue("AN04");
            //cell = row1.CreateCell(18); cell.SetCellValue("AN05");
            //cell = row1.CreateCell(19); cell.SetCellValue("AD01");
            //cell = row1.CreateCell(20); cell.SetCellValue("AD02");
            //cell = row1.CreateCell(21); cell.SetCellValue("AD03");
            //cell = row1.CreateCell(22); cell.SetCellValue("AD04");
            //cell = row1.CreateCell(23); cell.SetCellValue("AD05");
            //cell = row1.CreateCell(24); cell.SetCellValue("LuReportsToId");
            //cell = row1.CreateCell(25); cell.SetCellValue("LuIsAlteryxUser");

            //#endregion
            //#region 2nd row
            //cell2 = row2.CreateCell(0); cell2.SetCellValue("First Name");
            //cell2 = row2.CreateCell(1); cell2.SetCellValue("Last Name");
            //cell2 = row2.CreateCell(2); cell2.SetCellValue("Email");
            //cell2 = row2.CreateCell(3); cell2.SetCellValue("Phone");
            //cell2 = row2.CreateCell(4); cell2.SetCellValue("Block Notification");
            //cell2 = row2.CreateCell(5); cell2.SetCellValue("Comments");
            //#endregion
            //workbook.CreateSheet();
            #endregion
            #region Portfolio
            ISheet sheet2 = workbook.CreateSheet("UserPortfolios");
            IRow row3 = sheet2.CreateRow(0);
            cell = row3.CreateCell(0); cell.SetCellValue("UserEmailID");
            cell = row3.CreateCell(1); cell.SetCellValue("UserRole");
            cell = row3.CreateCell(2); cell.SetCellValue("PrimaryChannel");
            cell = row3.CreateCell(3); cell.SetCellValue("Channel");
            cell = row3.CreateCell(4); cell.SetCellValue("BusinessUnit");
            row3.ZeroHeight = true;
            IRow row4 = sheet2.CreateRow(1);
            cell = row4.CreateCell(0); cell.SetCellValue("User Email");
            cell = row4.CreateCell(1); cell.SetCellValue("User Role");
            cell = row4.CreateCell(2); cell.SetCellValue("Primary Channel");
            cell = row4.CreateCell(3); cell.SetCellValue("Channel");
            cell = row4.CreateCell(4); cell.SetCellValue("Business Unit");
            workbook.CreateSheet();
            #endregion
            //workbook.RemoveSheetAt(workbook.GetSheetIndex("Sheet1"));
            
            FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);

            workbook.Write(xfile);
            xfile.Close();

            var usertemplatepath = ConfigurationManager.AppSettings["UploadTemplatePath"] + "\\Users Upload\\" + System.Web.HttpContext.Current.Session["CompanyCode"] + "_UsersUpload.xlsx";
            if (System.IO.File.Exists(usertemplatepath))
            {
                return File(usertemplatepath, "application/pdf", System.Web.HttpContext.Current.Session["CompanyCode"] + "_UsersUpload.xlsx");
            }
            TempData["Error"] = "No File found";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        //This method will download Payee Record To an excel based on Role loggedIn
        public ActionResult DownloadPayeeReport()
        {
            /*ILCompanySpecificColumnsRestClient RestClientCSCRC = new LCompanySpecificColumnsRestClient();
            var APIData = RestClientCSCRC.GetPayeeColumnsByCompanyIdForGrid(CompanyId).OrderByDescending(p=>p.LcscDisplayOnForm);
            DataTable PayeeData = Globals.GetPayeeList(string.Empty,true);//Payee Data to be added in excel file
            var ListOfPayeeIds = string.Join(",", PayeeData.AsEnumerable().Select(r => r.Field<Int64>("Id")).ToList());
            //GetParent List
            ILPayeesRestClient LPARC = new LPayeesRestClient();
            var ParentList = LPARC.GetParentListByPayeeId(ListOfPayeeIds);
            //Get Portfolio Data
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var PortfolioData = LPORC.GetByEntityIdList("LPayees",ListOfPayeeIds);
            //Generate a Workbook having Sheet to contain data as wellas Portfolios
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Payees");
            IRow row1 = sheet1.CreateRow(0);
            //First row of  headers sameas DB columns
            //Extra Columns added by RK
            ICell Extracell = row1.CreateCell(0);
            Extracell.SetCellValue("GiveCMRole");
            Extracell = row1.CreateCell(1);
            Extracell.SetCellValue("LpParentCode");
            for (int j = 0; j < APIData.Count(); j++)
            {
                ICell cell = row1.CreateCell(j+2);
                string columnName = APIData.ElementAt(j).ColumnName.ToString();
                if (columnName != "WFComments")
                {
                    if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                        columnName = APIData.ElementAt(j).LcscColumnName;
                    else
                        columnName = "Lp" + APIData.ElementAt(j).LcscColumnName;
                    cell.SetCellValue(columnName);
                }
                // GC is used to avoid error System.argument exception
                GC.Collect();
            }
            row1.ZeroHeight = true;
            IRow row2 = sheet1.CreateRow(1);
            //Extra Columns added by RK
            Extracell = row2.CreateCell(0);
            Extracell.SetCellValue("GiveCMRole");
            Extracell = row2.CreateCell(1);
            Extracell.SetCellValue("LpParentCode");
            //Second row of  headers sameas Company Specific Labels
            for (int j = 0; j < APIData.Count(); j++)
            {
                ICell cell = row2.CreateCell(j+2);
                string ColumnLabel = "";
                string columnName = APIData.ElementAt(j).LcscColumnName;
                if (columnName != "WFComments")
                {
                    if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                        columnName = APIData.ElementAt(j).LcscColumnName;
                    else
                        columnName = "Lp" + APIData.ElementAt(j).LcscColumnName;
                    ColumnLabel = APIData.ElementAt(j).LcscLabel;
                    if (string.IsNullOrEmpty(ColumnLabel))
                        ColumnLabel = APIData.ElementAt(j).LcscColumnName;
                    if (APIData.ElementAt(j).LcscDisplayOnForm)
                        cell.SetCellValue(ColumnLabel);
                }
                   
                // GC is used to avoid error System.argument exception
                GC.Collect();
            }


            //loops through data  
            for (int i = 0; i < PayeeData.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 2);
                //Extra Columns added by RK
                Extracell = row.CreateCell(0);
                Extracell.SetCellValue(PayeeData.Rows[i]["LpFinOpsRoles"].ToString());
                Extracell = row.CreateCell(1);
                Extracell.SetCellValue(ParentList.ElementAt(i));
                for (int j = 0; j < APIData.Count(); j++)
                {
                    ICell cell = row.CreateCell(j+2);
                    string columnName = APIData.ElementAt(j).LcscColumnName;
                    if (columnName != "WFComments")
                    {
                        if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                            columnName = APIData.ElementAt(j).LcscColumnName;
                        else
                            columnName = "Lp" + APIData.ElementAt(j).LcscColumnName;
                        if (APIData.ElementAt(j).LcscDisplayOnForm)
                            cell.SetCellValue(PayeeData.Rows[i][columnName].ToString());
                    }
                  
                }
            }

            //Portfolio Sheet
            ISheet sheetPort = workbook.CreateSheet("Portfolios");
            IRow rowPort = sheetPort.CreateRow(0);
            ICell Portfoliocell = rowPort.CreateCell(0);
            Portfoliocell.SetCellValue("Payee Code");
            Portfoliocell = rowPort.CreateCell(1);
            Portfoliocell.SetCellValue("Primary Channel");
            Portfoliocell = rowPort.CreateCell(2);
            Portfoliocell.SetCellValue("Channel");
            Portfoliocell = rowPort.CreateCell(3);
            Portfoliocell.SetCellValue("Business Unit");
            //loops through Portfolio 
            for (int i = 0; i < PortfolioData.Count(); i++)
            {
                IRow row = sheetPort.CreateRow(i + 1);
                Portfoliocell = row.CreateCell(0);
                Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).LpPayeeCode);
                Portfoliocell = row.CreateCell(1);
                Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).RcPrimaryChannel);
                Portfoliocell = row.CreateCell(2);
                Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).RcName);
                Portfoliocell = row.CreateCell(3);
                Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).LpBusinessUnit);
            }
                /* IWorkbook workbook = new XSSFWorkbook();
                 ICell cell;
                 ISheet sheet1 = workbook.CreateSheet("Payees");
                 IRow row1 = sheet1.CreateRow(0);
                 string columnName = "";
                 string strInitial = "Lp";
                 int iDx = 0;
                 cell = row1.CreateCell(iDx);
                 cell.SetCellValue("GiveCMRole");
                 iDx++;
                 cell = row1.CreateCell(iDx);
                 cell.SetCellValue("LpParentCode");
                 iDx++;
                 for (int j = 0; j < APIData.Count(); j++)
                 {
                     if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                     {
                         columnName = APIData.ElementAt(j).LcscColumnName;
                         if (columnName != "WFComments")
                         {
                             if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                 columnName = APIData.ElementAt(j).LcscColumnName;
                             else
                                 columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                             cell = row1.CreateCell(iDx);
                             cell.SetCellValue(columnName);
                         }
                         iDx++;
                         if (columnName == "WFComments") iDx = iDx - 1;
                     }

                 }
                 cell = row1.CreateCell(iDx);
                 cell.SetCellValue("WFComments");
                 iDx++;
                 for (int k = 0; k < APIData.Count(); k++)
                 {
                     if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
                     {
                         columnName = APIData.ElementAt(k).LcscColumnName;
                         if (columnName != "WFComments")
                         {
                             if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                 columnName = APIData.ElementAt(k).LcscColumnName;
                             else
                                 columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
                             cell = row1.CreateCell(iDx);
                             cell.SetCellValue(columnName);

                         }
                         iDx++;
                         if (columnName == "WFComments") iDx = iDx - 1;
                     }
                 }
                 row1.ZeroHeight = true;
                 iDx = 0;
                 IRow row2 = sheet1.CreateRow(1);
                 cell = row2.CreateCell(iDx);
                 cell.SetCellValue("GiveCMRole (Y/N)");
                 iDx++;
                 cell = row2.CreateCell(iDx);
                 cell.SetCellValue("ParentCode");
                 iDx++;
                 string columnVal = "";
                 var APIData1 = RestClientCSCRC.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                 for (int j = 0; j < APIData1.Count(); j++)
                 {
                     if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
                     {

                         if (APIData1.ElementAt(j).LcscLabel == null)
                             columnVal = APIData1.ElementAt(j).LcscColumnName;
                         else
                             columnVal = APIData1.ElementAt(j).LcscLabel;
                         if (columnVal != "WFComments")
                         {
                             cell = row2.CreateCell(iDx);
                             if (APIData1.ElementAt(j).LcscIsMandatory)
                             {
                                 cell.SetCellValue(columnVal + " *");
                             }
                             else
                             {
                                 cell.SetCellValue(columnVal);
                             }
                         }

                         iDx++;
                         if (columnVal == "WFComments") iDx = iDx - 1;
                     }
                 }
                 cell = row2.CreateCell(iDx);//
                 cell.SetCellValue("Comments");
                 //Now Add Payee Data in File

                 for (var k = 0; k < PayeeData.Rows.Count; k++)
                 {
                     iDx = 2;
                     IRow datarow = sheet1.CreateRow(k + 3);
                     for (int m = 0; m < APIData.Count(); m++)
                     {
                         if (!Convert.ToBoolean(APIData.ElementAt(m).LcscDisplayOnForm))
                         {
                             columnName = APIData.ElementAt(m).LcscColumnName;
                             if (columnName != "WFComments")
                             {
                                 if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                     columnName = APIData.ElementAt(m).LcscColumnName;
                                 else
                                     columnName = strInitial + APIData.ElementAt(m).LcscColumnName;
                                 if (columnName == "LpFinOpsRoles")
                                 {
                                     NPOI.SS.UserModel.ICell Datacell = datarow.CreateCell(0);
                                     Datacell.SetCellValue(PayeeData.Rows[k].Field<string>(columnName));
                                 }
                                 else
                                 {
                                     NPOI.SS.UserModel.ICell Datacell = datarow.CreateCell(iDx);
                                     Datacell.SetCellValue(PayeeData.Rows[k].Field<string>(columnName));
                                 }
                             }
                             iDx++;
                             if (columnVal == "WFComments") iDx = iDx - 1;
                         }
                     }
                 }
                 //Portfolio
                 ISheet sheetPort = workbook.CreateSheet("Portfolios");
                 IRow rowPort = sheetPort.CreateRow(0);
                 cell = rowPort.CreateCell(0);
                 cell.SetCellValue("Payee Code");
                 cell = rowPort.CreateCell(1);
                 cell.SetCellValue("Primary Channel");
                 cell = rowPort.CreateCell(2);
                 cell.SetCellValue("Channel");
                 cell = rowPort.CreateCell(3);
                 cell.SetCellValue("Business Unit");
                 workbook.CreateSheet();
                 GC.Collect();
                 //FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                 //workbook.Write(xfile);
               
                MemoryStream ms = new MemoryStream();
            workbook.Write(ms);  */
            //NOW Payee Report is generated from API and file name is returned same as RawData
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var result = LPRC.MyPayeeReport(CompanyId,LoggedInUserId,LoggedInRole, string.Empty,LoggedInUserName,true);
            //Added below code for this issue :- My Payees report (Change download mechanism to be same as raw data)
            if (!string.IsNullOrEmpty(result))
            {
                byte[] FileData = DownloadFromS3(result);
                if (FileData == null)
                {
                    TempData["Error"] = "No file/Data found";
                }
                else
                {
                    return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result);
                }
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
       
        private void CreateDownloadableTemplate(string strFileType)
        {

            ILCompanySpecificColumnsRestClient RestClientCSCRC = new LCompanySpecificColumnsRestClient();
            IRCommisionTypesRestClient RestClientCTRC = new RCommissionTypesRestClient();
            #region using NPOI dll
            try
            {
                if (strFileType == "Payee")
                {
                    #region Generate Payee template
                    var APIData = RestClientCSCRC.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                    string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
                    string strFileName = strUploadBasePath + "\\Payee Upload\\" + CompanyCode + "_PayeesUpload.xlsx";
                    //string strUploadBasePath = ConfigurationManager.AppSettings["TempDocumentPath"].ToString();
                    //string strFileName = strUploadBasePath + "\\" + CompanyCode + "_PayeesUpload.xlsx";
                    //NPOI.HSSF.UserModel.HSSFWorkbook workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    IWorkbook workbook = new XSSFWorkbook();
                    ICell cell;
                    ISheet sheet1 = workbook.CreateSheet("Payees");
                    IRow row1 = sheet1.CreateRow(0);
                    string columnName = "";
                    string strInitial = "Lp";
                    int iDx = 0;
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("GiveCMRole");
                    iDx++;
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("LpParentCode");
                    iDx++;
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("LpPrimaryChannel");//RK Added on 14022018 as discussed with JAS
                    iDx++;
                    for (int j = 0; j < APIData.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(j).LcscColumnName;
                            if (columnName != "WFComments")
                            {
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(j).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);

                            }
                            iDx++;
                            if (columnName == "WFComments") iDx = iDx - 1;
                        }

                    }
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("WFComments");
                    iDx++;
                    for (int k = 0; k < APIData.Count(); k++)
                    {
                        if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(k).LcscColumnName;
                            if (columnName != "WFComments")
                            {
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(k).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);

                            }
                            iDx++;
                            if (columnName == "WFComments") iDx = iDx - 1;
                        }
                    }
                    row1.ZeroHeight = true;
                    iDx = 0;
                    IRow row2 = sheet1.CreateRow(1);
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("GiveCMRole (Y/N)");
                    iDx++;
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("ParentCode");
                    iDx++;
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("Primary Channel *");//RK Added on 14022018 as discussed with JAS
                    iDx++;
                    string columnVal = "";
                    var APIData1 = RestClientCSCRC.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                    for (int j = 0; j < APIData1.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
                        {

                            if (APIData1.ElementAt(j).LcscLabel == null)
                                columnVal = APIData1.ElementAt(j).LcscColumnName;
                            else
                                columnVal = APIData1.ElementAt(j).LcscLabel;
                            if (columnVal != "WFComments")
                            {
                                cell = row2.CreateCell(iDx);
                                if (APIData1.ElementAt(j).LcscIsMandatory)
                                {
                                    cell.SetCellValue(columnVal + " *");
                                }
                                else
                                {
                                    cell.SetCellValue(columnVal);
                                }
                            }

                            iDx++;
                            if (columnVal == "WFComments") iDx = iDx - 1;
                        }
                    }
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("Comments");

                    //Portfolio
                    ISheet sheetPort = workbook.CreateSheet("Portfolios");
                    IRow rowPort = sheetPort.CreateRow(0);
                    cell = rowPort.CreateCell(0);
                    cell.SetCellValue("Payee Code");
                    cell = rowPort.CreateCell(1);
                    cell.SetCellValue("Primary Channel");
                    cell = rowPort.CreateCell(2);
                    cell.SetCellValue("Channel");
                    cell = rowPort.CreateCell(3);
                    cell.SetCellValue("Business Unit");
                    workbook.CreateSheet();
                    GC.Collect();
                    FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    workbook.Write(xfile);
                    xfile.Close();
                    #endregion
                }
                else
                {
                    Boolean blnLongTemplate = false;
                    if (LoggedInRole == "Claims Analyst") blnLongTemplate = true;
                    var APIData = RestClientCSCRC.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
                    string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
                    string strFileName = strUploadBasePath + "\\Claims Upload\\" + CompanyCode + "_ClaimsUpload.xlsx";
                    //Create Directory if Not Present
                    if (!System.IO.Directory.Exists(strUploadBasePath + "\\Claims Upload\\" + CompanyCode))
                    {
                        System.IO.Directory.CreateDirectory(strUploadBasePath + "\\Claims Upload\\" + CompanyCode);
                    }
                    //Delete previous template
                    if (System.IO.File.Exists(strFileName))
                    {
                        System.IO.File.Delete(strFileName);
                    }
                    IWorkbook workbook = new XSSFWorkbook();
                    ICell cell;
                    ICell cell2;
                    ISheet sheet1 = workbook.CreateSheet("Claims");
                    IRow row1 = sheet1.CreateRow(0);
                    string columnName = "";
                    string strInitial = "Lc";
                    int iDx = 0;
                    row1.ZeroHeight = true;
                    IRow row2 = sheet1.CreateRow(1);

                    if (blnLongTemplate)
                    {
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcClaimId");
                        iDx++;
                        for (int j = 0; j < APIData.Count(); j++)
                        {
                            if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                            {
                                columnName = APIData.ElementAt(j).LcscColumnName;
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(j).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);
                                iDx++;
                            }

                        }
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("WFComments");
                        iDx++;
                        for (int k = 0; k < APIData.Count(); k++)
                        {
                            if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
                            {
                                columnName = APIData.ElementAt(k).LcscColumnName;
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(k).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);
                                iDx++;
                            }
                        }
                        row1.ZeroHeight = true;
                        iDx = 0;
                        //IRow row2 = sheet1.CreateRow(1);
                        cell = row2.CreateCell(iDx);
                        cell.SetCellValue("Claim Id");
                        iDx++;
                        string columnVal = "";
                        var APIData1 = RestClientCSCRC.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
                        for (int j = 0; j < APIData1.Count(); j++)
                        {
                            if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
                            {
                                if (APIData1.ElementAt(j).LcscLabel == null)
                                    columnVal = APIData1.ElementAt(j).LcscColumnName;
                                else
                                    columnVal = APIData1.ElementAt(j).LcscLabel;
                                cell = row2.CreateCell(iDx);
                                if (APIData1.ElementAt(j).LcscIsMandatory)
                                {
                                    cell.SetCellValue(columnVal + " *");
                                }
                                else
                                {
                                    cell.SetCellValue(columnVal);
                                }
                                iDx++;
                            }
                        }
                        cell = row2.CreateCell(iDx);
                        cell.SetCellValue("Comments");
                        iDx++;
                        workbook.CreateSheet();
                        GC.Collect();
                    }
                    else
                    {
                        string strLcColumnName = "";
                        string columnVal = "";
                        iDx = 0;
                        for (int j = 0; j < APIData.Count(); j++)
                        {
                            if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                            {
                                columnName = APIData.ElementAt(j).LcscColumnName;
                                strLcColumnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                {
                                    columnName = APIData.ElementAt(j).LcscColumnName;
                                    if (APIData.ElementAt(j).LcscLabel == null)
                                        columnVal = APIData.ElementAt(j).LcscColumnName;
                                    else
                                        columnVal = APIData.ElementAt(j).LcscLabel;
                                    cell2 = row2.CreateCell(iDx);
                                    if (APIData.ElementAt(j).LcscIsMandatory)
                                    {
                                        columnVal = columnVal + " *";
                                    }
                                    
                                    cell = row1.CreateCell(iDx);
                                    cell.SetCellValue(columnName);
                                    cell2 = row2.CreateCell(iDx);
                                    cell2.SetCellValue(columnVal);
                                    iDx++;
                                }
                                else
                                {
                                    if (strLcColumnName != "LcAllocationDate" && strLcColumnName != "LcAlreadyPaidAmount" && strLcColumnName != "LcAlreadyPaidDate" && strLcColumnName != "LcAlreadyPaidDealer" && strLcColumnName != "LcClaimBatchNumber"
                                        && strLcColumnName != "LcClawbackAmount" && strLcColumnName != "LcClawbackPayeeCode" && strLcColumnName != "LcLastReclaimDate" && strLcColumnName != "LcRejectionReasonId" && strLcColumnName != "LcPaymentAmount"
                                        && strLcColumnName != "LcPaymentBatchNumber" && strLcColumnName != "LcReasonNonAutoPayment")
                                    {
                                        columnName = strLcColumnName;
                                        cell = row1.CreateCell(iDx);
                                        cell.SetCellValue(strLcColumnName);
                                        if (APIData.ElementAt(j).LcscLabel == null)
                                            columnVal = APIData.ElementAt(j).LcscColumnName;
                                        else
                                            columnVal = APIData.ElementAt(j).LcscLabel;
                                        //cell2 = row2.CreateCell(iDx);
                                        if (APIData.ElementAt(j).LcscIsMandatory)
                                        {
                                            columnVal = columnVal + " *";
                                        }
                                        cell = row1.CreateCell(iDx);
                                        cell.SetCellValue(columnName);
                                        cell2 = row2.CreateCell(iDx);
                                        cell2.SetCellValue(columnVal);
                                        iDx++;
                                    }

                                }
                            }
                        }
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("WFComments");
                        cell = row2.CreateCell(iDx);
                        cell.SetCellValue("Comments");
                        iDx++;
                        for (int i = 0; i < APIData.Count(); i++)
                        {
                            columnName = APIData.ElementAt(i).LcscColumnName;
                            strLcColumnName = strInitial + APIData.ElementAt(i).LcscColumnName;
                            if (!Convert.ToBoolean(APIData.ElementAt(i).LcscDisplayOnForm))
                            {
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                {
                                    columnName = APIData.ElementAt(i).LcscColumnName;
                                    cell = row1.CreateCell(iDx);
                                    cell.SetCellValue(columnName);
                                    iDx++;
                                }
                                else if(strLcColumnName != "LcAllocationDate" && strLcColumnName != "LcAlreadyPaidAmount" && strLcColumnName != "LcAlreadyPaidDate" && strLcColumnName != "LcAlreadyPaidDealer" && strLcColumnName != "LcClaimBatchNumber"
                                        && strLcColumnName != "LcClawbackAmount" && strLcColumnName != "LcClawbackPayeeCode" && strLcColumnName != "LcLastReclaimDate" && strLcColumnName != "LcRejectionReasonId" && strLcColumnName != "LcPaymentAmount"
                                        && strLcColumnName != "LcPaymentBatchNumber" && strLcColumnName != "LcReasonNonAutoPayment")
                                {
                                    
                                    
                                    columnName = strLcColumnName;
                                    cell = row1.CreateCell(iDx);
                                    cell.SetCellValue(strLcColumnName);
                                    iDx++;
                                }
                            }
                        }
                        #region Extra columns for short template
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcAllocationDate");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcAlreadyPaidAmount");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcAlreadyPaidDate");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcAlreadyPaidDealer");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcClaimBatchNumber");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcClawbackAmount");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcClawbackPayeeCode");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcLastReclaimDate");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcRejectionReasonId");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcPaymentAmount");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcPaymentBatchNumber");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcReasonNonAutoPayment");
                        iDx++;
                        cell = row1.CreateCell(iDx);
                        cell.SetCellValue("LcClaimId");
                        iDx++;
                        #endregion  
                        //row1.ZeroHeight = true;
                        iDx = 0;
                        
                        
                        workbook.CreateSheet();
                        GC.Collect();

                    }
                    #region ComissionTypes
                    ICell cell3;
                    
                    ISheet sheet2 = workbook.CreateSheet("Commission Types");
                    IRow row3 = sheet2.CreateRow(0);
                    
                    iDx = 0;
                    cell3 = row3.CreateCell(0);
                    cell3.SetCellValue("Commission Types");
                    iDx++;
                    IRow NRow;
                    ICell NCell;
                    var APIData2 = RestClientCTRC.GetByCompanyId(CompanyId);
                    for (int i = 0; i < APIData2.Count(); i++)
                    {
                        NRow = sheet2.CreateRow(iDx);
                        NCell = NRow.CreateCell(0);
                        NCell.SetCellValue(APIData2.ElementAt(i).RctName);
                        iDx++;
                    }
                    #endregion

                    FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    
                    workbook.Write(xfile);
                    xfile.Close();
                }

            }
            catch (Exception ex)
            {
                ErrorLogsRestClient er = new ErrorLogsRestClient();
                GErrorLogViewModel errlog = new GErrorLogViewModel();
                errlog.GelController = "LCompanSpecificColumns";
                errlog.GelErrorDateTime = DateTime.Now;
                errlog.GelErrorDescription = ex.ToString();
                errlog.GelErrorOwner = "";
                errlog.GelErrorType = "test";
                errlog.GelFieldName = "create excel";
                errlog.GelMethod = "CreateDownloadableTemplate";
                errlog.GelResolution = "";
                errlog.GelSOSBatchNumber = 0;
                errlog.GelSourceProject = "WebAPP";
                errlog.GelStackTrace = ex.StackTrace.ToString();
                errlog.GelUserName = "SOS";
                er.Add(errlog);
            }

            #endregion
        }

        /*
         this method has been added to convert filestream to byte array because the Amazon file stream connection get closed as we move
        out of DownloadFromS3 method
             */
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        /// <summary>
        /// Ready the ey value from Database
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetValue(string Key)
        {
            IGKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            var Policy = KVRC.GetByName(Key, 1);
            if (Policy != null)
                return WebUtility.HtmlDecode(Policy.GkvValue);
            else
                return "";
        }

        //section to download file from S3 drive drectly 
        public byte[] DownloadFromS3(string FileName)
        {
            string _awsAccessKey = GetValue("sos_accesskey");// SOSAWSAccessKey"); // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = GetValue("sos_secretkey");// SOSAWSSecretKey"); //ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            byte[] FileData;
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode , LoggedInUserName, FileName),//R3.1 CompanyCode.ToLower() is removed
                };
               
                using (GetObjectResponse response = client.GetObject(request))
                {
                    //string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), FileName);
                    //if (!System.IO.File.Exists(dest))
                    //{
                        FileData= ReadFully(response.ResponseStream);//response.WriteResponseStreamToFile(dest);
                   // }
                }
            }
            return FileData;
        }
        public ActionResult DownLoadCommon(string FilePath, string FileName, string from)
        {
            try
            {
                var xx = FileName.Split('.');
                var FileExtension = xx.LastOrDefault();
                var FileType = Globals.GetFileContentType(FileExtension);

                var FileNameWithoutPath = FileName.Split('/');
                var zz = FileNameWithoutPath.LastOrDefault();

                if (from == "a2s")
                {
                    FileName = FileName.Replace("$", " ");
                    FileName = FileName.Replace("~", "&");
                    FilePath = FilePath.Replace("$", " ");
                    FilePath = FilePath.Replace("~", "&");
                    var data = GetFilesFromAWS(FileName, FilePath);

                    return File(data, FileType, FileName);
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            return null;
        }

        public byte[] GetFilesFromAWS(string filename, string FilePath)
        {
            //filename = filename.Replace("$", " ");
            string _awsAccessKey = Globals.GetValue("int_accesskey"); // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = Globals.GetValue("int_secretkey"); //ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];


            byte[] FileData;
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    //Key = string.Format("{0}{1}", FilePath, filename),
                    Key = FilePath,
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    FileData = ReadFully(response.ResponseStream);
                }
            }
            return FileData;

        }

    }
}