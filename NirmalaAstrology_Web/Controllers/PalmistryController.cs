using System;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    [SessionState(SessionStateBehavior.Required)]
    public class PalmistryController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Palmistry";
            var model = new PalmistryViewModel();
            FillLists(model);
            PrefillName(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PalmistryViewModel model)
        {
            ViewBag.Title = "Palmistry";
            if (model == null) model = new PalmistryViewModel();
            FillLists(model);

            var hasSelection =
                (model.SelectedIds != null && model.SelectedIds.Length > 0) ||
                !string.IsNullOrWhiteSpace(model.MountCode) ||
                !string.IsNullOrWhiteSpace(model.LuckyCode) ||
                !string.IsNullOrWhiteSpace(model.RemedyCode);

            if (!hasSelection)
                ModelState.AddModelError("", "Select at least one palm feature, or Mount / Lucky / Remedy.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var service = CreateService();
                model.Result = service.Build(
                    model.NativeName,
                    model.SelectedIds,
                    model.MountCode,
                    model.LuckyCode,
                    model.RemedyCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Palmistry] " + ex);
                ModelState.AddModelError("", "Palmistry reading failed. Check App_Data palmistry JSON files.");
            }
            return View(model);
        }

        private void FillLists(PalmistryViewModel model)
        {
            var service = CreateService();
            model.Options = service.LoadOptions();
            model.MountOptions = service.MountSelectList(model.MountCode);
            model.LuckyOptions = service.LuckySelectList(model.LuckyCode);
            model.RemedyOptions = service.RemedySelectList(model.RemedyCode);
            if (model.SelectedIds == null)
                model.SelectedIds = new string[0];
        }

        private void PrefillName(PalmistryViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.NativeName)) return;
            var birth = BirthDataSessionStore.Get(Session);
            if (birth != null && !string.IsNullOrWhiteSpace(birth.Name))
                model.NativeName = birth.Name;
        }

        private PalmistryService CreateService()
        {
            return new PalmistryService(
                Server.MapPath("~/App_Data/PalmIndex.txt"),
                Server.MapPath("~/App_Data/palmistry_readings.json"),
                Server.MapPath("~/App_Data/palmistry_mount_readings.json"),
                Server.MapPath("~/App_Data/palmistry_lucky_readings.json"),
                Server.MapPath("~/App_Data/palmistry_remedy_readings.json"));
        }
    }
}
