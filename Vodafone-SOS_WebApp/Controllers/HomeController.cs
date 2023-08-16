using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;
using System.Data.OleDb;
using Ionic.Zip;
using System.Configuration;
using Vodafone_SOS_WebApp.Utilities;
using System.Net;
using System.IO;

namespace Vodafone_SOS_WebApp.Controllers
{
    //[SessionExpire] //Commented this line because it is not letting index page to be loaded. Need to restore this line after investigation of issue and finding a resolution to this.
    [HandleCustomError]
    public class HomeController : Controller
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        ILAuditRestClient laudit = new LAuditRestClient();
        IRWorkFlowsRestClient rworkflows = new RWorkFlowsRestClient();

        public ActionResult ErrorPage()
        {
            return View("Error");
        }
        //Generic Method to download Supporting Doc
        public ActionResult DownloadSupportingDocument(int EntityId, string EntityType)
        {
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var SupportingDocument = LSDRC.GetByEntityType(EntityType, EntityId);
            string FileName = "";
            String FileNameWithPath = "";
            using (ZipFile zip = new ZipFile())
            {
                foreach (var SD in SupportingDocument)
                {
                    if (SupportingDocument.Count() > 1)
                    {
                        var yy = SD.LsdFilePath + "/" + SD.LsdFileName;
                        zip.AddFile(yy, "");
                    }
                    else
                    {
                        FileName = SD.LsdFileName;
                        FileNameWithPath = SD.LsdFilePath + "/" + SD.LsdFileName;
                    }
                }
                //Download file if not attachments are present and make a zip if multiple files are present
                if (SupportingDocument.Count()>0)
                {
                    if(zip.Count()>0)
                    {
                        if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/SupportTicket.zip"))
                        {
                            System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/SupportTicket.zip");
                        }
                        var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                        DirectoryInfo dir = new DirectoryInfo(FilePath);
                        dir.Refresh();
                        if (!dir.Exists) dir.Create();
                        zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/SupportTicket.zip");
                        return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/SupportTicket.zip", "application/zip", "ExportSupportTicket.zip");
                    }
                    else if (!System.IO.File.Exists(FileNameWithPath))//if no file found
                    {
                        TempData["Error"] ="No Data or Supporting Files are available" ;
                        return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
                    }
                    else
                    {
                        var FileType =Globals.GetFileContentType(FileName.Split('.').LastOrDefault());

                        return File(FileNameWithPath, FileType, FileName);
                    }
                }
                //else
                //{
                //    if (System.IO.File.Exists(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "SupportTicket.zip"))
                //    {
                //        System.IO.File.Delete(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "SupportTicket.zip");
                //    }
                //    zip.Save(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "SupportTicket.zip");
                //    return File(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "SupportTicket.zip", "application/zip", "ExportSupportTicket.zip");
                //}
            }
            TempData["Error"] = "No Data or Supporting Files are available";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //[ControllerActionFilter]
        public ActionResult CompanyDashboard()
        {
            IGKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            var Policy = KVRC.GetByName("FirstTimeLoginPolicy", CompanyId);
            if (Policy != null)
                ViewBag.PolicyText =WebUtility.HtmlDecode(Policy.GkvValue);
            return View();
        }

        public JsonResult SaveMFARoles(string RoleList)
        {
            IAspnetRolesRestClient ASRRC = new AspnetRolesRestClient();
            ASRRC.UpdateMFAForRoles(RoleList,CompanyCode);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Method to get MFA Enabled Roles
        public JsonResult GetMFARoles()
        {
            IAspnetRolesRestClient ASRC = new AspnetRolesRestClient();
            var ApiData = ASRC.GetMFARoles(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Below index method is a annonymous method and is the Login Page of the application. Please do not remove comment from [ControllerActionFilter]
        //[ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }

       // [ControllerActionFilter]
        public ActionResult AccountAnalystDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "AccountAnalyst Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult ControllerDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Controller Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult MonitorDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Monitor Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult ChannelManagerDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "ChannelManager Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult FunctionalityComingSoon()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Under Construction";
            return View("FunctionalityComingSoon");
        }
        
        [ControllerActionFilter]
        public ActionResult NoRolesAssigned()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Role Not Assigned";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult AnalystDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Reporting Analyst Welcome Page";
            
            return View();
        }

        [ControllerActionFilter]
        public ActionResult HOFODashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "HOFO Welcome Page";
            return View();
        }

        //[ControllerActionFilter]
        public ActionResult HRDashboard()
        {
            return View();
        }

        [ControllerActionFilter]
        public ActionResult L2AdminDashboard()
        {
            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            ILEmailBucketRestClient LEBRC=new LEmailBucketRestClient();// add to call RestClient Method
            ILSupportTicketsRestClient STRC = new LSupportTicketsRestClient();
            LEmailBucketViewModel model = new LEmailBucketViewModel();
           
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var Company = System.Web.HttpContext.Current.Session["CompanyId"];
            ViewBag.CompanyId = new SelectList(GCRC.GetAll(), "Id", "GcCompanyName", Company);
            System.Web.HttpContext.Current.Session["Title"] = "L2 Admin Welcome Page";
            //Changes Done By RG To add Min Date and Count from LEmailBucket table
            var Count = LEBRC.GetEmailBucketcounts();

            //method call to get the ticket count for SOS
            var SOSTicket = STRC.GetL2SOSTicketcounts();

            //method call to get teh ticket count for ALTERYX

            var ALTERYX = STRC.GetL2ALTTicketcounts();
            ViewBag.SOSTicket = SOSTicket; //adding to the viewbag to receive on L2Admin page
            ViewBag.ALTERYX = ALTERYX; //adding to the viewbag to receive on L2Admin page
            ViewBag.Count = Count;
            var MinDate = LEBRC.GetEmailBucketMinDate();
            if (MinDate != null)
            {
                //Change MinDate from JSON to 29/JUL/2017 12:37:22 PM format
                ViewBag.MinDate = MinDate.Value.ToString("dd-MMM hh:mm tt");
            }
            else
            {
                ViewBag.MinDate = "NA";
            }
            var MaxDate = LEBRC.GetEmailBucketMaxDate();
            ViewBag.MaxDate = MaxDate.ToString("dd-MMM hh:mm tt");
            var MaxStatus = LEBRC.GetEmailBucketMaxStatus();
            ViewBag.MaxStatus = MaxStatus;

            //method to get the list of company code and add in view Bag to get on the L2Admin page anduse to fill the dropdown list
            ViewBag.GcCode = GetAllCompanyCode(null);

            //calling method to get the list of dynamic columns for the new item grid on L2AdminPage
            var NewItemscolumnlist = GetNewItemscolumnlist(null, null, null, null,null);
          
            ViewBag.NewItemscolumnlist = NewItemscolumnlist.Data;

            var CompletedItemscolumnlist = GetCompletedItemscolumnlist(null, null, null, null);
            ViewBag.CompletedItemscolumnlist = CompletedItemscolumnlist.Data;

            // ViewBag.System = GetCompanyName(null); //dropdown for company name
            return View();
        }


        public JsonResult GetCompletedItemscolumnlist(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            //var qry = Request.QueryString;
            var FilterQuery = "";
            var ApiData = rworkflows.GetCompletedListcolumnlist(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// created by RS
        /// method to get the list of dynamic columns for the new item tab on L2Admin Page
        /// </summary>
        public JsonResult GetNewItemscolumnlist(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum,Nullable<int> Intervalid)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            if (Intervalid == null) Intervalid = 1;            
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = laudit.GetNewItemscolumnlist(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery,Convert.ToInt32(Intervalid));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// created by RS
        /// //method to get the list of company code
        /// </summary>
        private SelectList GetAllCompanyCode(int? Selected)
        {
            IGCompaniesRestClient gc = new GCompaniesRestClient();
            var ApiData = gc.GetAll();
            var x = new SelectList(ApiData, "Id", "GcCode", Selected);
            return x;
        }



        [ControllerActionFilter]
        public ActionResult ManagerDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manager Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult AuditorDashboard(AuditFormViewModel AFVM)
        {
            IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
            System.Web.HttpContext.Current.Session["Title"] = "Auditor Welcome Page";
            var Workflows = RWFRC.Get();
            ViewBag.Entity = new SelectList(Workflows, "RwfName", "RwfName");
            //if no entity is selected then add current date in model for end date
            if (string.IsNullOrEmpty(AFVM.Entity))
            {
                AFVM.StartDate = DateTime.UtcNow.AddMonths(-3);
                AFVM.EndDate = DateTime.UtcNow;
            }
                return View(AFVM);
           
        }

        [ControllerActionFilter]
        public ActionResult GroupAdminDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Group Admin Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult SOSAdminDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "SOS Admin Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult SalesOperationsDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Sales Op Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult SystemAnalystDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "System Analyst Welcome Page";
            
            return View();
        }

        [ControllerActionFilter]
        public ActionResult ClaimsAnalystDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Claims Analyst Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult PayeeDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Payee Welcome Page";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        //method used to save the return url in session variable to return to that page on clicking back button throught Application
        [ControllerActionFilter]
        public JsonResult SaveReturnPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                System.Web.HttpContext.Current.Session["from"] = path;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        //This method is used  to display menu items  in Layout file by getting data from MGMenusAspnetRoles
        [ControllerActionFilter]
        public JsonResult GetMenuItems()
        {
            IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
            var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            //If role is defined in session for current user then dsplay its menu items
            if(!string.IsNullOrEmpty(Role))
            {
                //As per current logic disscussed with VG . Only those menu which have url will be clickable .
                var MenuItems = MGARC.GetByUserRole(Role,CompanyCode).Select(p => new { id = p.MgmarMenuId, text = (string.IsNullOrEmpty(p.GmMenuUrl)) ?"<a href='#'>"+ p.GmMenuName+"</a>" : "<a href='" + p.GmMenuUrl + "'>" + p.GmMenuName + "</a>", parentid = p.GmParentId, p.GmOrdinalPosition }).OrderBy(p => p.GmOrdinalPosition).ToList();
                return Json(MenuItems, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //This method will Change companyName in session
        [ControllerActionFilter]
        public ActionResult ChangeCompanyname(int CompanyId)
        {
            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            var Companies = GCRC.GetById(CompanyId);            
            //Update session role
            if (!string.IsNullOrEmpty(Companies.GcCompanyName))
                System.Web.HttpContext.Current.Session["CompanyName"] = Companies.GcCompanyName;
            System.Web.HttpContext.Current.Session["CompanyId"] = CompanyId;
            System.Web.HttpContext.Current.Session["CompanyCode"] = Companies.GcCode;
            //Changes By SG- for changing UserRoleId in session after updating Company.
            var UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
            IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
            var UserRoleId = ARRC.GetByUserRoleCompanyCode(Companies.GcCode, UserRole);
            System.Web.HttpContext.Current.Session["UserRoleId"] = UserRoleId;
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //This method  will load data in dropdown of user role
        [ControllerActionFilter]
        public JsonResult GetRolesList()
        {
            var Roles = System.Web.HttpContext.Current.Session["Roles"] as List<AspnetRoleViewModel>;
            if(Roles.Count()>0)
            return Json(Roles.Select(p=>p.Name),JsonRequestBehavior.AllowGet);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        ////this method loads data in portfolio grid that mathches current user portfolio -- Added this Global method here as could not add it in Globals Shubham
        [ControllerActionFilter]
        public JsonResult GetPortfolioGrid()
        {
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var ApiData = LPORC.GetByUserId(LoggedInUserId,RoleId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //[ControllerActionFilter]
        public JsonResult GetPortfolioByRefTypeIDGrid(string RefFileid)
        {
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var ApiData = LPORC.GetByUserIdAndRefTypeID(LoggedInUserId, RoleId, Convert.ToInt32(RefFileid));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // [ControllerActionFilter] As same method is used in multiple screens
        public JsonResult GetPortfolioForDropDown()
        {
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var ApiData = LPORC.GetByUserId(LoggedInUserId,RoleId).Select(p=>new {p.Id,Portfolio=p.RcName+" ("+p.RcPrimaryChannel+")" }).GroupBy(p=>p.Portfolio).Select(p=>p.FirstOrDefault());
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetPortfolioGridForEdit(int TransactionId,string EntityType,string Role)
        {
            if(Role==null)
            {
                Role = string.Empty;
            }
            
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            var CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var ApiData = LPORC.GetByLoggedInUserIdForEdit(LoggedInUserId,TransactionId, EntityType,RoleId,Role,CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This methos will be called when user clicks on go button after changing role
        [ControllerActionFilter]
        public ActionResult ChangeUserRole(string UserRole)
        {
            //Update session role
            Boolean IsDashboard = false;
            if (TempData["AuditorCompanyCode"] != null)
            {
                System.Web.HttpContext.Current.Session["CompanyCode"]= TempData["AuditorCompanyCode"];
                System.Web.HttpContext.Current.Session["CompanyId"] = TempData["AuditorCompanyId"];
                TempData["AuditorCompanyCode"] = null;
                TempData["AuditorCompanyId"] = null;
                //TempData.Remove("AuditorCompanyCode");
                //TempData.Remove("AuditorCompanyId");
            }
            if (!string.IsNullOrEmpty(UserRole))
            {
                System.Web.HttpContext.Current.Session["UserRole"] = UserRole;
                var RolesList = System.Web.HttpContext.Current.Session["Roles"] as List<AspnetRoleViewModel>;
                AspnetRoleViewModel user = new AspnetRoleViewModel();
                //Shivani - 12 Nov 2019 - to avoid Null exceptions occuring in PROD
                if (RolesList != null && RolesList.Count>0)
                {
                    user = RolesList.Where(p => p.Name == UserRole).FirstOrDefault();
                    if(user != null)
                    {
                        System.Web.HttpContext.Current.Session["UserRoleId"] = user.Id;
                        System.Web.HttpContext.Current.Session["ShowOnDashBoard"] = user.ShowDashboard;
                        IsDashboard = user.ShowDashboard;
                    }
                }
            }
            if (!IsDashboard)
            {
                switch (UserRole)
                {
                    case "Reporting Analyst":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Reporting Analyst Welcome Page", Role = " Reporting Analyst" });
                        //return RedirectToAction("AnalystDashBoard", "Home");
                    case "Claims Analyst":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Claims Analyst Welcome Page", Role = " Claims Analyst" });
                        //return RedirectToAction("ClaimsAnalystDashboard", "Home");
                    case "Payee":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Payee Welcome Page", Role = " Payee" });
                        //return RedirectToAction("PayeeDashboard", "Home");
                    case "Manager":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Manager Welcome Page", Role = " Manager" });
                        //return RedirectToAction("ManagerDashboard", "Home");
                    case "Sales Operations":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Sales Op Welcome Page", Role = " Sales Operations" });
                        //return RedirectToAction("SalesOperationsDashboard", "Home");
                    case "L3 Admin":
                        //return RedirectToAction("GenericDashboard", "Home", new { Title = "SOS Admin Welcome Page", Role = " L3Admin" });
                        return RedirectToAction("SOSAdminDashboard", "Home");
                    case "L2 Admin":
                        //return RedirectToAction("GenericDashboard", "Home", new { Title = "SOS Admin Welcome Page", Role = " L2Admin" });
                        return RedirectToAction("L2AdminDashboard", "Home");
                    case "L2 Alteryx":
                        // return RedirectToAction("GenericDashboard", "Home", new { Title = "Alteryx Admin Welcome Page", Role = " Alteryx Admin" });
                        return RedirectToAction("L2AdminDashboard", "Home");
                    case "L2 L2 Infra":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Infra Admin Welcome Page", Role = " Infra Admin" });
                    case "System Analyst":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "System Analyst Welcome Page", Role = " System Analyst" });
                        //return RedirectToAction("SystemAnalystDashboard", "Home");
                    case "Auditor":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Auditor Welcome Page", Role = " Auditor" });
                        //return RedirectToAction("AuditorDashboard", "Home");
                    case "Head of Finance Operations":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "HOFO Welcome Page", Role = " Head of Finance Operations" });
                        //return RedirectToAction("HOFODashboard", "Home");
                    case "Account Analyst":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Account Analyst Welcome Page", Role = " Account Analyst" });
                        //return RedirectToAction("AccountAnalystDashboard", "Home");
                    case "Controller":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Controller Welcome Page", Role = " Controller" });
                        //return RedirectToAction("ControllerDashboard", "Home");
                    case "Channel Manager":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "Channel Manager Welcome Page", Role = " Channel Manager" });
                        //return RedirectToAction("ChannelManagerDashboard", "Home");
                    case "HR":
                        return RedirectToAction("GenericDashboard", "Home", new { Title = "HR Welcome Page", Role = " HR" });
                        //ssreturn RedirectToAction("HRDashboard", "Home");
                }
            }
            else
            {
                return RedirectToAction("GraphicalDashboard", "Home");
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        public ActionResult GraphicalDashboard()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Graphical Dashboard";
            return View();
        }


     
        public PartialViewResult CreatePartialView(int GraphId, string GraphType, string strPeriods, string BatchStatus)
        {

            ViewBag.GraphId = GraphId;
            ViewBag.GraphType = GraphType;
            ViewBag.strPeriods = strPeriods;
            ViewBag.BatchStatus = BatchStatus;
            return PartialView("_GraphicalDashboardPartial");
        }

        // [ControllerActionFilter]
        public ActionResult GenericDashboard(string Title, string Role)
        {
            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            var Company = System.Web.HttpContext.Current.Session["CompanyId"] == null ? "" : System.Web.HttpContext.Current.Session["CompanyId"].ToString();

           // var Company = System.Web.HttpContext.Current.Session["CompanyId"];
            if (Company != null && Company != "")
            {
                ViewBag.CompanyId = new SelectList(GCRC.GetCompanyforAuditorAll(), "Id", "GcCompanyName", Company);
            }
            System.Web.HttpContext.Current.Session["Title"] = Title;
            ViewBag.RoleName = Role;
            return View();
        }

        public ActionResult ChangeUserRoleIDOntheBasisCompanyCode(int CompanyID)
        {
            if (TempData["AuditorCompanyCode"] == null)
            {
                TempData["AuditorCompanyCode"] = System.Web.HttpContext.Current.Session["CompanyCode"];
                TempData["AuditorCompanyId"] = System.Web.HttpContext.Current.Session["CompanyId"];
            }

            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            GCompanyViewModel data = GCRC.GetGCompaniesForAuditor(CompanyID);
            if (data != null)
            {
                System.Web.HttpContext.Current.Session["UserRoleId"] = data.Id;
                System.Web.HttpContext.Current.Session["CompanyId"] = CompanyID;
                System.Web.HttpContext.Current.Session["CompanyCode"] = data.GcCode;
                System.Web.HttpContext.Current.Session["CompanyName"] = data.GcCompanyName;


            }
            return RedirectToAction("GenericDashboard", "Home");
        }
    }
}