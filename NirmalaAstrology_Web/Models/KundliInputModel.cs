using System;
using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.Models
{
    public class KundliInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Time of birth is required")]
        [DataType(DataType.Time)]
        public TimeSpan TimeOfBirth { get; set; }

        public int? PlaceId { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Timezone is required")]
        public double Timezone { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public void PopulateFromDateTime()
        {
            if (DateOfBirth.Year > 1)
            {
                Day = DateOfBirth.Day;
                Month = DateOfBirth.Month;
                Year = DateOfBirth.Year;
            }
            if (TimeOfBirth.TotalMinutes > 0 || DateOfBirth.Year > 1)
            {
                Hour = TimeOfBirth.Hours;
                Minute = TimeOfBirth.Minutes;
            }
        }
    }
}
