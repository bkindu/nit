using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.Models
{
    /// <summary>
    /// Represents a Place (City/Town) for birth location lookup.
    /// Maps to the 'place' table (FK to 'country' table).
    /// </summary>
    public class PlaceModel
    {
        public int PlaceID { get; set; }

        [Required(ErrorMessage = "Place Name is required.")]
        [StringLength(100, ErrorMessage = "Place Name cannot exceed 100 characters.")]
        public string PlaceName { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int CountryID { get; set; }

        /// <summary>
        /// Joined from country table — not mapped directly to place table.
        /// </summary>
        public string CountryName { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required.")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "TimeZone is required.")]
        [StringLength(10)]
        public string TimeZone { get; set; }

        /// <summary>
        /// Standard Meridian Time Correction: 'E' = East, 'W' = West.
        /// </summary>
        public string SMTCorr { get; set; }

        /// <summary>
        /// Display label for dropdown: "PlaceName, CountryName"
        /// </summary>
        public string DisplayText
        {
            get
            {
                return string.IsNullOrEmpty(CountryName)
                    ? PlaceName
                    : $"{PlaceName}, {CountryName}";
            }
        }
    }
}