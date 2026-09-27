using System;
using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.Models
{
    /// <summary>
    /// Represents a Zodiac Sign.
    /// Maps to the Zodiac table in MySQL.
    /// </summary>
    public class ZodiacModel
    {
        public int ZodiacId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string HindiName { get; set; }

        [StringLength(200)]
        public string Icon { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public string Description { get; set; }

        public string LuckyNumber { get; set; }
        public string LuckyColor { get; set; }
        public string LuckyDay { get; set; }
        public string Planet { get; set; }
        public string Element { get; set; }
        public string Nature { get; set; }

        public string Strength { get; set; }
        public string Weakness { get; set; }

        public string Career { get; set; }
        public string Love { get; set; }
        public string Health { get; set; }
        public string Finance { get; set; }

        public string Personality { get; set; }
        public string Characteristics { get; set; }

        [StringLength(200)]
        public string CompatibleSigns { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// URL-friendly name for routing (e.g., "aries" from "Aries")
        /// </summary>
        public string UrlName
        {
            get { return Name?.ToLower().Replace(" ", "-"); }
        }
    }
}