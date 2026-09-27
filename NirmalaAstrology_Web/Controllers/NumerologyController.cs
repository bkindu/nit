using System.Web.Mvc;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>Admin Numerology module — legacy-equivalent name/DOB calculations (Admin only).</summary>
    [AdminAuthorize]
    public class NumerologyController : Controller
    {
        private readonly LegacyNumerologyService _legacyNumerology = new LegacyNumerologyService();

        public ActionResult Index()
        {
            ViewBag.FullAccess = true;
            var model = new NumerologyViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NumerologyViewModel input)
        {
            ViewBag.FullAccess = true;
            if (ModelState.IsValid)
            {
                input.Result = _legacyNumerology.Calculate(input.Name, input.DateOfBirth, input.NickName, input.VehicleOrPan);
            }

            return View(input);
        }
    }
}
