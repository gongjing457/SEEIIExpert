using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SEEIPro
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "EEmain", action = "Tools", id = UrlParameter.Optional });
            // defaults: new { controller = "Backstage", action = "Login", id = UrlParameter.Optional });HomePage
            // defaults: new { controller = "ExpertsManage", action = "Upload", id = UrlParameter.Optional });
        }
    }
}