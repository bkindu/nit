using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class VivahMuhurtaViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public VivahMuhurtaResult Result { get; set; }

        public VivahMuhurtaViewModel()
        {
            FromDate = new DateTime(2016, 4, 16);
            ToDate = new DateTime(2016, 6, 30);
        }
    }
}
