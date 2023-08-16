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
    public class LDropDownValuesController : PrimaryController
    {
        string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        string LoggedInUserId = System.Web.HttpContext.Current.Session["UserId"] as string;
        ILDropDownValuesRestClient RestClient = new LDropDownValuesRestClient();
        String DropdownName = "";
        int DropDownId = 0;
        // GET: LDropDownValues
        [ControllerActionFilter]
        public ActionResult Index(int Id, string DropDownName)
        {
            DropDownId = Id;
            DropdownName = DropDownName;
            ViewBag.DropDownName = DropDownName;
            ViewBag.DropDownId = Id;
            return View();
        }

        //Get List of DropdownValues for that DropDownId
        [ControllerActionFilter]
        public JsonResult GetDropDownValues(int DropDownId)
        {
            var ApiData = RestClient.GetByDropDownId(DropDownId).ToList();
            var LastEmptyRow = new LDropDownValueViewModel { };
           // ApiData.Add(LastEmptyRow);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult EditDropDownValues(object[] GridData,int DropDownId)
        {
            try
            {
                foreach (string Data in GridData)
                {
                    var GridArray = Data.Split(',');
                    var model = new LDropDownValueViewModel {  Id = Convert.ToInt32(GridArray[0]), LdvValue = GridArray[1], LdvDescription = GridArray[2] ,LdvDropDownId=DropDownId};
                    RestClient.Add(model, null);
                }
                //if (model.Id == 0)//entry does not exist in database
                //{
                //    RestClient.Add(model,null);
                //}
                //else
                //{
                //    RestClient.Update(model,null);
                //}
                var OutputJson = new { ErrorMessage = "Create DropDown Value", PopupMessage = "", RedirectToUrl = "/LDropDownValues/Index?Id="+DropDownId+ "&DropDownName=" + DropdownName};
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
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

        [ControllerActionFilter]
        public JsonResult DeleteDropDownValues(int Id,int DropdownId)
        {
            try
            {
                RestClient.Delete(Id,null, DropdownId);
                var OutputJson = new { ErrorMessage = "Create DropDown Value", PopupMessage = "", RedirectToUrl = "" };
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