using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// VB6 eDashasAll print layout: 3 Maha columns × 9 Antar rows
    /// ("Antar   Beginning  Enging", pipe separators).
    /// </summary>
    public static class DashaPrintFormatter
    {
        private static readonly Dictionary<string, string> Abbrev =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Ketu", "Ket" }, { "Sukra", "Suk" }, { "Surya", "Sur" },
                { "Chandra", "Cha" }, { "Mangla", "Man" }, { "Mangal", "Man" },
                { "Rahu", "Rah" }, { "Guru", "Gur" }, { "Sani", "San" }, { "Budha", "Bud" }
            };

        public static string Abbreviate(string lord)
        {
            if (string.IsNullOrWhiteSpace(lord)) return "---";
            string abbr;
            if (Abbrev.TryGetValue(lord.Trim(), out abbr)) return abbr;
            var t = lord.Trim();
            return t.Length <= 3 ? t : t.Substring(0, 3);
        }

        /// <summary>Full VB6-style monospaced page text (groups of 3 mahas).</summary>
        public static string FormatVimshottariPage(VimshottariDashaResult dasha)
        {
            if (dasha == null) return "";
            var sb = new StringBuilder();
            sb.AppendLine("VIMSHOTTARI DASHA TABLE");
            sb.AppendLine("----------------------");
            sb.AppendLine();
            sb.AppendLine("Mahadasa balance at birth : " + (dasha.BalanceText ?? "-"));
            sb.AppendLine();
            sb.AppendLine("Dates given below show ending moments of Dasa in AD Era.");
            sb.AppendLine();

            var mahas = dasha.MahaDashas ?? new List<MahaDashaBlock>();
            var antars = dasha.AntarDashas ?? new List<AntarDashaRow>();
            for (var group = 0; group < 3; group++)
            {
                var m1 = mahas.ElementAtOrDefault(group * 3);
                var m2 = mahas.ElementAtOrDefault(group * 3 + 1);
                var m3 = mahas.ElementAtOrDefault(group * 3 + 2);
                if (m1 == null && m2 == null && m3 == null) break;

                sb.AppendLine(PadCol(DashLine(28), 31) + PadCol(DashLine(28), 31) + PadCol(DashLine(28), 31));
                sb.AppendLine(
                    PadCol(MahaTitle(m1), 31) +
                    PadCol(MahaTitle(m2), 31) +
                    PadCol(MahaTitle(m3), 31));
                sb.AppendLine(
                    PadCol(MahaPeriod(m1), 31) +
                    PadCol(MahaPeriod(m2), 31) +
                    PadCol(MahaPeriod(m3), 31));
                sb.AppendLine(
                    PadCol(m1 != null ? (m1.AgeLabel ?? "") : "", 31) +
                    PadCol(m2 != null ? (m2.AgeLabel ?? "") : "", 31) +
                    PadCol(m3 != null ? (m3.AgeLabel ?? "") : "", 31));
                sb.AppendLine();
                sb.AppendLine(
                    PadCol("Antar   Beginning  Enging", 31) +
                    PadCol("Antar   Beginning  Enging", 31) +
                    PadCol("Antar   Beginning  Enging", 31));
                sb.AppendLine(
                    PadCol(DashLine(29), 30) +
                    PadCol(DashLine(29), 30) +
                    PadCol(DashLine(29), 30));

                var rows1 = PadAntarSlots(antars, m1, dasha.BirthDateLabel);
                var rows2 = PadAntarSlots(antars, m2, dasha.BirthDateLabel);
                var rows3 = PadAntarSlots(antars, m3, dasha.BirthDateLabel);
                for (var y = 0; y < 9; y++)
                {
                    sb.AppendLine(rows1[y] + "| " + rows2[y] + "| " + rows3[y]);
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine(PadCol(DashLine(28), 31) + PadCol(DashLine(28), 31) + PadCol(DashLine(28), 31));
            return sb.ToString().TrimEnd();
        }

        /// <summary>HTML-friendly groups for on-screen 3-column cards.</summary>
        public static List<DashaPrintGroup> BuildGroups(VimshottariDashaResult dasha)
        {
            var groups = new List<DashaPrintGroup>();
            if (dasha == null) return groups;
            var mahas = dasha.MahaDashas ?? new List<MahaDashaBlock>();
            var antars = dasha.AntarDashas ?? new List<AntarDashaRow>();
            for (var group = 0; group < 3; group++)
            {
                var cols = new List<DashaPrintColumn>();
                for (var c = 0; c < 3; c++)
                {
                    var m = mahas.ElementAtOrDefault(group * 3 + c);
                    if (m == null) continue;
                    var slots = PadAntarSlots(antars, m, dasha.BirthDateLabel);
                    var rows = new List<DashaPrintAntarRow>();
                    for (var i = 0; i < 9; i++)
                    {
                        var raw = slots[i];
                        if (raw.IndexOf("***********", StringComparison.Ordinal) >= 0)
                        {
                            rows.Add(new DashaPrintAntarRow { Label = "-", Range = "***********", IsBlank = true });
                        }
                        else
                        {
                            var sp = raw.IndexOf(' ');
                            rows.Add(new DashaPrintAntarRow
                            {
                                Label = sp > 0 ? raw.Substring(0, sp).Trim() : raw.Trim(),
                                Range = sp > 0 ? raw.Substring(sp + 1).Trim() : "",
                                IsBlank = false
                            });
                        }
                    }
                    cols.Add(new DashaPrintColumn
                    {
                        Title = MahaTitle(m),
                        Period = MahaPeriod(m),
                        AgeLabel = m.AgeLabel ?? "",
                        Antars = rows
                    });
                }
                if (cols.Count > 0)
                    groups.Add(new DashaPrintGroup { Columns = cols });
            }
            return groups;
        }

        private static string[] PadAntarSlots(List<AntarDashaRow> all, MahaDashaBlock maha, string birthDate)
        {
            var slots = new string[9];
            for (var i = 0; i < 9; i++)
                slots[i] = new string(' ', 10) + "***********" + new string(' ', 8);

            if (maha == null) return slots;

            var list = all.Where(a => a.MahaSequence == maha.Sequence).ToList();
            var pad = Math.Max(0, 9 - list.Count);
            for (var i = 0; i < list.Count && (pad + i) < 9; i++)
            {
                var a = list[i];
                var from = string.IsNullOrEmpty(a.FromDate) ? (birthDate ?? "") : a.FromDate;
                var to = a.ToDate ?? "";
                var label = Abbreviate(a.MahaLord) + "-" + Abbreviate(a.AntarLord);
                // VB6 cell ~ "San-Bud 08.02.1981-09.05.1981" (~29 chars before |)
                var range = from + "-" + to;
                slots[pad + i] = PadRight(label, 8) + range;
            }
            return slots;
        }

        private static string MahaTitle(MahaDashaBlock m)
        {
            if (m == null) return "";
            return "Maha Dasa of " + (m.Lord ?? "") + " (" + m.Years.ToString(CultureInfo.InvariantCulture) + "y)";
        }

        private static string MahaPeriod(MahaDashaBlock m)
        {
            if (m == null) return "";
            return (m.FromDate ?? "") + " to " + (m.ToDate ?? "");
        }

        private static string DashLine(int n)
        {
            return new string('-', n);
        }

        private static string PadCol(string text, int width)
        {
            return PadRight(text ?? "", width);
        }

        private static string PadRight(string text, int width)
        {
            if (text == null) text = "";
            if (text.Length >= width) return text.Substring(0, width);
            return text + new string(' ', width - text.Length);
        }
    }

    public sealed class DashaPrintGroup
    {
        public List<DashaPrintColumn> Columns { get; set; }
        public DashaPrintGroup() { Columns = new List<DashaPrintColumn>(); }
    }

    public sealed class DashaPrintColumn
    {
        public string Title { get; set; }
        public string Period { get; set; }
        public string AgeLabel { get; set; }
        public List<DashaPrintAntarRow> Antars { get; set; }
        public DashaPrintColumn() { Antars = new List<DashaPrintAntarRow>(); }
    }

    public sealed class DashaPrintAntarRow
    {
        public string Label { get; set; }
        public string Range { get; set; }
        public bool IsBlank { get; set; }
    }
}
