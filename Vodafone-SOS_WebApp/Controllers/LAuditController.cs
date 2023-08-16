using Ionic.Zip;
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

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LAuditController : PrimaryController
    {
        ILAuditRestClient RestClient = new LAuditRestClient();
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        // GET: LAudit

          [ControllerActionFilter]
        public ActionResult DownloadAuditReport(string Entity, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
        {
            //To get record as per date till 12AM
            //EndDate.Value.AddDays(1).AddMinutes(-1);
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var result = RestClient.DownloadAuditForReports(Entity, CompanyId, StartDatetime,EndDatetime,LoggedInUserName);
                var FileName = result;
                var CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + result;
                var FileType = Globals.GetFileContentType("zip") + ";charset=UTF-16";//"text/csv";
                //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
                var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
                Thread.Sleep(5000);
                DirectoryInfo dir1 = new DirectoryInfo(FilePath);
                dir1.Refresh();
                if (System.IO.File.Exists(CompleteFileName))
                {
                    return File(CompleteFileName,FileType,FileName);
                }
                    }
            TempData["Error"] = "No Data Found";
            return RedirectToAction("AuditorDashboard","Home");
        }


        //get grid data for audit page
        [ControllerActionFilter]
        public JsonResult GetAuditGrid(string Entity, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate, string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            //To get record as per date till 12AM
            EndDate.Value.AddDays(1).AddMinutes(-1);
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var qry = Request.QueryString;
                //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
                var FilterQuery = Globals.BuildQuery(qry);
                var ApiData = RestClient.GetAuditForReports(Entity, CompanyId, StartDatetime, EndDatetime, pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


       [ControllerActionFilter]
        public JsonResult GetAuditGridCounts(string Entity, Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
        {
            //To get record as per date till 12AM
            TempData["Entity"] = Entity;
            TempData["StartDate"] = StartDate;
            TempData["EndDate"] = Entity;

            EndDate.Value.AddDays(1).AddMinutes(-1);
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var ApiData = RestClient.GetAuditCountsForReports(Entity, CompanyId, StartDatetime, EndDatetime);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult RedirectToReview(int TransactionId,string EntityType)
        {
            //ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            //var SupportingDocuments = LSDRC.GetByEntityType(EntityType,TransactionId).ToList();
            //using (ZipFile zip = new ZipFile())
            //{
            //    foreach (var Document in SupportingDocuments)
            //    {
            //        var yy = Document.LsdFilePath + "/" + Document.LsdFileName;
            //        zip.AddFile(yy, "");
            //    }
            //    if (System.IO.File.Exists(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "ExportSupportingDocuments.zip"))
            //    {
            //        System.IO.File.Delete(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "ExportSupportingDocuments.zip");
            //    }
            //    if (zip.Count() > 0)
            //    {
            //        zip.Save(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "ExportSupportingDocuments.zip");
            //        return File(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/" + "ExportSupportingDocuments.zip", "application/zip", "ExportSupportingDocuments.zip");
            //    }
            //}
            //    TempData["Error"] = "No Data Found";
            //Changed Functionality to redirect to review
            switch (EntityType)
            {
                case "Users":
                    return RedirectToAction("Edit", "LUsers", new { FormType = "Review", TransactionId = TransactionId, WFConfigId = 0 });
                case "Payees":
                    return RedirectToAction("ViewPayeeDetails", "LPayees", new { TransactionId = TransactionId, WFConfigId = 0 });
                case "Claims":
                    return RedirectToAction("Edit", "LClaims", new { FormType = "disabled", TransactionId = TransactionId, WFConfigId =0 });
                case "Calc":
                    return RedirectToAction("Index", "LCalc", new { TransactionId = TransactionId, WFConfigId =0 });
                case "RawData":
                    return RedirectToAction("Index", "LRawData", new { TransactionId = TransactionId, WFConfigId =0 });
                case "Pay":
                    return RedirectToAction("Review", "LReadyToPay", new { id = TransactionId});
                case "ManualAdjustments":
                    return RedirectToAction("Review", "ManualAdjustments", new { TransactionId = TransactionId, WFConfigId =0 });
                case "RefFiles":
                    return RedirectToAction("Review", "LUploadedFiles", new { TransactionId = TransactionId, WFConfigId =0 });
                case "Schemes":
                    return RedirectToAction("Review", "LSchemes", new { TransactionId = TransactionId, WFConfigId =0 });
                case "Accruals":
                    return RedirectToAction("Review", "LAccruals", new { TransactionId = TransactionId, WFConfigId =0 });
                case "DocumentSets":
                    return RedirectToAction("Review", "LDocumentSets", new { TransactionId = TransactionId, WFConfigId =0 });
                default:
                    return RedirectToAction("AuditorDashboard", "Home");
            }
            
        }

        //public JsonResult GetCountsForNewItems()
        //{
        //    var Count = RestClient.GetCountsForNewItems();
        //    return Json(Count, JsonRequestBehavior.AllowGet);
        //}


        //[HttpGet]
        //public JsonResult GetNewItems(Nullable <int> pagesize, Nullable <int> pagenum, string sortdatafield, string sortorder)
        //{
        //    //RK R2.3 17112018 handled the null values for page size and page num
        //    if (pagesize == null) pagesize = 0;
        //    if (pagenum == null) pagenum = 0;
        //    var qry = Request.QueryString;
        //    var FilterQuery = Globals.BuildQuery(qry);
        //    var ApiData = RestClient.GetNewItems(Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), sortdatafield, sortorder, FilterQuery);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        //GetNIForChart
        public JsonResult GetNIForChart()
        {
            var ApiData = RestClient.GetNIForChart();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // [ControllerActionFilter]
        public JsonResult GetDataForNewItems(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum, Nullable<int> Intervalid)
        {
            if (pagesize == 0) pagesize = 5;
            if (pagenum == 0) pagenum = 0;
            if (Intervalid == null) Intervalid = 1;
           
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            // FilterQuery = FilterQuery.Replace("'", "''");
            var ApiData = RestClient.GetDataForNewItemColumns(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery,Convert.ToInt32(Intervalid));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //GetCountsForNewItems
        public JsonResult GetCountsForNewItems()
            //string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum, Nullable<int> Intervalid
        {
            //if (pagesize == null) pagesize = 5;
            //if (pagenum == null) pagenum = 1;
            //if (Intervalid == null) Intervalid = 1;

            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetCountsForNewItems();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
    }
}