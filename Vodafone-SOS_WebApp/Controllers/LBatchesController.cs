//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LBatchesController : PrimaryController
    {
        string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Batches";
            var role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            if (role == "Reporting Analyst")
            {
                return RedirectToAction("AnalystIndex");
            }
            if (role == "Manager")
            {
                return RedirectToAction("ManagerIndex");
            }
            return View();
        }

        public JsonResult GetByUserForPayeeUploadGrid()
        {
            ILBatchesRestClient BRC = new LBatchesRestClient();
            var Data = BRC.GetByUserForPayeeUploadGrid(CompanyId,UserId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
    }
}