//Code Review for this file (from security perspective) done
//This process is no longer in use hence the file can be discarded if it is not required till version 1.3 release.
using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LCommissionBatchAllocationRulesController : PrimaryController
    {
        #region Methods abandoned by VG/RK on 23-04-2017
        //int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        //ILCommissionBatchAllocationRulesRestClient RestClient = new LCommissionBatchAllocationRulesRestClient();
        //public ActionResult Index()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Manage Allocate Commisions";
        //    return View();
        //}

        //// GET: LCommissionBatchAllocationRule
        ////This method return the view which displays the allocation rules grid
        //[ControllerActionFilter]
        //public ActionResult AllocateCommissionBatch()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Manage Allocate Commisions Batch";
        //    return View();
        //}
        ////This method returns reporting analyst list for the dropdown in view
        //public JsonResult GetReportingAnalystList()
        //{
        //    IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
        //    var Users = RestClient.GetReportingAnalystByCompanyId(CompanyId);

        //    var lJson = Json(Users, JsonRequestBehavior.AllowGet);
        //    return lJson;
        //}

        ////This method loads data in the Allocation rules grid
        //public JsonResult GetCommissionAllocationrules()
        //{
        //    var ApiData = RestClient.GetLCommissionBatchAllocationRulesByCompanyId(CompanyId).ToList();
        //    var LastRow = new LCommissionBatchAllacationRulesViewModel { UserName = "", LrdbarPrimaryChannel = "", LrdbarChannel = "", LrdbarBusinessUnit = "", LrdbarIsDefault = false,Id = 0 };//Adding last row which will be blank for addition
        //    ApiData.Add(LastRow);
        //    var result = ApiData;
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult DeleteCommissionAllocationRule(int id)
        //{
        //    try
        //    {
        //        RestClient.DeleteCommissionAllocationRule(id);
        //        TempData["Message"] = "Allocation Rules sucessfully deleted";
        //        return RedirectToAction("AllocateCommissionBatch");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
        //        return RedirectToAction("AllocateCommissionBatch");
        //    }
        //}


        ////this method saves the Grid data  
        //[HttpPost]
        //public ActionResult SaveCommissionAllocationRule(string model)
        //{
        //    try
        //    {
        //        //This method passes the comma seperated list of grid items to api for addition/updation
        //        RestClient.SaveCommissionAllocationRule(model, CompanyId);
        //        TempData["Message"] = "Allocation Rules sucessfully updated";
        //        return RedirectToAction("AllocateCommissionBatch");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
        //        return RedirectToAction("AllocateCommissionBatch");
        //    }
        //}
        #endregion
    }
}
