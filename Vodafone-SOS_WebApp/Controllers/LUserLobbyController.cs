using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LUserLobbyController : Controller
    {
        ILUserLobbyRestClient LobbyRC = new LUserLobbyRestClient();
        string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        string LoggedInUserId = System.Web.HttpContext.Current.Session["UserId"] as string;
        string Workflow = Convert.ToString(System.Web.HttpContext.Current.Session["Workflow"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        // GET: LUserLobby
        [ControllerActionFilter]
        public JsonResult RejectUser(int Id, string UserType)
        {
            LobbyRC.RejectUser(Id, LoggedInUserName, Convert.ToInt32(LoggedRoleId),null);
            return Json(string.Empty);
        }
        [ControllerActionFilter]
        public ActionResult AcceptLobbyUsers()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Accept Users";
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            System.Web.HttpContext.Current.Session["Workflow"] = WFName;
            return View();
        }
        [ControllerActionFilter]
        public JsonResult GetUsersFromLobby(string UserType)
        {
            
            var users = LobbyRC.GetUsersFromLobby(UserType, CompanyCode);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public ActionResult CreateUserFromLobby(int Id)
        {
            var model = LobbyRC.GetLobbyUserById(Id);
            return View(model);
        }
        [HttpGet]
        [ControllerActionFilter]
        public JsonResult SetWorkFlowInSession(string User,int LobbyUserId)
        {
            if("FinOps".Equals(User))
                System.Web.HttpContext.Current.Session["Workflow"] = "Users";
            else if("Payee".Equals(User))
                System.Web.HttpContext.Current.Session["Workflow"] = "Payees";
            //also Check user alreday in SOS, then we have to call nightlyJob
            string data = LobbyRC.CheckUserInSOSAndCallSP(User, LobbyUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getlobbycounts()
        {
            var ErrorCount = LobbyRC.Getlobbycounts(CompanyCode);
            return Json(ErrorCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLobbyGrid(Nullable<int> pagesize, Nullable<int> pagenum, string sortdatafield, string sortorder)
        {

            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            TempData["FilterLobbyQuery"] = FilterQuery;
            var ApiData = LobbyRC.GetLobbyGrid(Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), sortdatafield, sortorder, FilterQuery, CompanyCode);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

    }
}