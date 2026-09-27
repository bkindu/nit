using System;
using System.Collections.Generic;
using System.Linq;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Builds display-only Kundli details from an already calculated KundliResult.
    /// It does not recalculate planetary longitudes or alter the existing astronomy engine.
    /// </summary>
    public sealed class KundliDetailsBuilder
    {
        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        private static readonly string[] Nakshatras =
        {
            "", "Ashwini", "Bharani", "Krittika", "Rohini", "Mrigashira", "Ardra",
            "Punarvasu", "Pushya", "Ashlesha", "Magha", "Purva Phalguni", "Uttara Phalguni",
            "Hasta", "Chitra", "Swati", "Vishakha", "Anuradha", "Jyeshtha", "Mula",
            "Purva Ashadha", "Uttara Ashadha", "Shravana", "Dhanishtha", "Shatabhisha",
            "Purva Bhadrapada", "Uttara Bhadrapada", "Revati"
        };

        private static readonly string[] PayaByDifference =
        {
            "Svarna", "Rajata", "Tamra", "Lauha", "Rajata", "Svarna",
            "Tamra", "Lauha", "Rajata", "Tamra", "Svarna", "Lauha"
        };

        public KundliDetailsModel Build(KundliResult result)
        {
            return Build(result, null);
        }

        public KundliDetailsModel Build(KundliResult result, KundliWorkflowRequest request)
        {
            if (result == null) throw new ArgumentNullException("result");

            var planets = result.PlanetPositions ?? new List<PlanetPosition>();
            var ascendant = planets.FirstOrDefault(p => p.House == 1 &&
                string.Equals(p.Name, "Ascendant", StringComparison.OrdinalIgnoreCase))
                ?? result.Ascendant;
            var moon = FindPlanet(planets, "Moon", "Mon", "Chandra");
            var sun = FindPlanet(planets, "Sun", "Surya");
            var mars = FindPlanet(planets, "Mars", "Mangal", "Mar");

            var moonRashi = moon == null ? 0 : moon.Rashi;
            var birthNakshatra = moon == null ? 0 : NakshatraNumber(moon.TotalDegrees);
            var details = new KundliDetailsModel
            {
                BirthRashi = moonRashi,
                BirthRashiName = RashiName(moonRashi),
                SunSignName = sun == null ? "" : RashiName(sun.Rashi),
                NakshatraNumber = birthNakshatra,
                NakshatraName = birthNakshatra > 0 ? Nakshatras[birthNakshatra] : "",
                NakshatraPada = moon == null ? 0 : NakshatraPada(moon.TotalDegrees),
                IsGandmool = IsGandmoolNakshatra(birthNakshatra),
                PayaRashiWise = PayaRashiWise(ascendant == null ? 0 : ascendant.Rashi, moonRashi),
                PayaNakshatraWise = PayaNakshatraWise(birthNakshatra),
                IsManglik = mars != null && new[] { 1, 4, 7, 8, 12 }.Contains(mars.House),
                PercentageNote = "House occupancy percentage is a display summary. Desktop Shadbala percentage requires the separate Shadbala calculation module."
            };

            if (request != null)
                details.Panchang = new LegacyPanchangCalculator().Calculate(result, request);

            details.RetrogradePlanets = planets.Where(p => p.IsRetrograde).Select(p => p.Name).ToList();
            details.RashiSandhiPlanets = planets.Where(p => IsRashiSandhi(p.DegreesInRashi)).Select(p => p.Name).ToList();
            details.Planets = planets.Select(p =>
            {
                var nak = NakshatraNumber(p.TotalDegrees);
                var rakv = AstroMath.ToRakvParts(p.TotalDegrees);
                var ua = UdayAstaLabel(p, result.JulianDayFraction);
                return new PlanetDetailModel
                {
                    Name = p.Name,
                    Code = PlanetCode(p.Name),
                    FullName = PlanetFullName(p.Name),
                    Rashi = p.Rashi,
                    RashiName = p.RashiName,
                    House = p.House,
                    TotalDegrees = p.TotalDegrees,
                    DegreesInRashi = p.DegreesInRashi,
                    IsRetrograde = p.IsRetrograde,
                    Motion = MotionLabel(p),
                    UdayAsta = ua,
                    NakshatraNumber = nak,
                    NakshatraPada = NakshatraPada(p.TotalDegrees),
                    NakshatraName = nak > 0 ? Nakshatras[nak] : "",
                    NakshatraLord = NakshatraLord(nak),
                    Dignity = Dignity(p),
                    DailyMotion = p.DailyMotion,
                    Gati = GatiText(p),
                    NakshatraDevta = NakshatraDevta(nak),
                    PositionRakv = AstroMath.FormatRakv(p.TotalDegrees),
                    Rashi0 = rakv.Rashi0
                };
            }).ToList();

            for (var house = 1; house <= 12; house++)
            {
                var houseData = (result.Houses ?? new List<HouseData>()).FirstOrDefault(h => h.HouseNumber == house);
                var count = details.Planets.Count(p => p.House == house);
                details.HousePercentages.Add(new HousePercentageModel
                {
                    House = house,
                    Rashi = houseData == null ? 0 : houseData.Rashi,
                    RashiName = houseData == null ? "" : houseData.RashiName,
                    PlanetCount = count,
                    OccupancyPercent = Math.Min(100, count * 20)
                });
            }

            return details;
        }

        private static PlanetPosition FindPlanet(IEnumerable<PlanetPosition> planets, params string[] names)
        {
            return planets.FirstOrDefault(p => names.Any(n => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase)));
        }

        private static string RashiName(int rashi)
        {
            return rashi >= 1 && rashi <= 12 ? Rashis[rashi] : "";
        }

        private static string PlanetCode(string name)
        {
            switch ((name ?? "").ToLowerInvariant())
            {
                case "ascendant": return "ASC";
                case "sun": return "SUN";
                case "moon": return "MON";
                case "mars": return "MAR";
                case "mercury": return "MER";
                case "jupiter": return "JUP";
                case "venus": return "VEN";
                case "saturn": return "SAT";
                case "rahu": return "RAH";
                case "ketu": return "KET";
                default: return name == null ? "" : name.ToUpperInvariant();
            }
        }

        private static string MotionLabel(PlanetPosition p)
        {
            if (p == null || string.Equals(p.Name, "Ascendant", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Lagna", StringComparison.OrdinalIgnoreCase)) return "-";
            if (p.IsRetrograde || string.Equals(p.Name, "Rahu", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Name, "Ketu", StringComparison.OrdinalIgnoreCase)) return "Vakri";
            if (string.Equals(p.Name, "Sun", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Name, "Moon", StringComparison.OrdinalIgnoreCase)) return "Sighar";
            return "Margi";
        }

        private static string UdayAstaLabel(PlanetPosition p, double dJdf)
        {
            if (p == null || string.Equals(p.Name, "Ascendant", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Lagna", StringComparison.OrdinalIgnoreCase)) return "-";
            if (string.Equals(p.Name, "Rahu", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Ketu", StringComparison.OrdinalIgnoreCase)) return "Astha";
            if (string.Equals(p.Name, "Sun", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Moon", StringComparison.OrdinalIgnoreCase)) return "Sadodya";

            // VB6 Uday_Astha table lookup for Mars..Saturn
            var looked = UdayAsthaLookup.ForPlanet(p.Name, dJdf);
            if (!string.IsNullOrEmpty(looked)) return looked;
            return "Uday";
        }

        private static string PlanetFullName(string name)
        {
            switch ((name ?? "").ToLowerInvariant())
            {
                case "ascendant":
                case "lagna": return "Lagna";
                case "sun": return "Surya";
                case "moon": return "Chandra";
                case "mars": return "Mangla";
                case "mercury": return "Budha";
                case "jupiter": return "Guru";
                case "venus": return "Sukra";
                case "saturn": return "Sani";
                case "rahu": return "Rahu";
                case "ketu": return "Ketu";
                default: return name ?? "";
            }
        }

        private static string GatiText(PlanetPosition p)
        {
            if (p == null || string.Equals(p.Name, "Ascendant", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Lagna", StringComparison.OrdinalIgnoreCase)) return "-";
            if (string.Equals(p.Name, "Rahu", StringComparison.OrdinalIgnoreCase)
                || string.Equals(p.Name, "Ketu", StringComparison.OrdinalIgnoreCase)) return "3:11";
            var isMoon = string.Equals(p.Name, "Moon", StringComparison.OrdinalIgnoreCase);
            return AstroMath.FormatGatiKala(p.DailyMotion, isMoon);
        }

        private static string NakshatraDevta(int nakshatra)
        {
            var devtas = new[] { "", "Aswini", "Yam", "Agani", "Brahma", "Chandra", "Shiv", "Aditi", "Guru", "Sarpa", "Pitar", "Bhag", "Aryama", "Surya", "Tvasta", "Vayu", "Indragni", "Mitar", "Inder", "Rakshas", "Jal", "Visvedev", "Vishnu", "Dasu", "Varun", "Ajpad", "Ahirbudhany", "Poosha" };
            return nakshatra >= 1 && nakshatra < devtas.Length ? devtas[nakshatra] : "-";
        }

        private static string Dignity(PlanetPosition p)
        {
            if (p == null) return "-";
            var r = p.Rashi;
            var n = (p.Name ?? "").ToLowerInvariant();
            if (n == "sun") return r == 1 ? "Ucha" : r == 7 ? "Neecha" : r == 5 ? "Swa" : new[] { 1, 4, 8, 9, 12 }.Contains(r) ? "Mitra" : new[] { 2, 7, 10, 11 }.Contains(r) ? "Satru" : "Sama";
            if (n == "moon") return r == 2 ? "Ucha" : r == 8 ? "Neecha" : r == 4 ? "Swa" : new[] { 3, 5, 6 }.Contains(r) ? "Mitra" : "Sama";
            if (n == "mars") return r == 10 ? "Ucha" : r == 4 ? "Neecha" : new[] { 1, 8 }.Contains(r) ? "Swa" : new[] { 4, 5, 9, 12 }.Contains(r) ? "Mitra" : new[] { 3, 6 }.Contains(r) ? "Satru" : "Sama";
            if (n == "mercury") return r == 6 ? "Ucha" : r == 12 ? "Neecha" : new[] { 3, 6 }.Contains(r) ? "Swa" : new[] { 2, 5, 7 }.Contains(r) ? "Mitra" : r == 4 ? "Satru" : "Sama";
            if (n == "jupiter") return r == 4 ? "Ucha" : r == 10 ? "Neecha" : new[] { 9, 12 }.Contains(r) ? "Swa" : new[] { 1, 4, 5, 8 }.Contains(r) ? "Mitra" : new[] { 2, 3, 6, 7 }.Contains(r) ? "Satru" : "Sama";
            if (n == "venus") return r == 12 ? "Ucha" : r == 6 ? "Neecha" : new[] { 2, 7 }.Contains(r) ? "Swa" : new[] { 3, 6, 10, 11 }.Contains(r) ? "Mitra" : new[] { 4, 5 }.Contains(r) ? "Satru" : "Sama";
            if (n == "saturn") return r == 7 ? "Ucha" : r == 1 ? "Neecha" : new[] { 10, 11 }.Contains(r) ? "Swa" : new[] { 2, 3, 6, 7 }.Contains(r) ? "Mitra" : new[] { 1, 4, 5, 8 }.Contains(r) ? "Satru" : "Sama";
            return "-";
        }

        private static string NakshatraLord(int nakshatra)
        {
            var lords = new[] { "", "Ket", "Ven", "Sur", "Cha", "Man", "Rah", "Gur", "San", "Bud" };
            if (nakshatra < 1 || nakshatra > 27) return "";
            return lords[((nakshatra - 1) % 9) + 1];
        }

        private static int NakshatraNumber(double longitude)
        {
            var normalized = Normalize(longitude);
            return Math.Min(27, (int)Math.Floor(normalized / (360.0 / 27.0)) + 1);
        }

        private static int NakshatraPada(double longitude)
        {
            var normalized = Normalize(longitude);
            var segment = normalized / (360.0 / 108.0);
            return Math.Min(4, (int)Math.Floor(segment % 4) + 1);
        }

        private static bool IsGandmoolNakshatra(int nakshatra)
        {
            return new[] { 1, 9, 10, 18, 19, 27 }.Contains(nakshatra);
        }

        private static string PayaRashiWise(int ascendantRashi, int moonRashi)
        {
            if (ascendantRashi < 1 || moonRashi < 1) return "";
            var difference = ((moonRashi - ascendantRashi) % 12 + 12) % 12;
            return PayaByDifference[difference];
        }

        private static string PayaNakshatraWise(int nakshatra)
        {
            if (nakshatra >= 6 && nakshatra <= 12) return "Rajata";
            if (nakshatra >= 18 && nakshatra <= 24) return "Svarna";
            if ((nakshatra >= 25 && nakshatra <= 27) || nakshatra <= 2) return "Tamra";
            if (nakshatra >= 3 && nakshatra <= 5) return "Lauha";
            return "";
        }

        private static bool IsRashiSandhi(double degreesInRashi)
        {
            return degreesInRashi >= 29.0 || degreesInRashi < 1.0;
        }

        private static double Normalize(double value)
        {
            var result = value % 360.0;
            return result < 0 ? result + 360.0 : result;
        }
    }
}
