using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;

namespace Orbital.Web.Controllers
{
    public abstract class Controller : System.Web.Mvc.Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
