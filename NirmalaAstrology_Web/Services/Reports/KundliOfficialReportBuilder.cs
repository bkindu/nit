using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Models.Reports;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Services.Reports
{
    /// <summary>
    /// Maps Kundli workflow calculation data into an OfficialReportDocument.
    /// Independent of any Razor form / screen layout.
    /// </summary>
    public sealed class KundliOfficialReportBuilder
    {
        public OfficialReportDocument Build(BirthDataSessionModel birth, KundliWorkflowResult workflow)
        {
            if (birth == null) throw new ArgumentNullException("birth");
            if (workflow == null) throw new ArgumentNullException("workflow");

            var name = string.IsNullOrWhiteSpace(birth.Name) ? "Native" : birth.Name.Trim();
            var choice = string.IsNullOrWhiteSpace(workflow.Choice) ? "DETAILED" : workflow.Choice;
            var doc = new OfficialReportDocument
            {
                ReportType = "Kundli",
                Title = "DETAILED HOROSCOPE REPORT",
                Subtitle = "Vedic birth chart analysis · " + choice,
                NativeName = name,
                ReferenceNo = "NA-K-" + DateTime.Now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture),
                SuggestedFileName = SanitizeFileName("NirmalaAstrology_" + name + "_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".pdf")
            };

            doc.Meta.Add(Item("Native", name));
            doc.Meta.Add(Item("Birth date", string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2}", birth.Day, birth.Month, birth.Year)));
            doc.Meta.Add(Item("Birth time", string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", birth.Hours, birth.Minutes)));
            doc.Meta.Add(Item("Place", (birth.Place ?? "") + (string.IsNullOrWhiteSpace(birth.Country) ? "" : ", " + birth.Country)));
            doc.Meta.Add(Item("Coordinates", string.Format(CultureInfo.InvariantCulture, "Lat {0:0.####} · Lon {1:0.####} · TZ {2:0.##}", birth.Latitude, birth.Longitude, birth.Timezone == 0 ? 5.5 : birth.Timezone)));
            doc.Meta.Add(Item("Generated", doc.GeneratedAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)));

            var details = workflow.Details;
            if (details != null)
            {
                var summary = new ReportSection { Heading = "Birth chart summary" };
                summary.KeyValues.Add(Kv("Lagna / Birth rashi", details.BirthRashiName));
                summary.KeyValues.Add(Kv("Sun sign", details.SunSignName));
                summary.KeyValues.Add(Kv("Janma nakshatra", string.Format(CultureInfo.InvariantCulture, "{0} ({1}/{2})", details.NakshatraName, details.NakshatraNumber, details.NakshatraPada)));
                summary.KeyValues.Add(Kv("Gandmool", details.IsGandmool ? "Yes" : "No"));
                summary.KeyValues.Add(Kv("Paya (rashi / nakshatra)", details.PayaRashiWise + " / " + details.PayaNakshatraWise));
                summary.KeyValues.Add(Kv("Manglik", details.IsManglik ? "Yes" : "No"));
                if (details.RetrogradePlanets != null && details.RetrogradePlanets.Count > 0)
                    summary.KeyValues.Add(Kv("Retrograde", string.Join(", ", details.RetrogradePlanets)));
                if (details.CombustPlanets != null && details.CombustPlanets.Count > 0)
                    summary.KeyValues.Add(Kv("Combust", string.Join(", ", details.CombustPlanets)));
                doc.Sections.Add(summary);

                if (details.Planets != null && details.Planets.Count > 0)
                {
                    var planetSection = new ReportSection { Heading = "Planetary positions" };
                    var table = new ReportTable
                    {
                        Headers = new List<string> { "Planet", "Sign", "House", "Longitude", "Nakshatra", "Pada", "Dignity", "Motion" },
                        ColumnWeights = new float[] { 1.2f, 1.2f, 0.7f, 1.1f, 1.3f, 0.6f, 1.0f, 0.9f }
                    };
                    foreach (var p in details.Planets)
                    {
                        table.Rows.Add(new[]
                        {
                            p.FullName ?? p.Name ?? "",
                            p.RashiName ?? "",
                            p.House.ToString(CultureInfo.InvariantCulture),
                            p.TotalDegrees.ToString("0.00", CultureInfo.InvariantCulture) + "°",
                            p.NakshatraName ?? "",
                            p.NakshatraPada.ToString(CultureInfo.InvariantCulture),
                            p.Dignity ?? "",
                            string.IsNullOrWhiteSpace(p.Motion) ? (p.IsRetrograde ? "R" : "D") : p.Motion
                        });
                    }
                    planetSection.Table = table;
                    doc.Sections.Add(planetSection);
                }

                if (details.HousePercentages != null && details.HousePercentages.Count > 0)
                {
                    var hs = new ReportSection { Heading = "House occupancy summary" };
                    if (!string.IsNullOrWhiteSpace(details.PercentageNote))
                        hs.Paragraphs.Add(details.PercentageNote);
                    var ht = new ReportTable
                    {
                        Headers = new List<string> { "House", "Rashi", "Planets", "Occupancy %" },
                        ColumnWeights = new float[] { 0.8f, 1.4f, 0.9f, 1.0f }
                    };
                    foreach (var h in details.HousePercentages)
                    {
                        ht.Rows.Add(new[]
                        {
                            h.House.ToString(CultureInfo.InvariantCulture),
                            h.RashiName ?? "",
                            h.PlanetCount.ToString(CultureInfo.InvariantCulture),
                            h.OccupancyPercent.ToString(CultureInfo.InvariantCulture) + "%"
                        });
                    }
                    hs.Table = ht;
                    doc.Sections.Add(hs);
                }

                if (details.Panchang != null)
                {
                    var pan = new ReportSection { Heading = "Panchang elements" };
                    pan.KeyValues.Add(Kv("Tithi", details.Panchang.Tithi));
                    pan.KeyValues.Add(Kv("Yoga", details.Panchang.Yoga));
                    pan.KeyValues.Add(Kv("Karana", details.Panchang.Karan));
                    pan.KeyValues.Add(Kv("Vaar", details.Panchang.VaarHindu ?? details.Panchang.VaarEnglish));
                    pan.KeyValues.Add(Kv("Paksha", details.Panchang.Paksha));
                    doc.Sections.Add(pan);
                }

                AppendDatabaseReadings(doc, details.DatabaseContent);
            }

            if (workflow.Dasha != null)
            {
                var dasha = workflow.Dasha;
                var pageText = DashaPrintFormatter.FormatVimshottariPage(dasha);
                var lines = pageText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                // First group (3 mahas) on one PDF section; remaining groups may need page breaks
                var chunks = ChunkMonospaceByBlankGroups(lines, 2);
                for (var i = 0; i < chunks.Count; i++)
                {
                    var sec = new ReportSection
                    {
                        Heading = i == 0 ? "VIMSHOTTARI DASHA TABLE" : "VIMSHOTTARI DASHA TABLE (cont.)",
                        PageBreakBefore = true
                    };
                    if (i == 0)
                    {
                        sec.KeyValues.Add(Kv("Mahadasa balance at birth", dasha.BalanceText));
                        sec.Paragraphs.Add("Dates given below show ending moments of Dasa in AD Era.");
                    }
                    foreach (var line in chunks[i])
                        sec.MonospaceLines.Add(line);
                    doc.Sections.Add(sec);
                }
            }

            if (workflow.Bhava != null && workflow.Bhava.Houses != null && workflow.Bhava.Houses.Count > 0)
            {
                var sec = new ReportSection { Heading = "Bhava spasta (chalit)" };
                var t = new ReportTable
                {
                    Headers = new List<string> { "House", "Madhya", "Sandhi" },
                    ColumnWeights = new float[] { 0.8f, 1.4f, 1.4f }
                };
                foreach (var h in workflow.Bhava.Houses)
                {
                    t.Rows.Add(new[]
                    {
                        h.HouseNumber.ToString(CultureInfo.InvariantCulture),
                        h.MadhyaDegrees.ToString("0.00", CultureInfo.InvariantCulture) + "°",
                        h.SandhiDegrees.ToString("0.00", CultureInfo.InvariantCulture) + "°"
                    });
                }
                sec.Table = t;
                doc.Sections.Add(sec);
            }

            if (workflow.Ganit != null && workflow.Ganit.Yogas != null && workflow.Ganit.Yogas.Yogas != null && workflow.Ganit.Yogas.Yogas.Count > 0)
            {
                var sec = new ReportSection { Heading = "Detected yogas", PageBreakBefore = true };
                foreach (var y in workflow.Ganit.Yogas.Yogas.Take(40))
                {
                    var line = y.Name ?? "Yoga";
                    if (!string.IsNullOrWhiteSpace(y.Category))
                        line = y.Category + ": " + line;
                    if (!string.IsNullOrWhiteSpace(y.Note))
                        line += " — " + y.Note;
                    sec.Paragraphs.Add(line);
                }
                doc.Sections.Add(sec);
            }

            if (workflow.Ganit != null && workflow.Ganit.SpecialLagnas != null && workflow.Ganit.SpecialLagnas.Lagnas != null)
            {
                var sec = new ReportSection { Heading = "Special lagnas" };
                var t = new ReportTable
                {
                    Headers = new List<string> { "Lagna", "Sign" },
                    ColumnWeights = new float[] { 1.5f, 1.5f }
                };
                foreach (var item in workflow.Ganit.SpecialLagnas.Lagnas)
                    t.Rows.Add(new[] { item.Name ?? "", item.SignName ?? "" });
                if (t.Rows.Count > 0)
                {
                    sec.Table = t;
                    doc.Sections.Add(sec);
                }
            }

            doc.Sections.Add(new ReportSection
            {
                Heading = "Disclaimer",
                Paragraphs = new List<string>
                {
                    "This report is generated from Nirmala Astrology's self-coded Vedic calculation engine for guidance and counselling. Interpretations should be reviewed with a qualified astrologer. Calculations preserve legacy VB6 Indu formulas; presentation only has been modernised for official letterhead output."
                }
            });

            return doc;
        }

        private static void AppendDatabaseReadings(OfficialReportDocument doc, KundliDatabaseContentModel content)
        {
            if (content == null) return;
            if (!string.IsNullOrWhiteSpace(content.NakshatraReading))
            {
                doc.Sections.Add(new ReportSection
                {
                    Heading = "Nakshatra reading",
                    PageBreakBefore = true,
                    Paragraphs = new List<string> { content.NakshatraReading.Trim() }
                });
            }
            if (!string.IsNullOrWhiteSpace(content.SignReading))
            {
                doc.Sections.Add(new ReportSection
                {
                    Heading = "Rashi reading",
                    Paragraphs = new List<string> { content.SignReading.Trim() }
                });
            }
            if (content.PlanetHouseReadings != null)
            {
                var withText = content.PlanetHouseReadings
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.BirthReading))
                    .Take(12)
                    .ToList();
                if (withText.Count > 0)
                {
                    var sec = new ReportSection { Heading = "Planet–house birth readings (selected)" };
                    foreach (var r in withText)
                    {
                        sec.Paragraphs.Add(string.Format(CultureInfo.InvariantCulture,
                            "{0} in house {1}: {2}", r.PlanetName ?? r.PlanetCode, r.House, r.BirthReading.Trim()));
                    }
                    doc.Sections.Add(sec);
                }
            }
        }

        private static ReportMetaItem Item(string label, string value)
        {
            return new ReportMetaItem { Label = label, Value = value ?? "" };
        }

        private static ReportKeyValue Kv(string label, string value)
        {
            return new ReportKeyValue { Label = label, Value = value ?? "" };
        }

        /// <summary>
        /// Split monospace dasha page into chunks separated by double blank lines
        /// (each VB6 group of 3 mahas), keeping at most <paramref name="groupsPerChunk"/> groups.
        /// </summary>
        private static List<List<string>> ChunkMonospaceByBlankGroups(string[] lines, int groupsPerChunk)
        {
            var result = new List<List<string>>();
            if (lines == null || lines.Length == 0) return result;

            // Skip title/balance preamble — already shown as KeyValues/Paragraphs
            var start = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null && lines[i].IndexOf("-----", StringComparison.Ordinal) >= 0
                    && lines[i].IndexOf("Maha", StringComparison.Ordinal) < 0)
                {
                    // look ahead for first "Maha Dasa of"
                    for (var j = i; j < lines.Length; j++)
                    {
                        if (lines[j] != null && lines[j].IndexOf("Maha Dasa of", StringComparison.Ordinal) >= 0)
                        {
                            start = Math.Max(0, j - 1);
                            break;
                        }
                    }
                    break;
                }
            }

            var current = new List<string>();
            var blankRun = 0;
            var groupsInChunk = 0;
            for (var i = start; i < lines.Length; i++)
            {
                var line = lines[i] ?? "";
                if (string.IsNullOrWhiteSpace(line))
                {
                    blankRun++;
                    current.Add("");
                    if (blankRun >= 2 && current.Any(l => l.IndexOf("Maha Dasa of", StringComparison.Ordinal) >= 0))
                    {
                        groupsInChunk++;
                        if (groupsInChunk >= groupsPerChunk)
                        {
                            // trim trailing blanks
                            while (current.Count > 0 && string.IsNullOrWhiteSpace(current[current.Count - 1]))
                                current.RemoveAt(current.Count - 1);
                            result.Add(current);
                            current = new List<string>();
                            groupsInChunk = 0;
                            blankRun = 0;
                            continue;
                        }
                    }
                }
                else
                {
                    blankRun = 0;
                    current.Add(line);
                }
            }
            while (current.Count > 0 && string.IsNullOrWhiteSpace(current[current.Count - 1]))
                current.RemoveAt(current.Count - 1);
            if (current.Count > 0) result.Add(current);
            if (result.Count == 0 && lines.Length > 0)
                result.Add(lines.ToList());
            return result;
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name)
                sb.Append(Array.IndexOf(invalid, ch) >= 0 ? '_' : ch);
            return sb.ToString();
        }
    }
}
