using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LMessagesController : PrimaryController
    {
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string UserId = System.Web.HttpContext.Current.Session["UserId"] as string;
        string strUserRole = System.Web.HttpContext.Current.Session["UserRole"] as string;
        string iRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        LMessagesRestClient RestClient = new LMessagesRestClient();
        // GET: LMessages
        public JsonResult GetMyMessages()
        {
            var ApiData = RestClient.GetMyMessages(UserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsersListToSendMessage(string RoleName, string Portfolios, string ValueType)
        {
            var ApiData = RestClient.GetUsersListToSendMessage(RoleName,Portfolios, CompanyId, ValueType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMySentMessages()
        {
            var ApiData = RestClient.GetMySentMessages(UserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsMessageBoardAvailable()
        {
            var ApiData = RestClient.IsMessageBoardAvailable(Convert.ToInt32(iRoleId));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetUnreadMessageCount()
        //{
        //    var ApiData = RestClient.GetUnreadMessageCount(UserId);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
        private SelectList GetRolesToSendMessage()
        {
            var ApiData = RestClient.GetRolesToSendMessage(iRoleId);
            var x = new SelectList(ApiData, "RecipientRoleID", "RecipientRoleName");
            return x;
        }
        public ActionResult Index()
        {
            var ApiData = RestClient.GetRolesToSendMessage(iRoleId);
            if(ApiData.Count()>0)
            {
                ViewBag.ComposeAllowed = "True";
            }
            else
            {
                ViewBag.ComposeAllowed = "False";
            }
            
            return View();
        }
        
        public ActionResult create()
        {
            ViewBag.RecipientRoleID = GetRolesToSendMessage();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult create(LMessageBoardViewModel MessageModel, string SelectedPortfolios)
        {
            List<string> strIds = new List<string>();
            var ApiData = RestClient.GetUsersListToSendMessage(MessageModel.RecipientRoleName, SelectedPortfolios, CompanyId, "Id");
            for (int i = 0; i < ApiData.Count(); i++)
            {
                strIds.Add(ApiData[i].ToString());
            }
            MessageModel.UsersToSendMessage = strIds;
            try
            {
                MessageModel.CompanyID = CompanyId;
                MessageModel.CreatedById = UserId;
                RestClient.Add(MessageModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(MessageModel);
            }
        }
        public ActionResult ViewMessage(int Id)
        {
            LMessageBoardViewModel model = new LMessageBoardViewModel();

            model = RestClient.GetById(Id);
            RestClient.SetMessageAsRead(model);
            return View(model);
        }
        public ActionResult Delete(int ID)
        {
            RestClient.Delete(ID);
            ViewBag.Message = "Message Deleted!!";
            return RedirectToAction("Index");
        }

        public ActionResult CustomMenu()
        {
            return View();
        }

        public JsonResult CustomMenuData()
        {
            IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
            var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            //If role is defined in session for current user then dsplay its menu items
            if (!string.IsNullOrEmpty(Role))
            {
                //As per current logic disscussed with VG . Only those menu which have url will be clickable .
                var MenuItems = MGARC.GetByUserRole(Role, CompanyCode).Select(p => new { id = p.MgmarMenuId, text = (string.IsNullOrEmpty(p.GmMenuUrl)) ? "<a href='#'>" + p.GmMenuName + "</a>" : "<a href='" + p.GmMenuUrl + "'>" + p.GmMenuName + "</a>", parentid = p.GmParentId, p.GmOrdinalPosition }).OrderBy(p => p.GmOrdinalPosition).ToList();
                return Json(MenuItems, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}