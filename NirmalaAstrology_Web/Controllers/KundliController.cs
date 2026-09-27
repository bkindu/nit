using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Controllers
{
    public class KundliController : Controller
    {
        private readonly PlaceRepository _placeRepo = new PlaceRepository();
        private readonly KundliDrawer _kundliDrawer = new KundliDrawer();

        // GET: /Kundli  (or /Kundli/Index)
        // Server-side defaults are set here as a fallback.
        // The client-side JS in Index.cshtml also auto-fills current date/time
        // and selects "Patiala" on page load, so the form is never empty.
        public ActionResult Index()
        {
            PopulateCities();

            // Default Name & Gender (used by the view's ViewBag fallback)
            if (ViewBag.Name == null) ViewBag.Name = "Native";
            if (ViewBag.Gender == null) ViewBag.Gender = "Male";

            var model = new KundliViewModel();

            // Prefer a real Patiala row when present (targeted search, not atlas dump).
            try
            {
                var patiala = FindPatialaPlace();
                if (patiala != null)
                    model.Place = patiala.PlaceID.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[KundliController.Index] Place lookup failed: " + ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(KundliViewModel input)
        {
            PopulateCities();

            // Read Name/Gender from Request.Form so we don't require
            // KundliViewModel to have these properties (they may not exist
            // on the legacy model class). Falls back to defaults if missing.
            ViewBag.Name = !string.IsNullOrEmpty(Request.Form["Name"]) ? Request.Form["Name"] : "Native";
            ViewBag.Gender = !string.IsNullOrEmpty(Request.Form["Gender"]) ? Request.Form["Gender"] : "Male";

            if (ModelState.IsValid && input.Day > 0 && input.Month > 0 && input.Year > 0)
            {
                try
                {
                    int placeId;
                    if (int.TryParse(input.Place, out placeId))
                    {
                        var place = _placeRepo.GetPlaceById(placeId);
                        if (place != null)
                        {
                            input.Latitude = (double)place.Latitude;
                            input.Longitude = (double)place.Longitude;
                            // TimeZone is read for completeness; the calculator
                            // derives timezone from lat/long internally.
                            double timezone = 0;
                            if (!double.TryParse(place.TimeZone, out timezone))
                            {
                                timezone = 5.5;
                            }
                            input.Timezone = timezone;
                        }
                    }

                    var calc = new KundliCalculator();
                    var result = calc.CalculateKundli(
                        input.Day, input.Month, input.Year,
                        input.Hour, input.Minute,
                        input.Longitude ?? 76.25, input.Latitude ?? 30.19,
                        input.Timezone ?? 5.30
                    );
                    input.Result = result;
                    ViewBag.KundliChartHtml = _kundliDrawer.RenderHtml(result, "Kundli");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Kundli Calculation Error: " + ex.Message);
                    ModelState.AddModelError("", "Error calculating Kundli. Please check your birth details and try again.");
                }
            }

            return View(input);
        }

        /// <summary>
        /// Seed dropdown with Patiala only; full place search is AJAX (Home/SearchPlace).
        /// </summary>
        private void PopulateCities()
        {
            var places = new List<PlaceModel>();
            try
            {
                var patiala = FindPatialaPlace();
                if (patiala != null)
                    places.Add(patiala);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[PopulateCities] Error loading places: " + ex.Message);
            }

            if (places.Count == 0)
            {
                places.Add(new PlaceModel
                {
                    PlaceID = 0,
                    PlaceName = "Patiala",
                    Latitude = 30.3398m,
                    Longitude = 76.3869m,
                    TimeZone = "5.5"
                });
            }

            var items = places.Select(p => new SelectListItem
            {
                Value = p.PlaceID.ToString(),
                Text = p.DisplayText ?? p.PlaceName
            }).ToList();

            ViewBag.Cities = new SelectList(items, "Value", "Text");
        }

        private PlaceModel FindPatialaPlace()
        {
            var hits = _placeRepo.SearchPlaces("Patiala") ?? new List<PlaceModel>();
            return hits.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.PlaceName) &&
                p.PlaceName.IndexOf("Patiala", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
