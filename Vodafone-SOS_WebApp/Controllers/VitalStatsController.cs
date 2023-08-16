using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using System.Configuration;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class VitalStatsController : PrimaryController
    {
        IVitalStatsRestClient RestClient = new VitalStatsRestClient();
        // GET: VitalStats
        //Thismethod is defined to return view to display the VitalStats  for All Opco
        [ControllerActionFilter]
        public ActionResult VitalStats()
        {
            return View();
        }
        //This method will get VitalStatsData  from Api to display in grid
        [ControllerActionFilter]
        public JsonResult VitalStatsForOpco(Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
        {
           var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var VitalStat = RestClient.GetVitalStatsForOpco(StartDatetime, EndDatetime);
                 return Json(VitalStat, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        //This Method is defined to return data in excel sheet when admin click on download button
        [ControllerActionFilter]
        public ActionResult DownloadVitalStatsReport(Nullable<DateTime> StartDate, Nullable<DateTime> EndDate)
        {
            var StartDatetime = (StartDate.HasValue) ? StartDate.Value.ToString("yyyy-MM-dd") : null;
            var EndDatetime = (EndDate.HasValue) ? EndDate.Value.ToString("yyyy-MM-dd") : null;
            if (StartDate.HasValue && EndDate.HasValue)
            {
                RestClient.DownloadVitalStatsForReport(StartDatetime, EndDatetime);
                var FileName = ConfigurationManager.AppSettings["ExportCalcDocumentPath"] + "/ExportVitalStatsReport.xlsx";
                if (System.IO.File.Exists(FileName))
                {
                    return File(FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportVitalStatsReport.xlsx");
                }
            }
            TempData["Error"] = "No Data found at ";
            return RedirectToAction("VitalStats");
        }
    }
}