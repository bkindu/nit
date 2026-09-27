using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class GocharPlanetRow
    {
        public string Planet { get; set; }
        public string Code { get; set; }
        public int TransitSign { get; set; }
        public string TransitSignName { get; set; }
        public int HouseFromNatalLagna { get; set; }
        public string Reading { get; set; }
    }

    public sealed class GocharPhalResult
    {
        public string NativeName { get; set; }
        public string BirthSummary { get; set; }
        public string TransitDateLabel { get; set; }
        public int NatalLagnaSign { get; set; }
        public string NatalLagnaName { get; set; }
        public List<GocharPlanetRow> Planets { get; set; }
        public List<string> Notes { get; set; }

        public GocharPhalResult()
        {
            Planets = new List<GocharPlanetRow>();
            Notes = new List<string>();
        }
    }
}
