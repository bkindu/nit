using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Models.Reports;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Services.Reports
{
    /// <summary>
    /// Public-only limited letterhead reports (never DETAILED / full admin modules).
    /// Kundli: 2 pages (basic info + chart). Numerology: basic core numbers only.
    /// </summary>
    public sealed class PublicLimitedReportBuilder
    {
        public OfficialReportDocument BuildKundliTwoPage(BirthDataSessionModel birth, KundliWorkflowResult workflow)
        {
            if (birth == null) throw new ArgumentNullException("birth");
            if (workflow == null) throw new ArgumentNullException("workflow");

            var name = string.IsNullOrWhiteSpace(birth.Name) ? "Native" : birth.Name.Trim();
            var doc = new OfficialReportDocument
            {
                ReportType = "PublicBasicKundli",
                Title = "BASIC KUNDLI REPORT",
                Subtitle = "2-page public summary · basic info + birth chart",
                NativeName = name,
                ReferenceNo = "NA-PB-" + DateTime.Now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture),
                SuggestedFileName = "NirmalaAstrology_BasicKundli_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".pdf"
            };

            doc.Meta.Add(new ReportMetaItem { Label = "Native", Value = name });
            doc.Meta.Add(new ReportMetaItem { Label = "Birth date", Value = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2}", birth.Day, birth.Month, birth.Year) });
            doc.Meta.Add(new ReportMetaItem { Label = "Birth time", Value = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", birth.Hours, birth.Minutes) });
            doc.Meta.Add(new ReportMetaItem { Label = "Place", Value = (birth.Place ?? "") + (string.IsNullOrWhiteSpace(birth.Country) ? "" : ", " + birth.Country) });

            var details = workflow.Details;
            var summary = new ReportSection { Heading = "Page 1 — Basic birth information" };
            if (details != null)
            {
                summary.KeyValues.Add(new ReportKeyValue { Label = "Lagna / Birth rashi", Value = details.BirthRashiName });
                summary.KeyValues.Add(new ReportKeyValue { Label = "Sun sign", Value = details.SunSignName });
                summary.KeyValues.Add(new ReportKeyValue { Label = "Janma nakshatra", Value = string.Format(CultureInfo.InvariantCulture, "{0} (Pada {1})", details.NakshatraName, details.NakshatraPada) });
                summary.KeyValues.Add(new ReportKeyValue { Label = "Gandmool", Value = details.IsGandmool ? "Yes" : "No" });
                summary.KeyValues.Add(new ReportKeyValue { Label = "Manglik", Value = details.IsManglik ? "Yes" : "No" });
                if (details.Panchang != null)
                {
                    summary.KeyValues.Add(new ReportKeyValue { Label = "Tithi", Value = details.Panchang.Tithi });
                    summary.KeyValues.Add(new ReportKeyValue { Label = "Yoga", Value = details.Panchang.Yoga });
                    summary.KeyValues.Add(new ReportKeyValue { Label = "Karan", Value = details.Panchang.Karan });
                }
            }
            else
            {
                summary.Paragraphs.Add("Basic birth summary was not available for the provided details.");
            }
            doc.Sections.Add(summary);

            if (details != null && details.Planets != null && details.Planets.Count > 0)
            {
                var planetSection = new ReportSection { Heading = "Planetary positions (basic)" };
                var table = new ReportTable
                {
                    Headers = new List<string> { "Planet", "RAKV", "Sign", "House", "Nakshatra" },
                    ColumnWeights = new float[] { 1.1f, 1.3f, 1.0f, 0.6f, 1.2f }
                };
                foreach (var p in details.Planets.Take(12))
                {
                    table.Rows.Add(new[]
                    {
                        p.FullName ?? p.Name ?? "",
                        p.PositionRakv ?? "",
                        p.RashiName ?? "",
                        p.House.ToString(CultureInfo.InvariantCulture),
                        p.NakshatraName ?? ""
                    });
                }
                planetSection.Table = table;
                doc.Sections.Add(planetSection);
            }

            doc.Sections.Add(new ReportSection
            {
                Heading = "Important note",
                Paragraphs = new List<string>
                {
                    "This public report is limited to basic information and the birth kundli chart only.",
                    "Full detailed horoscope (dashas, vargas, yogas, remedies, Lal Kitab, etc.) is available through Admin / consultation only."
                }
            });

            byte[] chartBytes = null;
            try
            {
                if (workflow.Calculation != null)
                    chartBytes = new NorthIndianKundliChartRenderer().RenderPng(workflow.Calculation, 720);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[PublicKundliChart] " + ex.Message);
            }

            doc.Sections.Add(new ReportSection
            {
                Heading = "Page 2 — Janam Lagna Kundli",
                PageBreakBefore = true,
                Paragraphs = chartBytes == null
                    ? new List<string> { "Chart could not be rendered for this birth data." }
                    : new List<string> { "North Indian diamond chart (rashi numbers with planet abbreviations)." },
                ImageBytes = chartBytes,
                ImageMaxHeight = 460
            });

            return doc;
        }

        public OfficialReportDocument BuildNumerologyBasic(NumerologyResultModel result, string name, DateTime dob)
        {
            if (result == null) throw new ArgumentNullException("result");
            var native = string.IsNullOrWhiteSpace(name) ? (result.Name ?? "Native") : name.Trim();
            var doc = new OfficialReportDocument
            {
                ReportType = "PublicBasicNumerology",
                Title = "BASIC NUMEROLOGY REPORT",
                Subtitle = "Public limited core numbers only",
                NativeName = native,
                ReferenceNo = "NA-PN-" + DateTime.Now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture),
                SuggestedFileName = "NirmalaAstrology_BasicNumerology_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".pdf"
            };

            doc.Meta.Add(new ReportMetaItem { Label = "Name", Value = native });
            doc.Meta.Add(new ReportMetaItem { Label = "Date of birth", Value = dob.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) });

            var core = new ReportSection { Heading = "Core numbers (basic)" };
            core.KeyValues.Add(new ReportKeyValue { Label = "Life Path", Value = result.LifePathNumber.ToString(CultureInfo.InvariantCulture) });
            core.KeyValues.Add(new ReportKeyValue { Label = "Destiny / Expression", Value = result.DestinyNumber.ToString(CultureInfo.InvariantCulture) });
            core.KeyValues.Add(new ReportKeyValue { Label = "Soul Urge", Value = result.SoulUrgeNumber.ToString(CultureInfo.InvariantCulture) });
            core.KeyValues.Add(new ReportKeyValue { Label = "Personality", Value = result.PersonalityNumber.ToString(CultureInfo.InvariantCulture) });
            core.KeyValues.Add(new ReportKeyValue { Label = "Birthday number", Value = result.BirthdayNumber.ToString(CultureInfo.InvariantCulture) });
            if (result.NameRootNumber > 0)
                core.KeyValues.Add(new ReportKeyValue { Label = "Name root number", Value = result.NameRootNumber.ToString(CultureInfo.InvariantCulture) });
            doc.Sections.Add(core);

            var readings = new ReportSection { Heading = "Short readings" };
            if (!string.IsNullOrWhiteSpace(result.LifePathInterpretation))
                readings.Paragraphs.Add("Life Path: " + TrimReading(result.LifePathInterpretation));
            if (!string.IsNullOrWhiteSpace(result.DestinyInterpretation))
                readings.Paragraphs.Add("Destiny: " + TrimReading(result.DestinyInterpretation));
            if (!string.IsNullOrWhiteSpace(result.BirthdayInterpretation))
                readings.Paragraphs.Add("Birthday: " + TrimReading(result.BirthdayInterpretation));
            doc.Sections.Add(readings);

            doc.Sections.Add(new ReportSection
            {
                Heading = "Note",
                Paragraphs = new List<string>
                {
                    "Pyramid, planes, essence periods, pinnacles, and name variations are Admin / full-report only."
                }
            });

            return doc;
        }

        private static string TrimReading(string text)
        {
            text = (text ?? "").Trim();
            if (text.Length <= 420) return text;
            return text.Substring(0, 417) + "...";
        }
    }
}
