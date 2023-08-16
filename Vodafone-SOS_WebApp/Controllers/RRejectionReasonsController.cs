//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RRejectionReasonsController : PrimaryController
    {
        IRRejectionReasonsRestClient RestClient = new RRejectionReasonsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        [ControllerActionFilter]
        public JsonResult GetRejectionReasons()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        // GET: RRejectionReasons
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage ClaimsApprove/Reject Reasons";
            return View();
        }

        // GET: RRejectionReasons/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Claims Approve/Reject Reason";
            return View();
        }

        // POST: RRejectionReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( RRejectionReasonViewModel RRRVM)
        {
            try
            {
                    RRRVM.RrrCompanyId = CompanyId;
                    RestClient.Add(RRRVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RRRVM);
            }
        }

        // GET: RRejectionReasons/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Claims Approve/Reject Reasons";
            RRejectionReasonViewModel rRejectionReasonViewModel = RestClient.GetById(id);
            
            if (rRejectionReasonViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rRejectionReasonViewModel);
        }

        // POST: RRejectionReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( RRejectionReasonViewModel rRejectionReasonViewModel)
        {
            try
            {
                    rRejectionReasonViewModel.RrrCompanyId = CompanyId;
                    RestClient.Update(rRejectionReasonViewModel);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {//
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(rRejectionReasonViewModel);
            }
        }

        // GET: RRejectionReasons/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete Rejection Reason";
            RRejectionReasonViewModel rRejectionReasonViewModel = RestClient.GetById(id);
            if (rRejectionReasonViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rRejectionReasonViewModel);
        }

        // POST: RRejectionReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RRejectionReasonViewModel RRRVM)
        {
            try
            {
                RestClient.Delete(RRRVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RRRVM);
            }
        }
    }
}
