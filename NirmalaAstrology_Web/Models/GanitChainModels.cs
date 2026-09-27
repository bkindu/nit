using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class GanitChainResult
    {
        public PanchadhaResult Panchadha { get; set; }
        public GrahaBalaResult GrahaBala { get; set; }
        public SpecialLagnaResult SpecialLagnas { get; set; }
        public UpGrahaResult UpGrahas { get; set; }
        public GrahaAvasthaResult Avasthas { get; set; }
        public KinshukadiResult Kinshukadi { get; set; }
        public YogaDetectionResult Yogas { get; set; }
        public ExtraDashaBundle ExtraDashas { get; set; }
    }

    public sealed class PanchadhaResult
    {
        public string[,] Labels { get; set; } // [2..8,2..8]
        public List<string> PlanetNames { get; set; }
        public List<PanchadhaRow> Rows { get; set; }

        public PanchadhaResult()
        {
            Labels = new string[9, 9];
            PlanetNames = new List<string> { "", "", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn" };
            Rows = new List<PanchadhaRow>();
        }
    }

    public sealed class PanchadhaRow
    {
        public string Planet { get; set; }
        public List<string> Relations { get; set; }
    }

    public sealed class GrahaBalaResult
    {
        public List<GrahaBalaPlanetRow> Planets { get; set; }
        public List<BhavaBalaRow> Bhavas { get; set; }

        public GrahaBalaResult()
        {
            Planets = new List<GrahaBalaPlanetRow>();
            Bhavas = new List<BhavaBalaRow>();
        }
    }

    public sealed class GrahaBalaPlanetRow
    {
        public int PlanetIndex { get; set; }
        public string Planet { get; set; }
        public string PlanetName { get; set; }
        public double Ucha { get; set; }
        public double SaptaVarga { get; set; }
        public double OjaYugma { get; set; }
        public double Kendra { get; set; }
        public double Drekkana { get; set; }
        public double Sthana { get; set; }
        public double Dig { get; set; }
        public double Natonnata { get; set; }
        public double Paksha { get; set; }
        public double Tribhaga { get; set; }
        public double Isb { get; set; }
        public double Ayana { get; set; }
        public double Kala { get; set; }
        public double Cheshta { get; set; }
        public double Naisarga { get; set; }
        public double Drishti { get; set; }
        public double Shadbala { get; set; }
        public double ShadbalaTotal { get; set; }
        public double Rupas { get; set; }
        public double Required { get; set; }
        public double RequiredRupas { get; set; }
        public double Percent { get; set; }
    }

    public sealed class BhavaBalaRow
    {
        public int House { get; set; }
        public int LordPlanetIndex { get; set; }
        public string LordPlanetName { get; set; }
        public double Total { get; set; }
        public double Dig { get; set; }
        public double LordShadbala { get; set; }
    }

    public sealed class SpecialLagnaResult
    {
        public List<NamedSign> Lagnas { get; set; }
        public SpecialLagnaResult() { Lagnas = new List<NamedSign>(); }
    }

    public sealed class NamedSign
    {
        public string Name { get; set; }
        public int Sign { get; set; }
        public string SignName { get; set; }
        public double Longitude { get; set; }
    }

    public sealed class UpGrahaResult
    {
        /// <summary>Legacy flat list: Dhoomadi + Parasara Gulikadi.</summary>
        public List<NamedSign> Points { get; set; }
        public List<NamedSign> Dhoomadi { get; set; }
        public List<NamedSign> Parasara { get; set; }
        public List<NamedSign> Mantreshwar { get; set; }

        public UpGrahaResult()
        {
            Points = new List<NamedSign>();
            Dhoomadi = new List<NamedSign>();
            Parasara = new List<NamedSign>();
            Mantreshwar = new List<NamedSign>();
        }
    }

    public sealed class GrahaAvasthaResult
    {
        public List<GrahaAvasthaRow> Rows { get; set; }
        public GrahaAvasthaResult() { Rows = new List<GrahaAvasthaRow>(); }
    }

    public sealed class GrahaAvasthaRow
    {
        public string Planet { get; set; }
        public string Baladi { get; set; }
        public string Jagradadi { get; set; }
        public string Deeptadi { get; set; }
        public string CharadiKarak { get; set; }
        public string Arohi { get; set; }
        public string Pravas { get; set; }
    }

    public sealed class KinshukadiResult
    {
        public List<KinshukadiRow> Rows { get; set; }
        public KinshukadiResult() { Rows = new List<KinshukadiRow>(); }
    }

    public sealed class KinshukadiRow
    {
        public string Planet { get; set; }
        public string Shodasha { get; set; }
        public string DasaVarga { get; set; }
        public string SaptaVarga { get; set; }
        public int ShodashaCount { get; set; }
        public int DasaCount { get; set; }
        public int SaptaCount { get; set; }
    }

    public sealed class YogaDetectionResult
    {
        public List<DetectedYoga> Yogas { get; set; }
        public YogaDetectionResult() { Yogas = new List<DetectedYoga>(); }
    }

    public sealed class DetectedYoga
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Note { get; set; }
    }
}
