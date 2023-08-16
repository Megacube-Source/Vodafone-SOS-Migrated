//This controller is to be dropped. Tables got already renamed (05June2017). If nothing comes up after renaming then we can delete these code files too





using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    [CustomAuthorize("System Analyst")]
    public class RStatusesController : PrimaryController
    {
        IRStatusesRestClient RestClient =new  RStatusesRestClient();

        //method to get lit of statuses in grid of index page
        [ControllerActionFilter]
        public JsonResult GetStatuses()
        {
            var ApiData = RestClient.GetAll();
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }
        //method to get list of status owners for dropdown
        [ControllerActionFilter]
        private SelectList GetStatusOwner()
        {
            IRStatusOwnersRestClient RSORC = new RStatusOwnersRestClient();
            var StatusOwner = RSORC.GetAll();
            var OwnerList = new SelectList(StatusOwner, "Id", "RsoStatusOwner");
            return OwnerList;
        }
        //method to get list of status owners for dropdown for Edit Page
        [ControllerActionFilter]
        private SelectList GetStatusOwner(int id)
        {
            IRStatusOwnersRestClient RSORC = new RStatusOwnersRestClient();
            var StatusOwner = RSORC.GetAll();
            var OwnerList = new SelectList(StatusOwner, "Id", "RsoStatusOwner",id);
            return OwnerList;
        }

        // GET: RStatuses
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Statuses";
            return View();
        }

        //// GET: RStatuses/Details/5
        //public ActionResult Details(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RStatusViewModel rStatusViewModel = RestClient.GetById(id);
        //    if (rStatusViewModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rStatusViewModel);
        //}

        // GET: RStatuses/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Status";
            ViewBag.RsOwnerId = GetStatusOwner();
            return View();
        }

        // POST: RStatuses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(RStatusViewModel RSVM)
        {
            try
            {
                    RestClient.Add(RSVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.RsOwnerId = GetStatusOwner();
                return View(RSVM);
            }
        }

        // GET: RStatuses/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Status";
            RStatusViewModel rStatusViewModel = RestClient.GetById(id);
            ViewBag.RsOwnerId = GetStatusOwner(rStatusViewModel.RsOwnerId);
            if (rStatusViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rStatusViewModel);
        }

        // POST: RStatuses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( RStatusViewModel rStatusViewModel)
        {
            try
            {
                    RestClient.Update(rStatusViewModel);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.RsOwnerId = GetStatusOwner(rStatusViewModel.RsOwnerId);
                return View(rStatusViewModel);
            }
        }

        // GET: RStatuses/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete Status";
            RStatusViewModel rStatusViewModel = RestClient.GetById(id);
            if (rStatusViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rStatusViewModel);
        }

        // POST: RStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RStatusViewModel RSVM)
        {
            try
            {
                RStatusViewModel rStatusViewModel = RestClient.GetById(RSVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RSVM);
            }
        }

      
    }
}
