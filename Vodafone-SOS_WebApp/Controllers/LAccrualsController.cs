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
    [SessionExpire]
    [HandleCustomError]
    public class LAccrualsController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILAccrualsRestClient RestClient = new LAccrualsRestClient();

        [ControllerActionFilter]
        public ActionResult Review(int TransactionId, int WFConfigId)
        {
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId, LoggedInUserId, TransactionId);
            var AccrualsDetails = RestClient.GetById(TransactionId);
            ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();
            System.Web.HttpContext.Current.Session["Title"] = "Review Accrual";
            ViewBag.LaCommissionPeriod = new SelectList(LCPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpPeriodName");
            return View(AccrualsDetails);
        }

        // GET: LAccruals
        [ControllerActionFilter]
        public ActionResult Upload()
        {
            ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();
            System.Web.HttpContext.Current.Session["Title"] = "Upload Accrual";
            var model = new LAccrualViewModel();
            ViewBag.LaCommissionPeriod = new SelectList(LCPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpPeriodName");
            return View(model);
        }

        //This method is  called when we save form by clicking upload button to upload file in  LAccruals table
        [HttpPost]
        [ControllerActionFilter]
        //[ValidateAntiForgeryToken] - Commented by SS because this is creating issues when uploading files using AJAX
        public JsonResult UploadAccrual(LAccrualViewModel LAVM, HttpPostedFileBase[] File1, string PortfolioList, HttpPostedFileBase[] FileUpload)
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

                        /*OLD: Accruals : S:\<OpCo>\RefFiles\Accruals
                         NEW: Accruals : S:\<OpCo>\Accruals\<period>\<Name>        (for actual Accruaal file uploaded)*/
                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Accruals/"+LAVM.LaCommissionPeriod+"/"+LAVM.LaName);

                        fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                        //check if directory exists or not. iIf notcreate that directory
                        bool exists = System.IO.Directory.Exists(filePath);
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(filePath);
                        }

                        files.SaveAs(fileLocation);
                    }

                    LAVM.LaDescription = AttachedFileNames;

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
                            /*OLD: Accruals : S:\<OpCo>\RefFiles\Accruals\SupportingDocuments
                             NEW: Accruals : S:\<OpCo>\Accruals\<peiord>\<Name>\SupportingDocuments       (for attached supporting docs)*/
                            fileLocation = filePath + "/SupportingDocuments/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                            bool exists = System.IO.Directory.Exists(filePath + "/SupportingDocuments");
                            if (!exists)
                            {
                                System.IO.Directory.CreateDirectory(filePath + "/SupportingDocuments");
                            }
                            files.SaveAs(fileLocation);
                        }
                    }

                    if (!string.IsNullOrEmpty(LAVM.WFComments))//currently comments are hidden in form will remove this if not required
                    {
                        LAVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LAVM.WFComments;
                    }

                    LAVM.LaUpdatedDateTime = DateTime.UtcNow;
                    LAVM.LaCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    LAVM.LaCreatedDateTime = DateTime.UtcNow;
                    LAVM.WFCurrentOwnerId = LoggedInUserId;
                    LAVM.WFRequesterId = LoggedInUserId;
                    LAVM.LaUpdatedById = LoggedInUserId;
                    LAVM.LaUpdatedDateTime = DateTime.UtcNow;
                    LAVM.WFRequesterRoleId = LoggedInRoleId;
                    LAVM.WFCompanyId = CompanyId;
                    string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                    var s = RestClient.Add(LAVM, LoggedInRoleId, WorkflowName, AttachedFileNames, filePath, PortfolioList, SupportingFileNames,null);
                    //Auto approve workflow after addition
                    try//Auto Approve the Accrual and display error in case any validation fails
                    {
                        GGRC.UpdateActionStatus(WorkflowName,Convert.ToString(s), CompanyId, "Approve", LoggedInUserId, string.Empty,LoggedInRoleId, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    }
                }

                var OutputJson = new { ErrorMessage = "Accrual added sucessfully", PopupMessage = "", RedirectToUrl = "/GenericGrid/Index" };
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
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage="", RedirectToUrl = Globals.ErrorPageUrl};
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = ""};
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            //display a popup error message to user and keep it on same page
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
    }
}