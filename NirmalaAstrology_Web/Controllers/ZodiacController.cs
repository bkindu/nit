using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Zodiac listing and detail pages.
    /// Routes: /Zodiac, /Zodiac/Index, /Zodiac/{name} (e.g. /Zodiac/aries).
    /// </summary>
    public class ZodiacController : Controller
    {
        private readonly ZodiacRepository _zodiacRepo = new ZodiacRepository();

        /// <summary>GET: Zodiac / Zodiac/Index — all 12 signs.</summary>
        public ActionResult Index()
        {
            var zodiacs = SafeGetAll();
            ViewBag.Title = "Zodiac Signs | Nirmala Astrology";
            return View(zodiacs);
        }

        /// <summary>GET: Zodiac/{name} — one sign (e.g. aries).</summary>
        public ActionResult Detail(string name)
        {
            if (string.IsNullOrWhiteSpace(name)
                || string.Equals(name, "Index", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Detail", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Details", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index");
            }

            var model = new ZodiacDetailViewModel();
            var all = SafeGetAll();
            model.AllZodiacs = all;
            model.Zodiac = FindByName(all, name);

            if (model.Zodiac == null)
            {
                try
                {
                    model.Zodiac = _zodiacRepo.GetZodiacByName(name);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("[Zodiac.Detail] " + ex.Message);
                }
            }

            if (model.Zodiac == null)
            {
                return HttpNotFound("Zodiac sign not found: " + name);
            }

            ViewBag.Title = model.Zodiac.Name + " - " + model.Zodiac.HindiName + " | Nirmala Astrology";
            ViewBag.MetaDescription = model.Zodiac.Description;
            return View(model);
        }

        private List<ZodiacModel> SafeGetAll()
        {
            try
            {
                var list = _zodiacRepo.GetAllZodiacs();
                if (list != null && list.Count > 0) return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Zodiac.Index] " + ex.Message);
            }
            return GetStaticZodiacList();
        }

        private static ZodiacModel FindByName(IList<ZodiacModel> list, string name)
        {
            if (list == null || string.IsNullOrWhiteSpace(name)) return null;
            var key = name.Trim().ToLowerInvariant().Replace(" ", "-");
            return list.FirstOrDefault(z =>
                string.Equals(z.UrlName, key, StringComparison.OrdinalIgnoreCase)
                || string.Equals(z.Name, name, StringComparison.OrdinalIgnoreCase)
                || string.Equals((z.Name ?? "").Replace(" ", "-"), key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Fallback when MySQL Zodiac table is empty/unavailable.</summary>
        internal static List<ZodiacModel> GetStaticZodiacList()
        {
            return new List<ZodiacModel>
            {
                new ZodiacModel { ZodiacId = 1, Name = "Aries", HindiName = "Mesh", Element = "Fire", Planet = "Mars", Icon = "&#9800;", Description = "Aries is the first sign of the zodiac — bold, energetic, and pioneering." },
                new ZodiacModel { ZodiacId = 2, Name = "Taurus", HindiName = "Vrishabh", Element = "Earth", Planet = "Venus", Icon = "&#9801;", Description = "Taurus is steady, sensual, and values security and beauty." },
                new ZodiacModel { ZodiacId = 3, Name = "Gemini", HindiName = "Mithun", Element = "Air", Planet = "Mercury", Icon = "&#9802;", Description = "Gemini is curious, communicative, and adaptable." },
                new ZodiacModel { ZodiacId = 4, Name = "Cancer", HindiName = "Kark", Element = "Water", Planet = "Moon", Icon = "&#9803;", Description = "Cancer is nurturing, intuitive, and deeply connected to home." },
                new ZodiacModel { ZodiacId = 5, Name = "Leo", HindiName = "Singh", Element = "Fire", Planet = "Sun", Icon = "&#9804;", Description = "Leo is confident, creative, and naturally leadership-oriented." },
                new ZodiacModel { ZodiacId = 6, Name = "Virgo", HindiName = "Kanya", Element = "Earth", Planet = "Mercury", Icon = "&#9805;", Description = "Virgo is analytical, practical, and detail-focused." },
                new ZodiacModel { ZodiacId = 7, Name = "Libra", HindiName = "Tula", Element = "Air", Planet = "Venus", Icon = "&#9806;", Description = "Libra seeks balance, harmony, and partnership." },
                new ZodiacModel { ZodiacId = 8, Name = "Scorpio", HindiName = "Vrishchik", Element = "Water", Planet = "Mars", Icon = "&#9807;", Description = "Scorpio is intense, transformative, and deeply perceptive." },
                new ZodiacModel { ZodiacId = 9, Name = "Sagittarius", HindiName = "Dhanu", Element = "Fire", Planet = "Jupiter", Icon = "&#9808;", Description = "Sagittarius loves freedom, wisdom, and exploration." },
                new ZodiacModel { ZodiacId = 10, Name = "Capricorn", HindiName = "Makar", Element = "Earth", Planet = "Saturn", Icon = "&#9809;", Description = "Capricorn is disciplined, ambitious, and responsible." },
                new ZodiacModel { ZodiacId = 11, Name = "Aquarius", HindiName = "Kumbh", Element = "Air", Planet = "Saturn", Icon = "&#9810;", Description = "Aquarius is innovative, humanitarian, and independent." },
                new ZodiacModel { ZodiacId = 12, Name = "Pisces", HindiName = "Meen", Element = "Water", Planet = "Jupiter", Icon = "&#9811;", Description = "Pisces is compassionate, imaginative, and spiritually inclined." }
            };
        }
    }
}
