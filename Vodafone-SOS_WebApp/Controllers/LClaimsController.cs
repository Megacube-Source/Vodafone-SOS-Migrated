using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using Vodafone_SOS_WebApp.Utilities;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;
using System.Text.RegularExpressions;
using System.Threading;
using static Vodafone_SOS_WebApp.Utilities.Globals;
using System.Threading.Tasks;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LClaimsController : Controller// PrimaryController//RS with reference to :-RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        ILClaimsRestClient RestClient = new LClaimsRestClient();
        ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
        ILAttachmentsRestClient LARC = new LAttachmentsRestClient();
        IRRejectionReasonsRestClient RRRRC = new RRejectionReasonsRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ISp_UpdateClaimsStatusRestClient UCSRC = new Sp_UpdateClaimsStatusRestClient();
        ILCompanySpecificColumnsRestClient LCSC = new LCompanySpecificColumnsRestClient();
        ILPayeesRestClient LPRC = new LPayeesRestClient();
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
        string FirstName = Convert.ToString(System.Web.HttpContext.Current.Session["FirstName"]);
        string LastName = System.Web.HttpContext.Current.Session["LastName"] as string;
        string CurrentUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        

        /*DEAD CODE STARTS HERE//
        //This method redirects user to its index (which displays the list of Claims) as per its role  
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Claims";
            //This variable gets the logged in user role from session
            var role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            //If the user role is payee then check whether payee is authorized to enter claim or not 
            //This variable is defined to to pass information to view whether user is authorized to enter claim or not
            bool CanRaiseClaim = false;
            switch(role)
            {
                case "Payee":
                ILPayeesRestClient LPRC = new LPayeesRestClient();
                CanRaiseClaim = LPRC.CanRaiseClaims(CurrentUserId);
                ViewBag.CanRaiseClaim = CanRaiseClaim;
                break;
                case "Sales Operations":
                    //Sales Operations role can always raise the claim
                CanRaiseClaim = true;
                ViewBag.CanRaiseClaim = CanRaiseClaim;
                break;
                case "Claims Analyst":
                return RedirectToAction("AnalystIndex");
                case "Manager":
                return RedirectToAction("ManagerIndex");
            }

            return View();
        }

        //This method displays the index view of claims if logged in user role is manager
        public ActionResult ManagerIndex()
        {
            IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
            var Users = ARRC.GetClaimsAnalystByCompanyId(CompanyId);
            //These viewbag data is defined as per tab name in which it is displayed
            ViewBag.AllocateToAllocated = new SelectList(Users, "ID", "UserName");
            ViewBag.AllocateToAllocate = new SelectList(Users, "ID", "UserName");
            return View();
        }

        //This method displays the index view of claims if logged in user role is ClaimsAnalyst
        public ActionResult AnalystIndex()
        {
            IRRejectionReasonsRestClient RRRC = new RRejectionReasonsRestClient();
            //These viewbag data is defined as per tab name in which it is displayed
            ViewBag.RejectionReasonIdReInvestigate = new SelectList(RRRC.GetByCompanyId(CompanyId), "Id", "RrrReason");
            ViewBag.RejectionReasonIdPendingInvestigation = new SelectList(RRRC.GetByCompanyId(CompanyId), "Id", "RrrReason");
            return View();
        }

        //This method id defined to load data in  grid for Claims Analyst by passing Tab Name as parameter
        public JsonResult GetClaimsForAnalyst(string TabName)
        {
            IEnumerable<LClaimViewModel> ApiData=new List<LClaimViewModel>();
            switch (TabName)
            {
                case "PendingInvestigation":
                    ApiData = RestClient.GetByStatusNameAllocatedToId("Allocated",CompanyId,CurrentUserId);
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "ReInvestigate" :
                    ApiData = RestClient.GetByStatusNameAllocatedToId("ReInvestigate",CompanyId,CurrentUserId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Rejected":
                    ApiData = RestClient.GetByStatusName("Rejected",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "PendingApproval":
                    ApiData = RestClient.GetByStatusName("PendingApproval",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Approved":
                    ApiData = RestClient.GetByStatusName("Approved",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Upload":
                    ApiData = RestClient.GetByStatusName("PaymentFilePublished",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //This method loads the data in grid in the index page of manager
        public JsonResult GetClaimsForManager(string TabName)
        {
            IEnumerable<LClaimViewModel> ApiData = new List<LClaimViewModel>();
            switch (TabName)
            {
                case "Allocate":
                    ApiData = RestClient.GetByStatusName("Accepted",CompanyId);
                    return Json(ApiData, JsonRequestBehavior.AllowGet);

                case "Approve":
                    ApiData = RestClient.GetByStatusName("PendingApproval",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Allocated":
                    ApiData = RestClient.GetByStatusName("Allocated",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Approved":
                    ApiData = RestClient.GetByStatusName("Approved",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Paid":
                    ApiData = RestClient.GetByStatusName("PaymentFilePublished",CompanyId).ToList();
                    return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //This method loads data in the grid of claims for salesop/payee
        public JsonResult GetClaimsForPayeeOrSalesOp(string TabName)
        {
         IEnumerable<LClaimViewModel> ApiData=new List<LClaimViewModel>();
            switch(TabName)
            {
                case "Open":
                    if (UserRole == "Sales Operations")
                    {
                        ApiData = RestClient.GetByStatusNameCreatedById("Accepted", CompanyId,CurrentUserId);
                        return Json(ApiData, JsonRequestBehavior.AllowGet);
                    }
                    if (UserRole == "Payee")
                    {
                        //In case of payee claims displayed are either created by him or his children
                    var GridApiData = RestClient.GetByStatusNamePayeeUserIdForGrid("Accepted",CompanyId,CurrentUserId);
                    return Json(GridApiData, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case "ReExamine":
                    ApiData = RestClient.GetByStatusNameCreatedById("ReExamine",CompanyId,CurrentUserId).ToList();
                  return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Withdrawn":
                   ApiData = RestClient.GetByStatusName("Withdrawn",CompanyId).ToList();
                  return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Rejected":
                  ApiData = RestClient.GetByStatusName("Rejected",CompanyId).ToList();
                  return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Approved":
                    ApiData = RestClient.GetByStatusName("Approved",CompanyId).ToList();
                   return Json(ApiData, JsonRequestBehavior.AllowGet);
                case "Paid":
                    ApiData = RestClient.GetByStatusName("PayementFilePublished",CompanyId).ToList();
                   return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty,JsonRequestBehavior.AllowGet);
        }


        public FileResult DownloadClaimsGridByTabName(string TabName)
        {
           IEnumerable<DownloadClaimsGridViewModel> ApiData=new List<DownloadClaimsGridViewModel>();
            switch (TabName)
            {
                case "Open":
                    if (UserRole == "Sales Operations"|| UserRole == "Payee")
                    {
                        ApiData = RestClient.DownloadByStatusNameCreatedById("Accepted", CompanyId, CurrentUserId);
                        break;
                    }
                    //if (UserRole == "Payee")
                    //{
                    //    //In case of payee claims displayed are either created by him or his children
                    //    var GridApiData = RestClient.GetByStatusNamePayeeUserIdForGrid("Accepted", CompanyId, CurrentUserId);
                    //    break;
                    //}
                    break;
                case "ReExamine":
                    ApiData = RestClient.DownloadByStatusNameCreatedById("ReExamine", CompanyId, CurrentUserId).ToList();
                    break;
                case "Withdrawn":
                    ApiData = RestClient.DownloadByStatusName("Withdrawn", CompanyId).ToList();
                    break;
                case "Rejected":
                    ApiData = RestClient.DownloadByStatusName("Rejected", CompanyId).ToList();
                    break;
                case "Approved":
                    ApiData = RestClient.DownloadByStatusName("Approved", CompanyId).ToList();
                    break;
                case "Paid":
                    ApiData = RestClient.DownloadByStatusName("PayementFilePublished", CompanyId).ToList();
                    break;
            }
            var CompanySpecificColumns = LCSC.GetClaimsColumnsByCompanyId(CompanyId);
            var CfileLocation = ConfigurationManager.AppSettings["ClaimsDocumentPath"] + "/ExportClaimsGrid.csv";
            var FilesPath = ConfigurationManager.AppSettings["ClaimsDocumentPath"] + "/ExportClaimsGrid.csv";
            if (System.IO.File.Exists(CfileLocation))
                System.IO.File.Delete(CfileLocation);
            using (var CTextWriter = new StreamWriter(CfileLocation))
            using (var Ccsv = new CsvWriter(CTextWriter))
            {
               
                //The below lines of code converts the data returned from api to a datatable
                var tb = new DataTable(typeof(DownloadClaimsGridViewModel).Name);

                PropertyInfo[] props = typeof(DownloadClaimsGridViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                // LPayeeViewModel PayeeModel = new LPayeeViewModel();
                foreach (var prop in props)
                {
                    // var displayName=PayeeModel.GetDisplayName()
                    if (CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).Where(p => string.IsNullOrEmpty(p.LcscLabel) == false).Count() > 0)
                    {
                        tb.Columns.Add(CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).FirstOrDefault().LcscLabel);
                    }
                    else
                    {
                        tb.Columns.Add(prop.Name);
                    }
                }

                foreach (var item in ApiData)
                {
                    var values = new object[props.Length];
                    for (var i = 0; i < props.Length; i++)
                    {
                        values[i] = props[i].GetValue(item, null);
                    }

                    tb.Rows.Add(values);
                }


                foreach (DataColumn column in tb.Columns)
                {
                    Ccsv.WriteField(column.ColumnName);
                }
                Ccsv.NextRecord();

                foreach (DataRow row in tb.Rows)
                {
                    for (var i = 0; i < tb.Columns.Count; i++)
                    {
                        Ccsv.WriteField(row[i]);
                    }
                    Ccsv.NextRecord();
                }
            }

            return File(FilesPath,"text/csv","ExportClaimsGrid.csv");
        }
        // GET: ClaimsForms/Details/5
        //public ActionResult Details(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LClaimViewModel claimsFormsViewModel = RestClient.GetById(id);
        //    if (claimsFormsViewModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(claimsFormsViewModel);
        //}

    */
        //## DEAD CODE ENDS HERE

        //This methods returns a list of brands by passing companyid as parameter which will be shown in dropdown  
        [ControllerActionFilter]
        private SelectList GetBrands()
        {
            IRBrandsRestClient RBRC = new RBrandsRestClient();
            var ApiData = RBRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RbName");
            return x;
        }

        //This methods returns a list of brands by passing companyid as parameter which will be shown in dropdown where id passed is preselected  
        [ControllerActionFilter]
        private SelectList GetBrands(int id)
        {
            IRBrandsRestClient RBRC = new RBrandsRestClient();
            var ApiData = RBRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RbName", id);
            return x;
        }

        //This methods returns a list of CommissionType by passing companyid as parameter which will be shown in dropdown 
        [ControllerActionFilter]
        public SelectList GetCommissionType()
        {
            IRCommisionTypesRestClient RCTRC = new RCommissionTypesRestClient();
            var ApiData = RCTRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RctName");
            return x;
        }
        [ControllerActionFilter]
        private SelectList GetCommissionType(int id)
        {
            IRCommisionTypesRestClient RCTRC = new RCommissionTypesRestClient();
            var ApiData = RCTRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RctName", id);
            return x;
        }


        //This methods returns a list of ActivityType by passing companyid as parameter which will be shown in dropdown
        [ControllerActionFilter]
        public SelectList GetActivityType()
        {
            IRActiveTypesRestClient RATRC = new RActiveTypesRestClient();
            var ApiData = RATRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RatName");
            return x;
        }
        [ControllerActionFilter]
        private SelectList GetActivityType(int id)
        {
            IRActiveTypesRestClient RATRC = new RActiveTypesRestClient();
            var ApiData = RATRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RatName", id);
            return x;
        }

        //This methods returns a list of Device Type by passing companyid as parameter which will be shown in dropdown
        [ControllerActionFilter]
        private SelectList GetDeviceType()
        {
            IRDeviceTypesRestClient RDTRC = new RDeviceTypesRestClient();
            var ApiData = RDTRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RdtName");
            return x;
        }
        [ControllerActionFilter]
        private SelectList GetDeviceType(int id)
        {
            IRDeviceTypesRestClient RDTRC = new RDeviceTypesRestClient();
            var ApiData = RDTRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RdtName", id);
            return x;
        }

        //This methods returns a list of rejection Reason by passing companyid as parameter which will be shown in dropdown
        // [ControllerActionFilter]
        private SelectList GetrejectionReason()
        {
            IRRejectionReasonsRestClient RRRC = new RRejectionReasonsRestClient();
            var Rejection = RRRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(Rejection, "Id", "RrrReason");
            return x;
        }
        //[ControllerActionFilter]
        private SelectList GetrejectionReason(int id)
        {
            IRRejectionReasonsRestClient RRRC = new RRejectionReasonsRestClient();
            var Rejection = RRRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(Rejection, "Id", "RrrReason", id);
            return x;
        }

        //This methods returns a list of rejection Reason by passing companyid as parameter which will be shown in dropdown
        [ControllerActionFilter]
        private SelectList GetProductCode()
        {
            IRProductCodesRestClient RPCRC = new RProductCodesRestClient();
            var ProductCode = RPCRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ProductCode, "Id", "RpcProductCode");
            return x;
        }
        [ControllerActionFilter]
        private SelectList GetProductCode(int SelectedProductCode)
        {
            IRProductCodesRestClient RPCRC = new RProductCodesRestClient();
            var ProductCode = RPCRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ProductCode, "Id", "RpcProductCode", SelectedProductCode);
            return x;
        }
        private SelectList GetNextUserDetails( string workflow, int CompanyID, string LoggedInUserId, string LoggedinRoleID)
        {
            LClaimsRestClient RRRC = new LClaimsRestClient();
            var NextUsers = RRRC.GetNextUserDetails(workflow,CompanyId, LoggedInUserId, LoggedinRoleID);
            if (NextUsers != null)
            {
                if (NextUsers.Count() > 0)

                    foreach (var data in NextUsers)
                    {
                        TempData["NextRole"] = data.NextRole;
                    }


            }

            var x = new SelectList(NextUsers, "Id", "Email");
            return x;
        }

        //This method returns list of active payees in comany with with current user is associated
        //  [ControllerActionFilter]
        //  private SelectList GetPayeeCode()
        //{
        //    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();

        //    if(Role=="Payee")
        //    {
        //          var PayeeDetails = LPRC.GetByPayeeUserId(UserId);
        //          SelectList x;
        //          if (PayeeDetails.IsParent)
        //          {
        //            var Payees = LPRC.GetPayeeHierarchy(CompanyId, UserId);
        //             x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //          }
        //          else
        //          {
        //             var Payees =new[] { new {Id=PayeeDetails.Id,FullName=PayeeDetails.LpFirstName+" "+PayeeDetails.LpLastName +"("+PayeeDetails.LpPayeeCode+")"} };
        //              x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //          }

        //        return x;
        //    }
        //      else //if (Role == "Sales Operations" || Role == "Manager" || Role == "Claims Analyst")
        //      {
        //        var Payees = LPRC.GetPayeeForClaimsDropdown(CompanyId);
        //        var x = new SelectList(Payees,"Id","FullName");
        //        return x;
        //    }
        //     // return new SelectList(null);
        //}
        //  [ControllerActionFilter]
        //  private SelectList GetPayeeCode(string PayeeCode)
        //{
        //    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //      var PayeeDetails = LPRC.GetPayeeByPayeeCode(CompanyId,PayeeCode);
        //      if (Role == "Payee")
        //    {
        //          SelectList x;
        //          if (PayeeDetails.IsParent)
        //          {
        //              var Payees = LPRC.GetPayeeHierarchy(CompanyId, UserId);
        //              x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //          }
        //          else
        //          {
        //              var Payees = new[] { new { Id = PayeeDetails.Id, FullName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName+" ("+PayeeDetails.LpPayeeCode+")" } }.ToList();
        //              x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //          }

        //          return x;
        //      }
        //     else
        //      {
        //          var Payees = LPRC.GetPayeeForClaimsDropdown(CompanyId);
        //        var x = new SelectList(Payees, "Id", "FullName",PayeeDetails.Id);
        //        return x;
        //    }

        //  }

        // This method loads the create view to enter a claim

        public ActionResult CanRaiseClaimFromReview(Nullable<int> TransactionId)
        {
            var ClaimViewModel = new LClaimViewModel();
            if (TransactionId.HasValue)
                ClaimViewModel = RestClient.GetById(TransactionId.Value);
            //passing data of LCompanySpecificColumns data to the view to display the form labels dynamically as per LCompanyspecific columns

            string Username = LoggedInUserName;
            ClaimViewModel.WFRequesterId = LoggedInUserId;

            var batchname = Convert.ToString(this.Session["XBatchName"]);
            var batchno = Convert.ToString(this.Session["XBatchNumber"]);
            var AltTransactNo = Convert.ToString(this.Session["AlteryxTransactNo"]);

            //ClaimViewModel.Comments = "[" + Username + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] Reference: Claim against Calc Batch[" + batchname + "] Batch Number [" + batchno + "] Alteryx TransactionNumber [" + AltTransactNo + "]" + Environment.NewLine;
            ClaimViewModel.Comments = "Reference: Claim against Calc";
            //Batch[" + batchname + "] Batch Number [" + batchno + "] Alteryx TransactionNumber [" + AltTransactNo + "]" + Environment.NewLine;

            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            string formname = "Claims";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);
            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }

            var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
            ViewBag.CompanySpecificColumn = CompanySpecificData;

            //This line adds title to the Nav bar
            System.Web.HttpContext.Current.Session["Title"] = "Create Claim";
            ViewBag.SelectionMode = "singlerow";//used int the partial view to decide whether payee grid will have single selection or multiple
            //passing list of fields in viewbag which would be available as dropdown in form
            //ViewBag.LcPayeeId = Globals.GetPayeeCode(string.Empty);
            ViewBag.LcBrandId = GetBrands();
            ViewBag.LcCommissionTypeId = GetCommissionType();
            ViewBag.LcActivityTypeId = GetActivityType();
            ViewBag.LcDeviceTypeId = GetDeviceType();
            ViewBag.LcPaymentCommissionTypeId = GetCommissionType();
            ViewBag.LcProductCodeId = GetProductCode();
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");
            /*This section gives empty dropdown values in LDropDownValues*/
            ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);
            if (TransactionId != null)
            {
                ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);
            }
            //ClaimViewModel.LcscTooltip = CompanySpecificData.FirstOrDefault().LcscTooltip;

            /*section ends here*/
            return View(ClaimViewModel);
        }

        //
        // This method is called when we click on save button in the enter claim form.
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CanRaiseClaimFromReview(LClaimViewModel LCVM, HttpPostedFileBase[] File1)
        {
            try
            {
                string ClaimFiles = "";
                string ClaimFilePath = "";
                int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
                var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
                string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
                string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
                string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);

                //to check if Salesop/Payee  has added documents with claims or not
                if (File1[0] != null)
                {
                    ILPayeesRestClient LPARC = new LPayeesRestClient();
                    var PayeeDetails = LPARC.GetById(LCVM.LcPayeeId);
                    //This method will save claims documents in path specified in web.config files and return Claim filenames 
                    var Files = AttachSupportingDocs(File1, PayeeDetails.LpPayeeCode);
                    ClaimFiles = Files.FileName;
                    ClaimFilePath = Files.FilePath;
                }
                ILPayeesRestClient LPRC = new LPayeesRestClient();
                LCVM.FileName = ClaimFiles;

                if (LCVM.Id == 0)//Add record if Id=0 otherwise update it as th Payee and SO will have edit page which is same as create page
                {
                    //if comments are provided by the user then add them in database with Who and when information

                    if (LCVM.Comments != null)
                    {
                        LCVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LCVM.Comments;
                    }
                    LCVM.WFRequesterId = LoggedInUserId;
                    LCVM.WFCurrentOwnerId = LoggedInUserId;
                    LCVM.WFRequesterRoleId = LoggedInRoleId;
                    LCVM.LcCreatedDateTime = DateTime.UtcNow;
                    LCVM.LcCompanyId = CompanyId;
                    LCVM.WFCompanyId = CompanyId;
                    LCVM.LcCreatedById = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
                    //MS don't know what Workflow to pass
                    string Workflow = "Claims";
                    var s = RestClient.Add(LCVM, Workflow, ClaimFilePath, LoggedInRoleId, null);
                    //Auto approve workflow after addition
                    //string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                    //R2.3 RK 31102018, Restricted the call to updateactionstatus if the claim was created by CA.
                    if (UserRole != "Claims Analyst")
                    {
                        try//Auto Approve the Claim and display error in case any validation fails
                        {
                            GGRC.UpdateActionStatus(Workflow, Convert.ToString(s), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return RedirectToAction("PayeeCalc", "LCalc");
                        }
                    }
                }
                else
                {
                    LCVM.LcCompanyId = CompanyId;
                    LCVM.WFCompanyId = CompanyId;
                    //If user has provided comments then append them with the existing comments

                    if (!string.IsNullOrEmpty(LCVM.Comments))
                    {
                        LCVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LCVM.Comments + Environment.NewLine + LCVM.WFComments;
                    }
                    RestClient.Update(LCVM, ClaimFilePath, null);
                }
                return RedirectToAction("PayeeCalc", "LCalc");
            }
            catch (Exception ex)
            {
                //passing data of LCompanySpecificColumns data to the view to display the form labels dynamically as per LCompanyspecific columns
                ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumn = CompanySpecificData;
                ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
                ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");

                if (LCVM.LcProductCodeId.HasValue)
                {
                    ViewBag.LcProductCodeId = GetProductCode(LCVM.LcProductCodeId.Value);
                }
                else
                {
                    ViewBag.LcProductCodeId = GetProductCode();
                }

                //ViewBag.LcPayeeId = Globals.GetPayeeCode(LCVM.LcPayeeId,string.Empty);

                if (LCVM.LcBrandId.HasValue)
                {
                    ViewBag.LcBrandId = GetBrands(LCVM.LcBrandId.Value);
                }
                else
                {
                    ViewBag.LcBrandId = GetBrands();
                }
                if (LCVM.LcCommissionTypeId.HasValue)
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType(LCVM.LcCommissionTypeId.Value);
                }
                else
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType();
                }
                if (LCVM.LcActivityTypeId.HasValue)
                {
                    ViewBag.LcActivityTypeId = GetActivityType(LCVM.LcActivityTypeId.Value);
                }
                else
                {
                    ViewBag.LcActivityTypeId = GetActivityType();
                }
                if (LCVM.LcDeviceTypeId.HasValue)
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType(LCVM.LcDeviceTypeId.Value);
                }
                else
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType();
                }
                /*This section gives empty dropdown values in LDropDownValues*/
                ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                /*section ends here*/
                //This line is added to pass error message to the view 
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        //Display Error Message as Popup
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    default:
                        throw ex;
                }
            }
        }

        
       

        public JsonResult GetPayeeData()
        {
            LPayeeViewModel objPayee = LPRC.GetByPayeeUserId(CurrentUserId);
            return Json(objPayee, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Create(Nullable<int> TransactionId,string CalcPayeeCode, string CalcComment)
        {
            var ClaimViewModel = new LClaimViewModel();
            if(CalcPayeeCode != null && CalcPayeeCode != "")
            {
                TempData["Reclaim"] = "True";
                TempData["BackButton"] = "Close";
                ILPayeesRestClient RestClient = new LPayeesRestClient();
                ClaimViewModel.LcPayeeCode = CalcPayeeCode;
                ClaimViewModel.Comments =  CalcComment;

                int objPayeeID = RestClient.GetPayeeDetailsByPayeeCodeId(CalcPayeeCode);
                ClaimViewModel.LcPayeeCode = CalcPayeeCode;
                ClaimViewModel.LcPayeeId = objPayeeID;
                
            }
            
            if (TransactionId.HasValue)
                ClaimViewModel = RestClient.GetById(TransactionId.Value);
            //passing data of LCompanySpecificColumns data to the view to display the form labels dynamically as per LCompanyspecific columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            string formname = "Claims";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);
            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }
            
            

            var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
            ViewBag.CompanySpecificColumn = CompanySpecificData;

            //This line adds title to the Nav bar
            System.Web.HttpContext.Current.Session["Title"] = "Create Claim";
            ViewBag.SelectionMode = "singlerow";//used int the partial view to decide whether payee grid will have single selection or multiple
            //passing list of fields in viewbag which would be available as dropdown in form
            //ViewBag.LcPayeeId = Globals.GetPayeeCode(string.Empty);
            ViewBag.LcBrandId = GetBrands();
            ViewBag.LcCommissionTypeId = GetCommissionType();
            ViewBag.LcActivityTypeId = GetActivityType();
            ViewBag.LcDeviceTypeId = GetDeviceType();
            ViewBag.LcPaymentCommissionTypeId = GetCommissionType();
            ViewBag.LcProductCodeId = GetProductCode();
            ViewBag.LcRejectionReasonId = GetrejectionReason();
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");
            /*This section gives empty dropdown values in LDropDownValues*/
            ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);
          //  ViewBag.LUsers = RestClient.GetNextUserDetails(Workflow, CompanyId, LoggedInUserId, LoggedInRoleId);
          if(Workflow == null) { Workflow = "Claims"; }
            ViewBag.ForwordTo = GetNextUserDetails(Workflow, CompanyId, LoggedInUserId, LoggedInRoleId);

            if (TransactionId != null)
            {
                ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);
                ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LClaims",Convert.ToInt32(TransactionId));
            }
            //ClaimViewModel.LcscTooltip = CompanySpecificData.FirstOrDefault().LcscTooltip;
            ViewBag.CheckUserRole = UserRole;
            /*section ends here*/
            return View(ClaimViewModel);
        }
        //
        // This method is called when we click on save button in the enter claim form.
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LClaimViewModel LCVM, HttpPostedFileBase[] File1)
        {
            //string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            try
            {
                string ClaimFiles = "";
                string ClaimFilePath = "";
                //to check if Salesop/Payee  has added documents with claims or not
                if (LCVM.Id == 0)//Add record if Id=0 otherwise update it as th Payee and SO will have edit page which is same as create page
                {
                    if (File1[0] != null)
                    {
                        ILPayeesRestClient LPARC = new LPayeesRestClient();
                        var PayeeDetails = LPARC.GetById(LCVM.LcPayeeId);
                        //This method will save claims documents in path specified in web.config files and return Claim filenames 
                        var Files = AttachSupportingDocs(File1, PayeeDetails.LpPayeeCode);
                        ClaimFiles = Files.FileName;
                        ClaimFilePath = Files.FilePath;

                    }
                }
                ILPayeesRestClient LPRC = new LPayeesRestClient();
                LCVM.FileName = ClaimFiles;
                //The Below Auto Allocation Rules Concept has beeen discarded after the introduction of Flex Workflow
                ////Lines added to check for allocation rules in current opco and allocate claim to the analyst as per rules matched
                //ILAllocationRulesRestClient LARRC = new LAllocationRulesRestClient();
                //ILPayeesRestClient LPRC = new LPayeesRestClient();
                //var AllocationRules = LARRC.GetByCompanyId(CompanyId).ToList();
                //LCVM.LcStatus = "Accepted";
                //LCVM.FileName = ClaimFiles;
                //var PayeeDetails = LPRC.GetById(LCVM.LcPayeeId);
                //LCVM.LcPayeeCode = PayeeDetails.LpPayeeCode;
                //LCVM.LcPayeeName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName;
                //LCVM.LcParentName = PayeeDetails.ParentName;
                //LCVM.LcParentCode = PayeeDetails.ParentCode;
                //    foreach (var rule in AllocationRules)
                //    {
                //        switch (rule.LarKey)
                //        {
                //            case "Channel":
                //                if (PayeeDetails.RcName.Equals(rule.LarValue, StringComparison.OrdinalIgnoreCase))
                //                {
                //                    LCVM.LcStatus = "Allocated";
                //                    LCVM.LcAllocatedToId = rule.LarAllocatedToId;
                //                    break;
                //                }
                //                break;
                //            case "Payee Code":
                //                if (PayeeDetails.LpPayeeCode.Equals(rule.LarValue, StringComparison.OrdinalIgnoreCase))
                //                {
                //                    LCVM.LcStatus = "Allocated";
                //                    LCVM.LcAllocatedToId = rule.LarAllocatedToId;
                //                    break;
                //                }
                //                break;
                //        }
                //        if (LCVM.LcStatus == "Allocated")//break the loop once a rule is applied
                //        {
                //            break;
                //        }
                //}

                var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();

                if (LCVM.Id == 0)//Add record if Id=0 otherwise update it as th Payee and SO will have edit page which is same as create page
                {
                    //if comments are provided by the user then add them in database with Who and when information

                    if (LCVM.Comments != null)
                    {
                        LCVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LCVM.Comments;
                    }
                    LCVM.WFRequesterId = LoggedInUserId;
                    LCVM.WFCurrentOwnerId = LoggedInUserId;
                    LCVM.WFRequesterRoleId = LoggedInRoleId;
                    LCVM.LcCreatedDateTime = DateTime.UtcNow;
                    LCVM.LcCompanyId = CompanyId;
                    LCVM.WFCompanyId = CompanyId;
                    LCVM.LcCreatedById = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
                    var s = RestClient.Add(LCVM, Workflow, ClaimFilePath, LoggedInRoleId, null);
                    //Auto approve workflow after addition
                  //R2.9 SG 11022020, Restriction removed as we have Save button now.
                    //R2.3 RK 31102018, Restricted the call to updateactionstatus if the claim was created by CA.
                    //if (UserRole != "Claims Analyst")
                    {
                        try//Auto Approve the Claim and display error in case any validation fails
                        {
                            if(Workflow == null)
                            {
                                Workflow = "Claims";
                                System.Web.HttpContext.Current.Session["Workflow"] = Workflow;
                            }
                            if (LCVM.Type == "Submit")
                            {
                                if (LCVM.ForwordTo != "" && LCVM.ForwordTo != null)
                                {
                                    GGRC.UpdateActionStatus(Workflow, Convert.ToString(s), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId, LCVM.ForwordTo);
                                }
                                else
                                {
                                    GGRC.UpdateActionStatus(Workflow, Convert.ToString(s), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId, string.Empty);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return RedirectToAction("Index", "GenericGrid");
                        }
                    }
                }
                else
                {
                    LCVM.LcCompanyId = CompanyId;
                    LCVM.WFCompanyId = CompanyId;
                    //If user has provided comments then append them with the existing comments

                    if (!string.IsNullOrEmpty(LCVM.Comments))
                    {
                        LCVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LCVM.Comments + Environment.NewLine + LCVM.WFComments;
                    }
                    RestClient.Update(LCVM, ClaimFilePath, null);
                    if (LCVM.Type == "Submit")
                    {
                        if (LCVM.ForwordTo != "" && LCVM.ForwordTo != null)
                        {
                            GGRC.UpdateActionStatus(Workflow, Convert.ToString(LCVM.Id), CompanyId, "Approve", LoggedInUserId, string.Empty, LoggedInRoleId, LCVM.ForwordTo);
                        }
                    }
                }
                //if (TempData["Reclaim"] != null)
                //{
                //    if (Convert.ToString(TempData["Reclaim"]) == "True")
                //    {
                //        TempData["Reclaim"] = null;

                //        return RedirectToAction("PayeeCalc", "LCalc");
                //    }else
                //    {
                //        return RedirectToAction("Index", "GenericGrid");
                //    }
                //} else {
                return RedirectToAction("Index", "GenericGrid");
               // return View("Close");
                //}
            }
            catch (Exception ex)
            {
                //passing data of LCompanySpecificColumns data to the view to display the form labels dynamically as per LCompanyspecific columns
                ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumn = CompanySpecificData;
                ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
                ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");

                if (LCVM.LcProductCodeId.HasValue)
                {
                    ViewBag.LcProductCodeId = GetProductCode(LCVM.LcProductCodeId.Value);
                }
                else
                {
                    ViewBag.LcProductCodeId = GetProductCode();
                }

                //ViewBag.LcPayeeId = Globals.GetPayeeCode(LCVM.LcPayeeId,string.Empty);

                if (LCVM.LcBrandId.HasValue)
                {
                    ViewBag.LcBrandId = GetBrands(LCVM.LcBrandId.Value);
                }
                else
                {
                    ViewBag.LcBrandId = GetBrands();
                }
                if (LCVM.LcCommissionTypeId.HasValue)
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType(LCVM.LcCommissionTypeId.Value);
                }
                else
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType();
                }
                if (LCVM.LcActivityTypeId.HasValue)
                {
                    ViewBag.LcActivityTypeId = GetActivityType(LCVM.LcActivityTypeId.Value);
                }
                else
                {
                    ViewBag.LcActivityTypeId = GetActivityType();
                }
                if (LCVM.LcDeviceTypeId.HasValue)
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType(LCVM.LcDeviceTypeId.Value);
                }
                else
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType();
                }
                /*This section gives empty dropdown values in LDropDownValues*/
                ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                /*section ends here*/
                //This line is added to pass error message to the view 
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        //Display Error Message as Popup
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    default:
                        throw ex;
                }
            }
        }


        // This method loads the edit view of the claims
        [ControllerActionFilter]
        public ActionResult Edit(int TransactionId, string FormType, Nullable<int> WFConfigId)
        {

            /*This section gives empty dropdown values in LDropDownValues*/
            ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            /*section ends here*/
            //Get Claim details by passing id and pass it to the view
            LClaimViewModel claimsFormsViewModel = RestClient.GetById(TransactionId);

            if (claimsFormsViewModel.WFRequesterId == LoggedInUserId && (string.IsNullOrEmpty(FormType) || FormType == "Edit"))//Redirect to the Payee Edit form which is the create method
            {
                return RedirectToAction("Create", new { TransactionId = TransactionId });
            }

            //Get ActionItems to be displayed 
            if (WFConfigId.HasValue)
                ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId.Value, LoggedInRoleId, LoggedInUserId, TransactionId);
            //List of Atachments
            //Get Supporting Documents by passing Entity id
            ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LClaims", TransactionId);
            //Pass information whether form will be editable or not
            ViewBag.FormType = FormType;
            //passing data of LCompanySpecificColumns data to the view to display form as per LCompanySpecific columns table
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
            ViewBag.CompanySpecificColumnData = CompanySpecificData;

            //This line adds title to the form
            System.Web.HttpContext.Current.Session["Title"] = "Edit Claim";

            ViewBag.GetSudmitableorNot = RestClient.GetSudmitableorNot(TransactionId, Workflow, UserRole, CompanyId);
            ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);

            if (claimsFormsViewModel.LcProductCodeId.HasValue)
            {
                ViewBag.LcProductCodeId = GetProductCode(claimsFormsViewModel.LcProductCodeId.Value);
            }
            else
            {
                ViewBag.LcProductCodeId = GetProductCode();
            }
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");

            // ViewBag.LcPayeeId = Globals.GetPayeeCode(claimsFormsViewModel.LcPayeeId,string.Empty);


            if (claimsFormsViewModel.LcBrandId.HasValue)
            {
                ViewBag.LcBrandId = GetBrands(claimsFormsViewModel.LcBrandId.Value);
            }
            else
            {
                ViewBag.LcBrandId = GetBrands();
            }
            if (claimsFormsViewModel.LcCommissionTypeId.HasValue)
            {
                ViewBag.LcCommissionTypeId = GetCommissionType(claimsFormsViewModel.LcCommissionTypeId.Value);
            }
            else
            {
                ViewBag.LcCommissionTypeId = GetCommissionType();
            }
            if (claimsFormsViewModel.LcActivityTypeId.HasValue)
            {
                ViewBag.LcActivityTypeId = GetActivityType(claimsFormsViewModel.LcActivityTypeId.Value);
            }
            else
            {
                ViewBag.LcActivityTypeId = GetActivityType();
            }
            if (claimsFormsViewModel.LcDeviceTypeId.HasValue)
            {
                ViewBag.LcDeviceTypeId = GetDeviceType(claimsFormsViewModel.LcDeviceTypeId.Value);
            }
            else
            {
                ViewBag.LcDeviceTypeId = GetDeviceType();
            }
            if (claimsFormsViewModel == null)
            {
                return HttpNotFound();
            }

            string formname = "Claims";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);
            
            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }
            ViewBag.LcRejectionReasonId = GetrejectionReason();
            ViewBag.RChannel = RestClient.GetByRChannelByTransactionID(TransactionId);
            return View(claimsFormsViewModel);
        }

        // This method is called when we clickon the save button in the edit page of claims
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //The Validate AntiForgeryToken is removed to avoid the error when for is opened in disabled mode the token cannot be validated.
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ControllerActionFilter]
        public ActionResult Edit(LClaimViewModel claimsFormsViewModel, HttpPostedFileBase[] File1)
        {
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
           
            try
            {
                string ClaimFiles = "";
                string ClaimFilePath = "";
                //to check if Salesop/Payee  has added documents with claims or not
                //if (File1[0] != null)
                //{
                //    ILPayeesRestClient LPARC = new LPayeesRestClient();
                //    var PayeeDetails = LPARC.GetById(claimsFormsViewModel.LcPayeeId);
                //    //This method will save claims documents in path specified in web.config files and return Claim filenames 
                //    var Files = AttachSupportingDocs(File1, PayeeDetails.LpPayeeCode);
                //    ClaimFiles = Files.FileName;
                //    ClaimFilePath = Files.FilePath;
                //}
                claimsFormsViewModel.FileName = ClaimFiles;
                claimsFormsViewModel.WFCompanyId = CompanyId;
                claimsFormsViewModel.LcCompanyId = CompanyId;

                var Username = System.Web.HttpContext.Current.Session["UserName"].ToString();
                //If user has provided comments then append them with the existing comments

                if (!string.IsNullOrEmpty(claimsFormsViewModel.Comments))
                {
                    claimsFormsViewModel.WFComments = "[" + Username + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + claimsFormsViewModel.Comments + Environment.NewLine + claimsFormsViewModel.WFComments;
                }

                RestClient.Update(claimsFormsViewModel, ClaimFilePath, null);
                if (claimsFormsViewModel.SubmitClicked == "True")
                {
                    if (RestClient.GetSudmitableorNot(claimsFormsViewModel.Id, Workflow, UserRole, CompanyId) == "True")
                    {
                        string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                        GGRC.UpdateActionStatus(WorkflowName, Convert.ToString(claimsFormsViewModel.Id), CompanyId, "Approve", LoggedInUserId, "", LoggedInRoleId, string.Empty);
                    }
                }

                
                return RedirectToAction("Index", "GenericGrid");

            }
            catch (Exception ex)
            {

                /*This section gives empty dropdown values in LDropDownValues*/
                ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                /*section ends here*/
                ViewBag.LcRejectionReasonId = GetrejectionReason();
                //Get Supporting Documents by passing Entity id
                ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LClaims", claimsFormsViewModel.Id);
                //passing data of LCompanySpecificColumns data to the view to display the form labels dynamically as per LCompanyspecific columns
                //ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumnData = CompanySpecificData;
                //This line is added to pass error message in view data
                ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
                ViewBag.LcCommissionPeriod = new SelectList(CPRC.GetByCompanyId(CompanyId), "LcpPeriodName", "LcpStatus");

                if (claimsFormsViewModel.LcProductCodeId.HasValue)
                {
                    ViewBag.LcProductCodeId = GetProductCode(claimsFormsViewModel.LcProductCodeId.Value);
                }
                else
                {
                    ViewBag.LcProductCodeId = GetProductCode();
                }

                // ViewBag.LcPayeeId = Globals.GetPayeeCode(claimsFormsViewModel.LcPayeeId,string.Empty);

                if (claimsFormsViewModel.LcBrandId.HasValue)
                {
                    ViewBag.LcBrandId = GetBrands(claimsFormsViewModel.LcBrandId.Value);
                }
                else
                {
                    ViewBag.LcBrandId = GetBrands();
                }
                if (claimsFormsViewModel.LcCommissionTypeId.HasValue)
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType(claimsFormsViewModel.LcCommissionTypeId.Value);
                }
                else
                {
                    ViewBag.LcCommissionTypeId = GetCommissionType();
                }
                if (claimsFormsViewModel.LcActivityTypeId.HasValue)
                {
                    ViewBag.LcActivityTypeId = GetActivityType(claimsFormsViewModel.LcActivityTypeId.Value);
                }
                else
                {
                    ViewBag.LcActivityTypeId = GetActivityType();
                }
                if (claimsFormsViewModel.LcDeviceTypeId.HasValue)
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType(claimsFormsViewModel.LcDeviceTypeId.Value);
                }
                else
                {
                    ViewBag.LcDeviceTypeId = GetDeviceType();
                }

                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(claimsFormsViewModel);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(claimsFormsViewModel);
                    default:
                        throw ex;
                }
            }
        }

        // This method loads the delete view of the claims
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Claim";
            LClaimViewModel claimsFormsViewModel = RestClient.GetById(id);
            if (claimsFormsViewModel == null)
            {
                return HttpNotFound();
            }
            return View(claimsFormsViewModel);
        }

        // This method is called when user clicks on the delete button on the delete page of claims
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(LClaimViewModel LCVM)
        {
            try
            {
                RestClient.Delete(LCVM.Id, null);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(LCVM);
                    default:
                        throw ex;
                }
            }
        }

        /*
         * DEAD CODE STARTS HERE
        //This method is called when we click on withdraw button in Open, ReExamineor Rejected Tab in view
        public ActionResult WithdrawClaims(string[] ClaimId)
        {
            try
            {
                //This method passes a comma seperated list of claims along with who and when columns to stored procedure to update data in one api call
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
                model.ClaimsList=ClaimId.ElementAt(0);
                model.IsReClaim=false;
                model.StatusName = "Withdrawn"; //passes withdrawn status as string
               
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                //Pass data to view as popup if any error occurs
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index");
            }
        }

        //This method is called when we click on Accept button in view either from ReExamine or from Rejected tab
        public ActionResult ReSubmitClaim(string[] ClaimId)
        {
            try
            {
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
               
                model.ApprovedBy = null; model.ApprovedDate = null;
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "Accepted";//string of status passed as hardcoded in stored procedure
               
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index");
            }
        }

        //This method is called when SalesOp/Payee clicks on the reclaim button for a rejected claim
        public ActionResult ReClaimClaim(string[] ClaimId)
        {
            try
            {
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
                model.ApprovedBy = null; model.ApprovedDate = null;
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = true;
                model.LastReClaimDate = DateTime.UtcNow;
                model.StatusName = "Accepted";
                //string of status passed as hardcoded in stored procedure
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index");
            }
        }

        //This method is called when Manager Allocates Claim to ClaimsAnalyst
        public ActionResult AllocateClaim(string[] ClaimId,string AllocateToId)
        {
            try
            {
                var AllocatedByUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
                model.AllocatedBy = AllocatedByUserId;
                model.AllocateTo = AllocateToId;
                model.AllocationDate =DateTime.UtcNow;
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "Allocated";
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("ManagerIndex");
               // return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("ManagerIndex");
            }
        }

        //This methos is called when Manager clicks on approve button in grid
        public ActionResult ApproveClaim(string[] ClaimId)
        {
            try
            {
                var UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
               
                model.ApprovedBy = UserId;
                model.ApprovedDate = DateTime.UtcNow;
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "Approved";
                //withdrawn status string
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("ManagerIndex");
            }
            catch (Exception ex)
            {
               // TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("ManagerIndex");
            }
        }

        //This method is called when Manager clicks on ReInvestigate button on the grid
        public ActionResult ReInvestigateClaim(string[] ClaimId)
        {
            try
            {
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
                
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "ReInvestigate";
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("AnalystIndex");
            }
            catch (Exception ex)
            {
               // TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("ManagerIndex");
            }
        }

        //This method is called when a Analyst rejects the claim by clicking on reject button
        public ActionResult RejectedClaim(string[] ClaimId,int RejectionReasonId)
        {
            try
            {
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
               
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.RejectionReasonId =RejectionReasonId;
                model.StatusName = "Rejected";
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("AnalystIndex");
             
               // return View(model);
              
            }
            catch (Exception ex)
            {
               // TempData["Error"] = ex.Message;
                return RedirectToAction("AnalystIndex");
            }
        }

      
        //This method is called when Analyst clicks on reExamine button on the grid
        public ActionResult ReExamine(string[] ClaimId,string Comments,int offset)
        {
            try
            {
                var UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
                
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "ReExamine";
                if (!string.IsNullOrEmpty(Comments))
                {
                    model.Comments = "[" + UserId + "] [" + DateTime.UtcNow.AddMinutes(-offset) + "] " + Comments;
                }

                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("AnalystIndex");
            }
            catch (Exception ex)
            {
              //  TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("AnalystIndex");
            }
        }

        //method to send claim for approval by analyst to manager
        public ActionResult PendingApproval(string[] ClaimId, string Comments,int offset)
        {
            try
            {
                var UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
                Sp_UpdateStatusViewModel model = new Sp_UpdateStatusViewModel();
               
                model.ClaimsList = ClaimId.ElementAt(0);
                model.IsReClaim = false;
                model.StatusName = "PendingApproval";
                if (!string.IsNullOrEmpty(Comments))
                {
                    model.Comments = "[" + UserId + "] [" + DateTime.UtcNow.AddMinutes(-offset) + "] " + Comments;
                }
                UCSRC.UpdateClaimsStatus(model);
                return RedirectToAction("AnalystIndex");
            }
            catch (Exception ex)
            {
               // TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("AnalystIndex");
            }
        }
        */
        //##DEAD CODE ENDS HERE

        //method to download dynamic template of LClaims as per company specific columns
        [ControllerActionFilter]
        public ActionResult ExportClaimsTemplateToExcel()
        {
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var ColumnList = LCSCRC.GetClaimsDownloadTemplateByCompanyId(CompanyId);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < ColumnList.Count(); j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = ColumnList.ElementAt(j).LcscLabel;
                cell.SetCellValue(columnName);
                //method to size column width and GC is used to avoid error System.argument exception

                GC.Collect();
            }



            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ClaimsTemplate.xlsx"));
                Response.BinaryWrite(exportData.ToArray());
                Response.End();
            }
            return View("Index", "GenericGrid");
            // return RedirectToAction("Index");
        }

        //This method is used to upload claims from excel sheet
        [ControllerActionFilter]
        public ActionResult UploadClaims()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Upload Claims";
            return View();
        }

        //This method is called when user clicks on upload button to upload multiple claims from excel sheet
        [HttpPost]
        [ControllerActionFilter]
        public ActionResult UploadClaims(HttpPostedFileBase File1)
        {
            string fileLocation = "";
            TempData["Message"] = "";
            ILCompanySpecificColumnsRestClient CSCRC = new LCompanySpecificColumnsRestClient();
            try
            {
                if (Request.Files["File1"].ContentLength > 0)
                {
                    // var Random=new Random();
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + fileExtension;
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //The file uploaded will be saved in the location defined in web config
                        ////////fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ClaimsDocumentPath"], "\\"+ CompanyCode.ToUpper() + "\\upload\\claims\\"+FileNames);
                        ////////DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["ClaimsDocumentPath"]+ "\\" + CompanyCode.ToUpper() + "\\upload\\claims\\");
                        ////////if (!di.Exists) di.Create();
                        ////////Request.Files["File1"].SaveAs(fileLocation);
                        //RK using SDK here
                        string filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/upload/claims");
                        if (!Globals.FolderExistsInS3(filePath))
                        {
                            Globals.CreateFolderInS3Root(filePath);
                        }
                        Globals.UploadToS3(File1.InputStream, ManipulatedFileName, filePath);
                        
                       
                        bool IsUploadedDataValid = true;
                        var ModelList = new List<LClaimViewModel>();
                        var ErroredList = new List<LClaimViewModel>();
                        #region OLD Code before R2.1.3
                        ////////1. Get Column list from LCompanySpecificColumns (Label + Columns)
                        ////////2. Loop through Label list obtained in step no 1
                        ////////3. Get corresponding header label from excel sheet
                        ////////4. If any of corresponding header is missing then raise error (could not find this column)
                        ////////5  Excess columns would be excluded and data would be uploaded
                        ////////6  insert into LClaims (Column List) select columnList from ExcelSheet  
                        ////////make a list of record to display in grid if record is Invalid


                        ////////The below code  gets reference data from database which would be used to validate reference data.
                        //////IRCommisionTypesRestClient RCTRC = new RCommissionTypesRestClient();
                        //////var CommissionType = RCTRC.GetByCompanyId(CompanyId);
                        //////IRDeviceTypesRestClient RDTRC = new RDeviceTypesRestClient();
                        //////var DeviceType = RDTRC.GetByCompanyId(CompanyId);
                        //////IRBrandsRestClient RBRC = new RBrandsRestClient();
                        //////var BrandType = RBRC.GetByCompanyId(CompanyId);
                        //////IRActiveTypesRestClient ATRC = new RActiveTypesRestClient();
                        //////var ActivityType = ATRC.GetByCompanyId(CompanyId);
                        //////IRProductCodesRestClient PCode = new RProductCodesRestClient();
                        //////var Productcode = PCode.GetByCompanyId(CompanyId);
                        //////ILPayeesRestClient PAyees = new LPayeesRestClient();
                        //////var PAyeesList = PAyees.GetActivePayee(CompanyId);
                        ////////The above lines runs a for loop from third row of data in excelsheet as the first line includes the lables to be displayed to users 
                        ////////In this method every row is read one by one and its data is stored in a LClaimViewModel object
                        //////for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        //////{
                        //////    var model = new LClaimViewModel();
                        //////    string ErrorList = "";
                        //////    #region Flex Columns
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A01"].ToString()))
                        //////    {
                        //////        model.A01 = (ds.Tables[0].Rows[i]["A01"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A02"].ToString()))
                        //////    {
                        //////        model.A02 = (ds.Tables[0].Rows[i]["A02"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A03"].ToString()))
                        //////    {
                        //////        model.A03 = (ds.Tables[0].Rows[i]["A03"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A04"].ToString()))
                        //////    {
                        //////        model.A04 = (ds.Tables[0].Rows[i]["A04"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A05"].ToString()))
                        //////    {
                        //////        model.A05 = (ds.Tables[0].Rows[i]["A05"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A06"].ToString()))
                        //////    {
                        //////        model.A06 = (ds.Tables[0].Rows[i]["A06"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A07"].ToString()))
                        //////    {
                        //////        model.A07 = (ds.Tables[0].Rows[i]["A07"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A08"].ToString()))
                        //////    {
                        //////        model.A08 = (ds.Tables[0].Rows[i]["A08"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A09"].ToString()))
                        //////    {
                        //////        model.A09 = (ds.Tables[0].Rows[i]["A09"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A10"].ToString()))
                        //////    {
                        //////        model.A10 = (ds.Tables[0].Rows[i]["A10"].ToString()).Trim();
                        //////    }
                        //////    #endregion
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcPayeeId"].ToString()))
                        //////    {
                        //////        model.LcPayeeCode = ds.Tables[0].Rows[i]["LcPayeeId"].ToString();
                        //////        if (PAyeesList.Where(p => p.LpPayeeCode.Equals(ds.Tables[0].Rows[i]["LcPayeeId"].ToString())).Count() > 0)
                        //////        {
                        //////            model.LcPayeeId = PAyeesList.Where(p => p.LpPayeeCode.Equals(ds.Tables[0].Rows[i]["LcPayeeId"].ToString())).FirstOrDefault().Id;
                        //////            if (UserRole == "Payee")
                        //////            {
                        //////                //Add SO and Channel Manager validation for payee when channel manager role is introduced
                        //////                //SS Added type to handle dynamic object returned
                        //////                IEnumerable<LPayeeViewModel> payeetree = PAyees.GetPayeeHierarchy(CompanyId, CurrentUserId,false);
                        //////                if(payeetree.Where(p => p.LpPayeeCode == ds.Tables[0].Rows[i]["LcPayeeId"].ToString()).Count() == 0)
                        //////                {
                        //////                    IsUploadedDataValid = false;
                        //////                    ErrorList = ErrorList + " | This Payee Code does not belongs to you hierarchy";
                        //////                }
                        //////                //string payeeTreeCode = PAyees.GetPayeeHierarchy(CompanyId, CurrentUserId).Where(p=> p.LpPayeeCode == ds.Tables[0].Rows[i]["LcPayeeCode"].ToString()).FirstOrDefault().LpPayeeCode;
                        //////                //if(payeeTreeCode=="")
                        //////                //{

                        //////                //}
                        //////            }
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Payee code not found in system";
                        //////        }


                        //////    }
                        //////    else
                        //////    {
                        //////        IsUploadedDataValid = false;
                        //////        ErrorList = ErrorList + " | Payee code is mandatory";
                        //////    }

                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcBrandId"].ToString()))
                        //////    {
                        //////        if (BrandType.Where(p => p.RbName.Equals(ds.Tables[0].Rows[i]["LcBrandId"].ToString(), StringComparison.OrdinalIgnoreCase)).Count() > 0)
                        //////        {
                        //////            model.LcBrandId = BrandType.FirstOrDefault().Id;
                        //////            //model.LcActivityTypeId = BrandType.FirstOrDefault().Id;
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Incorrect Input in Brand Type";
                        //////        }
                        //////    }
                        //////    //else SS commented because its not manadatory in DB
                        //////    //{
                        //////    //    IsUploadedDataValid = false;
                        //////    //    ErrorList = ErrorList + " | Brand is mandatory";
                        //////    //}

                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcConnectionDate"].ToString()))
                        //////    {
                        //////        var Dates = ds.Tables[0].Rows[i]["LcConnectionDate"].ToString();
                        //////        model.LcConnectionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["LcConnectionDate"].ToString());
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcOrderDate"].ToString()))
                        //////    {
                        //////        model.LcOrderDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["LcOrderDate"].ToString());
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcMSISDN"].ToString()))
                        //////    {
                        //////        model.LcMSISDN = (ds.Tables[0].Rows[i]["LcMSISDN"].ToString()).Trim();
                        //////    }
                        //////    //else  SS commented because its not manadatory in DB
                        //////    //{
                        //////    //    IsUploadedDataValid = false;
                        //////    //    ErrorList = ErrorList + " | MSISDN is mandatory";
                        //////    //}
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcBAN"].ToString()))
                        //////    {
                        //////        model.LcBAN = ds.Tables[0].Rows[i]["LcBAN"].ToString().Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcOrderNumber"].ToString()))
                        //////    {
                        //////        model.LcOrderNumber = (ds.Tables[0].Rows[i]["LcOrderNumber"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcCustomerName"].ToString()))
                        //////    {
                        //////        model.LcCustomerName = (ds.Tables[0].Rows[i]["LcCustomerName"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcProductCodeId"].ToString()))
                        //////    {
                        //////        if (Productcode.Where(p => p.RpcProductCode.Equals(ds.Tables[0].Rows[i]["LcProductCodeId"].ToString(), StringComparison.OrdinalIgnoreCase)).Count() > 0)
                        //////        {
                        //////           model.LcProductCodeId = Productcode.FirstOrDefault().Id;//converted to string for timebeing.
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Incorrect Input in Product Code";
                        //////        }
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcExpectedCommissionAmount"].ToString()))
                        //////    {
                        //////        try
                        //////        {
                        //////            model.LcExpectedCommissionAmount = Convert.ToDecimal(ds.Tables[0].Rows[i]["LcExpectedCommissionAmount"].ToString());
                        //////        }
                        //////        catch (Exception)
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Invalid value for Commission Amount";
                        //////        }
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcActivityTypeId"].ToString()))
                        //////    {
                        //////        if (ActivityType.Where(p => p.RatName.Equals(ds.Tables[0].Rows[i]["LcActivityTypeId"].ToString())).Count() > 0)
                        //////        {
                        //////            model.LcActivityTypeId = ActivityType.FirstOrDefault().Id;
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Incorrect Input in Activity Type";
                        //////        }
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcCommissionTypeId"].ToString()))
                        //////    {
                        //////        if (CommissionType.Where(p => p.RctName.Equals(ds.Tables[0].Rows[i]["LcCommissionTypeId"].ToString(), StringComparison.OrdinalIgnoreCase)).Count() > 0)
                        //////        {
                        //////            model.LcActivityTypeId = CommissionType.FirstOrDefault().Id;
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Incorrect Input in Commission Type";

                        //////        }
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcIMEI"].ToString()))
                        //////    {
                        //////        model.LcIMEI = (ds.Tables[0].Rows[i]["LcIMEI"].ToString()).Trim();
                        //////    }
                        //////    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcDeviceTypeId"].ToString()))
                        //////    {

                        //////        if (DeviceType.Where(p => p.RdtName.Equals(ds.Tables[0].Rows[i]["LcDeviceTypeId"].ToString(), StringComparison.OrdinalIgnoreCase)).Count() > 0)
                        //////        {
                        //////            model.LcDeviceTypeId = DeviceType.FirstOrDefault().Id;
                        //////        }
                        //////        else
                        //////        {
                        //////            IsUploadedDataValid = false;
                        //////            ErrorList = ErrorList + " | Incorrect Input in Device Type";

                        //////        }
                        //////    }
                        //////    model.LcIsReclaim = false;
                        //////    if (ds.Tables[0].Columns.Contains("LcIsReclaim"))//check if column exist
                        //////    {
                        //////        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcIsReclaim"].ToString()))
                        //////        {
                        //////            if (ds.Tables[0].Rows[i]["LcIsReclaim"].ToString() == "Y")
                        //////            {
                        //////                model.LcIsReclaim = true;
                        //////            }
                        //////        }
                        //////    }
                        //////    if (ds.Tables[0].Columns.Contains("WFComments"))
                        //////    {
                        //////        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["WFComments"].ToString()))
                        //////        {
                        //////            model.WFComments = (ds.Tables[0].Rows[i]["WFComments"].ToString()).Trim();
                        //////        }
                        //////    }

                        //////    #region Not Defined in Template. So Fileds value are not getting passed to API
                        //////    //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcClawbackAmount"].ToString()))
                        //////    //{
                        //////    //    model.LcClawbackAmount = Convert.ToInt32((ds.Tables[0].Rows[i]["LcClawbackAmount"].ToString()).Trim());
                        //////    //}
                        //////    //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcExpectedCommissionAmount"].ToString()))
                        //////    //{
                        //////    //    model.LcExpectedCommissionAmount = Convert.ToInt32(ds.Tables[0].Rows[i]["LcExpectedCommissionAmount"].ToString());
                        //////    //}
                        //////    //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcParentName"].ToString()))
                        //////    //{
                        //////    //    model.LcParentName = (ds.Tables[0].Rows[i]["LcParentName"].ToString()).Trim();
                        //////    //}
                        //////    //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcPayeeName"].ToString()))
                        //////    //{
                        //////    //    model.LcPayeeName = ds.Tables[0].Rows[i]["LcPayeeName"].ToString();
                        //////    //}
                        //////    //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LcReasonNonAutoPayment"].ToString()))
                        //////    //{

                        //////    //    model.LcReasonNonAutoPayment = ds.Tables[0].Rows[i]["LcReasonNonAutoPayment"].ToString();

                        //////    //}
                        //////    #endregion

                        //////    model.LcCreatedById = LoggedInUserId;
                        //////    model.LcCompanyId = CompanyId;
                        //////    model.WFCurrentOwnerId = LoggedInUserId;
                        //////    model.WFRequesterId = LoggedInUserId;
                        //////    model.WFRequesterRoleId = LoggedInRoleId;
                        //////    model.WFStatus = "Saved";
                        //////    model.LcCreatedDateTime = DateTime.UtcNow;
                        //////    //RK
                        //////    model.LcClaimId = 0;
                        //////    if (!ValidationSpecialChar(model))
                        //////    {
                        //////        IsUploadedDataValid = false;
                        //////        ErrorList = ErrorList + " | Special character not allowed.";
                        //////    }
                        //////    if (ErrorList != "" && ErrorList.Length > 2) model.ErrorMessage = ErrorList.Substring(2, ErrorList.Length - 2);
                        //////    //If no error is found so far row model is added in model list otherwise in ErroredPayeeList
                        //////    if (string.IsNullOrEmpty(ErrorList))
                        //////    {
                        //////        ModelList.Add(model);
                        //////    }
                        //////    else
                        //////    {
                        //////        ErroredList.Add(model);
                        //////    }

                        //////}
                        #endregion
                        //If no errors found in all of the rows then save the data in database as valid claims otherwise display bad rows to user for correction and reupload
                        if (IsUploadedDataValid)
                        {
                            //LoggedInRoleId,"Claims"
                            //var result = RestClient.UploadClaims(ModelList, FileNames, LoggedInRoleId, Workflow,null);
                            var result = RestClient.ValidateUploadClaims(ModelList, ManipulatedFileName, LoggedInRoleId, Workflow, null,CompanyId);
                            TempData["ClaimsModelList"] = result;
                            if(result != null)
                            {
                                if (result.Rows[0][0].ToString().Contains("Invalid Date Values"))
                                {
                                    TempData["Message"] = "Invalid Date Values found in File, Please supply date values in dd/MM/yyyy format and upload file again.";
                                    return View();
                                }
                                else if (result.Rows[0][0].ToString().Contains("Invalid Template File"))
                                {
                                    TempData["Message"] = "Invalid Template File, Please download fresh template and use the same for uploading claims.";
                                    return View();
                                }
                                else
                                {
                                    Globals.ExportFromDataTable(CompanyCode, LoggedInUserName, "ClaimsUploadError", (DataTable)result);
                                    var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/ClaimsUploadError.xlsx";
                                    DirectoryInfo dir = new DirectoryInfo(FilePath);
                                    dir.Refresh();
                                    if (System.IO.File.Exists(FilePath))
                                    {
                                        TempData["Message"] = "Record could not be validated, please refer to the downloaded excel file.";
                                        TempData["ErrorFilePath"] = FilePath;
                                        //return File(FilePath, "application/octet-stream", "ClaimsUploadError.xlsx");//application/vnd.openxmlformats-officedocument.spreadsheetml.shee)t
                                        return View();
                                    }
                                    else
                                    {
                                        TempData["Message"] = "Record could not be validated.";
                                        return View();
                                    }
                                }
                            }
                            else
                            {
                                TempData["Message"] = "Record has been validated sucessfully. Please press upload button to create your validated claims";
                                TempData["ClaimsModelList"] = "Valid Data";
                                //excelConnection1.Dispose();
                                //excelConnection.Dispose();
                                //return RedirectToAction("Index", "GenericGrid");
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Record could not be validated";
                            TempData["ClaimsModelList"] = ErroredList;
                            //excelConnection1.Dispose();
                            //excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            return View();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    default:
                        throw ex;
                }
            }
            return View();
        }
        public ActionResult DownloadErrorFile()
        {
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/ClaimsUploadError.xlsx";
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();
            if (System.IO.File.Exists(FilePath))
            {
                return File(FilePath, "application/octet-stream", "ClaimsUploadError.xlsx");//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            }
            else
            {
                return null;
            }
        }
        //[HttpPost]
        public string UploadValidClaims()
        {
            try
            {
                RestClient.UploadClaims(LoggedInRoleId, Workflow, null, CompanyId);
                TempData["Message"] = "Claims uploaded successfully.";
                //return RedirectToAction("Index","GenericGrid" , new { WorkFlow = "Claims" });
                return "Success";
            }
            catch (Exception)
            {
                TempData["Message"] = "Record could not be uploaded";
                //return View();
                return "Error";
            }
            
        }
        //This method is used to obtain errored grid data to displayed to user for correction if claims upload fails
        [ControllerActionFilter]
        public JsonResult PostUploadClaimsHierarchyGrid()
        {
            var ApiData = TempData["ClaimsModelList"] as DataTable;// as List<LClaimViewModel>;
            TempData["ClaimsModelList"] = ApiData;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //This method is defined to attach files with Claim while creation
        [ControllerActionFilter]
        public AttachedFilesViewModel AttachSupportingDocs(HttpPostedFileBase[] File1, string PayeeCode)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            string ClaimFileName = "";
            string PayeeUserFriendlyFileName = "";
            var CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            /*OLD: Claims : S:\<opco>\Claims\Documents\Attached
                     NEW: Claims : S:\<opco>\Claims\<claim id>\SupportingDocuments*/
            string filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/Claims/" + PayeeCode + "/SupportingDocuments");

            foreach (HttpPostedFileBase file in File1)
            {
                if (file.ContentLength > 0)
                {

                    var fileLocation = "";
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;


                    fileLocation = filePath + "/" + FileNames;
                    //check if directory exists or not. iIf notcreate that directory
                    bool exists = System.IO.Directory.Exists(filePath);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    file.SaveAs(fileLocation);

                    //made a comma seperated list of user friendly file names
                    if (string.IsNullOrEmpty(PayeeUserFriendlyFileName))
                    {
                        PayeeUserFriendlyFileName = name + fileExtension;
                    }
                    else
                    {
                        PayeeUserFriendlyFileName = PayeeUserFriendlyFileName + "," + name + fileExtension;
                    }
                    //made a comma seperated list of  file names
                    if (string.IsNullOrEmpty(ClaimFileName))
                    {
                        ClaimFileName = FileNames;
                    }
                    else
                    {
                        ClaimFileName = ClaimFileName + "," + FileNames;
                    }
                }
            }

            return new AttachedFilesViewModel { FileName = ClaimFileName, FilePath = filePath };
        }

        //Thismethod is defined to return view to display the claims for the selected filters//
        [ControllerActionFilter]
        public ActionResult ClaimsReport()
        {
            //passing data of LCompanySpecificColumns data to the view to display the column headers in Grid
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            ViewBag.CompanySpecificData = LCSCRC.GetClaimsColumnsByCompanyId(CompanyId).Where(p => p.LcscDisplayOnForm == true).OrderBy(p => p.LcscOrdinalPosition).ToList();
            ViewBag.SelectionMode = "checkbox";//used int the partial view to decide whether payee grid will have single selection or multiple
            //ViewBag.LcPayeeId = Globals.GetPayeeCode();
            System.Web.HttpContext.Current.Session["Title"] = "Manage Claims";
            return View();
        }

        //This method will get ClaimsData from Api to display in grid
        [ControllerActionFilter]
        public JsonResult GetClaimsReports(string PayeeId, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate, //int pagesize, int pagenum, string sortdatafield, string sortorder
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, Nullable<DateTime> AllocationDateFrom,
            Nullable<DateTime> AllocationDateTo, string AlreadyPaidDateFilter, Nullable<DateTime> AlreadyPaidDateFrom, Nullable<DateTime> AlreadyPaidDateTo, string ConnectionDateFilter,
            Nullable<DateTime> ConnectionDateFrom, Nullable<DateTime> ConnectionDateTo, string LastReclaimDateFilter, Nullable<DateTime> LastReclaimDateFrom,
            Nullable<DateTime> LastReclaimDateTo, string OrderDateFilter, Nullable<DateTime> OrderDateFrom, Nullable<DateTime> OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo,
            string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer, string RejectionReasonIds
            )
        {
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(LcPayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    LcPayeeId = string.Join(",", PayeeList);
            //}
            if (EndDate != null)
            {
                DateTime EndDate1 = Convert.ToDateTime(EndDate);
                EndDate1 = EndDate1.AddDays(1);
                EndDate = EndDate1;
            }
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            var AllocationDtFrom = (AllocationDateFrom.HasValue) ? AllocationDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var AllocationDtTo = (AllocationDateTo.HasValue) ? AllocationDateTo.Value.ToString("yyyy-MM-dd") : null;
            var AlreadyPaidDtFrom = (AlreadyPaidDateFrom.HasValue) ? AlreadyPaidDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var AlreadyPaidDtTo = (AlreadyPaidDateTo.HasValue) ? AlreadyPaidDateTo.Value.ToString("yyyy-MM-dd") : null;
            var ConnectionDtFrom = (ConnectionDateFrom.HasValue) ? ConnectionDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var ConnectionDtTo = (ConnectionDateTo.HasValue) ? ConnectionDateTo.Value.ToString("yyyy-MM-dd") : null;
            var LastReclaimDtFrom = (LastReclaimDateFrom.HasValue) ? LastReclaimDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var LastReclaimDtTo = (LastReclaimDateTo.HasValue) ? LastReclaimDateTo.Value.ToString("yyyy-MM-dd") : null;
            var OrderDtFrom = (OrderDateFrom.HasValue) ? OrderDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var OrderDtTo = (OrderDateTo.HasValue) ? OrderDateTo.Value.ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (AlreadyPaidAmountFrom == null || AlreadyPaidAmountFrom.ToLower() == "undefined") AlreadyPaidAmountFrom = "";
                if (AlreadyPaidAmountTo == null || AlreadyPaidAmountTo.ToLower() == "undefined") AlreadyPaidAmountTo = "";
                if (ClawbackAmountFrom == null || ClawbackAmountFrom.ToLower() == "undefined") ClawbackAmountFrom = "";
                if (ClawbackAmountTo == null || ClawbackAmountTo.ToLower() == "undefined") ClawbackAmountTo = "";
                if (ClawbackAmountTo == null || ClawbackAmountTo.ToLower() == "undefined") ClawbackAmountTo = "";
                if (ExpectedCommissionAmountFrom == null || ExpectedCommissionAmountFrom.ToLower() == "undefined") ExpectedCommissionAmountFrom = "";
                if (ExpectedCommissionAmountTo == null || ExpectedCommissionAmountTo.ToLower() == "undefined") ExpectedCommissionAmountTo = "";
                if (PaymentAmountFrom == null || PaymentAmountFrom.ToLower() == "undefined") PaymentAmountFrom = "";
                if (PaymentAmountTo == null || PaymentAmountTo.ToLower() == "undefined") PaymentAmountTo = "";
                var Claims = RestClient.GetClaimsForReports(PayeeId, CompanyId, StartDatetime, EndDatetime, //pagesize, pagenum, sortdatafield, sortorder, FilterQuery,
                     A01Filter, A01, A02Filter, A02, A03Filter, A03, A04Filter, A04, A05Filter, A05, A06Filter, A06, A07Filter, A07, A08Filter, A08, A09Filter, A09, A10Filter, A10,
                     AllocationDateFilter, AllocationDtFrom, AllocationDtTo, AlreadyPaidDateFilter, AlreadyPaidDtFrom, AlreadyPaidDtTo, ConnectionDateFilter,
                     ConnectionDtFrom, ConnectionDtTo, LastReclaimDateFilter, LastReclaimDtFrom, LastReclaimDtTo, OrderDateFilter, OrderDtFrom, OrderDtTo, AlreadyPaidAmountFilter,
                     AlreadyPaidAmountFrom, AlreadyPaidAmountTo, ClawbackAmountFilter, ClawbackAmountFrom, ClawbackAmountTo, ExpectedCommissionAmountFilter, ExpectedCommissionAmountFrom, ExpectedCommissionAmountTo, PaymentAmountFilter,
                     PaymentAmountFrom, PaymentAmountTo, BANFilter, BAN, CustomerNameFilter, CustomerName,
                     IMEIFilter, IMEI, MSISDNFilter, MSISDN, OrderNumberFilter, OrderNumber, PaymentBatchNumberFilter,
                     PaymentBatchNumber, ReasonNonAutoPaymentFilter, ReasonNonAutoPayment, ClaimBatchNumberFilter, ClaimBatchNumber,
                     ClawbackPayeeCodeFilter, ClawbackPayeeCode, BrandIds, CommissionTypeIds, DeviceTypeIds, PaymentCommissionTypeIds,
                     ProductCodeIds, StatusFilter, Status, CreatedByIds, ActivityTypeIds, AlreadyPaidDealer, RejectionReasonIds);
                return Json(Claims, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);



        }
        [ControllerActionFilter]
        //This method addedd By Ritu from Api to count grid data
        public JsonResult GetClaimsReportscounts(string PayeeId, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
        {
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(LcPayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    LcPayeeId = string.Join(",", PayeeList);
            //}
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            var Claims = RestClient.GetClaimsForReportCounts(PayeeId, CompanyId, StartDatetime, EndDatetime);
            return Json(Claims, JsonRequestBehavior.AllowGet);

        }
        //This method will download claims report in csv format
        [ControllerActionFilter]
        public ActionResult DownloadClaimsReport(string PayeeId, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate, //int pagesize, int pagenum, string sortdatafield, string sortorder
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, Nullable<DateTime> AllocationDateFrom,
            Nullable<DateTime> AllocationDateTo, string AlreadyPaidDateFilter, Nullable<DateTime> AlreadyPaidDateFrom, Nullable<DateTime> AlreadyPaidDateTo, string ConnectionDateFilter,
            Nullable<DateTime> ConnectionDateFrom, Nullable<DateTime> ConnectionDateTo, string LastReclaimDateFilter, Nullable<DateTime> LastReclaimDateFrom,
            Nullable<DateTime> LastReclaimDateTo, string OrderDateFilter, Nullable<DateTime> OrderDateFrom, Nullable<DateTime> OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo,
            string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer, string RejectionReasonIds)
        {
            //if Payee is not selected then get All Payees from Dropdown
            #region old code commented by RK
            //Old PArams //string LcPayeeId, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
            //if (string.IsNullOrEmpty(LcPayeeId))
            //{
            //    var PayeeList = Globals.GetPayeeList(string.Empty).Select(p => p.Id);
            //    LcPayeeId = string.Join(",", PayeeList);
            //}
            //var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            //var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            //var FileName = RestClient.DownloadClaimsForReports(LcPayeeId, CompanyId, StartDatetime, EndDatetime,LoggedInUserName);
            //var FilesPath = ConfigurationManager.AppSettings["ExportCalcDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + FileName;
            ////NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
            //Thread.Sleep(10000);
            //var FilePath = ConfigurationManager.AppSettings["ExportCalcDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName;
            //DirectoryInfo dir = new DirectoryInfo(FilePath);
            //dir.Refresh();
            #endregion
            //if (string.IsNullOrEmpty(LcPayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //   var PayeeList = PayeeData.Select(p => p.Id);
            //    LcPayeeId = string.Join(",", PayeeList);
            //}
            if (EndDate != null)
            {
                DateTime EndDate1 = Convert.ToDateTime(EndDate);
                EndDate1 = EndDate1.AddDays(1);
                EndDate = EndDate1;
            }
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            var AllocationDtFrom = (AllocationDateFrom.HasValue) ? AllocationDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var AllocationDtTo = (AllocationDateTo.HasValue) ? AllocationDateTo.Value.ToString("yyyy-MM-dd") : null;
            var AlreadyPaidDtFrom = (AlreadyPaidDateFrom.HasValue) ? AlreadyPaidDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var AlreadyPaidDtTo = (AlreadyPaidDateTo.HasValue) ? AlreadyPaidDateTo.Value.ToString("yyyy-MM-dd") : null;
            var ConnectionDtFrom = (ConnectionDateFrom.HasValue) ? ConnectionDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var ConnectionDtTo = (ConnectionDateTo.HasValue) ? ConnectionDateTo.Value.ToString("yyyy-MM-dd") : null;
            var LastReclaimDtFrom = (LastReclaimDateFrom.HasValue) ? LastReclaimDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var LastReclaimDtTo = (LastReclaimDateTo.HasValue) ? LastReclaimDateTo.Value.ToString("yyyy-MM-dd") : null;
            var OrderDtFrom = (OrderDateFrom.HasValue) ? OrderDateFrom.Value.ToString("yyyy-MM-dd") : null;
            var OrderDtTo = (OrderDateTo.HasValue) ? OrderDateTo.Value.ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (AlreadyPaidAmountFrom == null || AlreadyPaidAmountFrom.ToLower() == "undefined") AlreadyPaidAmountFrom = "";
                if (AlreadyPaidAmountTo == null || AlreadyPaidAmountTo.ToLower() == "undefined") AlreadyPaidAmountTo = "";
                if (ClawbackAmountFrom == null || ClawbackAmountFrom.ToLower() == "undefined") ClawbackAmountFrom = "";
                if (ClawbackAmountTo == null || ClawbackAmountTo.ToLower() == "undefined") ClawbackAmountTo = "";
                if (ClawbackAmountTo == null || ClawbackAmountTo.ToLower() == "undefined") ClawbackAmountTo = "";
                if (ExpectedCommissionAmountFrom == null || ExpectedCommissionAmountFrom.ToLower() == "undefined") ExpectedCommissionAmountFrom = "";
                if (ExpectedCommissionAmountTo == null || ExpectedCommissionAmountTo.ToLower() == "undefined") ExpectedCommissionAmountTo = "";
                if (PaymentAmountFrom == null || PaymentAmountFrom.ToLower() == "undefined") PaymentAmountFrom = "";
                if (PaymentAmountTo == null || PaymentAmountTo.ToLower() == "undefined") PaymentAmountTo = "";
                var FileName = RestClient.DownloadClaimsForReports(PayeeId, CompanyId, StartDatetime, EndDatetime,
                     A01Filter, A01, A02Filter, A02, A03Filter, A03, A04Filter, A04, A05Filter, A05, A06Filter, A06, A07Filter, A07, A08Filter, A08, A09Filter, A09, A10Filter, A10,
                     AllocationDateFilter, AllocationDtFrom, AllocationDtTo, AlreadyPaidDateFilter, AlreadyPaidDtFrom, AlreadyPaidDtTo, ConnectionDateFilter,
                     ConnectionDtFrom, ConnectionDtTo, LastReclaimDateFilter, LastReclaimDtFrom, LastReclaimDtTo, OrderDateFilter, OrderDtFrom, OrderDtTo, AlreadyPaidAmountFilter,
                     AlreadyPaidAmountFrom, AlreadyPaidAmountTo, ClawbackAmountFilter, ClawbackAmountFrom, ClawbackAmountTo, ExpectedCommissionAmountFilter, ExpectedCommissionAmountFrom, ExpectedCommissionAmountTo, PaymentAmountFilter,
                     PaymentAmountFrom, PaymentAmountTo, BANFilter, BAN, CustomerNameFilter, CustomerName,
                     IMEIFilter, IMEI, MSISDNFilter, MSISDN, OrderNumberFilter, OrderNumber, PaymentBatchNumberFilter,
                     PaymentBatchNumber, ReasonNonAutoPaymentFilter, ReasonNonAutoPayment, ClaimBatchNumberFilter, ClaimBatchNumber,
                     ClawbackPayeeCodeFilter, ClawbackPayeeCode, BrandIds, CommissionTypeIds, DeviceTypeIds, PaymentCommissionTypeIds,
                     ProductCodeIds, StatusFilter, Status, CreatedByIds, ActivityTypeIds, AlreadyPaidDealer, RejectionReasonIds);

                var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + FileName;
                //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
                Thread.Sleep(10000);
                var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                DirectoryInfo dir = new DirectoryInfo(FilePath);
                dir.Refresh();
                if (System.IO.File.Exists(FilesPath))
                {
                    return File(FilesPath, "application/octet-stream", FileName);//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                }
                TempData["Error"] = "No Data Found";
                return RedirectToAction("ClaimsReport");
            }
            else
            {
                TempData["Error"] = "No Data Found";
                return RedirectToAction("ClaimsReport");
            }

            #region Commented
            //var CompanySpecificColumns = LCSC.GetClaimsColumnsByCompanyId(CompanyId);
            //var CfileLocation = ConfigurationManager.AppSettings["ClaimsDocumentPath"] + "/ExportClaimsReport.csv";
            //var FilesPath = ConfigurationManager.AppSettings["ClaimsDocumentPath"] + "/ExportClaimsReport.csv";
            //if (System.IO.File.Exists(CfileLocation))
            //    System.IO.File.Delete(CfileLocation);
            //using (var CTextWriter = new StreamWriter(CfileLocation))
            //using (var Ccsv = new CsvWriter(CTextWriter))
            //{

            //    //The below lines of code converts the data returned from api to a datatable
            //    var tb = new DataTable(typeof(DownloadClaimsGridViewModel).Name);

            //    PropertyInfo[] props = typeof(DownloadClaimsGridViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //    // LPayeeViewModel PayeeModel = new LPayeeViewModel();
            //    foreach (var prop in props)
            //    {
            //        // var displayName=PayeeModel.GetDisplayName()
            //        if (CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).Where(p => string.IsNullOrEmpty(p.LcscLabel) == false).Count() > 0)
            //        {
            //            tb.Columns.Add(CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).FirstOrDefault().LcscLabel);
            //        }
            //        else
            //        {
            //            tb.Columns.Add(prop.Name);
            //        }
            //    }

            //    foreach (var item in ApiData)
            //    {
            //        var values = new object[props.Length];
            //        for (var i = 0; i < props.Length; i++)
            //        {
            //            values[i] = props[i].GetValue(item, null);
            //        }

            //        tb.Rows.Add(values);
            //    }


            //    foreach (DataColumn column in tb.Columns)
            //    {
            //        Ccsv.WriteField(column.ColumnName);
            //    }
            //    Ccsv.NextRecord();

            //    foreach (DataRow row in tb.Rows)
            //    {
            //        for (var i = 0; i < tb.Columns.Count; i++)
            //        {
            //            Ccsv.WriteField(row[i]);
            //        }
            //        Ccsv.NextRecord();
            //    }
            //}
            #endregion

        }


        System.Text.RegularExpressions.Match stringMatch;
        private Boolean ValidationSpecialChar(LClaimViewModel model)
        {
            if (!string.IsNullOrEmpty(model.A01)) { stringMatch = Regex.Match(model.A01, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A02)) { stringMatch = Regex.Match(model.A02, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A03)) { stringMatch = Regex.Match(model.A03, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A04)) { stringMatch = Regex.Match(model.A04, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A05)) { stringMatch = Regex.Match(model.A05, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A06)) { stringMatch = Regex.Match(model.A06, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A07)) { stringMatch = Regex.Match(model.A07, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A08)) { stringMatch = Regex.Match(model.A08, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A09)) { stringMatch = Regex.Match(model.A09, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A10)) { stringMatch = Regex.Match(model.A10, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            // if (!string.IsNullOrEmpty(model.LcPayeeCode)) { stringMatch = Regex.Match(model.LcPayeeCode, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LcMSISDN)) { stringMatch = Regex.Match(model.LcMSISDN, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LcBAN)) { stringMatch = Regex.Match(model.LcBAN, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LcOrderNumber)) { stringMatch = Regex.Match(model.LcOrderNumber, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LcCustomerName)) { stringMatch = Regex.Match(model.LcCustomerName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            //if (!string.IsNullOrEmpty(model.LcProductCode)) { stringMatch = Regex.Match(model.LcProductCode, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            //if (!string.IsNullOrEmpty(model.LcExpectedCommissionAmount)) { stringMatch = Regex.Match(model.LcExpectedCommissionAmount, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LcIMEI)) { stringMatch = Regex.Match(model.LcIMEI, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.WFComments)) { stringMatch = Regex.Match(model.WFComments, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }

            return true;
        }
        private Boolean blnCheckTemplateColumns(DataTable dt)
        {
            //if (!dt.Columns.Contains("LcPayeeCode")) return false; this column has been removed from lclaims
            if (!dt.Columns.Contains("LcBrandId")) return false;
            if (!dt.Columns.Contains("LcConnectionDate")) return false;
            if (!dt.Columns.Contains("LcOrderDate")) return false;
            if (!dt.Columns.Contains("LcMSISDN")) return false;
            if (!dt.Columns.Contains("LcBAN")) return false;
            if (!dt.Columns.Contains("LcOrderNumber")) return false;
            if (!dt.Columns.Contains("LcCustomerName")) return false;
            //if (!dt.Columns.Contains("LcProductCode")) return false;
            if (!dt.Columns.Contains("LcExpectedCommissionAmount")) return false;
            if (!dt.Columns.Contains("LcActivityTypeId")) return false;
            if (!dt.Columns.Contains("LcCommissionTypeId")) return false;
            if (!dt.Columns.Contains("LcIMEI")) return false;
            if (!dt.Columns.Contains("LcDeviceTypeId")) return false;
            // if (!dt.Columns.Contains("WFComments")) return false;
            //if (!dt.Columns.Contains("LcIsReclaim")) return false;

            if (!dt.Columns.Contains("A01")) return false;
            if (!dt.Columns.Contains("A02")) return false;
            if (!dt.Columns.Contains("A03")) return false;
            if (!dt.Columns.Contains("A04")) return false;
            if (!dt.Columns.Contains("A05")) return false;
            if (!dt.Columns.Contains("A06")) return false;
            if (!dt.Columns.Contains("A07")) return false;
            if (!dt.Columns.Contains("A08")) return false;
            if (!dt.Columns.Contains("A09")) return false;
            if (!dt.Columns.Contains("A10")) return false;
            return true;
        }


        //method to get dropdown values by passing dropdown id as parameter
        //[ControllerActionFilter]
        public JsonResult GetDropDownValue(int DropdownId, int TransactionId, string ColumnName, string FormType)
        {
            ILDropDownValuesRestClient LDDVRC = new LDropDownValuesRestClient();
            var SelectedValue = "";
            switch (FormType)
            {
                case "Payees":
                    var ApiData = LPRC.GetById(TransactionId);
                    SelectedValue = GetSelectedDropDownValue(ApiData, ColumnName);
                    break;
                case "Claims":
                    var ApiData1 = RestClient.GetById(TransactionId);
                    SelectedValue = GetSelectedDropDownValue(ApiData1, ColumnName);
                    break;
            }
            if (SelectedValue == null)
            {
                SelectedValue = string.Empty;
            }
            var LDropDownValues = LDDVRC.GetByDropDownId(DropdownId).Select(p => new { p.Id, p.LdvDescription, p.LdvValue, SelectedValue = SelectedValue });
            return Json(LDropDownValues, JsonRequestBehavior.AllowGet);
        }

        //Get slected dropdownvalue from model
        private string GetSelectedDropDownValue(dynamic Model, string ColumnName)
        {
            switch (ColumnName)
            {
                case "A01":
                    return Model.A01;
                case "A02":
                    return Model.A02;
                case "A03":
                    return Model.A03;
                case "A04":
                    return Model.A04;
                case "A05":
                    return Model.A05;
                case "A06":
                    return Model.A06;
                case "A07":
                    return Model.A07;
                case "A08":
                    return Model.A08;
                case "A09":
                    return Model.A09;
                case "A10":
                    return Model.A10;
                default:
                    return string.Empty;
            }
        }

        //Method to get Payee List for Claims reports
        //[ControllerActionFilter]
        //public JsonResult GetPayeeList()
        //{
        //    var ApiData = Globals.GetPayeeList(string.Empty,false);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetBrandsList()
        {
            RBrandsRestClient RRC = new RBrandsRestClient();
            var ApiData = RRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommissionTypeList()
        {
            RCommissionTypesRestClient RCTRC = new RCommissionTypesRestClient();
            var ApiData = RCTRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeviceTypeList()
        {
            RDeviceTypesRestClient RDTRC = new RDeviceTypesRestClient();
            var ApiData = RDTRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentCommissionTypeList()
        {
            RCommissionTypesRestClient CTRC = new RCommissionTypesRestClient();
            var ApiData = CTRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductCodeList()
        {
            RProductCodesRestClient PCRC = new RProductCodesRestClient();
            var ApiData = PCRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActivityTypeList()
        {
            RActiveTypesRestClient ATRC = new RActiveTypesRestClient();
            var ApiData = ATRC.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCreatedByList()
        {
            AspnetUsersRestClient AURC = new AspnetUsersRestClient();
            var ApiData = AURC.GetUserByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRejectionReasonList()
        {
            IRRejectionReasonsRestClient RRRC = new RRejectionReasonsRestClient();
            var ApiData = RRRC.GetDropDownDataByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);

        }
        [ControllerActionFilter]
        public ActionResult GetMyClaimsReport()
        {
            
            ILClaimsRestClient ILC = new LClaimsRestClient();
            var FileName = ILC.GetMyClaimsReport(LoggedInUserId, CompanyId, CompanyCode, LoggedInUserName);
            Thread.Sleep(3000);
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            //if (System.IO.File.Exists(FileName))
            //{
            //    return File(FileName, "application/pdf", "MyClaims.zip");
            //}
            var ByteData1 = Globals.DownloadFromS3("MyClaimsReport.xlsx", CompanyCode + "/" + LoggedInUserName+"/");
            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", "MyClaimsReport.xlsx");
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            
            
        }

        [ControllerActionFilter]
        public ActionResult GetMyClaimsAuditData()
        {

            ILClaimsRestClient ILC = new LClaimsRestClient();
            var FileName = ILC.GetMyClaimsAuditData(LoggedInUserId, CompanyId, CompanyCode, LoggedInUserName);
            Thread.Sleep(3000);
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            var ByteData1 = Globals.DownloadFromS3("MyClaimsAuditReport.xlsx", CompanyCode + "/" + LoggedInUserName + "/");
            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", "MyClaimsAuditReport.xlsx");
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }

        public ActionResult GetPayeesChannels(int PayeeID)
        {

            ILClaimsRestClient ILC = new LClaimsRestClient();
            var ApiData = ILC.GetPayeesChannels(PayeeID, CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UploadAutoAttachment(string id, string PayeeCode, string Type)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();
            GenericGridRestClient RestClient = new GenericGridRestClient();
            ILPayeesRestClient LPARC = new LPayeesRestClient();
            var PayeeDetails = LPARC.GetById(Convert.ToInt32(PayeeCode));

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel PayeeFiles = AttachPayeeFilesOneByOne(hpf, PayeeDetails.LpPayeeCode);

                    if (string.IsNullOrEmpty(FileDetails.FileName))
                    {
                        FileDetails.FileName = PayeeFiles.FileName;
                    }
                    else
                    {
                        FileDetails.FileName = FileDetails.FileName + "," + PayeeFiles.FileName;
                    }

                    FileDetails.FilePath = PayeeFiles.FilePath;
                }

                string data = RestClient.UpdateAttachment(Convert.ToInt32(id), LoggedInUserId, FileDetails.FileName, FileDetails.FilePath, Type);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            //   return Json("File uploaded successfully");
        }

        public AttachedFilesViewModel AttachPayeeFilesOneByOne(HttpPostedFileBase File1, string PayeeCode)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();


            var fileLocation = "";
            string fileExtension = System.IO.Path.GetExtension(File1.FileName);
            string name = System.IO.Path.GetFileNameWithoutExtension(File1.FileName);
            string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;

            var filePath = "";
             filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/Claims/" + PayeeCode + "/SupportingDocuments");

            fileLocation = filePath + "/" + FileNames;
            //check if directory exists or not. iIf notcreate that directory
            bool exists = System.IO.Directory.Exists(filePath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            File1.SaveAs(fileLocation);

            return new AttachedFilesViewModel { FileName = FileNames, FilePath = filePath };
        }

    }
}
