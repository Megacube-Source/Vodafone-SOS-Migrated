//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LWorkflowGridColumnController : PrimaryController
    {
        ILWorkflowGridColumnsRestClient RestClient = new LWorkflowGridColumnsRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        // GET: LWorkflowGridColumn
        [ControllerActionFilter]
        public ActionResult Index(string ConfigId,int RoleId, int CompanyId, int WFId,string Role)
        {
            ViewBag.ConfigId = ConfigId;
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);

            return View();
        }
        [ControllerActionFilter]
        public JsonResult GetColumns(Nullable<int> ConfigId,int RoleId)
        {//LrdtName
            var x = RestClient.GetByConfigId(ConfigId, RoleId);
           // return RedirectToAction("Index");
           return Json(x, JsonRequestBehavior.AllowGet);
        }


        // GET: LWorkFlowActionItem/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id, string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
        {
            LWorkflowGridColumnViewModel rModel = RestClient.GetById(id);
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Grid Column";
            if (rModel == null)
            {
                return HttpNotFound();
            }
            return View(rModel);
        }

        // POST: LWorkFlowActionItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(LWorkflowGridColumnViewModel model, string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
        {
            try
            {
                RestClient.Delete(model.Id);
                return RedirectToAction("Index", new { @ConfigId = ConfigId, @RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId, @Role = Role });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }
        private SelectList GetAscDescdropDown()
        {
            string[] AscDesc = { "asc", "desc"};
            var x = new SelectList(AscDesc);
            return x;
        }

        // GET: LWorkFlowActionItem/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id,string Role,int WFId,int CompanyId,int RoleId,int ConfigId)
        {
            ILWorkFlowConfigRestClient LWCRC = new LWorkFlowConfigRestClient();
            
            LWorkflowGridColumnViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit Grid Column";
            ViewBag.LwfgcAscDesc = GetAscDescdropDown();
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var ColumnNameDropDown = RestClient.GetColumnNameByWFId(WFId);
            ViewBag.LwfgcColumnName = new SelectList(ColumnNameDropDown, "COLUMN_NAME", "COLUMN_NAME",model.LwfgcColumnName);
            var Roles = LWCRC.GetRolesByCompanyId(CompanyId);
            ViewBag.RoleDropDown = new SelectList(Roles, "Id", "Name", RoleId);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: LWorkFlowActionItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(LWorkflowGridColumnViewModel model,int ConfigId,string Role, int WFId, int CompanyId, int RoleId)
        {
            try
            {
                model.LwfgcWfConfigId = ConfigId;
                RestClient.Update(model);
                ViewData["Message"] = "Model updated successfully";
                return RedirectToAction("Index",new { @ConfigId = ConfigId ,@RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId,@Role = Role });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: LWorkFlowActionItem
        [ControllerActionFilter]
        public ActionResult Create(string Role, int WFId,int CompanyId,int RoleId,int ConfigId)
        {
            ILWorkFlowConfigRestClient LWCRC = new LWorkFlowConfigRestClient();
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            ViewBag.LwfgcAscDesc = GetAscDescdropDown();
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var Roles = LWCRC.GetRolesByCompanyId(CompanyId);
            ViewBag.RoleDropDown = new SelectList(Roles, "Id", "Name", RoleId);

            var ColumnNameDropDown = RestClient.GetColumnNameByWFId(WFId);
            ViewBag.LwfgcColumnName = new SelectList(ColumnNameDropDown, "COLUMN_NAME", "COLUMN_NAME");
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            System.Web.HttpContext.Current.Session["Title"] = "Create Grid Column";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LWorkflowGridColumnViewModel model, int ConfigId, string Role, int WFId, int CompanyId, int RoleId)
        {

            try
            {
                model.LwfgcWfConfigId = ConfigId;
                    RestClient.Add(model);
                    return RedirectToAction("Index", new { @ConfigId = ConfigId, @RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId, @Role = Role });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return View(model);
            }

        }
    }
}