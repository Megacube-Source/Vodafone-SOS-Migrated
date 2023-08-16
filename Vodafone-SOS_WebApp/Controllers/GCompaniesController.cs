//Code Review for this file (from security perspective) done

using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GCompaniesController : PrimaryController
    {
        IGCompaniesRestClient RestClient = new GCompaniesRestClient();
        string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        [ControllerActionFilter]
        public JsonResult GetCompanies()
        {
            var ApiData = RestClient.GetAll();
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        // GET: GCompanies
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Companies";
            return View();
        }

        // GET: GCompanies/Details/5
        //[ControllerActionFilter]
        //public ActionResult Details(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    GCompanyViewModel gCompanyViewModel = RestClient.GetById(id);
        //    if (gCompanyViewModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(gCompanyViewModel);
        //}

        // GET: GCompanies/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Company";
            return View();
        }

        // POST: GCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( GCompanyViewModel GCVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   RestClient.Add(GCVM,null);
                    
                }

                return View(GCVM);
            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(GCVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(GCVM);
                    default:
                        throw ex;
                }

            }
        }

        // GET: GCompanies/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Company";
            GCompanyViewModel gCompanyViewModel = RestClient.GetById(id);
            if (gCompanyViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gCompanyViewModel);
        }

        // POST: GCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit([Bind(Include = "Id,GcCompanyName")] GCompanyViewModel gCompanyViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Update(gCompanyViewModel,null);
                    return RedirectToAction("Index");
                }
                return View(gCompanyViewModel);
            }
            catch(Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page in case of Type 1
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page from where request was initiated in case of Type 2
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(gCompanyViewModel);
                    case (int)ExceptionType.Type3:
                        //Send Exception Message to the error page which will be displayed as popup and then redirect User to error screen in case of type 3
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        //Display Exception Message as popup and remain in the same page from where request was initiated in case of type 4
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(gCompanyViewModel);
                    default:
                        //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App.
                        throw ex;
                }
            }
        }

        // GET: GCompanies/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Company";
            GCompanyViewModel gCompanyViewModel =RestClient.GetById(id);
            if (gCompanyViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gCompanyViewModel);
        }

        // POST: GCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(GCompanyViewModel GCVM)
        {
            try
            {
                RestClient.Delete(GCVM.Id,null);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(GCVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(GCVM);
                    default:
                        throw ex;
                }
            }
        }
    }
}
