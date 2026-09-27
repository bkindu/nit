using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class VivahMuhurtaSlot
    {
        public int Lagna { get; set; }
        public string LagnaName { get; set; }
        public string Remark { get; set; }
    }

    public sealed class VivahMuhurtaDay
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; }
        public string Weekday { get; set; }
        public List<VivahMuhurtaSlot> Slots { get; set; }

        public VivahMuhurtaDay()
        {
            Slots = new List<VivahMuhurtaSlot>();
        }
    }

    public sealed class VivahMuhurtaResult
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string CoverageNote { get; set; }
        public List<VivahMuhurtaDay> Days { get; set; }

        public VivahMuhurtaResult()
        {
            Days = new List<VivahMuhurtaDay>();
        }
    }
}
