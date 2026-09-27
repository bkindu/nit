using System;
using System.ComponentModel.DataAnnotations;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class BirthDataViewModel
    {
        public int? NativeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Nick Name")]
        [StringLength(80)]
        public string NickName { get; set; }

        [Display(Name = "Father's Name")]
        [StringLength(150)]
        public string FatherName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Birth hour is required.")]
        [Range(1, 12)]
        public int Hour { get; set; }

        [Required]
        [Range(0, 59)]
        public int Minute { get; set; }

        [Required]
        public string Meridiem { get; set; }

        public int? PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string CountryName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public double Timezone { get; set; }

        [Display(Name = "Reference / Address")]
        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [Display(Name = "Report")]
        public string ReportChoice { get; set; }

        public string Language { get; set; }
        public bool SaveRecord { get; set; }

        public BirthDataViewModel()
        {
            Gender = "M";
            Meridiem = "AM";
            ReportChoice = "KUNDALI";
            Language = "en";
            // Default sample used across the site (legacy reference chart)
            DateOfBirth = new DateTime(1981, 2, 8);
            Hour = 7;
            Minute = 47;
            PlaceName = "PATIALA";
            CountryName = "INDIA";
            Latitude = 30.19m;
            Longitude = 76.25m;
            Timezone = 5.30;
        }

        public TimeSpan To24HourTime()
        {
            var hour = Hour % 12;
            if (String.Equals(Meridiem, "PM", StringComparison.OrdinalIgnoreCase)) hour += 12;
            return new TimeSpan(hour, Minute, 0);
        }
    }
}
