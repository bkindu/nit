using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Services
{
public class KundliResult
    {
        public double Ayanamsa { get; set; }
        /// <summary>Nutated ayan (XLQQ) used when converting sayana→nirayana; for strength/upgraha reverse-add.</summary>
        public double AyanamsaNutated { get; set; }
        /// <summary>VB6 dRahuTrue (nirayana) for miscellaneous TRUE RAHU print.</summary>
        public double TrueRahu { get; set; }
        public double SiderealTime { get; set; }
        public PlanetPosition Ascendant { get; set; }
        public List<PlanetPosition> PlanetPositions { get; set; }
        public List<HouseData> Houses { get; set; }
        public double LagnaDegrees { get; set; }
        public double Lagna { get; set; }
        public double Sun { get; set; }
        public double Moon { get; set; }
        public double Mars { get; set; }
        public double Mercury { get; set; }
        public double Jupiter { get; set; }
        public double Venus { get; set; }
        public double Saturn { get; set; }
        public double Rahu { get; set; }
        public double Ketu { get; set; }
        /// <summary>Nirayana Dasham / MC from dPlt[11] (required for Bhava Spasta).</summary>
        public double Midheaven { get; set; }
        /// <summary>Astronomy-engine Julian day fraction (julday), not VB6 DT2JDF.</summary>
        public double JulianDayFraction { get; set; }
        public string AscendantRashiName { get; set; }
        public KundliResult() { PlanetPositions = new List<PlanetPosition>(); Houses = new List<HouseData>(); }
    }
}
