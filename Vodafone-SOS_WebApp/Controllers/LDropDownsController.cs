using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LDropDownsController : PrimaryController
    {
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        
        ILDropDownsRestClient RestClient = new LDropDownsRestClient();
        // GET: LDropDowns
        [ControllerActionFilter]
        public ActionResult Index()
        {
            ViewBag.DropDownCounts = RestClient.GetCountsByCompanyId(CompanyId);
            System.Web.HttpContext.Current.Session["Title"] = "Manage CustomDropdown";
            return View();
        }

        //Get List of Dropdowns for the Current Opco
        [ControllerActionFilter]
        public JsonResult GetDropDowns()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId).ToList();
            var LastEmptyRow = new LDropDownViewModel { };
           // ApiData.Add(LastEmptyRow);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ControllerActionFilter]
        public JsonResult EditDropDowns(object[] GridData)
        {
            try
            {
                foreach (string Data in GridData)
                {
                    var GridArray = Data.Split(',');
                    var model = new LDropDownViewModel{LdCompanyId=CompanyId,Id=Convert.ToInt32(GridArray[0]),LdName=GridArray[1],LdDescription=GridArray[2] };
                    RestClient.Add(model, null);
                }
                //if (model.Id == 0)//entry does not exist in database
                //{
                //    RestClient.Add(model, null);
                //}
                //else
                //{
                //    RestClient.Update(model, null);
                //}
                var OutputJson = new { ErrorMessage = "Create DropDown", PopupMessage = "", RedirectToUrl = "/LDropDowns/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    TempData["Error"] = ex.Data["ErrorMessage"] as string;
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LDropDowns/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LDropDowns/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LDropDowns/Index" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }

        [ControllerActionFilter]
        public JsonResult DeleteDropDowns(int Id)
        {
            try
            {
                RestClient.Delete(Id,null,CompanyId);
                var OutputJson = new { ErrorMessage = "Create DropDown ", PopupMessage = "", RedirectToUrl = "" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }

    }
}