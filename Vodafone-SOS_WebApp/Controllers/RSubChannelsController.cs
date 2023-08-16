//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]

    public class RSubChannelsController : PrimaryController
    {
        IRSubChannelsRestClient RestClient = new RSubChannelsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        // GET: RChannels
        private SelectList GetChannel()
        {
            IRChannelsRestClient RCRC = new RChannelsRestClient();
            var ApiData = RCRC.GetByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RcName");
            return x;
        }
        
        [ControllerActionFilter]
        public JsonResult GetSubChannels()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage SubChannels";
            return View();
        }

        // GET: RChannels/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create SubChannel";
            ViewBag.RscChannelId = GetChannel();
            return View();
        }

        // POST: RChannels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(RSubChannelViewModel RCVM)
        {
            try
            {
                    RestClient.Add(RCVM);
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RscChannelId = GetChannel();
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCVM);
            }
        }

        // GET: RChannels/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit SubChannel";
            RSubChannelViewModel RSubChannelViewModel = RestClient.GetById(id);
            ViewBag.RscChannelId = GetChannel();
            if (RSubChannelViewModel == null)
            {
                return HttpNotFound();
            }
            return View(RSubChannelViewModel);
        }

        // POST: RChannels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(RSubChannelViewModel RSubChannelViewModel)
        {
            try
            {
               
                    RestClient.Update(RSubChannelViewModel);
                    return RedirectToAction("Index");
              
            }
            catch (Exception ex)
            {
                ViewBag.RscChannelId = GetChannel();
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RSubChannelViewModel);
            }
        }

        // GET: RChannels/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete SubChannel";
            RSubChannelViewModel RSubChannelViewModel = RestClient.GetById(id);
            if (RSubChannelViewModel == null)
            {
                return HttpNotFound();
            }
            return View(RSubChannelViewModel);
        }

        // POST: RChannels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RSubChannelViewModel RCVM)
        {
            try
            {
                RestClient.Delete(RCVM.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCVM);
            }
        }
    }
}
