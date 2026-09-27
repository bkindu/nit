using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models.Reports
{
    /// <summary>Technology-agnostic official report document (not tied to any UI form).</summary>
    public sealed class OfficialReportDocument
    {
        public string ReportType { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string NativeName { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string SuggestedFileName { get; set; }
        public List<ReportMetaItem> Meta { get; set; }
        public List<ReportSection> Sections { get; set; }

        public OfficialReportDocument()
        {
            GeneratedAt = DateTime.Now;
            Meta = new List<ReportMetaItem>();
            Sections = new List<ReportSection>();
        }
    }

    public sealed class ReportMetaItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public sealed class ReportSection
    {
        public string Heading { get; set; }
        public bool PageBreakBefore { get; set; }
        public List<string> Paragraphs { get; set; }
        /// <summary>Monospaced VB6-style print lines (Courier) — e.g. Vimshottari 3-column dasha.</summary>
        public List<string> MonospaceLines { get; set; }
        public List<ReportKeyValue> KeyValues { get; set; }
        public ReportTable Table { get; set; }
        /// <summary>Optional PNG/JPEG image bytes drawn under the section (e.g. public kundli chart).</summary>
        public byte[] ImageBytes { get; set; }
        public double ImageMaxHeight { get; set; }

        public ReportSection()
        {
            Paragraphs = new List<string>();
            MonospaceLines = new List<string>();
            KeyValues = new List<ReportKeyValue>();
            ImageMaxHeight = 420;
        }
    }

    public sealed class ReportKeyValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public sealed class ReportTable
    {
        public List<string> Headers { get; set; }
        public List<string[]> Rows { get; set; }
        public float[] ColumnWeights { get; set; }

        public ReportTable()
        {
            Headers = new List<string>();
            Rows = new List<string[]>();
        }
    }
}
