//Code Review for this file (from security perspective) done

using System;
using System.Net;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class SupportSystemController : PrimaryController
    {
        ISupportSystemRestClient RestClient = new SupportSystemRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        #region Category
        private SelectList GetCategoryList()
        {
            var ApiData = RestClient.GetCategoryList(CompanyId);
            var x = new SelectList(ApiData, "Id", "RscName");
            return x;
        }

        [ControllerActionFilter]
        public JsonResult GetAllSupportCategories()
        {
            var ApiData = RestClient.GetCategoryList(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetAllSupportQuickTickets()
        {
            var ApiData = RestClient.GetAllSupportQuickTickets();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetCategoryDetails(int Id)
        {
            var ApiData = RestClient.GetCategoryById(Id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Categories()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Support System";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreateCategory(SupportSystemCategoriesViewModel SSCVM)
        {
            try
            {
                SSCVM.RscCompanyId = CompanyId;
                RestClient.AddCategory(SSCVM);
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(SSCVM);
            }
        }

        [ControllerActionFilter]
        public ActionResult EditCategory(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Category";
            SupportSystemCategoriesViewModel model = RestClient.GetCategoryById(id);
            ViewBag.RsqtCategoryId = GetCategoryList();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult EditCategory(SupportSystemCategoriesViewModel Category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category.RscCompanyId = CompanyId;
                    RestClient.UpdateCategory(Category);
                    return RedirectToAction("Categories");
                }
                return View(Category);
            }
            catch (Exception ex)
            {
                //ViewBag.RsqtCategoryId = GetCategoryList();
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(Category);
            }
        }

        [ControllerActionFilter]
        public ActionResult DeleteCategory(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Quick Ticket";
            SupportSystemCategoriesViewModel CategoryModel = RestClient.GetCategoryById(id);
            if (CategoryModel == null)
            {
                return HttpNotFound();
            }
            return View(CategoryModel);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(SupportSystemCategoriesViewModel Rsqt)
        {
            try
            {
                RestClient.DeleteCategory(Rsqt.Id);
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return RedirectToAction("Categories");
            }
        }
        #endregion

        #region Team
        //Currently not in user but might be used in future. //RK 23/04/2017
        [ControllerActionFilter]
        public ActionResult Teams()
        {
            return View();
        }

        [ControllerActionFilter]
        public ActionResult CreateTeam()
        {
            return View();
        }
        #endregion

        #region QuickTicket
        [ControllerActionFilter]
        public ActionResult QuickTickets()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Quick Tickets";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult CreateQuickTicket()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Quick Ticket";
            ViewBag.RsqtCategoryId = GetCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreateQuickTicket(SupportSystemQuickTicketsViewModel SSQTV)
        {
            try
            {
                RestClient.AddQuickTicket(SSQTV);
                return RedirectToAction("QuickTickets");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                ViewBag.RsqtCategoryId = GetCategoryList();
                return View(SSQTV);
            }
        }

        [ControllerActionFilter]
        public ActionResult EditQuickTicket(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Quick Ticket";
            SupportSystemQuickTicketsViewModel model = RestClient.GetQuickTicketById(id);
            ViewBag.RsqtCategoryId = GetCategoryList();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult EditQuickTicket(SupportSystemQuickTicketsViewModel QuickTicket)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.UpdateQuickTicket(QuickTicket);
                    return RedirectToAction("QuickTickets");
                }
                return View(QuickTicket);
            }
            catch (Exception ex)
            {
                ViewBag.RsqtCategoryId = GetCategoryList();
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(QuickTicket);
            }
        }

        [ControllerActionFilter]
        public ActionResult DeleteQuickTicket(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Quick Ticket";
            SupportSystemQuickTicketsViewModel QuickTicketModel = RestClient.GetQuickTicketById(id);
            if (QuickTicketModel == null)
            {
                return HttpNotFound();
            }
            return View(QuickTicketModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(SupportSystemQuickTicketsViewModel Rsqt)
        {
            try
            {
                RestClient.DeleteQuickTicket(Rsqt.Id);
                return RedirectToAction("QuickTickets");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(Rsqt);
            }
        }
        #endregion

        #region TicketStages
        private SelectList GetTicketStagesList()
        {
            var ApiData = RestClient.GetTicketStagesList();
            var x = new SelectList(ApiData, "Id", "RtsName");
            return x;
        }

        //[ControllerActionFilter]
        public JsonResult GetAllRTicketStages(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            var ApiData = RestClient.GetAllTicketStages();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetRTicketStageDetails(int Id)
        {
            var ApiData = RestClient.GetTicketStageById(Id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult TicketStages()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Support System";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult CreateTicketStage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreateTicketStage(SupportSystemStagesViewModel SSTSVM)
        {
            try
            {
                RestClient.AddTicketStage(SSTSVM);
                return RedirectToAction("TicketStages");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(SSTSVM);
            }
        }

        [ControllerActionFilter]
        public ActionResult EditTicketStage(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Ticket Stage";
            SupportSystemStagesViewModel model = RestClient.GetTicketStageById(id);
            ViewBag.RsqtTicketStageId = model.Id;
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult EditTicketStage(SupportSystemStagesViewModel TicketStage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.UpdateTicketStage(TicketStage);
                    return RedirectToAction("TicketStages");
                }
                return View(TicketStage);
            }
            catch (Exception ex)
            {
                //ViewBag.RsqtCategoryId = GetCategoryList();
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(TicketStage);
            }
        }

        [ControllerActionFilter]
        public ActionResult DeleteTicketStage(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Ticket Stage";
            SupportSystemStagesViewModel TicketStageModel = RestClient.GetTicketStageById(id);
            if (TicketStageModel == null)
            {
                return HttpNotFound();
            }
            return View(TicketStageModel);
        }

        [HttpPost, ActionName("DeleteTicketStage")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(SupportSystemStagesViewModel Rsqt)
        {
            try
            {
                RestClient.DeleteTktStage(Rsqt.Id);
                return RedirectToAction("TicketStages");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return RedirectToAction("TicketStages");
            }
        }
        #endregion

    }
}