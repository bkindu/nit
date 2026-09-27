using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class GocharPhalViewModel
    {
        public DateTime? TransitDate { get; set; }
        public bool HasBirthSession { get; set; }
        public string BirthSummary { get; set; }
        public string NativeName { get; set; }
        public GocharPhalResult Result { get; set; }

        public GocharPhalViewModel()
        {
            TransitDate = DateTime.Today;
        }
    }
}
