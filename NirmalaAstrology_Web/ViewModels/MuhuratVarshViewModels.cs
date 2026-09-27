using System;
using System.Globalization;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class MuhuratViewModel
    {
        public DateTime? Date { get; set; }
        public int? PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string CountryName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public double Timezone { get; set; }
        public MuhuratResult Result { get; set; }

        public MuhuratViewModel()
        {
            Timezone = 5.5;
            Date = DateTime.Today;
        }
    }

    public sealed class VarshPhalViewModel
    {
        public int TargetYear { get; set; }
        public string NativeName { get; set; }
        public bool HasBirthSession { get; set; }
        public string BirthSummary { get; set; }
        public VarshPhalResult Result { get; set; }

        public VarshPhalViewModel()
        {
            TargetYear = DateTime.Today.Year;
        }
    }
}
