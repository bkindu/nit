using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class KundliDetailsModel
    {
        public string BirthRashiName { get; set; }
        public int BirthRashi { get; set; }
        public string SunSignName { get; set; }
        public string NakshatraName { get; set; }
        public int NakshatraNumber { get; set; }
        public int NakshatraPada { get; set; }
        public bool IsGandmool { get; set; }
        public string PayaRashiWise { get; set; }
        public string PayaNakshatraWise { get; set; }
        public bool IsManglik { get; set; }
        public List<string> CombustPlanets { get; set; }
        public List<string> RetrogradePlanets { get; set; }
        public List<string> RashiSandhiPlanets { get; set; }
        public List<PlanetDetailModel> Planets { get; set; }
        public List<HousePercentageModel> HousePercentages { get; set; }
        public string PercentageNote { get; set; }
        public KundliDatabaseContentModel DatabaseContent { get; set; }
        public LegacyPanchangModel Panchang { get; set; }

        public KundliDetailsModel()
        {
            CombustPlanets = new List<string>();
            RetrogradePlanets = new List<string>();
            RashiSandhiPlanets = new List<string>();
            Planets = new List<PlanetDetailModel>();
            HousePercentages = new List<HousePercentageModel>();
            DatabaseContent = new KundliDatabaseContentModel();
            Panchang = new LegacyPanchangModel();
        }
    }

    public sealed class PlanetDetailModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string RashiName { get; set; }
        public int Rashi { get; set; }
        public int House { get; set; }
        public double TotalDegrees { get; set; }
        public double DegreesInRashi { get; set; }
        public bool IsRetrograde { get; set; }
        public string Motion { get; set; }
        public string UdayAsta { get; set; }
        public int NakshatraNumber { get; set; }
        public int NakshatraPada { get; set; }
        public string NakshatraName { get; set; }
        public string NakshatraLord { get; set; }
        public string Dignity { get; set; }
        public double? StrengthPercent { get; set; }
        public double DailyMotion { get; set; }
        public string NakshatraDevta { get; set; }
        public string Gati { get; set; }
        /// <summary>Legacy VB6 0-based RAKV display, e.g. 09:25:39:19 (not 10:…).</summary>
        public string PositionRakv { get; set; }
        /// <summary>0-based rashi digit used in PositionRakv (0=Mesha…11=Meena).</summary>
        public int Rashi0 { get; set; }
    }

    public sealed class HousePercentageModel
    {
        public int House { get; set; }
        public int Rashi { get; set; }
        public string RashiName { get; set; }
        public int OccupancyPercent { get; set; }
        public int PlanetCount { get; set; }
    }
}
