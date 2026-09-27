using System;
using System.Configuration;
using System.Web.Mvc;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Contact Us — sends via config-driven EmailService (no SMTP secrets in code).
    /// </summary>
    public class ContactController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated
                && User.IsInRole(AuthService.RoleAdmin))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendEmail(string name, string email, string subject, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                {
                    return Json(new { success = false, message = "All fields are required." });
                }

                var to = ConfigurationManager.AppSettings["Email:ContactTo"];
                if (string.IsNullOrWhiteSpace(to))
                    to = ConfigurationManager.AppSettings["Email:FromAddress"];
                if (string.IsNullOrWhiteSpace(to))
                    return Json(new { success = false, message = "Email is not configured on the server." });

                var body = EmailService.WrapHtml("New Contact Message",
                    "<table style='width:100%;border-collapse:collapse;'>"
                    + "<tr><td style='padding:8px;color:#888;width:120px;'>Name</td><td style='padding:8px;'>" + EmailService.HtmlEncode(name) + "</td></tr>"
                    + "<tr><td style='padding:8px;color:#888;'>Email</td><td style='padding:8px;'>" + EmailService.HtmlEncode(email) + "</td></tr>"
                    + "<tr><td style='padding:8px;color:#888;'>Subject</td><td style='padding:8px;'>" + EmailService.HtmlEncode(subject) + "</td></tr>"
                    + "<tr><td style='padding:8px;color:#888;'>Message</td><td style='padding:8px;'>" + EmailService.HtmlEncode(message).Replace("\n", "<br/>") + "</td></tr>"
                    + "<tr><td style='padding:8px;color:#888;'>Date</td><td style='padding:8px;'>" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") + "</td></tr>"
                    + "</table>"
                    + "<p style='font-size:12px;color:#777;'>Reply-to visitor: " + EmailService.HtmlEncode(email) + "</p>");

                string error;
                var ok = new EmailService().TrySend(to, "Nirmala Astrology", "Contact Form: " + subject, body, out error);
                if (!ok)
                    return Json(new { success = false, message = error ?? "Failed to send email. Please try again or contact us directly." });

                return Json(new { success = true, message = "Your message has been sent successfully! We will get back to you soon." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SendEmail Error] " + ex.GetType().Name + ": " + ex.Message);
                return Json(new { success = false, message = "Failed to send email. Please try again or contact us directly." });
            }
        }
    }
}
