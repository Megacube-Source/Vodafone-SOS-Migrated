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
    public class RStatusOwnersController : PrimaryController
    {
        IRStatusOwnersRestClient RestClient = new RStatusOwnersRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //method to get data in grid in index page 
        [ControllerActionFilter]
        public JsonResult GetStatusOwners()
        {
            var apiData = RestClient.GetAll() ;
            return Json(apiData,JsonRequestBehavior.AllowGet);
        }

        // GET: RStatusOwners
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Status Owners";
            return View();
        }

        // GET: RStatusOwners/Details/5
        //public ActionResult Details(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RStatusOwnerViewModel rStatusOwnerViewModel = RestClient.GetById(id);
        //    if (rStatusOwnerViewModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rStatusOwnerViewModel);
        //}

        // GET: RStatusOwners/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Status Owner";
            return View();
        }

        // POST: RStatusOwners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create([Bind(Include = "Id,RsoStatusOwner")] RStatusOwnerViewModel RSOVM)
        {
            try
            {
                    RestClient.Add(RSOVM);
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RSOVM);
            }
        }

        // GET: RStatusOwners/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Status Owner";
            RStatusOwnerViewModel rStatusOwnerViewModel =RestClient.GetById(id);
            if (rStatusOwnerViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rStatusOwnerViewModel);
        }

        // POST: RStatusOwners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit([Bind(Include = "Id,RsoStatusOwner")] RStatusOwnerViewModel rStatusOwnerViewModel)
        {
            if (ModelState.IsValid)
            {
                RestClient.Update(rStatusOwnerViewModel);
                return RedirectToAction("Index");
            }
            return View(rStatusOwnerViewModel);
        }

        // GET: RStatusOwners/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Status Owner";
            RStatusOwnerViewModel rStatusOwnerViewModel = RestClient.GetById(id);
            if (rStatusOwnerViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rStatusOwnerViewModel);
        }

        // POST: RStatusOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RStatusOwnerViewModel RSOVM)
        {
            try
            {
                RestClient.Delete(RSOVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(RSOVM);
            }
        }

    }
}
