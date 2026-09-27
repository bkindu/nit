using System;
using System.Collections.Generic;
using System.Globalization;

namespace NirmalaAstrology_Web.Models
{
    public sealed class MuhuratResult
    {
        public string DateLabel { get; set; }
        public string PlaceLabel { get; set; }
        public string WeekdayHindu { get; set; }
        public string SunriseText { get; set; }
        public string SunsetText { get; set; }
        public string AbhijitNote { get; set; }
        public string AbhijitWindow { get; set; }
        public string RahuKaalWindow { get; set; }
        public List<MuhuratSegment> ChaturGhatika { get; set; }
        public List<string> KaalShuddhiNotes { get; set; }

        public MuhuratResult()
        {
            ChaturGhatika = new List<MuhuratSegment>();
            KaalShuddhiNotes = new List<string>();
        }
    }

    public sealed class MuhuratSegment
    {
        public string Period { get; set; }
        public string Name { get; set; }
        public string FromText { get; set; }
        public string ToText { get; set; }
    }

    public sealed class VarshPhalResult
    {
        public string NativeName { get; set; }
        public int TargetYear { get; set; }
        public int CompletedYears { get; set; }
        public int AgeYearLabel { get; set; }
        public string ParveshDateText { get; set; }
        public string ParveshTimeText { get; set; }
        public int MunthaSign { get; set; }
        public string MunthaSignName { get; set; }
        public string BirthNakshatra { get; set; }
        public string VarshNakshatra { get; set; }
        public string VarshLagnaSignName { get; set; }
        public string BirthLagnaSignName { get; set; }
        public List<NamedLongitude> VarshPlanets { get; set; }
        public List<MuddaPeriod> Mudda { get; set; }
        public List<SahamRow> Sahams { get; set; }
        public List<string> Notes { get; set; }

        public VarshPhalResult()
        {
            VarshPlanets = new List<NamedLongitude>();
            Mudda = new List<MuddaPeriod>();
            Sahams = new List<SahamRow>();
            Notes = new List<string>();
        }
    }

    public sealed class NamedLongitude
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public int Sign { get; set; }
        public string SignName { get; set; }
        public int House { get; set; }
    }

    public sealed class MuddaPeriod
    {
        public string Lord { get; set; }
        public string EndDate { get; set; }
        public string Label { get; set; }
    }

    public sealed class SahamRow
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public string Rakv { get; set; }
    }
}
