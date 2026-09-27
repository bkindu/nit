using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Port of rLalKitab.Lal_Kitab — Janam houses + age digit remap + optional remedies.</summary>
    public sealed class LalKitabService
    {
        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        private static readonly string[] PlanetNames =
        {
            "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu"
        };

        // VB Mid("SUNMONMARMERJUPVENSATRAHKET", Y*3-2, 3) for Y=1..9 → planets 2..10
        private static readonly string[] PlanetCodes =
        {
            "", "", "SUN", "MON", "MAR", "MER", "JUP", "VEN", "SAT", "RAH", "KET"
        };

        private readonly string _digitsJsonPath;

        public LalKitabService(string kundliScriptPath, string digitsJsonPath)
        {
            _digitsJsonPath = digitsJsonPath;
        }

        public LalKitabResult Calculate(BirthDataSessionModel birth, int ageYear, string remedyMode)
        {
            if (birth == null) throw new ArgumentNullException("birth");
            if (ageYear < 1) ageYear = 1;
            if (ageYear > 110) ageYear = 110;
            if (string.IsNullOrWhiteSpace(remedyMode)) remedyMode = "Janam";
            var useJanam = remedyMode.Equals("Janam", StringComparison.OrdinalIgnoreCase);

            var calc = new KundliCalculator();
            var chart = calc.CalculateKundli(
                birth.Day, birth.Month, birth.Year, birth.Hours, birth.Minutes,
                birth.Longitude, birth.Latitude, birth.Timezone);

            var dPlt = new double[11];
            dPlt[1] = chart.Lagna;
            dPlt[2] = chart.Sun;
            dPlt[3] = chart.Moon;
            dPlt[4] = chart.Mars;
            dPlt[5] = chart.Mercury;
            dPlt[6] = chart.Jupiter;
            dPlt[7] = chart.Venus;
            dPlt[8] = chart.Saturn;
            dPlt[9] = chart.Rahu;
            dPlt[10] = chart.Ketu;

            var lagna = AstroMath.FnTgr(dPlt[1] / 30.0) + 1;
            var janamHouse = new int[11];
            var janamSign = new int[11];
            for (var z = 1; z <= 10; z++)
            {
                janamSign[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                janamHouse[z] = AstroMath.IFnR(janamSign[z] - lagna, 12) + 1;
            }

            var remap = LoadHouseRemap(ageYear);
            var varshHouse = new int[11];
            var varshSign = new int[11];
            varshHouse[1] = 1;
            varshSign[1] = lagna;
            for (var z = 2; z <= 10; z++)
            {
                varshHouse[z] = remap[janamHouse[z]];
                varshSign[z] = AstroMath.IFnR(varshHouse[z] + lagna - 1, 12);
                if (varshSign[z] == 0) varshSign[z] = 12;
            }

            var result = new LalKitabResult
            {
                NativeName = birth.Name ?? "Native",
                BirthSummary = string.Format("{0:00}.{1:00}.{2} {3:00}:{4:00} · {5}",
                    birth.Day, birth.Month, birth.Year, birth.Hours, birth.Minutes, birth.Place ?? ""),
                AgeYear = ageYear,
                RemedyMode = useJanam ? "Janam" : "Varsh",
                LagnaSign = lagna,
                LagnaSignName = Rashis[lagna],
                HouseRemap = remap
            };

            for (var z = 1; z <= 10; z++)
            {
                result.JanamPlanets.Add(Row(z, janamSign[z], janamHouse[z]));
                result.VarshPlanets.Add(Row(z, varshSign[z], varshHouse[z]));
            }

            var houseForRemedy = useJanam ? janamHouse : varshHouse;
            // VB Janam restores only 2..9; we include Ketu (10) for both modes.
            var readings = LoadLalKitabReadings();
            for (var z = 2; z <= 10; z++)
            {
                var house = houseForRemedy[z];
                var key = PlanetCodes[z] + "-" + house;
                string text;
                if (!readings.TryGetValue(PlanetCodes[z] + ":" + house, out text) || string.IsNullOrWhiteSpace(text))
                    text = "(No Lal Kitab reading in MySQL for " + key + ")";
                result.Remedies.Add(new LalKitabRemedyRow
                {
                    Planet = PlanetNames[z],
                    Code = PlanetCodes[z],
                    House = house,
                    Key = key,
                    Reading = text
                });
            }

            result.Notes.Add("Varsh houses from Lal_Kitab_Digits age row (not Tajika Varsh Parvesh).");
            result.Notes.Add("Remedies from tbl_planet_house_reading.lal_kitab_reading when migrated.");
            result.Notes.Add("Age year matches VB: Year(today) − Year(birth), overridable.");
            return result;
        }

        private static LalKitabPlanetRow Row(int z, int sign, int house)
        {
            return new LalKitabPlanetRow
            {
                Planet = PlanetNames[z],
                Code = z == 1 ? "LAG" : PlanetCodes[z],
                Sign = sign,
                SignName = Rashis[sign],
                House = house
            };
        }

        private int[] LoadHouseRemap(int ageYear)
        {
            var remap = new int[13];
            for (var i = 1; i <= 12; i++) remap[i] = i;
            if (string.IsNullOrEmpty(_digitsJsonPath) || !File.Exists(_digitsJsonPath))
                return remap;

            var json = File.ReadAllText(_digitsJsonPath);
            var rows = JsonConvert.DeserializeObject<List<DigitRow>>(json);
            if (rows == null) return remap;
            foreach (var row in rows)
            {
                if (row == null || row.id != ageYear || row.houses == null || row.houses.Count < 12)
                    continue;
                for (var i = 0; i < 12; i++)
                    remap[i + 1] = row.houses[i];
                break;
            }
            return remap;
        }

        private static Dictionary<string, string> LoadLalKitabReadings()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (var db = new DatabaseHelper())
                {
                    var dt = db.ExecuteDataTable(@"
SELECT p.planet_code, ph.house_id, ph.lal_kitab_reading
FROM tbl_planet_house_reading ph
JOIN tbl_planet p ON p.planet_id = ph.planet_id
JOIN tbl_language l ON l.language_id = ph.language_id
WHERE l.language_code = @Language
  AND ph.lal_kitab_reading IS NOT NULL
  AND TRIM(ph.lal_kitab_reading) <> ''",
                        new MySqlParameter("@Language", "en"));
                    foreach (DataRow row in dt.Rows)
                    {
                        var code = Convert.ToString(row["planet_code"]);
                        var house = Convert.ToInt32(row["house_id"]);
                        var text = Convert.ToString(row["lal_kitab_reading"]);
                        map[code + ":" + house] = text;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[LalKitab readings] " + ex.Message);
            }
            return map;
        }

        private sealed class DigitRow
        {
            public int id { get; set; }
            public List<int> houses { get; set; }
        }
    }
}
