using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WorkoProject.Utils
{
    public class VerifyUserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SessionManager.HasConnectedUser)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {{ "Controller", "Account" },
                                      { "Action", "Login" } });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
