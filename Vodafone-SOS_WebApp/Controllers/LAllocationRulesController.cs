//Code Review for this file (from security perspective) done
//This process is no longer in use hence the file can be discarded if it is not required till version 1.3 release.
using System;
//RK Commented during code review using System.Collections.Generic;
using System.Linq;
//RK Commented during code review using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire] //RK added during code review
    [HandleCustomError]//RK added during code review
    public class LAllocationRulesController : Controller
    {
        #region Dead Code on 10062017 by VG/RK
        //int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //ILAllocationRulesRestClient RestClient = new LAllocationRulesRestClient();
        //// GET: LAllocationRules
        ////This method return the view which displays the allocation rules grid
        //[ControllerActionFilter]
        //public ActionResult Index()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Manage Allocation Rules";
        //    return View();
        //}
        ////This method returns claims analyst list for the dropdown in view
        //[ControllerActionFilter]
        //public JsonResult GetClaimsAnalystList()
        //{
        //    IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
        //    var Users = ARRC.GetClaimsAnalystByCompanyId(CompanyId);
        //    return Json(Users,JsonRequestBehavior.AllowGet);
        //}
        ////This method loads data in the Allocation rules grid
        //[ControllerActionFilter]
        //public JsonResult GetAllocationrules() 
        //{
        //    var ApiData = RestClient.GetByCompanyId(CompanyId).ToList();
        //    var LastRow = new LAllocationRuleViewModel { UserName="", LarOrdinalPosition = ApiData.Count+1, LarValue = "", Id = 0 };//Adding last row which will be blank for addition
        //    ApiData.Add(LastRow);
        //    var result = ApiData.OrderBy(p=>p.LarOrdinalPosition);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]//RK added during code review
        //[ControllerActionFilter]
        //public ActionResult SaveGridData(string model)
        //{
        //    try
        //    {
        //        //This method passes the comma seperated list of grid items to api for addition/updation
        //        RestClient.AddGridData(model,CompanyId);
        //        TempData["Message"] = "Allocation Rules sucessfully updated";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
        //        return RedirectToAction("Index");
        //    }
        //}
        //[ControllerActionFilter]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        RestClient.Delete(id);
        //        TempData["Message"] = "Allocation Rules sucessfully deleted";
        //        return RedirectToAction("Index");
        //    }
        //    catch(Exception ex)
        //    {
        //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
        //        return RedirectToAction("Index");
        //    }
        //}
        ////Method added by shivanig
        //[ControllerActionFilter]
        //public ActionResult AllocateCommissions()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Manage Allocate Commisions";
        //    return View();
        //}
        #endregion

    }
}