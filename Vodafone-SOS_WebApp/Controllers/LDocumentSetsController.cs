using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LDocumentSetsController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILPayeesRestClient LPRC = new LPayeesRestClient();
        ILDocumentSetsRestClient RestClient = new LDocumentSetsRestClient();

        private SelectList GetCommissionPeriod()
        {
            ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();
            var CommissionPeriod = LCPRC.GetByCompanyId(CompanyId);
            var x = new SelectList(CommissionPeriod, "LcpPeriodName", "LcpPeriodName");
            return x;
        }

        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create DocumentSets";
            ViewBag.SelectionMode = "checkbox";//used int the partial view to decide whether payee grid will have single selection or multiple
            var model = new LDocumentSetsViewModel();
            ViewBag.LdsCommissionPeriod = GetCommissionPeriod();
            return View(model);
        }


        //This method is  called when we save form by clicking upload button to upload file in  LDocumentSetss table
        [HttpPost]
        [ControllerActionFilter]
        // [ValidateAntiForgeryToken]//Commented by SS as there are issues bcoz of this during file upload.
        public JsonResult UploadDocumentSets(LDocumentSetsViewModel LDSVM, HttpPostedFileBase[] File1, string PortfolioList, HttpPostedFileBase[] FileUpload,string PayeeList)
        {
            try
            {
                if (File1[0] != null)
                {
                    var UserName = System.Web.HttpContext.Current.Session["UserName"];
                    var Role = System.Web.HttpContext.Current.Session["UserRole"];
                    var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
                    string AttachedFileNames = "";
                    var SupportingFileNames = "";
                    string SupportingDocumentFilePath = "";
                    var filePath = "";

                    foreach (HttpPostedFileBase files in File1)
                    {
                        var fileLocation = "";
                        string fileExtension = System.IO.Path.GetExtension(files.FileName);
                        string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                        string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        if (string.IsNullOrEmpty(AttachedFileNames))
                        {
                            AttachedFileNames = ManipulatedFileName;
                        }
                        else
                        {
                            AttachedFileNames = AttachedFileNames + "," + ManipulatedFileName;
                        }
                        //SSConverting this mechinism of uploading file to SDK Methodology
                        /* filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/DocumentSets/" + LDSVM.LdsCommissionPeriod + "/Upload");

                         fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                         //check if directory exists or not.iIf notcreate that directory
                         bool exists = System.IO.Directory.Exists(filePath);
                         if (!exists)
                         {
                             System.IO.Directory.CreateDirectory(filePath);
                         }
                         */
                        filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/DocumentSets/" + LDSVM.LdsCommissionPeriod + "/Upload");
                        if(!Globals.FolderExistsInS3(filePath))
                        {
                            Globals.CreateFolderInS3Root(filePath);
                        }
                        Globals.UploadToS3(files.InputStream,ManipulatedFileName,filePath);
                        //While saving bucket path we append it with /
                        filePath += "/";
                        //files.SaveAs(fileLocation);
                    }
                    LDSVM.LdsDescription = (AttachedFileNames.Length>=1000)? AttachedFileNames.Substring(0, 990):AttachedFileNames;

                    if (FileUpload != null)
                    {
                        foreach (HttpPostedFileBase files in FileUpload)
                        {

                            var fileLocation = "";
                            string fileExtension = System.IO.Path.GetExtension(files.FileName);
                            string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                            string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                            if (string.IsNullOrEmpty(SupportingFileNames))
                            {
                                SupportingFileNames = ManipulatedFileName;
                            }
                            else
                            {
                                SupportingFileNames = SupportingFileNames + "," + ManipulatedFileName;
                            }

                            //fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/DocumentSets/" +LDSVM.LdsCommissionPeriod + " / SupportingDocuments/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension);//As disscussed with VG file name will have datetime stamp as suffix
                            //SupportingDocumentFilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/DocumentSets/" + LDSVM.LdsCommissionPeriod + " / SupportingDocuments");                                                                                                                //check if directory exists or not. iIf notcreate that directory
                            //bool exists = System.IO.Directory.Exists(SupportingDocumentFilePath);
                            //if (!exists)
                            //{
                            //    System.IO.Directory.CreateDirectory(SupportingDocumentFilePath);
                            //}

                            //files.SaveAs(fileLocation);
                            //SS Changed the method to upload Supporting Doc in Ref Files to bucket
                           SupportingDocumentFilePath = ConfigurationManager.AppSettings["SOSBucketRootFolder"] + CompanyCode + "/DocumentSets/" + LDSVM.LdsCommissionPeriod + "/SupportingDocuments";
                            if (!Globals.FolderExistsInS3(SupportingDocumentFilePath))
                            {
                                Globals.CreateFolderInS3Root(SupportingDocumentFilePath);
                            }
                            Globals.UploadToS3(files.InputStream, ManipulatedFileName, SupportingDocumentFilePath);
                            //While saving bucket path we append it with /
                            SupportingDocumentFilePath =  SupportingDocumentFilePath + "/";
                        }
                    }

                    if (!string.IsNullOrEmpty(LDSVM.WFComments))
                    {
                        LDSVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LDSVM.WFComments;
                    }
                    LDSVM.LdsDocumentList = (AttachedFileNames.Length >= 1000) ? AttachedFileNames.Substring(0, 990) : AttachedFileNames;//AttachedFileNames;
                    LDSVM.LdsUpdatedDateTime = DateTime.UtcNow;
                    LDSVM.LdsCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    LDSVM.LdsCreatedDateTime = DateTime.UtcNow;
                    LDSVM.WFCompanyId = CompanyId;
                    LDSVM.WFCurrentOwnerId = LoggedInUserId;
                    LDSVM.WFRequesterId = LoggedInUserId;
                    LDSVM.LdsUpdatedById = LoggedInUserId;
                    LDSVM.LdsUpdatedDateTime = DateTime.UtcNow;
                    LDSVM.WFRequesterRoleId = LoggedInRoleId;
                    LDSVM.WFCompanyId = CompanyId;
                    LDSVM.PayeeListCarrier = PayeeList;
                    string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                    var s = RestClient.Add(LDSVM, LoggedInRoleId, WorkflowName, AttachedFileNames, filePath, PortfolioList, SupportingFileNames,SupportingDocumentFilePath,PayeeList,null);
                    //Auto approve workflow after addition
                    //var DocumentSetArray = s.Split(',').ToList();
                    //foreach (var DS in DocumentSetArray)
                    //{
                    //    try//Auto Approve the DocumentSets and display error in case any validation fails //
                    //    {

                    //        var DocumentSetId = Convert.ToInt32(DS);
                    //        GGRC.UpdateActionStatus(WorkflowName, Convert.ToString(DocumentSetId), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId, string.Empty);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        TempData["Error"] += ex.Data["ErrorMessage"].ToString();
                    //        continue;
                    //    }
                    //}
                }

                var OutputJson = new { ErrorMessage = "Payee Documents added sucessfully", PopupMessage = "", RedirectToUrl = "/GenericGrid/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be uploaded";
                    throw ex;
                }
            }
        }

        [ControllerActionFilter]
        public ActionResult Review(int TransactionId, int WFConfigId)
        {
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId, LoggedInUserId, TransactionId);
            var DocumentSetsDetails = RestClient.GetById(TransactionId);
            ViewBag.LdsCommissionPeriod = GetCommissionPeriod();
            return View(DocumentSetsDetails);
        }

        //Get List of Payees for dropdown
        //[ControllerActionFilter]
        //public JsonResult GetPayeeList()
        //{
        //    var ApiData = Globals.GetPayeeList(string.Empty,false);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
    }
}