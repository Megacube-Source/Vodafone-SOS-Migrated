//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RBrandsController : PrimaryController
    {
        IRBrandsRestClient RestClient = new RBrandsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        
        //method to return list of device types for the index page grig
        [ControllerActionFilter]
        public JsonResult GetBrands()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // GET: RBrands
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Brands";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Brand";
            return View();
        }

        // POST: RBrands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(RBrandViewModel RBVM)
        {
            try
            {
                    RBVM.RbCompanyId = CompanyId;
                    RestClient.Add(RBVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(RBVM);
            }
        }

        // GET: RBrands/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            RBrandViewModel rBrandViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit Brand";
            if (rBrandViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rBrandViewModel);
        }

        // POST: RBrands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( RBrandViewModel rBrandViewModel)
        {
            try
            {
                    rBrandViewModel.RbCompanyId = CompanyId;
                    RestClient.Update(rBrandViewModel);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(rBrandViewModel);
            }
        }

        // GET: RBrands/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            RBrandViewModel rBrandViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Brand";
            if (rBrandViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rBrandViewModel);
        }

        // POST: RBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RBrandViewModel RBVM)
        {
            try
            {
                RestClient.Delete(RBVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RBVM);
            }
        }
    }
}
