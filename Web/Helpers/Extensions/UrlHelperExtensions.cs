using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        private const string _imagePath = "~/content/images/";
        private const string _stylesheetPath = "~/content/styles/";
        private const string _scriptPath = "~/content/scripts/";

        public static string Image(this UrlHelper helper, string fileName)
        {
            return helper.Content(string.Concat(_imagePath, fileName));
        }

        public static string Stylesheet(this UrlHelper helper, string fileName)
        {
            return helper.Content(string.Concat(_stylesheetPath, fileName));
        }

        public static string JavaScript(this UrlHelper helper, string fileName)
        {
            return helper.Content(string.Concat(_scriptPath, fileName));
        }
    }
}