//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RProductCodesController : PrimaryController
    {
        IRProductCodesRestClient RestClient = new RProductCodesRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
  
        //method to return list of device types for the index page grig
        [ControllerActionFilter]
        public JsonResult GetProductCodes()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // GET: RProductCodes
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage ProductCodes";
            return View();
        }


        // GET: RProductCodes/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create ProductCode";
            return View();
        }

        // POST: RProductCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(RProductCodeViewModel RPCVM)
        {
            try
            {
                RPCVM.RpcCompanyId = CompanyId;
                RestClient.Add(RPCVM);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(RPCVM);
            }
        }

        // GET: RProductCodes/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            RProductCodeViewModel rProductCodeViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit ProductCode";
            if (rProductCodeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rProductCodeViewModel);
        }

        // POST: RProductCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(RProductCodeViewModel rProductCodeViewModel)
        {
            try
            {
                rProductCodeViewModel.RpcCompanyId = CompanyId;
                RestClient.Update(rProductCodeViewModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(rProductCodeViewModel);
            }
        }

        // GET: RProductCodes/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            RProductCodeViewModel rProductCodeViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete ProductCode";
            if (rProductCodeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rProductCodeViewModel);
        }

        // POST: RProductCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RProductCodeViewModel RPCVM)
        {
            try
            {
                RestClient.Delete(RPCVM.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RPCVM);
            }
        }
    }
}