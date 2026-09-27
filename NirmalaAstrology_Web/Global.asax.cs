using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            try { new AuthService().EnsureSchemaAndSeed(); }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine("[Auth bootstrap] " + ex.Message); }
            try { new CommerceSchemaService().EnsureSchemaAndSeed(); }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine("[Commerce bootstrap] " + ex.Message); }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                return;
            try
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || ticket.Expired) return;
                var roles = string.IsNullOrWhiteSpace(ticket.UserData)
                    ? new string[0]
                    : ticket.UserData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var identity = new FormsIdentity(ticket);
                Context.User = new GenericPrincipal(identity, roles);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[PostAuthenticate] " + ex.Message);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                System.Diagnostics.Trace.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
