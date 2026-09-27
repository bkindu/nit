using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class Varga16Result
    {
        /// <summary>iSodsVrg[varga 1..16][planet 1..10] as sign numbers 1..12.</summary>
        public int[,] Signs { get; set; }
        public List<VargaChartRow> Charts { get; set; }
        public List<VargaTableRow> TableRows { get; set; }
        public List<VargaDeityRow> Deities { get; set; }

        public Varga16Result()
        {
            Signs = new int[17, 11];
            Charts = new List<VargaChartRow>();
            TableRows = new List<VargaTableRow>();
            Deities = new List<VargaDeityRow>();
        }
    }

    public sealed class VargaChartRow
    {
        public int Index { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public int LagnaSign { get; set; }
        public string LagnaSignName { get; set; }
        public List<VargaPlanetSign> Planets { get; set; }

        public VargaChartRow()
        {
            Planets = new List<VargaPlanetSign>();
        }
    }

    public sealed class VargaPlanetSign
    {
        public string PlanetName { get; set; }
        public int Sign { get; set; }
        public string SignName { get; set; }
        public int HouseFromLagna { get; set; }
    }

    public sealed class VargaTableRow
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int[] PlanetSigns { get; set; }
    }
}
