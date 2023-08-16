//Code Review for this file (from security perspective) done

using System;
using System.Net;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GKeyValuesController : Controller
    {
        IGKeyValuesRestClient RestClient = new GKeyValuesRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        [ControllerActionFilter]
        public JsonResult GetKeyValues()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get counts for keyvalues detail data
        /// </summary>
        public JsonResult GetCountsForGKeyValueForConfiguration()
        {
            var SummaryCount = RestClient.CountsForGKeyValueForConfiguration();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get all the keyvalues detail from GKeyValue and display on grid
        /// </summary>
        [HttpGet]
        public JsonResult GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetGKeyValueForConfiguration(pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

       

        // GET: GKeyValues
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Key Values";
            return View();
        }

        // GET: GKeyValues/Details/5
        [ControllerActionFilter]
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GKeyValueViewModel gKeyValueViewModel =RestClient.GetById(id);
            if (gKeyValueViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gKeyValueViewModel);
        }

        // GET: GKeyValues/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Key Value";
            return View();
        }

        // POST: GKeyValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.      
       [HttpPost]
       [ValidateInput(false)]
        public ActionResult Create(GKeyValueViewModel GKVM, int compid)
        {
            try
            {

                if (GKVM.GkvValue == null)
                {
                    GKVM.GkvValue = string.Empty;
                }

                GKVM.GkvCompanyId = compid;
                RestClient.Add(GKVM);               
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Key created successfully", RedirectToUrl = "/Home/L2AdminDashboard" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
                //if (ModelState.IsValid)
                //{
                // RestClient.Add(GKVM);
                //  return RedirectToAction("Index");
                // }
                //return View(GKVM);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                                                                   //return View(GKVM);
                var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                return Json(OutputJson2, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: GKeyValues/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Key Value";
            GKeyValueViewModel gKeyValueViewModel =RestClient.GetById(id);
            if (gKeyValueViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gKeyValueViewModel);
        }


        /// <summary>
        /// Created by Rakhi Singh
        /// Method to edit the selected Config and display all values of the fields on PopUp for L2Admin Page
        /// </summary>       
        [HttpGet]
        public JsonResult EditForL2PopUp(int id)
        {
            var ApiData = RestClient.GetById(id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Created by Rakhi Singh
        /// Method to update the selected Config and save in db and display on grid the updated value
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update(GKeyValueViewModel GKVM, int id, int companyid)
        {
            try
            {
                var ApiData = RestClient.GetById(id);
                ApiData.GcCompanyName = GKVM.GcCompanyName;
                ApiData.GkvCompanyId = companyid;
                ApiData.GkvDescription = GKVM.GkvDescription;
                ApiData.GkvKey = GKVM.GkvKey;

                if (GKVM.GkvValue == null)
                {
                    GKVM.GkvValue = string.Empty;
                }
                ApiData.GkvValue = GKVM.GkvValue;

               
                RestClient.Update(ApiData);            
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Key updated successfully", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

              
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                                                                   //return View(GKVM);
                var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                 return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                ////If exception generated from Api redirect control to view page where actions will be taken as per error type.
                //if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                //{
                //    TempData["Error"] = ex.Data["ErrorMessage"] as string;
                //    switch ((int)ex.Data["ErrorCode"])
                //    {
                //        case (int)ExceptionType.Type1:
                //            //redirect user to gneric error page and because of this error message is left blank.
                //            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                //            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                //        case (int)ExceptionType.Type2:
                //            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                //            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                //            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                //        case (int)ExceptionType.Type3:
                //            //Send Ex.Message to the error page which will be displayed as popup
                //            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                //            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                //        case (int)ExceptionType.Type4:
                //            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                //            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                //        default:
                //            throw ex;
                //    }
                //}
                //else
                //{
                //    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                //    TempData["Error"] = "Key could not be updated";
                //    throw ex;
                //}
            }

        }


        // POST: GKeyValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(GKeyValueViewModel gKeyValueViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Update(gKeyValueViewModel);
                    return RedirectToAction("Index");
                }
                return View(gKeyValueViewModel);
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(gKeyValueViewModel);
            }
        }

        // GET: GKeyValues/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Key Value";
            GKeyValueViewModel gKeyValueViewModel =RestClient.GetById(id);
            if (gKeyValueViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gKeyValueViewModel);
        }

        // POST: GKeyValues/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        //public ActionResult DeleteConfirmed(GKeyValueViewModel GKVM)
        /// <summary>
        /// Method updated by Rakhi Singh
        /// Method to delete the selected config data.
        /// </summary>      
        public ActionResult DeleteForL2(int id)
        {
            //try
            //{
            //    RestClient.Delete(GKVM.Id);
            //    return RedirectToAction("Index");
            //}
            //catch(Exception ex)
            //{
            //    ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
            //    return View(GKVM);
            //}
            try
            {
                RestClient.Delete(id);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Configuration deleted successfully", RedirectToUrl = "" };
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
                            var OutputJson1 = new { ErrorMessage = ex.Data["ErrorMessage"].ToString(), PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
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
                    var OutputJson = new { ErrorMessage = "Configuration could not be deleted", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Configuration could not be deleted";
                    //throw ex;
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
