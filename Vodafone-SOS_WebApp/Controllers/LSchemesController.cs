//Code Review for this file (from security perspective) done

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
    public class LSchemesController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILPayeesRestClient LPRC = new LPayeesRestClient();
        ILSchemesRestClient RestClient = new LSchemesRestClient();
        
        private SelectList GetYear()
        {
            var MyList = Enumerable.Range((DateTime.UtcNow.AddYears(-1).Year), 2100 - DateTime.UtcNow.AddYears(1).Year);
            var x = new SelectList(MyList, MyList);
            return x;
        }
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Scheme";
          
            ViewBag.LrfYear = GetYear();
            var model = new LSchemeViewModel();
            //model.LsMonth = DateTime.UtcNow.Month.ToString("D2");
            //model.LsYear = DateTime.UtcNow.Year;
            return View(model);
        }
        
        
        //This method is  called when we save form by clicking upload button to upload file in  LSchemes table
        [HttpPost]
        [ControllerActionFilter]
       // [ValidateAntiForgeryToken]//Commented by SS as there are issues bcoz of this during file upload.
        public JsonResult UploadScheme(LSchemeViewModel LSVM, HttpPostedFileBase[] File1, string PortfolioList, HttpPostedFileBase[] FileUpload)
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
                    var filePath = "";
                    
                    foreach (HttpPostedFileBase files in File1)
                    {
                        var fileLocation = "";
                        string fileExtension = System.IO.Path.GetExtension(files.FileName);
                        string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                        if (string.IsNullOrEmpty(AttachedFileNames))
                        {
                            AttachedFileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        }
                        else
                        {
                            AttachedFileNames = AttachedFileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        }

                        /*OLD: Scheme :   S:\<opco>\RefFiles\Schemes      (for actual Scheme uploaded)
                         NEW: Scheme :   S:\<opco>\Schemes\<Name>        (for actual Scheme uploaded)*/
                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Schemes/"+LSVM.LsName);
                       
                        fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                        //check if directory exists or not. iIf notcreate that directory
                        bool exists = System.IO.Directory.Exists(filePath);
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(filePath);
                        }

                        files.SaveAs(fileLocation);
                    }
                    LSVM.LsDescription = AttachedFileNames;
                    if (FileUpload != null)
                    {
                        foreach (HttpPostedFileBase files in FileUpload)
                        {

                            var fileLocation = "";
                            string fileExtension = System.IO.Path.GetExtension(files.FileName);
                            string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                            if (string.IsNullOrEmpty(SupportingFileNames))
                            {
                                SupportingFileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                            }
                            else
                            {
                                SupportingFileNames = SupportingFileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                            }


                            /*OLD: Scheme :   S:\<opco>\RefFiles\Schemes\SupportingDocuments    (for attached supporting docs)
                             NEW: Scheme :   S:\<opco>\Schemes\<Name>\SupportingDocuments       (for attached supporting docs)*/
                            fileLocation = filePath + "/SupportingDocuments/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                                                                                                                                                      //check if directory exists or not. iIf notcreate that directory
                            bool exists = System.IO.Directory.Exists(filePath + "/SupportingDocuments");
                            if (!exists)
                            {
                                System.IO.Directory.CreateDirectory(filePath + "/SupportingDocuments");
                            }

                            files.SaveAs(fileLocation);
                        }
                    }

                    if (!string.IsNullOrEmpty(LSVM.WFComments))//currently comments are hidden in form will remove this if not required
                    {
                        LSVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LSVM.WFComments;
                    }

                    LSVM.LsUpdatedDateTime = DateTime.UtcNow;
                    LSVM.LsCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    LSVM.LsCreatedDateTime = DateTime.UtcNow;
                    LSVM.LsCompanyId = CompanyId;
                    LSVM.WFCompanyId = CompanyId;//added by SG as required for Generic Workflow
                    LSVM.WFCurrentOwnerId = LoggedInUserId;
                    LSVM.WFRequesterId = LoggedInUserId;
                    LSVM.LsUpdatedById = LoggedInUserId;
                    LSVM.LsUpdatedDateTime = DateTime.UtcNow;
                    LSVM.WFRequesterRoleId = LoggedInRoleId;
                    LSVM.WFCompanyId = CompanyId;
                    LSVM.LsIsSchemeTested = false;
                    string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                    var s = RestClient.Add(LSVM, LoggedInRoleId, WorkflowName, AttachedFileNames, filePath, PortfolioList, SupportingFileNames,null);
                    //Auto approve workflow after addition
                    try//Auto Approve the Scheme and display erro in case any validation fails
                    {
                        GGRC.UpdateActionStatus(WorkflowName, Convert.ToString(s), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId,string.Empty);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    }
                }
                var OutputJson = new { ErrorMessage = "Sheme added sucessfully", PopupMessage = "", RedirectToUrl = "/GenericGrid/Index" };
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
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId,LoggedInUserId, TransactionId);
            var SchemeDetails = RestClient.GetById(TransactionId);
           
            return View(SchemeDetails);
        }


    }
}