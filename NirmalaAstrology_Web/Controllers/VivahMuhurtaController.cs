using System;
using System.Web.Mvc;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    public class VivahMuhurtaController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Vivah Muhurta";
            return View(new VivahMuhurtaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(VivahMuhurtaViewModel model)
        {
            ViewBag.Title = "Vivah Muhurta";
            if (model == null) model = new VivahMuhurtaViewModel();
            if (!model.FromDate.HasValue) ModelState.AddModelError("FromDate", "From date is required.");
            if (!model.ToDate.HasValue) ModelState.AddModelError("ToDate", "To date is required.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var service = new VivahMuhurtaService(Server.MapPath("~/App_Data/vivah_muhurta_puja.json"));
                model.Result = service.Search(model.FromDate.Value.Date, model.ToDate.Value.Date);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[VivahMuhurta] " + ex);
                ModelState.AddModelError("", "Vivah Muhurta lookup failed. Check App_Data vivah_muhurta_puja.json.");
            }
            return View(model);
        }
    }
}
