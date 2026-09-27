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
    public class VarshPhalController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Varsh Phal";
            var model = new VarshPhalViewModel();
            FillSession(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(VarshPhalViewModel model)
        {
            ViewBag.Title = "Varsh Phal";
            if (model == null) model = new VarshPhalViewModel();
            FillSession(model);

            if (!model.HasBirthSession)
                ModelState.AddModelError("", "Save birth data first (Birth Data hub), then open Varsh Phal.");
            if (model.TargetYear < 1900 || model.TargetYear > 2100)
                ModelState.AddModelError("TargetYear", "Enter a valid target year.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var birth = BirthDataSessionStore.Get(Session);
                var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
                var service = new VarshPhalService(scriptPath);
                model.Result = service.Calculate(birth, model.TargetYear);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[VarshPhal] " + ex);
                ModelState.AddModelError("", "Varsh Phal calculation failed. Confirm birth data and year.");
            }
            return View(model);
        }

        private void FillSession(VarshPhalViewModel model)
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
                "{0} · {1:00}.{2:00}.{3} {4:00}:{5:00} · {6} ({7:0.####}N, {8:0.####}E)",
                birth.Name ?? "Native", birth.Day, birth.Month, birth.Year,
                birth.Hours, birth.Minutes, birth.Place ?? "", birth.Latitude, birth.Longitude);
            if (model.TargetYear < birth.Year) model.TargetYear = DateTime.Today.Year;
        }
    }
}
