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
    public class LalKitabController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Lal Kitab";
            var model = new LalKitabViewModel();
            FillSession(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LalKitabViewModel model)
        {
            ViewBag.Title = "Lal Kitab";
            if (model == null) model = new LalKitabViewModel();
            FillSession(model);

            if (!model.HasBirthSession)
                ModelState.AddModelError("", "Save birth data first (Birth Data hub), then open Lal Kitab.");
            if (model.AgeYear < 1 || model.AgeYear > 110)
                ModelState.AddModelError("AgeYear", "Age year must be 1–110 (Lal_Kitab_Digits).");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var birth = BirthDataSessionStore.Get(Session);
                var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
                var digitsPath = Server.MapPath("~/App_Data/lal_kitab_digits.json");
                var service = new LalKitabService(scriptPath, digitsPath);
                model.Result = service.Calculate(birth, model.AgeYear, model.RemedyMode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[LalKitab] " + ex);
                ModelState.AddModelError("", "Lal Kitab calculation failed. Confirm birth data and age year.");
            }
            return View(model);
        }

        private void FillSession(LalKitabViewModel model)
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
            if (model.AgeYear < 1)
                model.AgeYear = Math.Max(1, Math.Min(110, DateTime.Today.Year - birth.Year));
            if (string.IsNullOrWhiteSpace(model.RemedyMode))
                model.RemedyMode = "Janam";
        }
    }
}
