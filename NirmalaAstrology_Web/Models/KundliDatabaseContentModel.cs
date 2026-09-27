using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class KundliDatabaseContentModel
    {
        public string NakshatraReading { get; set; }
        public string NakshatraCharanReading { get; set; }
        public string SignReading { get; set; }
        public string HouseReading { get; set; }
        public string UdayaAsthaRule { get; set; }
        public List<PlanetReadingModel> PlanetHouseReadings { get; set; }
        public List<PlanetReadingModel> PlanetSignReadings { get; set; }

        public KundliDatabaseContentModel()
        {
            PlanetHouseReadings = new List<PlanetReadingModel>();
            PlanetSignReadings = new List<PlanetReadingModel>();
        }
    }

    public sealed class PlanetReadingModel
    {
        public string PlanetCode { get; set; }
        public string PlanetName { get; set; }
        public int House { get; set; }
        public int Sign { get; set; }
        public string BirthReading { get; set; }
        public string GocharReading { get; set; }
        public string LalKitabReading { get; set; }
        public string SourceReading { get; set; }
    }
}
