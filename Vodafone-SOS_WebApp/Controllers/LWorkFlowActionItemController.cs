//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LWorkFlowActionItemController : PrimaryController
    {
        ILWorkFlowActionItemsRestClient RestClient = new LWorkFlowActionItemsRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        [ControllerActionFilter]
        public ActionResult Index(string ConfigId, int RoleId, int CompanyId, int WFId, string Role)
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
        public JsonResult GetActions( int ConfigId,int RoleId)
        {//LrdtName
            var x = RestClient.GetByConfigId(ConfigId,RoleId);
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        
        // GET: LWorkFlowActionItem/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id, string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
        {
            ILWorkFlowConfigRestClient LWCRC = new LWorkFlowConfigRestClient();
            LWorkFlowActionItemsViewModel model = RestClient.GetById(id);
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var Roles = LWCRC.GetRolesByCompanyId(CompanyId);
            ViewBag.RoleDropDown = new SelectList(Roles, "Id", "Name", RoleId);
            var ShowInTabRoles = LWCRC.GetByWFId(CompanyId, WFId);
            ViewBag.ShowInTabRolesDropDown = new SelectList(ShowInTabRoles, "Id", "Name", model.LwfaiShowInTabWFConfigId);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Action Item";
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: LWorkFlowActionItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(LWorkFlowActionItemsViewModel model, string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
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
        
        // GET: LWorkFlowActionItem/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id, string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
        {
            ILWorkFlowConfigRestClient LWCRC = new LWorkFlowConfigRestClient();
            LWorkFlowActionItemsViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit ActionItem";
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var Roles = LWCRC.GetRolesByCompanyId(CompanyId);
            ViewBag.RoleDropDown = new SelectList(Roles, "Id", "Name", RoleId);
            var ShowInTabRoles =  LWCRC.GetByWFId(CompanyId, WFId);
            ViewBag.ShowInTabRolesDropDown = new SelectList(ShowInTabRoles, "Id", "Name", model.LwfaiShowInTabWFConfigId);
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
        public ActionResult Edit(LWorkFlowActionItemsViewModel model, int ConfigId, string Role, int WFId, int CompanyId, int RoleId,int ShowInTabRolesDropDown)
        {
            try
            {
                model.LwfaiLoginWFConfigId = ConfigId;
                model.LwfaiShowInTabWFConfigId = ShowInTabRolesDropDown;
                RestClient.Update(model);
               // return RedirectToAction("Index");
                return RedirectToAction("Index", new { @ConfigId = ConfigId, @RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId, @Role = Role });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: LWorkFlowActionItem
        [ControllerActionFilter]
        public ActionResult Create(string Role, int WFId, int CompanyId, int RoleId, int ConfigId)
        {

            System.Web.HttpContext.Current.Session["Title"] = "Create Action Item";
            ILWorkFlowConfigRestClient LWCRC = new LWorkFlowConfigRestClient();
            System.Web.HttpContext.Current.Session["Title"] = "Edit ActionItem";
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.ConfigId = ConfigId;
            var WFDropDown = RWFRC.Get();
            ViewBag.WFId = new SelectList(WFDropDown, "Id", "RwfName", WFId);
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName", CompanyId);
            var Roles = LWCRC.GetRolesByCompanyId(CompanyId);
            ViewBag.RoleDropDown = new SelectList(Roles, "Id", "Name", RoleId);
            var ShowInTabRoles = LWCRC.GetByWFId(CompanyId, WFId);
            ViewBag.ShowInTabRolesDropDown = new SelectList(ShowInTabRoles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LWorkFlowActionItemsViewModel model, string Role, int WFId, int CompanyId, int RoleId, int ConfigId,int ShowInTabRolesDropDown)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    model.LwfaiLoginWFConfigId = ConfigId;
                    model.LwfaiShowInTabWFConfigId = ShowInTabRolesDropDown;
                    RestClient.Add(model);
                   // return RedirectToAction("Index");
                    return RedirectToAction("Index", new { @ConfigId = ConfigId, @RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId, @Role = Role });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                // return View(model);
                return RedirectToAction("Index", new { @ConfigId = ConfigId, @RoleId = RoleId, @CompanyId = CompanyId, @WFId = WFId, @Role = Role });
            }

        }
    }
}