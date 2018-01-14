using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebContent.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // *** Comment out this line to use convention-based routing.
            routes.MapMvcAttributeRoutes();

            // *** Uncomment this statement to use convention-based routing.
            //routes.MapRoute(
            //    name: "default",
            //    url: "{controller}/{action}/{*nodePath}",
            //    defaults: new { controller = "Home", action = "Index", nodePath = UrlParameter.Optional }
            //);
        }
    }
}
