//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LRawDataTablesController : PrimaryController
    {
        ILRawDataTablesRestClient RestClient = new LRawDataTablesRestClient();
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        // GET: LRawDataTables
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Map Rawdata Column";
            return View();
        }
        [ControllerActionFilter]
        public JsonResult GetRawDataTables()
        {
           var xx= RestClient.GetXTablesList(CompanyCode);
            return Json(xx,JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public ActionResult DeRegisterTable(int RawDataTableId)
        {
            try
            {
                RestClient.DeRegister(RawDataTableId);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index");
            }
        }
    }
}