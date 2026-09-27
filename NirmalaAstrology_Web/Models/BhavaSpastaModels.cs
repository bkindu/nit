using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class BhavaSpastaResult
    {
        public double Lagna { get; set; }
        public double Midheaven { get; set; }
        public List<BhavaHouseRow> Houses { get; set; }
        public List<ChalitPlanetRow> ChalitPlanets { get; set; }

        public BhavaSpastaResult()
        {
            Houses = new List<BhavaHouseRow>();
            ChalitPlanets = new List<ChalitPlanetRow>();
        }
    }

    public sealed class BhavaHouseRow
    {
        public int HouseNumber { get; set; }
        public string HouseName { get; set; }
        public double MadhyaDegrees { get; set; }
        public double SandhiDegrees { get; set; }
        public string MadhyaRakv { get; set; }
        public string SandhiRakv { get; set; }
    }

    public sealed class ChalitPlanetRow
    {
        public string PlanetName { get; set; }
        public int PlanetIndex { get; set; }
        public double Longitude { get; set; }
        public int ChalitHouse { get; set; }
        public int RashiHouse { get; set; }
    }
}
