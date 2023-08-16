//Code Review for this file (from security perspective) done

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LCommissionPeriodsController : PrimaryController
    {
        
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        // GET: LCommissionPeriods
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Commission Periods";
            return View();
        }

        //method for loading grid data on analyst dashboard
        [ControllerActionFilter]
        public JsonResult GetPeriodGrid(String TabName)
        {
            IEnumerable<LCommissionPeriodViewModel> ApiData = new List<LCommissionPeriodViewModel>();
            ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();

            switch (TabName)
            {
                case "Open":
                    ApiData = LCPRC.GetByCompanyIdStatus(CompanyId, "Open");
                    break;
                case "Locked":
                    ApiData = LCPRC.GetByCompanyIdStatus(CompanyId, "Locked");
                    break;
                case "Unlocked":
                    ApiData = LCPRC.GetByCompanyIdStatus(CompanyId, "Unlocked");
                    break;
                case "Closed":
                    ApiData = LCPRC.GetByCompanyIdStatus(CompanyId, "Closed");
                    break;
            }
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public ActionResult UpdateStatus(int id, string Status)
        {
            ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();
            var CommissionPeriod = LCPRC.GetById(id);
            CommissionPeriod.LcpStatus = Status;
            LCPRC.Update(CommissionPeriod);

            return RedirectToAction("Index", "LCommissionPeriods");
        }
        
    }
}