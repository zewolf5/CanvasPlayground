﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CanvasPlayground
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("");

            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: "index.html"
            );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "api", action = "values", id = UrlParameter.Optional }
            //);
        }
    }
}
