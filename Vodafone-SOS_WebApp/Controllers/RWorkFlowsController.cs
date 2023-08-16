using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.ViewModels;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RWorkFlowsController : PrimaryController
    {
        IRWorkFlowsRestClient RestClient = new RWorkFlowsRestClient();

        [ControllerActionFilter]
        public JsonResult GetWorkFlows()
        {
            var ApiData = RestClient.Get();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage WorkFlows";
            return View();
        }

        // GET: RWorkFlows
        [ControllerActionFilter]
        public ActionResult CreateRWorkFlows()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create WorkFlows";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreateRWorkFlows(RWorkFlowViewModel RWFVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Add(RWFVM);
                    TempData["Message"] = "WorkFlow created successfully";
                    return RedirectToAction("Index");
                }
                return View(RWFVM);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return View(RWFVM);
            }
        }

        // GET: RWorkFlows/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            RWorkFlowViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit WorkFlow";
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: RWorkFlows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(RWorkFlowViewModel model)
        {
            try
            {
                RestClient.Update(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: RWorkFlows/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            RWorkFlowViewModel rModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete WorkFlow";
            if (rModel == null)
            {
                return HttpNotFound();
            }
            return View(rModel);
        }

        // POST: RWorkFlows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RWorkFlowViewModel model)
        {
            try
            {
                RestClient.Delete(model.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        //public JsonResult GetCountsForCompletedItems()
        //{
        //    var SummaryCount = RestClient.GetCountsForCompletedItems();
        //    return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        //}


        //[HttpGet]
        //public JsonResult GetCompletedItems(int pagesize, int pagenum, string sortdatafield, string sortorder)
        //{
        //    var qry = Request.QueryString;
        //    var FilterQuery = Globals.BuildQuery(qry);
        //    var ApiData = RestClient.GetCompletedItems();
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
        //Method to get the total counts for Exception for Summary
        public JsonResult GetCountsForCompletedItems()
        {
            var ItemCount = RestClient.GetCountsForCompletedItems();
            return Json(ItemCount, JsonRequestBehavior.AllowGet);
        }

        //Method to get the summary data
        public JsonResult GetCompletedItems(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetCompletedItems(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery); 
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
    }
}