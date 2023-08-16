using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WIAM_SOS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //"Default", "{*path}",

                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute("Default", "{*path}",
            //    new { controller = "ManageAccess", action = "Index", id = UrlParameter.Optional },
            //    new RouteValueDictionary
            //    {
            //         { "type", "Customer|Admin" }
            //    });
        }
    }
}
