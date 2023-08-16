using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LNotificationController : PrimaryController
    {
        // GET: LNotification
        ILNotificationRestClient RestClient = new LNotificationRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        //[ControllerActionFilter]
        public JsonResult GetNotification()
        {
            var ApiData = RestClient.GetNotificationByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Notification";
            return View();
        }

        // [ControllerActionFilter]
        public ActionResult UpdateNotification(int id, Boolean IsActive)
        {
            RestClient.Update(id,IsActive);
            return Json(id, JsonRequestBehavior.AllowGet);
        }
    }
}