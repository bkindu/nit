using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Services.Reports
{
    /// <summary>
    /// Server-side North Indian diamond kundli chart for public 2-page PDF (not a UI screenshot).
    /// </summary>
    public sealed class NorthIndianKundliChartRenderer
    {
        private static readonly string[] ShortNames =
        {
            "Sur", "Cha", "Man", "Bud", "Gur", "Suk", "San", "Rah", "Ket", "Asc"
        };

        public byte[] RenderPng(KundliResult result, int size = 720)
        {
            if (result == null) throw new ArgumentNullException("result");
            size = Math.Max(480, Math.Min(900, size));

            using (var bmp = new Bitmap(size, size))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(Color.FromArgb(255, 254, 245));

                var margin = size * 0.06f;
                var left = margin;
                var top = margin;
                var box = size - 2 * margin;
                var right = left + box;
                var bottom = top + box;
                var midX = left + box / 2f;
                var midY = top + box / 2f;

                using (var pen = new Pen(Color.FromArgb(139, 69, 19), Math.Max(2f, size / 280f)))
                {
                    g.DrawRectangle(pen, left, top, box, box);
                    g.DrawLine(pen, left, top, right, bottom);
                    g.DrawLine(pen, right, top, left, bottom);
                    g.DrawLine(pen, midX, top, left, midY);
                    g.DrawLine(pen, midX, top, right, midY);
                    g.DrawLine(pen, midX, bottom, left, midY);
                    g.DrawLine(pen, midX, bottom, right, midY);
                }

                var centers = BuildHouseCenters(left, top, box);
                var houseMap = (result.Houses ?? new List<HouseData>()).ToDictionary(h => h.HouseNumber, h => h);

                using (var rashiFont = new Font("Times New Roman", Math.Max(11f, size / 42f), FontStyle.Bold))
                using (var planetFont = new Font("Times New Roman", Math.Max(9f, size / 52f), FontStyle.Regular))
                using (var titleBrush = new SolidBrush(Color.FromArgb(139, 0, 0)))
                using (var textBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
                {
                    for (var h = 1; h <= 12; h++)
                    {
                        var c = centers[h];
                        HouseData hd;
                        houseMap.TryGetValue(h, out hd);
                        var rashi = hd != null ? hd.Rashi.ToString() : "";
                        if (string.IsNullOrEmpty(rashi) && hd != null && !string.IsNullOrWhiteSpace(hd.RashiName))
                            rashi = AbbreviateRashi(hd.RashiName);

                        var rashiSize = g.MeasureString(rashi, rashiFont);
                        g.DrawString(rashi, rashiFont, titleBrush, c.X - rashiSize.Width / 2f, c.Y - rashiSize.Height - 2);

                        var planets = FormatPlanets(hd, result, h);
                        if (!string.IsNullOrEmpty(planets))
                        {
                            var pSize = g.MeasureString(planets, planetFont);
                            g.DrawString(planets, planetFont, textBrush, c.X - pSize.Width / 2f, c.Y + 2);
                        }
                    }
                }

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// House text centers from legacy VB6 <c>Print_Kundli_Sng</c> / <c>Content/Kun.svg</c> (400×400).
        /// H1 = center Lagna; H2 top-left, then counter-clockwise to H12 top-right.
        /// Do not mirror this layout — PDF must match Kun.svg used on Result page.
        /// </summary>
        private static PointF[] BuildHouseCenters(float left, float top, float box)
        {
            // Anchors copied from Kun.svg id=h1..h12 (x,y).
            float[,] kun = {
                { 0f, 0f },
                { 188.44442f, 168.33312f }, // 1 center (LAG)
                { 95.55482f, 82.22136f },   // 2 top-left
                { 64.44348f, 108.88821f },  // 3 left
                { 161.11085f, 207.77781f }, // 4 left-center
                { 67.77684f, 304.44518f },  // 5 bottom-left
                { 94.4437f, 327.77868f },   // 6 bottom-leftish
                { 193.3333f, 242.22251f },  // 7 bottom
                { 291.11179f, 334.4454f },  // 8 bottom-right
                { 322.22312f, 305.5563f },  // 9 lower-right
                { 224.44464f, 208.88893f }, // 10 right-center
                { 312.22305f, 106.66597f }, // 11 upper-right
                { 283.33395f, 77.77688f }   // 12 top-right
            };

            const float svgSize = 400f;
            var centers = new PointF[13];
            for (var h = 1; h <= 12; h++)
            {
                centers[h] = new PointF(
                    left + (kun[h, 0] / svgSize) * box,
                    top + (kun[h, 1] / svgSize) * box);
            }
            return centers;
        }

        private static string FormatPlanets(HouseData hd, KundliResult result, int houseNo)
        {
            var list = new List<string>();
            if (hd != null && hd.Planets != null)
            {
                foreach (var p in hd.Planets)
                {
                    var s = AbbreviatePlanet(p);
                    if (!string.IsNullOrEmpty(s) && !list.Contains(s)) list.Add(s);
                }
            }
            if (list.Count == 0 && result.PlanetPositions != null)
            {
                foreach (var p in result.PlanetPositions.Where(x => x.House == houseNo))
                {
                    var s = AbbreviatePlanet(p.Name);
                    if (!string.IsNullOrEmpty(s) && !list.Contains(s)) list.Add(s);
                }
            }
            if (houseNo == 1) list.Insert(0, "Lag");
            return string.Join(" ", list.Take(5));
        }

        private static string AbbreviatePlanet(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var n = name.Trim().ToLowerInvariant();
            if (n.Contains("sun") || n.Contains("surya")) return "Su";
            if (n.Contains("moon") || n.Contains("chandra")) return "Mo";
            if (n.Contains("mars") || n.Contains("mangal")) return "Ma";
            if (n.Contains("mercury") || n.Contains("budh")) return "Me";
            if (n.Contains("jupiter") || n.Contains("guru")) return "Ju";
            if (n.Contains("venus") || n.Contains("shukra") || n.Contains("sukra")) return "Ve";
            if (n.Contains("saturn") || n.Contains("shani") || n.Contains("sani")) return "Sa";
            if (n.Contains("rahu")) return "Ra";
            if (n.Contains("ketu")) return "Ke";
            if (n.Length <= 3) return name.Trim();
            return name.Trim().Substring(0, 2);
        }

        private static string AbbreviateRashi(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Aries", "1" }, { "Taurus", "2" }, { "Gemini", "3" }, { "Cancer", "4" },
                { "Leo", "5" }, { "Virgo", "6" }, { "Libra", "7" }, { "Scorpio", "8" },
                { "Sagittarius", "9" }, { "Capricorn", "10" }, { "Aquarius", "11" }, { "Pisces", "12" },
                { "Mesh", "1" }, { "Vrishabh", "2" }, { "Mithun", "3" }, { "Kark", "4" },
                { "Simha", "5" }, { "Kanya", "6" }, { "Tula", "7" }, { "Vrishchik", "8" },
                { "Dhanu", "9" }, { "Makar", "10" }, { "Kumbh", "11" }, { "Meen", "12" }
            };
            string v;
            return map.TryGetValue(name.Trim(), out v) ? v : name.Trim().Substring(0, Math.Min(3, name.Trim().Length));
        }
    }
}
