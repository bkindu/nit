using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Models
{
    public class KundliViewModel
    {
        [Required(ErrorMessage = "Day is required")]
        public int Day { get; set; }

        [Required(ErrorMessage = "Month is required")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Hour is required")]
        public int Hour { get; set; }

        [Required(ErrorMessage = "Minute is required")]
        public int Minute { get; set; }

        public string Place { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Timezone { get; set; }

        public bool HasResults { get { return Result != null; } }
        public KundliResult Result { get; set; }

        /// <summary>Default sample: 08-Feb-1981 07:47 Patiala (legacy reference).</summary>
        public KundliViewModel()
        {
            Day = 8;
            Month = 2;
            Year = 1981;
            Hour = 7;
            Minute = 47;
            Latitude = 30.19;
            Longitude = 76.25;
            Timezone = 5.30;
        }
    }
}
