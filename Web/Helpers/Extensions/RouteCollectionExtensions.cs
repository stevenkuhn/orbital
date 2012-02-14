using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Orbital.Web.Helpers;

namespace System.Web.Routing
{
    public static class RouteCollectionExtension
    {
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
        {
            return routes.MapRouteLowerCase(name, url, defaults, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            return routes.MapRouteLowerCase(name, url, defaults, constraints, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            Route route = new LowercaseRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary(),
            };

            route.DataTokens["routename"] = name;

            if (namespaces != null && namespaces.Length > 0)
                route.DataTokens["Namespaces"] = (object)namespaces;

            routes.Add(name, route);

            return route;
        }
    }
}