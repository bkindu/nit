using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class AshtakavargaResult
    {
        public string School { get; set; }
        public List<AshtakavargaChart> Charts { get; set; }
        public int[] SarvaBySign { get; set; }
        public int[] TrikonaSarvaBySign { get; set; }
        public int[] EkadhipatyaSarvaBySign { get; set; }
        public List<AshtakavargaPindaRow> Pindas { get; set; }

        public AshtakavargaResult()
        {
            School = "Non_Parasar";
            Charts = new List<AshtakavargaChart>();
            SarvaBySign = new int[13];
            TrikonaSarvaBySign = new int[13];
            EkadhipatyaSarvaBySign = new int[13];
            Pindas = new List<AshtakavargaPindaRow>();
        }
    }

    public sealed class AshtakavargaChart
    {
        public int PlanetIndex { get; set; }
        public string PlanetName { get; set; }
        public int[] BindusBySign { get; set; }
        public int[] TrikonaBySign { get; set; }
        public int[] EkadhipatyaBySign { get; set; }
        public int TotalBindus { get; set; }
    }

    public sealed class AshtakavargaPindaRow
    {
        public string PlanetName { get; set; }
        public int RasiPinda { get; set; }
        public int GrahaPinda { get; set; }
        public int TotalPinda { get; set; }
    }
}
