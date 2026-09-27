using System.Web.Mvc;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Admin Dashboard";
            ViewBag.Reports = new ReportAccessService().GetAll();
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        /// <summary>
        /// Sends a branded test email. Requires App_Data/smtp.local.config with mailbox password.
        /// Example: /Admin/Dashboard/SendTestEmail?to=user@example.com
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendTestEmail(string to)
        {
            var target = string.IsNullOrWhiteSpace(to) ? "kumarbirinder@gmail.com" : to.Trim();
            var body = EmailService.WrapHtml(
                "Email template test",
                "<p>This is a test message from the Nirmala Astrology web application.</p>"
                + "<p>If you received this, SMTP branding and delivery are working.</p>"
                + "<ul>"
                + "<li>Short Kundli / Numerology PDFs are <strong>FREE</strong> (no payment).</li>"
                + "<li>Appointments and Detailed reports remain paid / admin workflows.</li>"
                + "</ul>");
            string error;
            var ok = new EmailService().TrySend(target, "Nirmala Test", "Nirmala Astrology — email template test", body, out error);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok
                ? ("Test email sent to " + target + ".")
                : ("Test email failed: " + (error ?? "unknown error"));
            return RedirectToAction("Index");
        }
    }
}
