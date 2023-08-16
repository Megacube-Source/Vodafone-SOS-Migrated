//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class TroubleShootingController : PrimaryController
    {
        ITroubleShootingRestClient TSRC = new TroubleShootingRestClient();
        public ActionResult WIAM()
        {
            return View();
        }

        public JsonResult CheckADAccountExist(string Email)
        {
            var Data = TSRC.CheckADAccountExist(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayeeData(string Email)
        {
            var Data = TSRC.GetPayeeData(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserLobbyData(string Email)
        {
            var Data = TSRC.GetUserLobbyData(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserData(string Email)
        {
            var Data = TSRC.GetUserData(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateLoginStatus(string Email)
        {
            var Data = TSRC.CheckADAccountExist(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateActivateCreationLogin(string Email)
        {
            var Data = TSRC.UpdateActivateCreationLogin(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDeActivateCreationLogin(string Email)
        {
            var Data = TSRC.UpdateDeActivateCreationLogin(Email);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
    }
}