using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class PrimaryController : Controller
    {
        internal string ControllerName = string.Empty;
        internal string ActionName = string.Empty;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            ControllerName = Convert.ToString(filterContext.RouteData.Values["controller"]);
            ActionName = Convert.ToString(filterContext.RouteData.Values["action"]);
            HttpContext context = System.Web.HttpContext.Current;
            if (context.Session != null)
            {
                if (ControllerName != "LAudit" && ActionName != "RedirectToReview")
                {
                    try
                    {
                        string referer = Request.ServerVariables["HTTP_REFERER"];
                        if (string.IsNullOrEmpty(referer))
                        {
                            Session.Abandon();
                            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" }));
                        }
                    }
                    catch
                    {

                    }

                    if (System.Web.HttpContext.Current.Session["FirstName"] == null || System.Web.HttpContext.Current.Session["LastName"] == null || System.Web.HttpContext.Current.Session["CompanyCode"] == null || System.Web.HttpContext.Current.Session["CompanyId"] == null || System.Web.HttpContext.Current.Session["UserId"] == null || System.Web.HttpContext.Current.Session["UserRoleId"] == null || System.Web.HttpContext.Current.Session["UserRole"] == null)
                    {
                        filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" }));
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
            
           


        }

        
    }
}