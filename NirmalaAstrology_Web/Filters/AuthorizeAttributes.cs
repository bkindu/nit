using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Filters
{
    /// <summary>Requires authenticated Admin role (server-side).</summary>
    public sealed class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.User == null || httpContext.User.Identity == null)
                return false;
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
            return httpContext.User.IsInRole(AuthService.RoleAdmin);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User != null
                && filterContext.HttpContext.User.Identity != null
                && filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    area = "",
                    controller = "Auth",
                    action = "AccessDenied"
                }));
                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                area = "",
                controller = "Auth",
                action = "Login",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            }));
        }
    }

    /// <summary>Requires a report permission code for current principal.</summary>
    public sealed class ReportAuthorizeAttribute : AuthorizeAttribute
    {
        public string ReportCode { get; set; }

        public ReportAuthorizeAttribute(string reportCode)
        {
            ReportCode = reportCode;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return new ReportAccessService().CanAccess(ReportCode, httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var isAuth = filterContext.HttpContext.User != null
                && filterContext.HttpContext.User.Identity != null
                && filterContext.HttpContext.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    area = "",
                    controller = "Auth",
                    action = "Login",
                    returnUrl = filterContext.HttpContext.Request.RawUrl
                }));
                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                area = "",
                controller = "Auth",
                action = "AccessDenied"
            }));
        }
    }
}
