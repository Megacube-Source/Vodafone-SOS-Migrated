//Code Review for this file (from security perspective) done
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
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
    public class LPayController : PrimaryController
    {
        ILPayRestClient RestClient = new LPayRestClient();
        ILBatchesRestClient LBRC = new LBatchesRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string LoggedInRoleName = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        int LoggedInLUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);

        private SelectList GetStatus()
        {
            string[] Status = {  "PayPublished"};
            var x = new SelectList(Status, "PayPublished");
            return x;
        }

        [ControllerActionFilter]
        public ActionResult Index(int TransactionId,int WFConfigId)
        {
            //Get ActionItems to be displayed 
            var BatchDetails = LBRC.GetById(TransactionId);
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId,LoggedInRoleId,LoggedInUserId,BatchDetails.Id);

            //Pass the batch level coments to be displayed on the view
            ViewBag.Comment = BatchDetails.LbComments;

            //Pass SOSBatchNumber to view for further use
            ViewBag.SOSBatchNumber = BatchDetails.LbBatchNumber;

            //Pass status wise row counts to view to be displayed on tab headers
            ViewBag.PayRowCount = RestClient.GetLPayCounts(BatchDetails.LbBatchNumber,CompanyId);

            //Set the title of session
            System.Web.HttpContext.Current.Session["Title"] = "Manage Pay";

            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var PayApiData = LCSCRC.GetPayColumnsByCompanyIdForGrid(CompanyId).OrderBy(p=>p.LcscOrdinalPosition).ToList();
            //Now populate the DataType column Special case for Payulations part
            foreach (var CompanySpecificColumn in PayApiData)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            //
            ViewBag.CompSpecificColumn = PayApiData;
            return View();
        }

        [ControllerActionFilter]//RK added during code review
        public JsonResult GetPayeePayCounts(string PayeeId, string CommissionPeriodId, string TabName, string PortfolioList)
        {
            int GridCounts = 0;
            //Get Channel List if PortfolioList is null
            if (string.IsNullOrEmpty(PortfolioList))
            {
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                PortfolioList = string.Join(",", LPORC.GetByUserId(LoggedInUserId, LoggedInRoleId).Select(p => p.Id));
            }
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList, false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            GridCounts = RestClient.GetPayForPayeeCounts(PayeeId, CompanyId, CommissionPeriodId, "PayPublished", PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
            ViewBag.CPId = CommissionPeriodId;
            ViewBag.PId = PayeeId;
            return Json(GridCounts, JsonRequestBehavior.AllowGet);
        }

        //[ControllerActionFilter]
        public JsonResult GetGridSummary(string PayeeId, string CommissionPeriodId, bool BatchNochecked, bool PrimaryChannelchecked, bool CommPeriodchecked, bool Payeechecked, string Status, bool CommyTypechecked)
        {
            if (Status == null)
            {
                Status = "";
            }
           
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty, false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            IEnumerable<dynamic> ApiData = RestClient.GetSummaryForPayee(PayeeId, CompanyId, CommissionPeriodId, BatchNochecked, PrimaryChannelchecked, CommPeriodchecked, Payeechecked, Status,LoggedInRoleName,LoggedInUserId, CommyTypechecked);
            ViewBag.Status = GetStatus();
            ViewBag.CPId = CommissionPeriodId;
            ViewBag.PId = PayeeId;

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        [ControllerActionFilter]
        public JsonResult GetPayeePayReports(string PayeeId, string CommissionPeriodId, string TabName, string sortdatafield, string sortorder, int pagesize, int pagenum, string PortfolioList)
        {
            //changes for filtering,sorting,paging

            //Get Channel List if PortfolioList is null
            if (string.IsNullOrEmpty(PortfolioList))
            {
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                PortfolioList = string.Join(",", LPORC.GetByUserId(LoggedInUserId, LoggedInRoleId).Select(p => p.Id));
            }
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList, false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            IEnumerable<dynamic> ApiData = new List<dynamic>();
            ApiData = RestClient.GetPayForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "PayPublished", pagesize, pagenum, sortdatafield, sortorder, FilterQuery, PortfolioList, LoggedInRoleName, LoggedInLUserId, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommissionPeriod()
        {
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(CompanyId);
            //  var x = new SelectList(ApiData, "Id", "LcpStatus");
            //return x;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPayeeList(string PortfolioList)
        //{
        //    if (PortfolioList == null)
        //        PortfolioList = string.Empty;
        //    var ApiData = Globals.GetPayeeList(PortfolioList, false);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

         [ControllerActionFilter]
        public ActionResult PayReview(string CommissionPeriod, string PortfolioList, bool? LoadCalcGrid)
        {
            if (!LoadCalcGrid.HasValue)
                LoadCalcGrid = false;
            ViewBag.LoadCalcGrid = LoadCalcGrid;
            ViewBag.SelectionMode = "checkbox";//used int the partial view to decide whether payee grid will have single selection or multiple
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificColumns = LCSCRC.GetPayColumnsByCompanyIdForGrid(CompanyId).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in CompanySpecificColumns)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            ViewBag.CompSpecificColumn = CompanySpecificColumns;
            var ComPeriod = "";
            if (!string.IsNullOrEmpty(CommissionPeriod))
            {
                ComPeriod = CommissionPeriod;
            }
            ILBatchesRestClient BRC = new LBatchesRestClient();
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            var Batches = BRC.GetByCommPeriodIdList(ComPeriod, CompanyId, PortfolioList, LoggedInRoleId,LoggedInUserId,LoggedInRoleName).Select(p => new { p.LbBatchNumber, FullName = p.LbBatchName + " (" + p.LbBatchNumber + ")" });
            ViewBag.BatchNos = new SelectList(Batches, "LbBatchNumber", "FullName");
            ViewBag.Status = GetStatus();
            System.Web.HttpContext.Current.Session["Title"] = "Manage Payment review";
            SaveSelectedPortfolio(CommissionPeriod);
            return View();
        }

        public JsonResult SaveSelectedPortfolio(string periods)
        {
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            // var ApiData = CPRC.GetById(Convert.ToInt16(periods));
            //var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            //var jsonSerialiser = new JavaScriptSerializer();
            GGRC.SaveUserPreferenceData(LoggedInUserId, "PayReview", "Period", "FieldValue", "0", periods, ObjSession._UserSessionID);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedPortfolioFromUserPreference()
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            var ApiData = GGRC.GetUserPreferenceData(LoggedInUserId, "PayReview", "Period", "FieldValue", "0", ObjSession._UserSessionID);
            return Json(ApiData.AsEnumerable(), JsonRequestBehavior.AllowGet);
        }


        [ControllerActionFilter]
        public ActionResult DownloadPayeePayFile(string PayeeId, string CommissionPeriodId, int TabIndex, string PortfolioList)
        {
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList, false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            string FileName = "";
                    FileName = RestClient.DownloadPayForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "PayPublished", LoggedInUserName, CompanyCode, PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
            Thread.Sleep(3000);//Sleep to add delay to avoid issues in getting file from S Drive
            var CompleteFileName = ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + FileName;
            //Added By Sachin on 13/6/2018 for issue in 2.1 - Error while accessing DownloadPayeePayFile method from Lpay Controller.
            if (FileName != null || FileName != "")
            {
                if (System.IO.File.Exists(CompleteFileName))
                {
                    return File(CompleteFileName, "application/unknown", FileName);
                }
                else
                {
                    TempData["Error"] = "No File/Data found";
                    return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
                }
            }
            else
            {
                TempData["Error"] = "No File/Data found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }



        //This method will be called from View from Javascript code when Analyst submits a batch for Maager approval
        [ControllerActionFilter]
        public ActionResult UpdateBatchStatus(int SOSBatchNumber, string BatchLevelComment, string NewStatus)
        {
            //Update status in LBatches and XBatches tables. Below code will update both.
            ILBatchesRestClient LBRC = new LBatchesRestClient();
            LBRC.UpdateBatchStatus(SOSBatchNumber, BatchLevelComment, NewStatus,null);
            //TempData["Message"] = string.Format("The batch status has been successfully changed to {0}", NewStatus);Commented code as directed by JS
            return RedirectToAction("PayIndex", "LBatches");
        }

        //method to get list of Pay data on Index Page
        //Note : The variables pagesize and pagenum are default variables for jqxgrid . Do not change the naming of these otherwise paging in jqxgrid may not work.
        [ControllerActionFilter]
        public JsonResult GetLPay(int SOSBatchNumber, string Status, int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            if (sortdatafield == null)
                sortdatafield = string.Empty;
            if (sortorder == null)
            {
                sortorder = string.Empty;
            }
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetLPayForGrid(SOSBatchNumber, pagesize, pagenum,sortdatafield,sortorder,FilterQuery,CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


      

    }
}