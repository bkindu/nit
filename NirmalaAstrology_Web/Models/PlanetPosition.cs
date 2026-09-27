using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Services
{
public class PlanetPosition
    {
        public string Name { get; set; }
        public int Rashi { get; set; }           // Sign number 1-12 (Aries=1 ... Pisces=12)
        public double DegreesInRashi { get; set; } // Degrees within the sign (0-30)
        public double TotalDegrees { get; set; }   // Total sidereal longitude (0-360)
        public int House { get; set; }               // House placement (1-12)
        public bool IsRetrograde { get; set; }      // True if planet is retrograde (Vakri)
        public double DailyMotion { get; set; }      // Legacy Gati in degrees per day

        private static readonly string[] RashiNames = {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        public string PlanetName { get { return Name; } }
        public string Retrograde { get { return IsRetrograde ? "R" : null; } }
        public string RashiName { get { return Rashi > 0 && Rashi <= 12 ? RashiNames[Rashi] : "?"; } }
        public int Degrees { get { return (int)DegreesInRashi; } }
        public int Minutes { get { return (int)((DegreesInRashi - (int)DegreesInRashi) * 60); } }
    }
}
