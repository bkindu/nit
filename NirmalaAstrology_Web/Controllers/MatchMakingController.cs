using System;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    [SessionState(SessionStateBehavior.Required)]
    public class MatchMakingController : Controller
    {
        public const string FemaleSessionKey = "NirmalaAstrology.BirthData.Female";

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Match Making";
            var model = new MatchMakingViewModel();
            var male = BirthDataSessionStore.Get(Session);
            if (male != null) model.Male = ToView(male, "M");
            var female = Session[FemaleSessionKey] as BirthDataSessionModel;
            if (female != null) model.Female = ToView(female, "F");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MatchMakingViewModel model)
        {
            ViewBag.Title = "Match Making";
            if (model.Male == null) model.Male = new BirthDataViewModel { Gender = "M" };
            if (model.Female == null) model.Female = new BirthDataViewModel { Gender = "F" };

            if (!model.Male.DateOfBirth.HasValue) ModelState.AddModelError("Male.DateOfBirth", "Male date of birth is required.");
            if (!model.Female.DateOfBirth.HasValue) ModelState.AddModelError("Female.DateOfBirth", "Female date of birth is required.");
            if (!model.Male.Latitude.HasValue || !model.Male.Longitude.HasValue)
                ModelState.AddModelError("Male.PlaceId", "Select the male birth place so latitude/longitude are filled.");
            if (!model.Female.Latitude.HasValue || !model.Female.Longitude.HasValue)
                ModelState.AddModelError("Female.PlaceId", "Select the female birth place so latitude/longitude are filled.");
            if (!ModelState.IsValid) return View(model);

            BirthDataSessionStore.Set(Session, ToSession(model.Male, "Male"));
            Session[FemaleSessionKey] = ToSession(model.Female, "Female");

            try
            {
                var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
                var service = new MatchMakingService(scriptPath);
                model.Result = service.Compare(ToInput(model.Male), ToInput(model.Female));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[MatchMaking] " + ex);
                ModelState.AddModelError("", "Match making could not be completed. Confirm tbl_match_det has been migrated.");
            }
            return View(model);
        }

        private static BirthDataViewModel ToView(BirthDataSessionModel session, string gender)
        {
            var hour24 = session.Hours;
            var meridiem = hour24 >= 12 ? "PM" : "AM";
            var hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12;
            DateTime? dob = null;
            if (session.Year > 0 && session.Month > 0 && session.Day > 0)
                dob = new DateTime(session.Year, session.Month, session.Day);
            return new BirthDataViewModel
            {
                Name = session.Name,
                Gender = gender,
                DateOfBirth = dob,
                Hour = hour12,
                Minute = session.Minutes,
                Meridiem = meridiem,
                PlaceId = session.PlaceId,
                PlaceName = session.Place,
                CountryName = session.Country,
                Latitude = Convert.ToDecimal(session.Latitude),
                Longitude = Convert.ToDecimal(session.Longitude),
                Timezone = session.Timezone == 0 ? 5.5 : session.Timezone
            };
        }

        private static BirthDataSessionModel ToSession(BirthDataViewModel model, string fallbackName)
        {
            var time = model.To24HourTime();
            var dob = model.DateOfBirth.GetValueOrDefault(DateTime.Today);
            return new BirthDataSessionModel
            {
                Name = String.IsNullOrWhiteSpace(model.Name) ? fallbackName : model.Name,
                Gender = model.Gender,
                Day = dob.Day,
                Month = dob.Month,
                Year = dob.Year,
                Hours = time.Hours,
                Minutes = time.Minutes,
                Latitude = Convert.ToDouble(model.Latitude.GetValueOrDefault()),
                Longitude = Convert.ToDouble(model.Longitude.GetValueOrDefault()),
                Timezone = model.Timezone == 0 ? 5.5 : model.Timezone,
                PlaceId = model.PlaceId,
                Place = model.PlaceName,
                Country = model.CountryName,
                ReportChoice = "MATCH_MAKING"
            };
        }

        private static MatchPersonInput ToInput(BirthDataViewModel model)
        {
            var time = model.To24HourTime();
            var dob = model.DateOfBirth.GetValueOrDefault();
            return new MatchPersonInput
            {
                Name = model.Name,
                Day = dob.Day,
                Month = dob.Month,
                Year = dob.Year,
                Hour = time.Hours,
                Minute = time.Minutes,
                Latitude = Convert.ToDouble(model.Latitude.GetValueOrDefault()),
                Longitude = Convert.ToDouble(model.Longitude.GetValueOrDefault()),
                Timezone = model.Timezone == 0 ? 5.5 : model.Timezone,
                Place = model.PlaceName
            };
        }
    }
}
