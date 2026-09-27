using System.Web.Mvc;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated
                && User.IsInRole(AuthService.RoleAdmin))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ViewBag.Title = "Payment";
            ViewBag.UpiId = CommerceSchemaService.UpiId;
            ViewBag.Gateway = CommerceSchemaService.PaymentGateway;
            return View();
        }
    }
}
