using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Vodafone_SOS_WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PreSendRequestHeaders()
        {
            // Response.Headers.Remove("Server");
            Response.Headers.Set("Server", "Cloud");
            //Response.Headers.Set("X-Powered-By", "Megacube Pty Ltd");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            //Response.Headers.Remove("X-Powered-By");
            //Response.Headers.Remove("X-SourceFiles");
            //Response.Headers.Remove("X-SourceFiles");
            Response.Headers.Remove("X-Frame-Options");

           // Response.Headers.Add("Content-Security-Policy","default-src 'none';script-src 'self' 'unsafe-inline' 'unsafe-eval';style-src 'self' 'unsafe-inline' ;" +"img-src 'self' data:;font-src 'self';connect-src 'self';form-action 'self'");
        }
    }
}
