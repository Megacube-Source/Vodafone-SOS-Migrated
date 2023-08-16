//Code Review for this file (from security perspective) done

using System;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LSetupRoleSecurityController : PrimaryController
    {
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        IMAspnetRolesGAuthorizableObjectsRestClient RestClient = new MAspnetRolesGAuthorizableObjectsRestClient();
        string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        // GET: LSetupRoleSecurity
        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        //method is used to display mapping grid to map xschema tables column with the lraw data columns
        [ControllerActionFilter]
        public ActionResult MappingDataColumns()
        {
            var CompanyDropdown = GCRC.GetAll();
            ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
            return View();
        }

        //This method gets XSchema Tables list  by CompanyId as a parameter
        [ControllerActionFilter]
        public JsonResult GetRolesByCompanyId(int CompanyId)
        {//LrdtName
            var TableDropdown = RestClient.GetRolesByCompanyId(CompanyId);
            var x = new SelectList(TableDropdown, "RoleId", "RoleName");
            return Json(x, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetColumnMappingGrid(string RoleId)
        {
                var ApiData = RestClient.GetObjectsColumns(RoleId);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult PostGridData(string model, string RoleId)
        {
            try
            {
                //This method passes the comma seperated list of grid items to api for addition/updation
                RestClient.SaveGrid(model,RoleId);
                TempData["Message"] = "Grid Data is updated successfully.";
                return RedirectToAction("MappingDataColumns");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to update Grid Data";
                var CompanyDropdown = GCRC.GetAll();
                ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
                return RedirectToAction("MappingDataColumns");
            }
        }
    }
}