using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WIAM_SOS
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-SourceFiles");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Set("Server", "Cloud");
            
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Add("X-Frame-Options", "DENY");
        }

    }
}
