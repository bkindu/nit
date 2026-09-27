using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    [SessionState(SessionStateBehavior.Required)]
    public class BirthDataController : Controller
    {
        private readonly NativeRepository _natives = new NativeRepository();

        [HttpGet]
        public ActionResult Index(int? id)
        {
            ViewBag.Title = "Birth Data";
            var model = new BirthDataViewModel();
            if (id.HasValue)
            {
                try
                {
                    var native = _natives.GetById(id.Value);
                    if (native != null) model = FromNative(native);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "The native record could not be loaded. Confirm tbl_native exists.";
                    System.Diagnostics.Trace.WriteLine("[BirthData.Index] " + ex.Message);
                }
            }
            else
            {
                var session = BirthDataSessionStore.Get(Session);
                if (session != null) model = FromSession(session);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BirthDataViewModel model, string command)
        {
            ViewBag.Title = "Birth Data";
            if (!ModelState.IsValid)
                return View(model);

            var time = model.To24HourTime();
            var dob = model.DateOfBirth.GetValueOrDefault(DateTime.Today);
            var session = new BirthDataSessionModel
            {
                Name = model.Name,
                Gender = ExpandGender(model.Gender),
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
                ReportChoice = String.IsNullOrWhiteSpace(model.ReportChoice) ? "KUNDALI" : model.ReportChoice.ToUpperInvariant()
            };
            BirthDataSessionStore.Set(Session, session);

            if (model.SaveRecord || String.Equals(command, "save", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var native = ToNative(model, dob, time);
                    if (model.NativeId.HasValue && model.NativeId.Value > 0)
                    {
                        native.NativeId = model.NativeId.Value;
                        _natives.Update(native);
                        TempData["SuccessMessage"] = "Birth data updated.";
                    }
                    else
                    {
                        model.NativeId = _natives.Save(native);
                        TempData["SuccessMessage"] = "Birth data saved as native record #" + model.NativeId + ".";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("[BirthData.Save] " + ex.Message);
                    TempData["ErrorMessage"] = "The record could not be saved. Run App_Data/04_tbl_native.sql on MySQL.";
                    return View(model);
                }
            }

            if (String.Equals(command, "match", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "MatchMaking");
            if (String.Equals(command, "numerology", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Numerology");

            return RedirectToAction("KundliResult", "Home");
        }

        private static string ExpandGender(string gender)
        {
            if (String.IsNullOrWhiteSpace(gender)) return "Male";
            var code = gender.Trim().Substring(0, 1).ToUpperInvariant();
            return code == "F" ? "Female" : (code == "O" ? "Other" : "Male");
        }

        private static BirthDataViewModel FromSession(BirthDataSessionModel session)
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
                Gender = String.IsNullOrWhiteSpace(session.Gender) ? "M" : session.Gender.Substring(0, 1).ToUpperInvariant(),
                DateOfBirth = dob,
                Hour = hour12,
                Minute = session.Minutes,
                Meridiem = meridiem,
                PlaceId = session.PlaceId,
                PlaceName = session.Place,
                CountryName = session.Country,
                Latitude = Convert.ToDecimal(session.Latitude),
                Longitude = Convert.ToDecimal(session.Longitude),
                Timezone = session.Timezone,
                ReportChoice = session.ReportChoice
            };
        }

        private static BirthDataViewModel FromNative(NativeRecordModel native)
        {
            var time = native.BirthTime.GetValueOrDefault(new TimeSpan(7, 47, 0));
            var hour24 = time.Hours;
            var meridiem = hour24 >= 12 ? "PM" : "AM";
            var hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12;
            return new BirthDataViewModel
            {
                NativeId = native.NativeId,
                Name = native.NativeName,
                NickName = native.NickName,
                FatherName = native.FatherName,
                Gender = native.Gender,
                DateOfBirth = native.BirthDate,
                Hour = hour12,
                Minute = time.Minutes,
                Meridiem = meridiem,
                PlaceId = native.PlaceId,
                PlaceName = native.BirthPlace,
                CountryName = native.CountryName,
                Latitude = native.Latitude,
                Longitude = native.Longitude,
                Timezone = native.TimeZone.HasValue ? Convert.ToDouble(native.TimeZone.Value) : 5.5,
                Address = native.AddressText,
                Remarks = native.Remarks,
                SaveRecord = true
            };
        }

        private static NativeRecordModel ToNative(BirthDataViewModel model, DateTime dob, TimeSpan time)
        {
            return new NativeRecordModel
            {
                NativeName = model.Name,
                Gender = model.Gender,
                BirthDate = dob,
                BirthDateText = dob.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                BirthTime = time,
                BirthTimeText = time.ToString(@"hh\:mm"),
                BirthPlace = model.PlaceName,
                CountryName = model.CountryName,
                PlaceId = model.PlaceId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                TimeZone = Convert.ToDecimal(model.Timezone),
                NickName = model.NickName,
                FatherName = model.FatherName,
                AddressText = model.Address,
                Remarks = model.Remarks
            };
        }
    }
}
