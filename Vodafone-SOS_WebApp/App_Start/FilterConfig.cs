using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace Vodafone_SOS_WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    public class HandleCustomError : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext FilterContext)
        {
            string ParameterString = "";
            //Gets the controller name
            var Controller = FilterContext.RouteData.Values["controller"].ToString();
            //gets the action name
            string actionName = FilterContext.RouteData.Values["action"].ToString();
            //Add parameter List
            //var ParameterList = FilterContext.RouteData.Values.ToDictionary(x => x.Key, x => x.Value);
            var ParamList = FilterContext.HttpContext.Request.QueryString;
            if (ParamList != null)
                ParameterString = string.Join(",", ParamList);
            Type ControllerType = FilterContext.Controller.GetType();
            var statusMessage = FilterContext.Exception;
            // Add Error Log to database Table
            var model = new GErrorLogViewModel { GelUserName = System.Web.HttpContext.Current.Session["UserName"] as string, GelController = Controller, GelMethod = actionName, GelErrorDateTime = DateTime.UtcNow, GelStackTrace = statusMessage.ToString(), GelErrorDescription = ParameterString, GelSourceProject = "[Vodafone-SOS WebApp]", GelStatus = "New" };
            IErrorLogsRestClient ELRC = new ErrorLogsRestClient();
            ELRC.Add(model);
            //Send mail to  l2 admin
            var Subject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
            string Body;
            var UserName = (System.Web.HttpContext.Current.Session["UserName"] != null) ? System.Web.HttpContext.Current.Session["UserName"].ToString() : "";
            Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + "</td></tr><tr><td>Controller</td><td>" + Controller + "</td></tr><tr><td>Method Name</td><td>" + actionName + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + UserName + "</td></tr><tr><td>Stack Trace</td><td>" + statusMessage + "</td></tr></tr><tr><td>Parameters</td><td>" + ParameterString + "</td></tr></table>";

            Globals.SendEmail(Body, Subject, ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailCc"], null);
            FilterContext.ExceptionHandled = true;
            FilterContext.Result = new ViewResult
            {
                ViewName = "Error"
            };
        }
    }
    //method added by shubham to handle session expiration
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;
        //    //to check if session object is null
        //    if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["UserName"] as string))
        //    {
        //        System.Web.HttpContext.Current.Session.Clear();
        //        filterContext.Result = new RedirectResult("/Home/Index");
        //        return;
        //    }
        //    base.OnActionExecuting(filterContext);
        //}
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Gets the controller name
            var Controller = filterContext.RouteData.Values["controller"].ToString();
            //gets the action name
            string actionName = filterContext.RouteData.Values["action"].ToString();
            HttpContext context = HttpContext.Current;
            try
            {

                if (context.Session != null)
                {
                    if (context.Session.IsNewSession)
                    {
                        string sessionCookie = context.Request.Headers["Cookie"];

                        if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        {
                            //Vodafone_SOS_WebApp.Controllers.AccountController AC = new Controllers.AccountController();
                            //AC.LogOff();
                            FormsAuthentication.SignOut();
                            //string redirectTo = "~/Home/Index";

                            string redirectTo = string.Format("~/Home/Index");
                            filterContext.Result = new RedirectResult(redirectTo);
                            return;



                            //if (!string.IsNullOrEmpty(context.Request.RawUrl))
                            //{
                            //    redirectTo = string.Format("~/Home/Index?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                            //    filterContext.Result = new RedirectResult(redirectTo);
                            //    return;
                            //}

                        }
                    }
                    Boolean blnFound = false;
                    for (int i = 0; i < Globals.LstSessionIDs.Count; i++)
                    {
                        if (context.Session.SessionID == Globals.LstSessionIDs[i]._UserSessionID)
                        {
                            blnFound = true;
                        }
                    }
                    if (!blnFound)
                    {
                        FormsAuthentication.SignOut();
                        //string redirectTo = "~/Home/Index";
                        string redirectTo = string.Format("~/Home/Index");
                        filterContext.Result = new RedirectResult(redirectTo);
                        return;
                    }
                }
                else
                {
                    //If session is null, user is reditrected to Login Page
                    string redirectTo = string.Format("~/Home/Index");
                    filterContext.Controller.TempData["Error"] = "Session TimedOut";
                    filterContext.Result = new RedirectResult(redirectTo);
                    return;
                }



            }
            catch (Exception ex)
            {
                // Add Error Log to database Table
                var model = new GErrorLogViewModel { GelUserName = System.Web.HttpContext.Current.Session["UserName"] as string, GelController = Controller, GelMethod = actionName, GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.StackTrace.ToString(), GelErrorDescription = "", GelSourceProject = "[Vodafone-SOS WebApp]", GelStatus = "New" };
                IErrorLogsRestClient ELRC = new ErrorLogsRestClient();
                ELRC.Add(model);
            }


            base.OnActionExecuting(filterContext);


        }

    }

    //class for customauthorization
    // RK during code review. Below class is depriciated, PLEASE DONOT USE IT FURTHER, INSTEAD USE ControllerActionFilterAttribute
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["UserRole"] as string))
                {
                    var currentRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    if (currentRole.Equals(role, StringComparison.OrdinalIgnoreCase))
                    {
                        authorize = true;
                    }
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var currentRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            if (currentRole != null)
            {

                // filterContext.Result = new RedirectResult("/Home/Index");//new HttpUnauthorizedResult();
                filterContext.Result = new ViewResult
                {
                    ViewName = "UnAuthorized"
                };
            }
            else
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }

        }
    }


    //class for dynamic authorization of Contorller Actio
    //Added by ShivaniG
    public class ControllerActionFilterAttribute : ActionFilterAttribute
    {
        private string CurrentActionKey;
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IMAspnetRolesGAuthorizableObjectsRestClient client = new MAspnetRolesGAuthorizableObjectsRestClient();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CurrentActionKey = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName +
                              "-" + filterContext.ActionDescriptor.ActionName;
            var UsrRoleId = System.Web.HttpContext.Current.Session["UserRoleId"];//getting the current UserRoleId

            if (UsrRoleId != null)
            {
                string UserRoleId = UsrRoleId.ToString();
                int matchFound = client.GetCount(UserRoleId, CurrentActionKey);

                if (matchFound != 0)
                {
                    return;
                }

                filterContext.Result = new ViewResult
                {
                    ViewName = "UnAuthorized"
                };
            }
            else
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }

        }
    }

    public class CheckReportSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
         
            if(HttpContext.Current.Session["CompanyCode"] == null || HttpContext.Current.Session["UserName"] == null || HttpContext.Current.Session["UserRole"] == null  || HttpContext.Current.Session["from"] == null)
            {
                string redirectTo = string.Format("~/Home/Index");
                filterContext.Result = new RedirectResult(redirectTo);
                return;
            }


        }
    }
}

