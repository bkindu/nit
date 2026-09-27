using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class LalKitabResult
    {
        public string NativeName { get; set; }
        public string BirthSummary { get; set; }
        public int AgeYear { get; set; }
        public string RemedyMode { get; set; }
        public int LagnaSign { get; set; }
        public string LagnaSignName { get; set; }
        public int[] HouseRemap { get; set; }
        public List<LalKitabPlanetRow> JanamPlanets { get; set; }
        public List<LalKitabPlanetRow> VarshPlanets { get; set; }
        public List<LalKitabRemedyRow> Remedies { get; set; }
        public List<string> Notes { get; set; }

        public LalKitabResult()
        {
            HouseRemap = new int[13];
            JanamPlanets = new List<LalKitabPlanetRow>();
            VarshPlanets = new List<LalKitabPlanetRow>();
            Remedies = new List<LalKitabRemedyRow>();
            Notes = new List<string>();
        }
    }

    public sealed class LalKitabPlanetRow
    {
        public string Planet { get; set; }
        public string Code { get; set; }
        public int Sign { get; set; }
        public string SignName { get; set; }
        public int House { get; set; }
    }

    public sealed class LalKitabRemedyRow
    {
        public string Planet { get; set; }
        public string Code { get; set; }
        public int House { get; set; }
        public string Key { get; set; }
        public string Reading { get; set; }
    }
}
