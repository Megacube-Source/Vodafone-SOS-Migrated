using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using System.Collections;
using System.Web.SessionState;
using Vodafone_SOS_WebApp.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    //commented by shubham sharma as this attribute will not work for now as database connectivity to this project is removed
    // [Authorize]
    // [SessionExpire]//Commented this line because it is not letting index page to be loaded. Need to restore this line after investigation of issue and finding a resolution to this.
    [HandleCustomError]
    public class AccountController : Controller
    {
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        IAccountsRestClient ARC = new AccountsRestClient();
        IAspnetUsersRestClient AUR = new AspnetUsersRestClient();
        IGUserActivityLogRestClient GUA = new GUserActivityLogRestClient();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

       
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl,string ClientIPAddress)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                    //return View(model);
                }
                IAccountsRestClient ACRC = new AccountsRestClient();
                var result = ACRC.GetUser(model);
                if (result != null)
                {
                    //Check if User has accepted Policy otherwise redirect him to the page to do the same
                    if(!result.PolicyAccepted)
                    {
                        return RedirectToAction("AcceptPolicies", "Account",result);
                    }

                   

                    string SelectedLandingPageUserRole = ARC.GetSelectedLandingPage(result.Id);
                    string SelectedLandingPageUserRoleID = string.Empty;
                    Boolean SelectedLandingPageUserRoleShowOnDashBoard = false;

                    if (SelectedLandingPageUserRole == null || SelectedLandingPageUserRole == "")
                    {
                        SelectedLandingPageUserRole = result.Roles.ElementAt(0).Name;
                        SelectedLandingPageUserRoleID = result.Roles.ElementAt(0).Id;
                        SelectedLandingPageUserRoleShowOnDashBoard = result.Roles.ElementAt(0).ShowDashboard;
                    }
                    else
                    {
                        foreach (AspnetRoleViewModel objRoles in result.Roles)
                        {
                            if (objRoles.Name == SelectedLandingPageUserRole)
                            {
                                SelectedLandingPageUserRoleID = objRoles.Id;
                                SelectedLandingPageUserRoleShowOnDashBoard = objRoles.ShowDashboard;
                            }
                        }
                    }

                    Globals.SetSessionVariable(result.GcCompanyId, result.Roles, result.Id, result.Email, result.FirstName, result.LastName, result.PhoneNumber, result.LUserId, SelectedLandingPageUserRole, SelectedLandingPageUserRoleID, SelectedLandingPageUserRoleShowOnDashBoard);
                    Globals.LogUserEvent(result.Id, "LoggedIn", "Self Login", true, result.Id, result.GcCompanyId, ClientIPAddress);


                    if (result.ObjScurityQuestion == null || result.ObjScurityQuestion.Count == 0)
                    {
                        //return RedirectToAction("ResetSecurityQuestionAnswer", "Account", result);
                        Session["LandingPage"] = "SecurityQuestions";
                        TempData["SelectedLandingPageUserRoleShowOnDashBoard"] = SelectedLandingPageUserRoleShowOnDashBoard;
                        TempData["SelectedLandingPageUserRole"] = SelectedLandingPageUserRole;
                        return RedirectToAction("ResetSecurityQuestionAnswer", "Account", new { @Email = model.Email });
                    }

                    


                    //if (!result.Roles.ElementAt(0).ShowDashboard)
                    if (!SelectedLandingPageUserRoleShowOnDashBoard)
                    {
                        // switch (result.Roles.ElementAt(0).Name)
                         switch (SelectedLandingPageUserRole)
                        {
                            case "L3 Admin":
                                return RedirectToAction("SOSAdminDashboard", "Home");
                            case "L2 Admin":
                                return RedirectToAction("L2AdminDashboard", "Home");
                            case "L2 Alteryx":
                                return RedirectToAction("L2AdminDashboard", "Home");
                            default:
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "", Role = " " + SelectedLandingPageUserRole });
                            

                            ////case "Reporting Analyst":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Reporting Analyst Welcome Page", Role = " Reporting Analyst" });
                            //////return RedirectToAction("AnalystDashBoard", "Home");
                            ////case "Claims Analyst":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Claims Analyst Welcome Page", Role = " Claims Analyst" });
                            //////return RedirectToAction("ClaimsAnalystDashboard", "Home");
                            ////case "Payee":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Payee Welcome Page", Role = " Payee" });
                            //////return RedirectToAction("PayeeDashboard", "Home");
                            ////case "Manager":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Manager Welcome Page", Role = " Manager" });
                            //////return RedirectToAction("ManagerDashboard", "Home");
                            ////case "Sales Operations":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Sales Op Welcome Page", Role = " Sales Operations" });
                            //////return RedirectToAction("SalesOperationsDashboard", "Home");
                            ////case "L3 Admin":
                            ////    return RedirectToAction("SOSAdminDashboard", "Home");
                            ////case "L2 Admin":
                            ////    return RedirectToAction("L2AdminDashboard", "Home");
                            ////case "L2 Alteryx":
                            ////    // return RedirectToAction("GenericDashboard", "Home", new { Title = "Alteryx Admin Welcome Page", Role = " Alteryx Admin" });
                            ////    return RedirectToAction("L2AdminDashboard", "Home");
                            ////case "L2 L2 Infra":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Infra Admin Welcome Page", Role = " Infra Admin" });
                            ////case "System Analyst":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "System Analyst Welcome Page", Role = " System Analyst" });
                            //////return RedirectToAction("SystemAnalystDashboard", "Home");
                            ////case "Auditor":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Auditor Welcome Page", Role = " Auditor" });
                            //////return RedirectToAction("AuditorDashboard", "Home");
                            ////case "Head of Finance Operations":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "HOFO Welcome Page", Role = " Head of Finance Operations" });
                            //////return RedirectToAction("HOFODashboard", "Home");
                            ////case "Account Analyst":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Account Analyst Welcome Page", Role = " Account Analyst" });
                            //////return RedirectToAction("AccountAnalystDashboard", "Home");
                            ////case "Controller":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Controller Welcome Page", Role = " Controller" });
                            //////return RedirectToAction("ControllerDashboard", "Home");
                            ////case "Channel Manager":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Channel Manager Welcome Page", Role = " Channel Manager" });
                            //////return RedirectToAction("ChannelManagerDashboard", "Home");
                            ////case "Monitor":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "Monitor Welcome Page", Role = " Monitor" });
                            ////    //return RedirectToAction("MonitorDashboard", "Home");
                            ////case "HR":
                            ////    return RedirectToAction("GenericDashboard", "Home", new { Title = "HR Welcome Page", Role = " HR" });
                            //////return RedirectToAction("HRDashboard", "Home");
                            ////default:
                            ////    return RedirectToAction("NoRolesAssigned", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("GraphicalDashboard", "Home");
                    }
                    
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                //switch (result)
                //{
                //    case SignInStatus.Success:
                //        return RedirectToLocal(returnUrl);
                //    case SignInStatus.LockedOut:
                //        return View("Lockout");
                //    case SignInStatus.RequiresVerification:
                //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                //    case SignInStatus.Failure:
                //    default:
                //        ModelState.AddModelError("", "Invalid login attempt.");
                //        return View(model);
                //}
                TempData["Message"] = "Role not assigned to user";
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                //AD Integration code changes starts here
                if (ex.Data["ErrorMessage"].ToString().Contains("reset"))
                {
                    return RedirectToAction("ResetPassword", "Account",new { Email = model.Email});
                }
                if (ex.Data["ErrorMessage"].ToString().Contains("MFAScreen"))
                {
                    TempData["Login"] = model;
                    return RedirectToAction("MFAScreen"/*, "Account",model*/);
                }

                TempData["Message"] = ex.Data["ErrorMessage"];//"UserName or Password is Incorrect";
                return RedirectToAction("Index", "Home");
            }
        }

        //Policy Acceptance Page
        public ActionResult AcceptPolicies(LoginViewModel model)
        {
            IGKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            var Policy= KVRC.GetByName("FirstTimeLoginPolicy", model.GcCompanyId);
            if(Policy!=null)
            ViewBag.PolicyText = Policy.GkvValue;
            return View(model);
        }

        //Update Policy Acceptance and login User
        public ActionResult UpdateAcceptPolicies(string UserId,string Email,string ClientIPAddress)
        {
            IAccountsRestClient ACRC = new AccountsRestClient();
            ACRC.UpdatePolicyCheck(UserId);
            var result = ACRC.GetUserInformation(Email);
            if (result != null)
            {
                //Check if User has accepted Policy otherwise redirect him to the page to do the same
                if (!result.PolicyAccepted)
                {
                    return RedirectToAction("AcceptPolicies", "Account", result);
                }
                string SelectedLandingPageUserRole = ARC.GetSelectedLandingPage(result.Id);
                string SelectedLandingPageUserRoleID = string.Empty;
                Boolean SelectedLandingPageUserRoleShowOnDashBoard = false;

                if (SelectedLandingPageUserRole == null || SelectedLandingPageUserRole == "")
                {
                    SelectedLandingPageUserRole = result.Roles.ElementAt(0).Name;
                    SelectedLandingPageUserRoleID = result.Roles.ElementAt(0).Id;
                    SelectedLandingPageUserRoleShowOnDashBoard = result.Roles.ElementAt(0).ShowDashboard;
                }
                else
                {
                    foreach (AspnetRoleViewModel objRoles in result.Roles)
                    {
                        if (objRoles.Name == SelectedLandingPageUserRole)
                        {
                            SelectedLandingPageUserRoleID = objRoles.Id;
                            SelectedLandingPageUserRoleShowOnDashBoard = objRoles.ShowDashboard;
                        }
                    }
                }



                Globals.SetSessionVariable(result.GcCompanyId, result.Roles, result.Id, result.Email, result.FirstName, result.LastName, result.PhoneNumber, result.LUserId, SelectedLandingPageUserRole, SelectedLandingPageUserRoleID, SelectedLandingPageUserRoleShowOnDashBoard);
                Globals.LogUserEvent(result.Id, "LoggedIn", "Self Login", true, result.Id, result.GcCompanyId, ClientIPAddress);
                // if (!result.Roles.ElementAt(0).ShowDashboard)
                if (!SelectedLandingPageUserRoleShowOnDashBoard)
                {
                    // switch (result.Roles.ElementAt(0).Name)
                    switch (SelectedLandingPageUserRole)
                    {
                        case "L3 Admin":
                            return RedirectToAction("SOSAdminDashboard", "Home");
                        case "L2 Admin":
                            return RedirectToAction("L2AdminDashboard", "Home");
                        case "L2 Alteryx":
                            return RedirectToAction("L2AdminDashboard", "Home");
                        default:
                            return RedirectToAction("GenericDashboard", "Home", new { Title = "", Role = " " + SelectedLandingPageUserRole });

                    }
                }
                else
                {
                    return RedirectToAction("GraphicalDashboard", "Home");
                }
               
            }
            
            TempData["Message"] = "Role not assigned to user";
            return RedirectToAction("Index", "Home");
        }

        //MFA OTP Screen
        public ActionResult MFAScreen(/*LoginViewModel model*/ )
        {
            LoginViewModel model = new LoginViewModel();
            model = TempData["Login"] as LoginViewModel;
            
            return View(model);
        }

        [HttpPost]
        public ActionResult PostMFAScreen(LoginViewModel model)
        {
            try
            {
                IAccountsRestClient ACRC = new AccountsRestClient();
                var result = ACRC.VerifyMFAOtp(model.Email,model.MFAOTP);
                if (result != null)
                {
                    //Check if User has accepted Policy otherwise redirect him to the page to do the same
                    if (!result.PolicyAccepted)
                    {
                        return RedirectToAction("AcceptPolicies", "Account", result);
                    }
                    string SelectedLandingPageUserRole = ARC.GetSelectedLandingPage(result.Id);
                    string SelectedLandingPageUserRoleID = string.Empty;
                    Boolean SelectedLandingPageUserRoleShowOnDashBoard = false;

                    if (SelectedLandingPageUserRole == null || SelectedLandingPageUserRole == "")
                    {
                        SelectedLandingPageUserRole = result.Roles.ElementAt(0).Name;
                        SelectedLandingPageUserRoleID = result.Roles.ElementAt(0).Id;
                        SelectedLandingPageUserRoleShowOnDashBoard = result.Roles.ElementAt(0).ShowDashboard;
                    }
                    else
                    {
                        foreach (AspnetRoleViewModel objRoles in result.Roles)
                        {
                            if (objRoles.Name == SelectedLandingPageUserRole)
                            {
                                SelectedLandingPageUserRoleID = objRoles.Id;
                                SelectedLandingPageUserRoleShowOnDashBoard = objRoles.ShowDashboard;
                            }
                        }
                    }

                    Globals.SetSessionVariable(result.GcCompanyId, result.Roles, result.Id, result.Email, result.FirstName, result.LastName, result.PhoneNumber, result.LUserId, SelectedLandingPageUserRole, SelectedLandingPageUserRoleID, SelectedLandingPageUserRoleShowOnDashBoard);
                    Globals.LogUserEvent(result.Id, "LoggedIn", "Self Login", true, result.Id, result.GcCompanyId, model.ClientIPAddress);
                    //if (!result.Roles.ElementAt(0).ShowDashboard)
                    if (!SelectedLandingPageUserRoleShowOnDashBoard)
                    {
                        switch (SelectedLandingPageUserRole)
                        {
                            case "L3 Admin":
                                return RedirectToAction("SOSAdminDashboard", "Home");
                            case "L2 Admin":
                                return RedirectToAction("L2AdminDashboard", "Home");
                            case "L2 Alteryx":
                                return RedirectToAction("L2AdminDashboard", "Home");
                            default:
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "", Role = " " + SelectedLandingPageUserRole });

                        }
                    }
                    else
                    {
                        return RedirectToAction("GraphicalDashboard", "Home");
                    }

                }

                TempData["Message"] = "Role not assigned to user";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                if (ex.Data["ErrorMessage"].ToString().Contains("reset"))
                {
                    return RedirectToAction("ResetPassword", "Account", new { Email = model.Email });
                }
                TempData["Message"] = ex.Data["ErrorMessage"];//"UserName or Password is Incorrect";
                TempData["Login"] = model;
                return RedirectToAction("MFAScreen"/*, "Account",model*/);
            }
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Register New User";
            IAspnetRolesRestClient ARRc = new AspnetRolesRestClient();
            ViewBag.Roles = new SelectList(ARRc.GetByCompanyCode(CompanyCode),"Name","Name");
            return View();
        }

        //
        // POST: /Account/Registe
        //method to register user using UI and passing company Id from session
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IAccountsRestClient ACRC = new AccountsRestClient();
                    model.GcCompanyId = Convert.ToInt16(System.Web.HttpContext.Current.Session["CompanyId"]);
                    ACRC.Register(model);
                    TempData["Message"] = "User successfully registered";
                    return RedirectToAction("Index","Home");
                }
                //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //    var result = await UserManager.CreateAsync(user, model.Password);
                //    if (result.Succeeded)
                //    {
                //        await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //        // Send an email with this link
                //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //        return RedirectToAction("Index", "Home");
                //    }
                //    AddErrors(result);
                //}

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        //[AllowAnonymous]
        //public ActionResult RegisterUserTikdam()
        //{
        //    IAspnetRolesRestClient ARRc = new AspnetRolesRestClient();
        //    ViewBag.Roles = new SelectList(ARRc.GetAll(), "Name", "Name");
        //    IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        //    var Company = GCRC.GetAll();
        //    ViewBag.GcCompanyId = new SelectList(Company, "Id", "GcCompanyName");
        //    return View();
        //}

        ////
        //// POST: /Account/Registe
        ////method to register user using UI and passing company Id of group
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> RegisterUserTikdam(RegisterViewModel model)
        //{
        //    try
        //    {
        //            IAccountsRestClient ACRC = new AccountsRestClient();
        //        model.GcCompanyId = model.GcCompanyId;//1;//passing companyid =1 which is companyId of group99 in aws
        //            ACRC.Register(model);
        //            TempData["Message"] = "User successfully registered";
        //            return RedirectToAction("RegisterUserTikdam");
             
        //    }
        //    catch (Exception ex)
        //    {
        //        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        //        var Company = GCRC.GetAll();
        //        ViewBag.GcCompanyId = new SelectList(Company, "Id", "GcCompanyName");
        //        IAspnetRolesRestClient ARRc = new AspnetRolesRestClient();
        //        ViewBag.Roles = new SelectList(ARRc.GetAll(), "Name", "Name");
        //        TempData["Message"] = ex.Data["ErrorMessage"];
        //        return View(model);
        //    }
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        //[ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult ForgotPassword(string Email)
        {
            IAccountsRestClient ACRC = new AccountsRestClient();
            try
            {
                string UserId = ACRC.GetIdByEmail(Email).Id;
                return RedirectToAction("GetQuestions", "Account",new { @UserId = UserId,@Email = Email, @Retry = false });
            }
            catch(Exception ex)
            { 
                TempData["Message"] = "This User does not exist in the system. Please Provide valid Email in order to reset password";
                return RedirectToAction("Index", "Home");
            }
        }

        //This method is not used in ForgotPassword process-SG
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Check the mail id is valid or not.

                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //public ActionResult SendEmailConfirmation(string Email)
        //{
        //    return View();
        //}
        // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult GetQuestions(string UserId, string Email,bool? Retry)
        {
            //user can try to answer/change the question maximum of 5 times. So maintaining a counter
            if (System.Web.HttpContext.Current.Session["OTPRetryCounter"] == null)
            {
                System.Web.HttpContext.Current.Session["OTPRetryCounter"] = 0;
            }
            else
            {
                int OTPRetryCounter = (int)System.Web.HttpContext.Current.Session["OTPRetryCounter"];
                if (OTPRetryCounter == 5)
                {
                    TempData["Message"] = "You did not answer the question. Please contact to L2 Support.";
                    return RedirectToAction("Index", "Home");
                }
                System.Web.HttpContext.Current.Session["OTPRetryCounter"] = OTPRetryCounter + 1;
            }

            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
            ChangePasswordBindingModel GSQ = new ChangePasswordBindingModel();
            var CPBM = GSQRC.GetQuestionAnswersByUserId(UserId);
            var GSQuestionsDropdown = GSQRC.GetQuestionAnswersByUserId(UserId);
            ViewBag.UserId = UserId;
            ViewBag.Email = Email;
            GSQ.Email = Email;
            GSQ.MAuqsqUserId = UserId;
            //when user provides incorrect answer, display the same question again.
            if (TempData["Model"] != null && !(Retry==null?false:(bool)Retry))//shivani - 21 Nov2019 -Handled null exception by converting Retry to nullable
            {
                GSQ = (ChangePasswordBindingModel)TempData["Model"];
                //NOW dropdown of question is replaced with Html Text in view
                ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "MAuqsqQuestionId", "GsqQuestion", GSQ.Question1);
                return View(GSQ);
            }
            List<MAspnetUsersGScurityQuestionViewModel> GSQList = new List<MAspnetUsersGScurityQuestionViewModel>();
            int index = 0;
            //Genetrateing a random number to get a random question
            if (CPBM.Count() > 0)
            {
                foreach (var item in CPBM)
                {
                    item.Id = index ;
                    GSQList.Add(item);
                    index++ ;
                }
                Random rnd = new Random();
                int rndValue = rnd.Next(0, CPBM.Count());
                GSQ.Question1 = GSQList.ElementAt(rndValue).MAuqsqQuestionId;
                // int rndValue1 = rnd.Next(0, 3);
                //while(rndValue1 == rndValue)
                //{
                //    rndValue1 = rnd.Next(0, 3);
                //}
                //GSQ.Question2 = GSQList.ElementAt(rndValue1).MAuqsqQuestionId;
               
                ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "MAuqsqQuestionId", "GsqQuestion", GSQ.Question1);
                //   ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", GSQ.Question2);
                return View(GSQ);
            }
            else
            {
                TempData["Message"] = "There are no security questions for the UserId. Please contact to the Support at support@vodafonelite.com";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult GetQuestions(ChangePasswordBindingModel model)
        {
            try
            {
                var result = ARC.GenerateOTP(model);
                return RedirectToAction("GenerateOTP", "Account",new { @Email = model.Email, @UserId = model.MAuqsqUserId});
            }
            catch(Exception ex)
            {
                int OTPRetryCounter = (int)System.Web.HttpContext.Current.Session["OTPRetryCounter"];
                if (OTPRetryCounter > 5)
                {
                    TempData["Message"] = "You did not answer the question. Please contact to L2 Support.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                TempData["Model"] = model;
                return RedirectToAction("GetQuestions", "Account", new { @Email = model.Email, @UserId = model.MAuqsqUserId, @Retry = false });
                //return View(model);
            }
        }

        [HttpGet]
       // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB.
        public ActionResult GenerateOTP(string Email, string UserId)
        {
            var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
            ViewBag.Email = Email;
            ViewBag.UserId = UserId;
            ViewBag.OTPValidity = OTPValidity;
            return View();
           
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
      // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB.
        public ActionResult GenerateOTP(string OTP, string UserId,string Email)
        {
            try
            {
                var result = ARC.VerifyOTP(OTP, Email, UserId);
                ViewBag.Email = Email;
                ViewBag.UserId = UserId;
                return RedirectToAction("SetPassword", "Account", new { @UserId = UserId, @Email = Email });

            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return RedirectToAction("GenerateOTP", "Account", new { @Email = Email, @UserId = UserId });

            }
        }


        [HttpGet]
        // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult SetPassword(string UserId,string Email)
        {
            ViewBag.UserId = UserId;
            ViewBag.Email = Email;
            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            var user = ARC.GetIdByEmail(Email);
            var CompanyId = user.GcCompanyId.Value;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyId(CompanyId, UserId);
            return View();
        }
        
        // POST: /Account/SetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult SetPassword(ChangePasswordBindingModel model,string UserId, string Email)
        {
            IADAccountRestClient ADRC = new ADAccountRestClient();

            try
            {
                var result = ADRC.ChangeUserPassword(model);
              
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.Email = Email;
                return RedirectToAction("SetPassword", "Account",new { @UserId = UserId, @Email = Email});
            }
           
        }

       //This method is not used in ForgotPassword Process-SG
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string Email)
        {
            //Changes by RG for Reset paswword Question and Answer
            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            ChangePasswordBindingModel model = new ChangePasswordBindingModel();
           
                var user = ARC.GetIdByEmail(Email);
                var CPBM = GSQRC.GetQuestionAnswersByUserId(user.Id);
            //It Is assumed that there will be three records from db
                if (CPBM.Count() > 0)
                {

                    model.Question1 = CPBM.ElementAt(0).MAuqsqQuestionId;
                    model.Question2 = CPBM.ElementAt(1).MAuqsqQuestionId;
                    model.Question3 = CPBM.ElementAt(2).MAuqsqQuestionId;
                    model.Answer1 = CPBM.ElementAt(0).MAugsqAnswer;
                    model.Answer2 = CPBM.ElementAt(1).MAugsqAnswer;
                    model.Answer3 = CPBM.ElementAt(2).MAugsqAnswer;
                }
                ViewBag.Email = Email;

                var GSQuestionsDropdown = GSQRC.GetSecurityQuestions();
                ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question1);
                ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question2);
                ViewBag.Question3 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question3);

            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            var CompanyId = user.GcCompanyId.Value;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyId(CompanyId,user.Id);
                return Email == null ? View("Error") : View(model);
           
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
       // [ControllerActionFilter]
        public  ActionResult ResetPassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //AD integration changes- sg
            IADAccountRestClient ADRC = new ADAccountRestClient();
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();

            try
            {
                //changes by RG to post Question Answer in Reset Password
               model.MAuqsqUserId = ARC.GetIdByEmail(model.Email).Id;
                var CPBM = GSARC.GetQuestionAnswersByUserId(model.MAuqsqUserId);
                if (CPBM.Count() == 0)
                {
                    var resultq = GSARC.AddQuestionAnswers(model);
                   
                }
              else
                {
                    UpdateQuestionAnswers(model.MAuqsqUserId, model);
                }
                var result = ADRC.ChangeUserPassword(model);
                TempData["Message"] = "Password updated successfully.";
                return RedirectToAction("ResetPasswordConfirmation", "Account");

            }
            catch (Exception ex)
            {
                ViewBag.Email = model.Email;
                ViewBag.Question1 = model.Question1;
                ViewBag.Question2 = model.Question2;
                ViewBag.Question3 = model.Question3;
                    ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                // return View(model);
                //Email is required parameter, so RedirectToACtion is having Email parameter.
                return RedirectToAction("ResetPassword", "Account", new  { @Email = model.Email });
            }
            /* var user = await UserManager.FindByNameAsync(model.Email);
             if (user == null)
             {
                 // Don't reveal that the user does not exist
                 return RedirectToAction("ResetPasswordConfirmation", "Account");
             }
             var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
             if (result.Succeeded)
             {
                 return RedirectToAction("ResetPasswordConfirmation", "Account");
             }
            AddErrors(result);
            return View();*/
        }

        //PUT :Method added by RG to overwite saved db data
      //  [ControllerActionFilter]
        public void UpdateQuestionAnswers(string Email,ChangePasswordBindingModel model)
         {
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
            GSARC.PutQuestionAnswer(Email,model);
           
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
       // [ControllerActionFilter]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult LogOff(string ClientIPAddress)
        {
            int iCompId = 0;
            string struserID = "";
            try
            {
                //RK Kept this code in try section as during session timeout, the session values are not available.
                iCompId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"].ToString());
                struserID = System.Web.HttpContext.Current.Session["UserId"].ToString();
            }
            catch (Exception ex)
            {
            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //SG - 15Nov2018 -As a part of security Implementations
            //Killing session along with clearing sessionId and Cache
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.AppendHeader("Cache-Control", "no-store");
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (iCompId > 0 && struserID != "")
            {
                Globals.LogUserEvent(struserID, "LoggedOut", "Self LogOff", true, struserID, iCompId, ClientIPAddress);
            }

            


            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpGet]
        public ActionResult getBannerText()
        {
            IAccountsRestClient ARC = new AccountsRestClient();
            try
            {              
                var result = ARC.getBannerText();                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                
                if (!string.IsNullOrEmpty(ex.Data["Error"] as string))
                {
                    TempData["Error"] = ex.Data["Error"] as string;
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            return Redirect(Globals.ErrorPageUrl);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                            return Redirect(ex.Data["RedirectToUrl"] as string);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return Redirect(ex.Data["RedirectToUrl"] as string);
                        case (int)ExceptionType.Type4:
                            ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                            return Redirect(ex.Data["RedirectToUrl"] as string);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Something went wrong";
                    throw ex;
                }
            }
        }


        //
        public JsonResult GetUserActivityLogCounts(string EmailId)
        {
            
            var ApiData = GUA.GetUserActivityLogCounts(EmailId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //GetUserbyEmailId
        public JsonResult GetUserbyEmailId(string EmailId)
        {

            var ApiData = AUR.GetByEmailId(EmailId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Checkandgetdetail(string EmailId)
        {
            var ApiData = AUR.GetUserByEmailId(EmailId);
            ApiData = ApiData.Replace('"', ' ').Trim();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserActivityLog(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum, string EmailId)
        {
            if (pagesize == 0) pagesize = 20;
            if (pagenum == 0) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = GUA.GetUserActivityLog(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery, EmailId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        [AllowAnonymous]
        public ActionResult UserSetting(string Email)
        {
            //Changes by RG for Reset paswword Question and Answer
            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            ChangePasswordBindingModel model = new ChangePasswordBindingModel();

            var user = ARC.GetIdByEmail(Email);
            var CPBM = GSQRC.GetQuestionAnswersByUserId(user.Id);
            //It Is assumed that there will be three records from db
            if (CPBM.Count() > 0)
            {

                model.Question1 = CPBM.ElementAt(0).MAuqsqQuestionId;
                model.Question2 = CPBM.ElementAt(1).MAuqsqQuestionId;
                model.Question3 = CPBM.ElementAt(2).MAuqsqQuestionId;
                model.Answer1 = CPBM.ElementAt(0).MAugsqAnswer;
                model.Answer2 = CPBM.ElementAt(1).MAugsqAnswer;
                model.Answer3 = CPBM.ElementAt(2).MAugsqAnswer;
            }
            ViewBag.Email = Email;

            var GSQuestionsDropdown = GSQRC.GetSecurityQuestions();
            ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question1);
            ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question2);
            ViewBag.Question3 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model.Question3);

            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            var CompanyId = user.GcCompanyId.Value;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyId(CompanyId, user.Id);
            return Email == null ? View("Error") : View(model);

        }

        [AllowAnonymous]
        public ActionResult ResetUserPreference(string Email)
        {
            ViewBag.Email = Email;
            return View();

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetUserPassword(string Email)
        {
            IAccountsRestClient ARC = new AccountsRestClient();
            ViewBag.Email = Email;
            // ChangePasswordBindingModel model = new ChangePasswordBindingModel();
            ResetPasswordBindingModel model = new ResetPasswordBindingModel();
            var user = ARC.GetIdByEmail(Email);          
            ViewBag.Email = Email;
            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            var CompanyId = user.GcCompanyId.Value;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyId(CompanyId, user.Id);
            return Email == null ? View("Error") : View(model);

        }


        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // [ControllerActionFilter]
        [HttpPost]
        public ActionResult ResetUserPassword( ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //AD integration changes- sg
            IADAccountRestClient ADRC = new ADAccountRestClient();
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            ChangePasswordBindingModel cc = new ChangePasswordBindingModel();
            cc.Email = model.Email;
            cc.Password = model.Password;
            cc.NewPassword = model.NewPassword;
            cc.ConfirmPassword = model.ConfirmPassword;
            try
            {
                
                var result = ADRC.ChangeUserPassword(cc);
               // TempData["Message"] = "Password updated successfully.";
                return RedirectToAction("ResetPasswordConfirmation", "Account");

            }
            catch (Exception ex)
            {
                ViewBag.Email = model.Email;
             
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                // return View(model);
                //Email is required parameter, so RedirectToACtion is having Email parameter.
                return RedirectToAction("UserSetting", "Account", new { @Email = model.Email });
            }
            
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetSecurityQuestionAnswer(string Email)
        {
            //Changes by RG for Reset paswword Question and Answer
            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            ChangePasswordBindingModel model = new ChangePasswordBindingModel();
            ResetSecurityQuestionsBindingModel model1 = new ResetSecurityQuestionsBindingModel();

            var user = ARC.GetIdByEmail(Email);
            var CPBM = GSQRC.GetQuestionAnswersByUserId(user.Id);
            //It Is assumed that there will be three records from db
            if (CPBM.Count() > 0)
            {

                model1.Question1 = CPBM.ElementAt(0).MAuqsqQuestionId;
                model1.Question2 = CPBM.ElementAt(1).MAuqsqQuestionId;
                model1.Question3 = CPBM.ElementAt(2).MAuqsqQuestionId;
                model1.Answer1 = CPBM.ElementAt(0).MAugsqAnswer;
                model1.Answer2 = CPBM.ElementAt(1).MAugsqAnswer;
                model1.Answer3 = CPBM.ElementAt(2).MAugsqAnswer;
            }
            ViewBag.Email = Email;

            var GSQuestionsDropdown = GSQRC.GetSecurityQuestions();
            ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question1);
            ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question2);
            ViewBag.Question3 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question3);

            return Email == null ? View("Error") : View(model1);

        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // [ControllerActionFilter]
        public ActionResult ResetSecurityQuestionAnswer(ResetSecurityQuestionsBindingModel model1)
        {

            if (!ModelState.IsValid)
            {
                return View(model1);
            }
            ChangePasswordBindingModel model = new ChangePasswordBindingModel();
            model.Id = model1.Id;
            model.Question1 =  model1.Question1;
            model.Question2 = model1.Question2;
            model.Question3 = model1.Question3;
            model.Answer1 = model1.Answer1;
            model.Answer2 = model1.Answer2;
            model.Answer3 = model1.Answer3;
            model.MAuqsqQuestionId = model1.MAuqsqQuestionId;
            model.GsqQuestion = model1.GsqQuestion;
            model.MAuqsqUserId = model1.MAuqsqUserId;
            model.Email = model1.Email;


            //AD integration changes- sg
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();

            try
            {
                //changes by RG to post Question Answer in Reset Password
                model.MAuqsqUserId = ARC.GetIdByEmail(model.Email).Id;
                var CPBM = GSARC.GetQuestionAnswersByUserId(model.MAuqsqUserId);
                if (CPBM.Count() == 0)
                {
                    var resultq = GSARC.AddQuestionAnswers(model);

                }
                else
                {
                    UpdateQuestionAnswers(model.MAuqsqUserId, model);
                }
                //var GSQuestionsDropdown = GSQRC.GetSecurityQuestions();
                // ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question1);
                // ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question2);
                // ViewBag.Question3 = new SelectList(GSQuestionsDropdown, "Id", "GsqQuestion", model1.Question3);
                // TempData["Message"] = "Security answers updated successfully.";
                //return View(model1);
                // return RedirectToAction("ResetSecurityQuestionAnswer", "Account", new { @Email = model.Email }); UserSetting


                if (Session["LandingPage"] != null && Convert.ToString(Session["LandingPage"]) == "SecurityQuestions")
                {
                    Session["LandingPage"] = null;
                    if (!Convert.ToBoolean(TempData["SelectedLandingPageUserRoleShowOnDashBoard"]))
                    {
                        // switch (result.Roles.ElementAt(0).Name)
                        switch (Convert.ToString(TempData["SelectedLandingPageUserRole"]))
                        {
                            case "Reporting Analyst":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Reporting Analyst Welcome Page", Role = " Reporting Analyst" });
                                //return RedirectToAction("AnalystDashBoard", "Home
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Payee Welcome Page", Role = " Payee" });
                            //return RedirectToAction("PayeeDashboard", "Home");
                            case "Manager":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Manager Welcome Page", Role = " Manager" });
                            //return RedirectToAction("ManagerDashboard", "Home");
                            case "Sales Operations":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Sales Op Welcome Page", Role = " Sales Operations" });
                            //return RedirectToAction("SalesOperationsDashboard", "Home");
                            case "L3 Admin":
                                return RedirectToAction("SOSAdminDashboard", "Home");
                            case "L2 Admin":
                                return RedirectToAction("L2AdminDashboard", "Home");
                            case "L2 Alteryx":
                                // return RedirectToAction("GenericDashboard", "Home", new { Title = "Alteryx Admin Welcome Page", Role = " Alteryx Admin" });
                                return RedirectToAction("L2AdminDashboard", "Home");
                            case "L2 L2 Infra":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Infra Admin Welcome Page", Role = " Infra Admin" });
                            case "System Analyst":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "System Analyst Welcome Page", Role = " System Analyst" });
                            //return RedirectToAction("SystemAnalystDashboard", "Home");
                            case "Auditor":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Auditor Welcome Page", Role = " Auditor" });
                            //return RedirectToAction("AuditorDashboard", "Home");
                            case "Head of Finance Operations":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "HOFO Welcome Page", Role = " Head of Finance Operations" });
                            //return RedirectToAction("HOFODashboard", "Home");
                            case "Account Analyst":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Account Analyst Welcome Page", Role = " Account Analyst" });
                            //return RedirectToAction("AccountAnalystDashboard", "Home");
                            case "Controller":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Controller Welcome Page", Role = " Controller" });
                            //return RedirectToAction("ControllerDashboard", "Home");
                            case "Channel Manager":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Channel Manager Welcome Page", Role = " Channel Manager" });
                            //return RedirectToAction("ChannelManagerDashboard", "Home");
                            case "Monitor":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "Monitor Welcome Page", Role = " Monitor" });
                            //return RedirectToAction("MonitorDashboard", "Home");
                            case "HR":
                                return RedirectToAction("GenericDashboard", "Home", new { Title = "HR Welcome Page", Role = " HR" });
                            //return RedirectToAction("HRDashboard", "Home");
                            default:
                                return RedirectToAction("NoRolesAssigned", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("GraphicalDashboard", "Home");
                    }
                }
                else
                {

                    return RedirectToAction("UserSetting", "Account", new { @Email = model.Email });
                }

            }
            catch (Exception ex)
            {
                ViewBag.Email = model.Email;
                ViewBag.Question1 = model.Question1;
                ViewBag.Question2 = model.Question2;
                ViewBag.Question3 = model.Question3;
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                // return View(model);
                //Email is required parameter, so RedirectToACtion is having Email parameter.
                return RedirectToAction("UserSetting", "Account", new { @Email = model.Email });
            }
            
        }

        public JsonResult GetSelectedLandingPage()
        {
            string data = string.Empty;
            IAccountsRestClient ARC = new AccountsRestClient();
            try
            {

                 data =  ARC.GetSelectedLandingPage(UserId);
            }
            catch(Exception ex)
            {

            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatSelectedLandingPage(string SelectedRole)
        {
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
            IAccountsRestClient ARC = new AccountsRestClient();
            try
            {

                ARC.UpdateSelectedLandingPage(UserId, SelectedRole, CompanyId);
            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }





    }
}