using System;

namespace NirmalaAstrology_Web.Models
{
    [Serializable]
    public sealed class BirthDataSessionModel
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Timezone { get; set; }
        public int? PlaceId { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }
        public string ReportChoice { get; set; }
    }
}
