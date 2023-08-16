using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LEmailBucketController : Controller
    {
        // GET: LEmailBucket
        //public ActionResult Index()
        //{
        //    return View();
        //}
        LEmailBucketRestClient RestClient = new LEmailBucketRestClient();

        //method to get the data's count for summary tab for L2Admin Page
        public JsonResult GetEmailBucketSummaryCounts()
        {
            var SummaryCount = RestClient.GetEmailBucketSummaryCounts();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }

        //method to get the data for summary tab for L2Admin Page
        public JsonResult GetEmailBucketSummaryForDashBoard(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetEmailBucketSummary(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //method to get the data for Email Bucket Chart
        public JsonResult GetEmailBucketChart()
        {
            var ApiData = RestClient.GetEmailBucketChart();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        //method to get the data's count for the detail tab for L2Admin Page
        public JsonResult GetEmailBucketDetailCounts()
        {
            var SummaryCount = RestClient.GetEmailBucketDetailCounts();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }

        //method to get the data for detail tab for L2Admin Page
        [HttpGet]
        public JsonResult GetEmailBucketDetailForDashBoard(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetEmailBucketDetail(pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
    }
}