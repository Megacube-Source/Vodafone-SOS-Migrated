//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RCommissionTypesController : PrimaryController
    {
        IRCommisionTypesRestClient RestClient = new RCommissionTypesRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        //method to get list of data in grid of index page
        [ControllerActionFilter]
        public JsonResult GetCommissionTypes()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }


        // GET: RCommissionTypes
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Commission Types";
            return View();
        }

        // GET: RCommissionTypes/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Commission Type";
            return View();
        }

        // POST: RCommissionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( RCommissionTypeViewModel RCTVM)
        {
            try
            {
                    RCTVM.RctCompanyId = CompanyId;
                    RestClient.Add(RCTVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCTVM);
            }
        }

        // GET: RCommissionTypes/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Commission Type";
            RCommissionTypeViewModel rCommissionTypeViewModel = RestClient.GetById(id);
            if (rCommissionTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rCommissionTypeViewModel);
        }

        // POST: RCommissionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( RCommissionTypeViewModel rCommissionTypeViewModel)
        {
            try
            {
                    rCommissionTypeViewModel.RctCompanyId = CompanyId;
                    RestClient.Update(rCommissionTypeViewModel);
                    return RedirectToAction("Index");
            }catch(Exception ex)
            {

                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(rCommissionTypeViewModel);
            }
        }

        // GET: RCommissionTypes/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete Commission Type";
            RCommissionTypeViewModel rCommissionTypeViewModel = RestClient.GetById(id);
            if (rCommissionTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rCommissionTypeViewModel);
        }

        // POST: RCommissionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RCommissionTypeViewModel RCTVM)
        {
            try
            {
                RestClient.Delete(RCTVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCTVM);
            }
        }
    }
}
