using System;
using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.Models
{
    /// <summary>
    /// Represents a Customer / Kundli request.
    /// Maps to the Customer table in MySQL.
    /// </summary>
    public class CustomerModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Time of Birth is required.")]
        [Display(Name = "Time of Birth")]
        public TimeSpan BirthTime { get; set; }

        [Required(ErrorMessage = "Birth Place is required.")]
        [Display(Name = "Birth Place")]
        public int PlaceId { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [StringLength(50)]
        public string Timezone { get; set; }

        public DateTime CreatedDate { get; set; }

        // For display purposes - not stored in DB
        public string PlaceName { get; set; }
        public string Gender { get; set; }

        /// <summary>Default sample: 08-Feb-1981 07:47 Patiala.</summary>
        public CustomerModel()
        {
            Name = "Native";
            DOB = new DateTime(1981, 2, 8);
            BirthTime = new TimeSpan(7, 47, 0);
            Latitude = 30.19m;
            Longitude = 76.25m;
            Timezone = "5.30";
            PlaceName = "PATIALA";
            Gender = "Male";
        }
    }
}