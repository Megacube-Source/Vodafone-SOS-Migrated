//Code Review for this file (from security perspective) done

using System;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;
using System.Web.Mvc;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LWorkFlowActionParameterController : PrimaryController
    {

        ILWorkFlowActionParametersRestClient RestClient = new LWorkFlowActionParametersRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();

        // GET: LWorkFlowActionParameter
        [ControllerActionFilter]
        public ActionResult Index(int ActionItemId, int CompanyId, int WFId)
        {
            ViewBag.ActionItemId = ActionItemId;
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetGrid(int ActionItemId)
        {//LrdtName
            var x = RestClient.GetByActionItemId(ActionItemId);
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        
        // GET: LWorkFlowActionParameter/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id, int CompanyId, int WFId)
        {
            LWorkFlowActionParameterViewModel rModel = RestClient.GetById(id);
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Action Parameter";
            if (rModel == null)
            {
                return HttpNotFound();
            }
            return View(rModel);
        }

        // POST: LWorkFlowActionParameter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(LWorkFlowActionParameterViewModel model)
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
        
        // GET: LWorkFlowActionParameter/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id, int ActionItemId, int WFId, int CompanyId)
        {
            ILWorkFlowActionItemsRestClient LARC = new LWorkFlowActionItemsRestClient();
            LWorkFlowActionParameterViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit Action Parameter";
            ViewBag.ActionItemId = ActionItemId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var ActionItemVar = LARC.GetById(ActionItemId);
            ViewBag.ActionItem = ActionItemVar.LwfaiActionItemName;
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: LWorkFlowActionParameter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(LWorkFlowActionParameterViewModel model, int ActionItemId ,int CompanyId,int WFId)
        {
            try
            {
                model.WFActionItemId = ActionItemId;
                RestClient.Update(model);
                return RedirectToAction("Index", new { ActionItemId = ActionItemId, @CompanyId = CompanyId, @WFId = WFId });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: LWorkFlowActionParameter
        [ControllerActionFilter]
        public ActionResult Create(int ActionItemId, int CompanyId, int WFId)
        {
            ILWorkFlowActionItemsRestClient LARC = new LWorkFlowActionItemsRestClient();
            System.Web.HttpContext.Current.Session["Title"] = "Create Action Parameters";
            ViewBag.ActionItemId = ActionItemId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
           var  ActionItemVar = LARC.GetById(ActionItemId);
            ViewBag.ActionItem = ActionItemVar.LwfaiActionItemName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LWorkFlowActionParameterViewModel model, int ActionItemId, int CompanyId, int WFId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.WFActionItemId = ActionItemId;
                    RestClient.Add(model);
                    return RedirectToAction("Index", new { ActionItemId = ActionItemId, @CompanyId = CompanyId, @WFId = WFId });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return View(model);
            }
        }
    }
}