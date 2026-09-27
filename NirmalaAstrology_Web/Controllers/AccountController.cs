using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _auth = new AuthService();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.Title = "Create Account";
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ViewBag.Title = "Create Account";
            if (model == null) model = new RegisterViewModel();
            if (!ModelState.IsValid) return View(model);

            string error;
            var user = _auth.RegisterPublicUser(model.Username, model.Password, model.DisplayName, model.Email, model.Mobile, out error);
            if (user == null)
            {
                ModelState.AddModelError("", error ?? "Unable to create account.");
                return View(model);
            }

            SignIn(user, false);
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Title = "My Account";
            var user = _auth.GetByUsername(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Auth");
            var appts = new AppointmentService().GetForUser(user.UserId);
            ViewBag.Appointments = appts;
            return View(new ProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Mobile = user.Mobile,
                Roles = user.Roles
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileViewModel model)
        {
            ViewBag.Title = "My Account";
            var user = _auth.GetByUsername(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Auth");
            model.Username = user.Username;
            model.UserId = user.UserId;
            model.Roles = user.Roles;

            if (!ModelState.IsValid)
            {
                ViewBag.Appointments = new AppointmentService().GetForUser(user.UserId);
                return View(model);
            }

            string error;
            if (!_auth.UpdateProfile(user.UserId, model.DisplayName, model.Email, model.Mobile, out error))
            {
                ModelState.AddModelError("", error ?? "Update failed.");
                ViewBag.Appointments = new AppointmentService().GetForUser(user.UserId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Change Password";
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            ViewBag.Title = "Change Password";
            if (!ModelState.IsValid) return View(model);
            string error;
            if (!_auth.ChangePassword(User.Identity.Name, model.CurrentPassword, model.NewPassword, out error))
            {
                ModelState.AddModelError("", error ?? "Unable to change password.");
                return View(model);
            }
            TempData["SuccessMessage"] = "Password changed.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Title = "Forgot Password";
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.Title = "Forgot Password";
            if (!ModelState.IsValid) return View(model);

            if (!EmailService.IsEnabled())
            {
                ModelState.AddModelError("", "Email/SMTP is not enabled on the server. Configure App_Data/smtp.local.config (Email:Enabled=true) or ask admin to reset your password.");
                return View(model);
            }

            var user = _auth.FindByEmail(model.Email);
            // Always show the same outcome text when user is unknown (anti-enumeration),
            // but if user exists and mail fails, show a clear SMTP error.
            if (user == null)
            {
                TempData["SuccessMessage"] = "If that email exists, a temporary password has been sent.";
                return RedirectToAction("Login", "Auth");
            }

            var temp = Guid.NewGuid().ToString("N").Substring(0, 10);
            var body = EmailService.WrapHtml("Password reset",
                "<p>Hello " + EmailService.HtmlEncode(user.DisplayName) + ",</p>"
                + "<p>Your temporary password is: <strong>" + EmailService.HtmlEncode(temp) + "</strong></p>"
                + "<p>Sign in and change it immediately.</p>");

            string emailError;
            var sent = new EmailService().TrySend(user.Email, user.DisplayName, "Nirmala Astrology — temporary password", body, out emailError);
            if (!sent)
            {
                ModelState.AddModelError("", emailError ?? "Could not send email. Check SMTP settings.");
                return View(model);
            }

            string setError;
            if (!_auth.SetPassword(user.UserId, temp, out setError))
            {
                ModelState.AddModelError("", setError ?? "Password could not be updated.");
                return View(model);
            }

            TempData["SuccessMessage"] = "If that email exists, a temporary password has been sent.";
            return RedirectToAction("Login", "Auth");
        }

        private void SignIn(Models.AppUser user, bool rememberMe)
        {
            var roles = user.Roles == null ? "" : string.Join(",", user.Roles);
            var ticket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddHours(8),
                rememberMe,
                roles);
            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted) { HttpOnly = true };
            if (rememberMe) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
        }
    }
}
