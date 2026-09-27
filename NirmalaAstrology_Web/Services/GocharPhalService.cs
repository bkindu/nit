using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// MVP port of Text_Phalit.Gochar_Phal (English):
    /// transit planet sign → house from natal Lagna → Plt9xHs12 Gochar_Read_E
    /// (MySQL tbl_planet_house_reading.gochar_reading).
    /// Full RasPrves entry/exit Jantri windows deferred.
    /// </summary>
    public sealed class GocharPhalService
    {
        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        // VB Mid("SATRAHKETJUPMARSUNVENMERMON", …) for iPlt 1..8 — skip KET
        private static readonly int[] TransitPlanetIndexes = { 8, 9, 6, 4, 2, 7, 5 }; // SAT RAH JUP MAR SUN VEN MER
        private static readonly string[] TransitCodes = { "SAT", "RAH", "JUP", "MAR", "SUN", "VEN", "MER" };
        private static readonly string[] TransitNames = { "Saturn", "Rahu", "Jupiter", "Mars", "Sun", "Venus", "Mercury" };

        public GocharPhalService(string kundliScriptPath = null)
        {
        }

        public GocharPhalResult Calculate(BirthDataSessionModel birth, DateTime transitDate)
        {
            if (birth == null) throw new ArgumentNullException("birth");

            var calc = new KundliCalculator();
            var natal = calc.CalculateKundli(
                birth.Day, birth.Month, birth.Year, birth.Hours, birth.Minutes,
                birth.Longitude, birth.Latitude, birth.Timezone);
            var lagna = AstroMath.FnTgr(natal.Lagna / 30.0) + 1;

            var transit = calc.CalculateKundli(
                transitDate.Day, transitDate.Month, transitDate.Year, 12, 0,
                birth.Longitude, birth.Latitude, birth.Timezone > 0 ? birth.Timezone : 5.5);

            var longitudes = new double[11];
            longitudes[2] = transit.Sun;
            longitudes[3] = transit.Moon;
            longitudes[4] = transit.Mars;
            longitudes[5] = transit.Mercury;
            longitudes[6] = transit.Jupiter;
            longitudes[7] = transit.Venus;
            longitudes[8] = transit.Saturn;
            longitudes[9] = transit.Rahu;
            longitudes[10] = transit.Ketu;

            var readings = LoadGocharReadings();
            var result = new GocharPhalResult
            {
                NativeName = birth.Name ?? "Native",
                BirthSummary = string.Format("{0:00}.{1:00}.{2} {3:00}:{4:00} · {5}",
                    birth.Day, birth.Month, birth.Year, birth.Hours, birth.Minutes, birth.Place ?? ""),
                TransitDateLabel = transitDate.ToString("dd.MM.yyyy"),
                NatalLagnaSign = lagna,
                NatalLagnaName = Rashis[lagna]
            };

            for (var i = 0; i < TransitPlanetIndexes.Length; i++)
            {
                var idx = TransitPlanetIndexes[i];
                var sign = AstroMath.FnTgr(longitudes[idx] / 30.0) + 1;
                var house = AstroMath.IFnR(sign - lagna, 12) + 1;
                var code = TransitCodes[i];
                string text;
                if (!readings.TryGetValue(code + ":" + house, out text) || string.IsNullOrWhiteSpace(text))
                    text = "(No gochar reading in MySQL for " + code + "-" + house + ")";

                result.Planets.Add(new GocharPlanetRow
                {
                    Planet = TransitNames[i],
                    Code = code,
                    TransitSign = sign,
                    TransitSignName = Rashis[sign],
                    HouseFromNatalLagna = house,
                    Reading = text
                });
            }

            result.Notes.Add("Houses counted from natal Lagna (VB Gochar_Phal iHouseNum).");
            result.Notes.Add("Moon and Ketu omitted to match VB loop (KET skipped; Moon separate Gochar_Moon).");
            result.Notes.Add("Transit positions from kundli engine at 12:00 on transit date (not RasPrves entry/exit windows).");
            return result;
        }

        private static Dictionary<string, string> LoadGocharReadings()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (var db = new DatabaseHelper())
                {
                    var dt = db.ExecuteDataTable(@"
SELECT p.planet_code, ph.house_id, ph.gochar_reading
FROM tbl_planet_house_reading ph
JOIN tbl_planet p ON p.planet_id = ph.planet_id
JOIN tbl_language l ON l.language_id = ph.language_id
WHERE l.language_code = @Language
  AND ph.gochar_reading IS NOT NULL
  AND TRIM(ph.gochar_reading) <> ''",
                        new MySqlParameter("@Language", "en"));
                    foreach (DataRow row in dt.Rows)
                    {
                        var code = Convert.ToString(row["planet_code"]);
                        var house = Convert.ToInt32(row["house_id"]);
                        map[code + ":" + house] = Convert.ToString(row["gochar_reading"]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Gochar readings] " + ex.Message);
            }
            return map;
        }
    }
}
