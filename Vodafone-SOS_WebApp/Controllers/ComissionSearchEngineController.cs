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

namespace Vodafone_SOS_WebApp.Controllers
{
    public class ComissionSearchEngineController : PrimaryController
    {
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string LoggedInRoleName = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        int LoggedInLUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);

        ISearchEngineRestClient RestClient = new SearchEngineRestClient();

        [ControllerActionFilter]
        public ActionResult SearchEngine()
        {
            LClaimsController objClaims = new LClaimsController();
            ViewBag.ActivityTypeId = objClaims.GetActivityType();
            ViewBag.CommissionTypeId = objClaims.GetCommissionType();
            ViewBag.SelectionMode = "checkbox";
            return View();
        }

        public JsonResult GetSearchEngineCount(bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus,  string SelectedTab)
        {

            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiDataCount = RestClient.GetSummaryCount(CompanyId, LoggedInUserName, "", LoggedInUserId, LoggedInRoleName, ChkSubscriberNumber, SubscriberNumber, ChkCustomerSegment, CustomerSegment, ChkActivityType, ActivityType, ChkActivationOrder, ActivationOrder, ChkCommType, CommissionType, ChkChannel, Channel, ChkParentPayee, PayeeParent, ChkSubChannel, SubChannel, ChkPayee, Payees, ChkPeriod, Period, ChkBatchStatus, BatchStatus, SelectedTab);

            return Json(ApiDataCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSearchEngine(bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string sortdatafield, string sortorder, int pagesize, int pagenum, string SelectedTab)
        {
            Session["ChkSubscriberNumber"] = ChkSubscriberNumber;
            Session["SubscriberNumber"] = SubscriberNumber;
            Session["ChkCustomerSegment"] = ChkCustomerSegment;
            Session["CustomerSegment"] = CustomerSegment;
            Session["ChkActivityType"] = ChkActivityType;
            Session["ActivityType"] = ActivityType;
            Session["ChkActivationOrder"] = ChkActivationOrder;
            Session["ActivationOrder"] = ActivationOrder;
            Session["ChkCommType"] = ChkCommType;
            Session["CommissionType"] = CommissionType;
            Session["ChkChannel"] = ChkChannel;
            Session["Channel"] = Channel;
            Session["ChkParentPayee"] = ChkParentPayee;
            Session["PayeeParent"] = PayeeParent;
            Session["ChkSubChannel"] = ChkSubChannel;
            Session["SubChannel"] = SubChannel;
            Session["ChkPayee"] = ChkPayee;
            Session["Payees"] = Payees;
            Session["ChkPeriod"] = ChkPeriod;
            Session["Period"] = Period;
            Session["ChkBatchStatus"] = ChkBatchStatus;
            Session["BatchStatus"] = BatchStatus;
            Session["SelectedTab"] = SelectedTab;
            Session["Channel"] = Channel;
            

            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            TempData["FilterQuery"] = FilterQuery;
            var ApiData = RestClient.GetSummary(CompanyId, LoggedInUserName, "", LoggedInUserId, LoggedInRoleName, ChkSubscriberNumber, SubscriberNumber, ChkCustomerSegment, CustomerSegment, ChkActivityType, ActivityType, ChkActivationOrder, ActivationOrder, ChkCommType, CommissionType, ChkChannel, Channel, ChkParentPayee, PayeeParent, ChkSubChannel, SubChannel, ChkPayee, Payees, ChkPeriod, Period, ChkBatchStatus, BatchStatus, sortdatafield, sortorder, pagesize, pagenum, FilterQuery, SelectedTab);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //  public ActionResult DownloadFile(bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
        //                                    bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string SelectedTab)
        public ActionResult DownloadFile(string SelectedTab)
        {
            //  var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            // var FilterQuery = Globals.BuildQuery(qry);

            bool ChkSubscriberNumber = Session["ChkSubscriberNumber"] == null ? false : Convert.ToBoolean(Session["ChkSubscriberNumber"]);
            string SubscriberNumber = Session["SubscriberNumber"] == null ? "" : Convert.ToString(Session["SubscriberNumber"]);

            bool ChkCustomerSegment = Session["ChkCustomerSegment"] == null ? false : Convert.ToBoolean(Session["ChkCustomerSegment"]);
            string CustomerSegment = Session["CustomerSegment"] == null ? "" : Convert.ToString(Session["CustomerSegment"]);

            bool ChkActivityType = Session["ChkActivityType"] == null ? false : Convert.ToBoolean(Session["ChkActivityType"]);
            string ActivityType = Session["ActivityType"] == null ? "" : Convert.ToString(Session["ActivityType"]);

            bool ChkActivationOrder = Session["ChkActivationOrder"] == null ? false : Convert.ToBoolean(Session["ChkActivationOrder"]);
            string ActivationOrder = Session["ActivationOrder"] == null ? "" : Convert.ToString(Session["ActivationOrder"]);

            bool ChkCommType = Session["ChkCommType"] == null ? false : Convert.ToBoolean(Session["ChkCommType"]);
            string CommissionType = Session["CommissionType"] == null ? "" : Convert.ToString(Session["CommissionType"]);

            bool ChkChannel = Session["ChkChannel"] == null ? false : Convert.ToBoolean(Session["ChkChannel"]);
            string Channel = Session["Channel"] == null ? "" : Convert.ToString(Session["Channel"]);

            bool ChkParentPayee = Session["ChkParentPayee"] == null ? false : Convert.ToBoolean(Session["ChkParentPayee"]);
            string PayeeParent = Session["PayeeParent"] == null ? "" : Convert.ToString(Session["PayeeParent"]);

            bool ChkSubChannel = Session["ChkSubChannel"] == null ? false : Convert.ToBoolean(Session["ChkSubChannel"]);
            string SubChannel = Session["SubChannel"] == null ? "" : Convert.ToString(Session["SubChannel"]);

            bool ChkPayee = Session["ChkPayee"] == null ? false : Convert.ToBoolean(Session["ChkPayee"]);
            string Payees = Session["Payees"] == null ? "" : Convert.ToString(Session["Payees"]);

            bool ChkPeriod = Session["ChkPeriod"] == null ? false : Convert.ToBoolean(Session["ChkPeriod"]);
            string Period = Session["Period"] == null ? "" : Convert.ToString(Session["Period"]);

            bool ChkBatchStatus = Session["ChkBatchStatus"] == null ? false : Convert.ToBoolean(Session["ChkBatchStatus"]);
            string BatchStatus = Session["BatchStatus"] == null ? "" : Convert.ToString(Session["BatchStatus"]);

            string FilterQuery = Session["FilterQuery"] == null ? "" : Convert.ToString(Session["FilterQuery"]);


            string FileName = RestClient.DownloadFile(CompanyCode, LoggedInUserId, LoggedInRoleName, CompanyId, ChkSubscriberNumber, SubscriberNumber, ChkCustomerSegment, CustomerSegment, ChkActivityType, ActivityType, ChkActivationOrder, ActivationOrder, ChkCommType, CommissionType, ChkChannel, Channel, ChkParentPayee, PayeeParent, ChkSubChannel, SubChannel, ChkPayee, Payees, ChkPeriod, Period, ChkBatchStatus, BatchStatus, "", "", 9999999, 0, FilterQuery, SelectedTab, LoggedInUserName);

            Thread.Sleep(3000);//Sleep to add delay to avoid issues in getting file from S Drive

            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine");
            di.Refresh();

            var ByteData1 = Globals.DownloadFromS3(FileName, CompanyCode + "/" + LoggedInUserName + "/SearchEngine/");

            //float mb = (ByteData1.Length / 1024f) / 1024f;

            //double TotalSize =  Math.Round(BitConverter.ToDouble(ByteData1, 0), 2);

            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", FileName);
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            //Files download below error out hence we have used the above mention code. We are passing the file data instead of whole file itself.
            //var CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine/" + FileName;
            //if (System.IO.File.Exists(CompleteFileName))
            //{
            //    return File(CompleteFileName, "application/unknown", FileName);
            //}
            //else
            //{
            //    TempData["Error"] = "No File/Data found";
            //    return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            //}
        }




    }
}