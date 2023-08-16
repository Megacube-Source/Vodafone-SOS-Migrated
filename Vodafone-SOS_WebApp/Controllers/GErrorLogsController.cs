
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GErrorLogsController : Controller
    {
        IErrorLogsRestClient RestClient = new ErrorLogsRestClient();
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        // GET: GErrorLogs
        public ActionResult GErrorLogs()
        {
            return View();
        }

        //This method will get GErrorLogData from Api to display in grid
        //[ControllerActionFilter]
        public JsonResult GErrorLogGrid(Nullable<int> pagesize, Nullable<int> pagenum, string sortdatafield, string sortorder)
        {

            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            TempData["FilterExceptionQuery"] = FilterQuery;
            var ApiData = RestClient.GetGErrorlogGrid(Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), sortdatafield, sortorder, FilterQuery);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This method addedd  to count grid data from Api
        [ControllerActionFilter]
        public JsonResult GetGErrorLogcounts()
        {
            var ErrorCount = RestClient.GetGErrorLogscounts();
            return Json(ErrorCount, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to get the total counts of Exception records for Summary tab on L2Admin Page
        /// </summary>       
        public JsonResult GetExceptionSummaryCounts()
        {
            var ExceptionCount = RestClient.GetExceptionSummaryCounts();
            return Json(ExceptionCount, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        ///  Method to get the data for summary tab of exception on L2Admin Page
        /// </summary>
        public JsonResult GetExceptionSummary(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetExceptionSummary(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to show the data on chart for Exception
        /// </summary>
        //public JsonResult GetExceptionChart()
        //{
        //    var ApiData = RestClient.GetExceptionChart();
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult DownloadExceptionFile()
        {

            string FilterQuery = Session["FilterExceptionQuery"] == null ? "" : Convert.ToString(Session["FilterExceptionQuery"]);
            string FileName = RestClient.DownloadExceptionFile(CompanyCode, LoggedInUserName, FilterQuery);

            Thread.Sleep(3000);//Sleep to add delay to avoid issues in getting file from S Drive

            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception");
            di.Refresh();

            var ByteData1 = Globals.DownloadFromS3(FileName, CompanyCode + "/" + LoggedInUserName + "/Exception/");

           

            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", FileName);
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            
        }

    }
}