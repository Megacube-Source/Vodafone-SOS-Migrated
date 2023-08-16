//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LWorkFlowConfigController : PrimaryController
    {
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        ILWorkFlowConfigRestClient RestClient = new LWorkFlowConfigRestClient();


        // GET: RWorkFlowConfiguration
        [ControllerActionFilter]
        public ActionResult Index()
        {

            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
            var WFDropDown = RWFRC.Get();
            ViewBag.WFID = new SelectList(WFDropDown, "Id", "RwfName");
            return View();
        }


        [ControllerActionFilter]
        public JsonResult GetGrid(int CompanyId, int WFId)
        {//LrdtName
            var x = RestClient.GetByWFId(CompanyId,WFId);
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        
        // GET: RWorkFlows/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            LWorkflowConfigViewModel rModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete WorkFlow Configuration";
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
        public ActionResult DeleteConfirmed(LWorkflowConfigViewModel model)
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


        // GET: RWorkFlows/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            LWorkflowConfigViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit WorkFlow Configuration";
            ViewBag.LwfcActingAs = GetActingAsdropDown();
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.LwfcCompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
            var WFDropDown = RWFRC.Get();
            ViewBag.LwfcWorkFlowId = new SelectList(WFDropDown, "Id", "RwfName");
            var Roles = RestClient.GetRolesByCompanyId(model.LwfcCompanyId);
            ViewBag.LwfcRoleId = new SelectList(Roles, "Id", "Name",model.Name);
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
        public ActionResult Edit(LWorkflowConfigViewModel model)
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

        // GET: RWorkFlows
        [ControllerActionFilter]
        public ActionResult Create(int CompanyId, int WFId)
        {

            System.Web.HttpContext.Current.Session["Title"] = "Create WorkFlow Configuration";
            ViewBag.LwfcActingAs = GetActingAsdropDown();
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.LwfcCompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName",CompanyId);
            var WFDropDown = RWFRC.Get();
            ViewBag.LwfcWorkFlowId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var Roles = RestClient.GetRolesByCompanyId(CompanyId);
            ViewBag.LwfcRoleId = new SelectList(Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LWorkflowConfigViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Add(model);
                    TempData["Message"] = "WorkFlow Configuration added successfully";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return View(model);
            }

        }


        //This method will return dropdown data to the ActingAs 
        [ControllerActionFilter]
        private SelectList GetActingAsdropDown()
        {
            string[] ActingAs = { "Manager","Requester","Analyst" };
            var x = new SelectList(ActingAs);
            return x;
        }
        //This method gets XSchema Tables list  by CompanyId as a parameter
        [ControllerActionFilter]
        public JsonResult GetRolesByCompanyId(int CompanyId)
        {//LrdtName
            var TableDropdown = RestClient .GetRolesByCompanyId(CompanyId);
            var x = new SelectList(TableDropdown, "Id", "RoleName");
            return Json(x, JsonRequestBehavior.AllowGet);
        }


        //This method will return dropdown data to the ActingAs 
        /*   private SelectList GetRoleDropDown(int id)
           {
               var roles = RestClient.GetParentDropDown(CompanyId);
               var x = new SelectList(roles, "Id", "FullName", id);
               return x;
           }*/

    }
}