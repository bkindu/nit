using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class PalmistryOption
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Group { get; set; }
    }

    public sealed class PalmistrySection
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
    }

    public sealed class PalmistryResult
    {
        public string NativeName { get; set; }
        public string FullReading { get; set; }
        public List<PalmistrySection> Sections { get; set; }

        public PalmistryResult()
        {
            Sections = new List<PalmistrySection>();
        }
    }
}
