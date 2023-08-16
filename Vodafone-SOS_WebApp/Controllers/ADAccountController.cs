using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class ADAccountController : PrimaryController
    {

        IADAccountRestClient RestClient = new ADAccountRestClient();
        // GET: ADAccount
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Welcome Page";
            return View();
        }

        // GET: ADAccount
        public ActionResult Create()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Create Active Directory User";
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LoginViewModel model)
        {
            //  string replacedString = ReplaceSpecialChar(model.EmailAddress);
            //  model.EmailAddress = replacedString;
            // model.SamAccountName = replacedString;
            try
            {
                RestClient.CreateUser(model);
                ViewData["Message"] = "User created successfully.";
                return View(new LoginViewModel());
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: ADAccount
        public ActionResult ChangePassword(string Email)
        {
            ViewBag.Email = Email;
            System.Web.HttpContext.Current.Session["Title"] = "Change Password";            
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordBindingModel model)
        {
            try
            {
                RestClient.ChangeUserPassword(model);
               // ViewData["SuccessMessage"] = "Password updated successfully.";
                TempData["Message"] = "Password updated successfully.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }


        // GET: ADAccount
        public ActionResult SetUserPassword()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Change Password";
            return View();
        }

        [HttpPost]
        public ActionResult SetUserPassword(ChangePasswordBindingModel model)
        {
            try
            {
                RestClient.ChangeUserPassword(model);
                // ViewData["SuccessMessage"] = "Password updated successfully.";
                TempData["Message"] = "Password updated successfully.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }


        // GET: ADAccount
        public ActionResult Login()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Login User";

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                var result = RestClient.LoginUser(model);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                   
                    return RedirectToAction("ChangePassword", "ADAccount", new { Email = model.Email });
                 }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }



        // GET: ADAccount
        public ActionResult Update()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Edit User Details";

            return View();
        }

        [HttpPost]
        public ActionResult Update(LoginViewModel model)
        {
            try
            {
                RestClient.UpdateDetails(model);
                return View();
               
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        // GET: ADAccount
        public ActionResult Delete()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Remove User";

            return View();
        }


        [ControllerActionFilter]
        [HttpPost]
        public ActionResult Delete(LoginViewModel model)
        {
            try
            {
                 RestClient.DeleteADUser(model);
                return View();

            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        // GET: ADAccount
        public ActionResult Enable()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Enable AD User";

            return View();
        }
        [ControllerActionFilter]
        [HttpPost]
        public ActionResult Enable(LoginViewModel model)
        {
            try
            {
                model.Status = true;
                RestClient.EnableDisableADUser(model);
                return View();

            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        // GET: ADAccount
        public ActionResult Disable()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Disable AD User";
            return View();
        }

        [ControllerActionFilter]
        [HttpPost]
        public ActionResult Disable(LoginViewModel model)
        {
            try
            {
                model.Status = false;
                RestClient.EnableDisableADUser(model);
                return View();

            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        // GET: ADAccount
        public ActionResult Unlock()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Unlock User";
            return View();
        }

        [ControllerActionFilter]
        [HttpPost]
        public ActionResult Unlock(UnlockUserViewModel model)
        {
            try
            {
                bool emailchk;
                emailchk=RestClient.UnlockADUser(model);
                if(emailchk==true)
                TempData["Message"] = "User Unlocked successfully.";
                //Thread.Sleep(300);
                //return RedirectToAction("L2AdminDashboard", "Home");
                
                else
                    TempData["Error"] = "Please Check the Email";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                //ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        // GET: ADAccount
        public ActionResult CreateLogin()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Login";
            CreateLoginViewModel model = new CreateLoginViewModel(); 
            ViewBag.UserType = GetUserType(); ;
            return View(model);
        }
        private SelectList GetUserType()
        { 
            IList<SelectListItem> UserType = new List<SelectListItem>
                                            {
                                                new SelectListItem { Text = "Payee", Value = "Payee"},
                                                new SelectListItem { Text = "User", Value = "User"},
                                            };
            var x = new SelectList(UserType, "Value", "Text" ,"Select");
            return x;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreateLogin(CreateLoginViewModel model)
        {
            try
            {
                ViewBag.UserType = GetUserType();
                RestClient.CreateLogin(model);
               
                TempData["SuccessMessage"] = "Data Updated successfully";

                var OutputJson = new { ErrorMessage = "", PopupMessage = "Data  updated successfully", RedirectToUrl = "/ADAccount/CreateLogin" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                return Json(OutputJson2, JsonRequestBehavior.AllowGet);
            }
        }

        //this method replaces all specail characters with the underscore(_)
        //Abandoned because of Non Use VG/RK - 23/04/2017
        //public string ReplaceSpecialChar(string username)
        //{
        //    string newstr = Regex.Replace(username, @"[^0-9a-zA-Z]+", "_");
        //    return newstr;
        //}

    }
}