//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RDeviceTypesController : PrimaryController
    {
        IRDeviceTypesRestClient RestClient = new RDeviceTypesRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        //method to return list of device types for the index page grig
        [ControllerActionFilter]
        public JsonResult GetDeviceTypes()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        // GET: RDeviceTypes
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Device Types";
            return View();
        }

        // GET: RDeviceTypes/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Device Type";
            return View();
        }

        // POST: RDeviceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( RDeviceTypeViewModel RDTVM)
        {
            try
            {
                    RDTVM.RdtCompanyId = CompanyId;
                    RestClient.Add(RDTVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(RDTVM);
            }
        }

        // GET: RDeviceTypes/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            RDeviceTypeViewModel rDeviceTypeViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit Device Type";
            if (rDeviceTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rDeviceTypeViewModel);
        }

        // POST: RDeviceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(RDeviceTypeViewModel rDeviceTypeViewModel)
        {
            try
            {
                    rDeviceTypeViewModel.RdtCompanyId = CompanyId;
                    RestClient.Update(rDeviceTypeViewModel);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return View(rDeviceTypeViewModel);
            }
        }

        // GET: RDeviceTypes/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            RDeviceTypeViewModel rDeviceTypeViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Device Type";
            if (rDeviceTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rDeviceTypeViewModel);
        }

        // POST: RDeviceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RDeviceTypeViewModel RDTVM)
        {
            try
            {
                RestClient.Delete(RDTVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RDTVM);
            }
        }
    }
}
