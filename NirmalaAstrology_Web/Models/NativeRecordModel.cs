using System;
using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.Models
{
    public sealed class NativeRecordModel
    {
        public int NativeId { get; set; }
        public int? SerialNo { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string NativeName { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }

        public DateTime? BirthDate { get; set; }
        public string BirthDateText { get; set; }
        public TimeSpan? BirthTime { get; set; }
        public string BirthTimeText { get; set; }
        public string BirthPlace { get; set; }
        public string CountryName { get; set; }
        public int? PlaceId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? TimeZone { get; set; }
        public string NickName { get; set; }
        public string FatherName { get; set; }
        public string AddressText { get; set; }
        public string Remarks { get; set; }
        public bool TempRecord { get; set; }
        public string LifeSpheres { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class NativeSearchRequest
    {
        public string Name { get; set; }
        public string Place { get; set; }
        public string BirthDate { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public int? SerialNo { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public NativeSearchRequest()
        {
            Page = 1;
            PageSize = 25;
        }
    }
}
