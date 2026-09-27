using System.Web.Mvc;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Custom error pages. Must exist so customErrors redirects do not cascade-fail.
    /// </summary>
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View(); // Views/Error/Index.cshtml
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("NotFound");
        }
    }
}
