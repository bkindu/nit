using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    [SessionState(SessionStateBehavior.Required)]
    public class GocharController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Gochar Phal";
            var model = new GocharPhalViewModel();
            FillSession(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GocharPhalViewModel model)
        {
            ViewBag.Title = "Gochar Phal";
            if (model == null) model = new GocharPhalViewModel();
            FillSession(model);

            if (!model.HasBirthSession)
                ModelState.AddModelError("", "Save birth data first (Birth Data hub), then open Gochar Phal.");
            if (!model.TransitDate.HasValue)
                ModelState.AddModelError("TransitDate", "Transit date is required.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var birth = BirthDataSessionStore.Get(Session);
                var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
                var service = new GocharPhalService(scriptPath);
                model.Result = service.Calculate(birth, model.TransitDate.Value.Date);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Gochar] " + ex);
                ModelState.AddModelError("", "Gochar calculation failed. Confirm birth data and transit date.");
            }
            return View(model);
        }

        private void FillSession(GocharPhalViewModel model)
        {
            var birth = BirthDataSessionStore.Get(Session);
            if (birth == null || birth.Year < 1)
            {
                model.HasBirthSession = false;
                model.BirthSummary = "No birth data in session.";
                return;
            }
            model.HasBirthSession = true;
            model.NativeName = birth.Name;
            model.BirthSummary = string.Format(CultureInfo.InvariantCulture,
                "{0} · {1:00}.{2:00}.{3} {4:00}:{5:00} · {6}",
                birth.Name ?? "Native", birth.Day, birth.Month, birth.Year,
                birth.Hours, birth.Minutes, birth.Place ?? "");
            if (!model.TransitDate.HasValue)
                model.TransitDate = DateTime.Today;
        }
    }
}
