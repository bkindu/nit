using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Title = "Admin Login";
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            ViewBag.Title = "Admin Login";
            if (model == null) model = new LoginViewModel();
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View(model);
            }

            try
            {
                var auth = new AuthService();
                var user = auth.ValidateUser(model.Username, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                var roles = user.Roles == null ? "" : string.Join(",", user.Roles);
                var ticket = new FormsAuthenticationTicket(
                    1,
                    user.Username,
                    DateTime.Now,
                    DateTime.Now.AddHours(8),
                    model.RememberMe,
                    roles);
                var encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
                {
                    HttpOnly = true
                };
                if (model.RememberMe)
                    cookie.Expires = ticket.Expiration;
                Response.Cookies.Add(cookie);

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                if (user.IsInRole(AuthService.RoleAdmin))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                return RedirectToAction("Index", "Account");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Auth.Login] " + ex);
                ModelState.AddModelError("", "Unable to sign in. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            ViewBag.Title = "Access Denied";
            return View();
        }
    }
}
