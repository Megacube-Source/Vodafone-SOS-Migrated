//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    
    public class RActiveTypesController : PrimaryController
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IRActiveTypesRestClient RestClient = new RActiveTypesRestClient();

        [ControllerActionFilter]
        public JsonResult GetActiveTypes()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        // GET: RActiveTypes
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Activity Types";
            return View();
        }

        //[ControllerActionFilter]
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Activity Types";
            return View();
        }

        // POST: RActiveTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( RActiveTypeViewModel RATVM)
        {
            try
            {
                    RATVM.RatCompanyId = CompanyId;
                    RestClient.Add(RATVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RATVM);
            }
        }

        // GET: RActiveTypes/Edit/5
        //[ControllerActionFilter]
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Activity Types";
            RActiveTypeViewModel rActiveTypeViewModel = RestClient.GetById(id);
            if (rActiveTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rActiveTypeViewModel);
        }

        // POST: RActiveTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(RActiveTypeViewModel rActiveTypeViewModel)
        {
            try
            {
                    rActiveTypeViewModel.RatCompanyId = CompanyId;
                    RestClient.Update(rActiveTypeViewModel);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(rActiveTypeViewModel);
            }
        }

        // GET: RActiveTypes/Delete/5
        //[ControllerActionFilter]
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete Activity Types";
            RActiveTypeViewModel rActiveTypeViewModel = RestClient.GetById(id);
            if (rActiveTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rActiveTypeViewModel);
        }

        // POST: RActiveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RActiveTypeViewModel RATVM)
        {
            try
            {
                RestClient.Delete(RATVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RATVM);
            }
        }
    }
}
