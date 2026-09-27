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
    public class MuhuratController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Muhurat";
            var model = new MuhuratViewModel();
            var session = BirthDataSessionStore.Get(Session);
            if (session != null)
            {
                model.Latitude = (decimal)session.Latitude;
                model.Longitude = (decimal)session.Longitude;
                model.Timezone = session.Timezone > 0 ? session.Timezone : 5.5;
                model.PlaceId = session.PlaceId;
                model.PlaceName = session.Place;
                model.CountryName = session.Country;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MuhuratViewModel model)
        {
            ViewBag.Title = "Muhurat";
            if (model == null) model = new MuhuratViewModel();
            if (!model.Date.HasValue) ModelState.AddModelError("Date", "Date is required.");
            if (!model.Latitude.HasValue || !model.Longitude.HasValue)
                ModelState.AddModelError("PlaceName", "Select a place so latitude/longitude are filled.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var service = new MuhuratService();
                model.Result = service.Calculate(
                    model.Date.Value.Date,
                    (double)model.Latitude.Value,
                    (double)model.Longitude.Value,
                    model.Timezone > 0 ? model.Timezone : 5.5,
                    (model.PlaceName ?? "") + (string.IsNullOrWhiteSpace(model.CountryName) ? "" : ", " + model.CountryName));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Muhurat] " + ex);
                ModelState.AddModelError("", "Muhurat calculation failed. Check date and place.");
            }
            return View(model);
        }
    }
}
