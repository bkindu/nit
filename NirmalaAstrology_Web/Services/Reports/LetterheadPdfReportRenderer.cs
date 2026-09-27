using System;
using System.Collections.Generic;
using System.IO;
using NirmalaAstrology_Web.Models.Reports;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace NirmalaAstrology_Web.Services.Reports
{
    /// <summary>
    /// Renders OfficialReportDocument to A4 PDF with full-page letterhead background.
    /// Content never overlaps header/footer branding baked into the PNG.
    /// </summary>
    public sealed class LetterheadPdfReportRenderer
    {
        // A4 points (72 dpi): 595.27 x 841.89
        // Safe content box derived from letterhead analysis (~25% header, ~12% footer, ~0.75" sides).
        private const double MarginLeft = 54;   // ~19 mm
        private const double MarginRight = 54;
        private const double MarginTop = 168;   // ~59 mm — below letterhead header block
        private const double MarginBottom = 92; // ~32 mm — above footer/zodiac row

        private readonly string _letterheadPath;

        public LetterheadPdfReportRenderer(string letterheadImagePath)
        {
            if (string.IsNullOrWhiteSpace(letterheadImagePath))
                throw new ArgumentNullException("letterheadImagePath");
            _letterheadPath = letterheadImagePath;
        }

        public byte[] Render(OfficialReportDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");
            if (!File.Exists(_letterheadPath))
                throw new FileNotFoundException("Official letterhead image was not found.", _letterheadPath);

            using (var letterhead = XImage.FromFile(_letterheadPath))
            using (var pdf = new PdfDocument())
            {
                pdf.Info.Title = document.Title ?? "Nirmala Astrology Report";
                pdf.Info.Author = "Nirmala Astrology";
                pdf.Info.Creator = "Nirmala Astrology Official Report Engine";
                pdf.Info.Subject = document.Subtitle ?? document.ReportType;

                var state = new RenderState(pdf, letterhead, document);
                state.EnsurePage();
                DrawTitleBlock(state);
                DrawMetaBlock(state);

                foreach (var section in document.Sections ?? new List<ReportSection>())
                {
                    if (section == null) continue;
                    if (section.PageBreakBefore)
                        state.NewPage();
                    DrawSection(state, section);
                }

                // PdfSharp allows only one XGraphics per page — release content gfx before page numbers.
                state.DisposeRetained();
                NumberPages(pdf, document);
                using (var ms = new MemoryStream())
                {
                    pdf.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        private static void DrawTitleBlock(RenderState state)
        {
            var titleFont = new XFont("Times New Roman", 16, XFontStyle.Bold);
            var subFont = new XFont("Times New Roman", 10, XFontStyle.Italic);
            var refFont = new XFont("Times New Roman", 9, XFontStyle.Regular);

            state.DrawCentered(state.Document.Title ?? "OFFICIAL REPORT", titleFont, XBrushes.DarkRed);
            state.Y += 4;
            if (!string.IsNullOrWhiteSpace(state.Document.Subtitle))
            {
                state.DrawCentered(state.Document.Subtitle, subFont, XBrushes.SaddleBrown);
                state.Y += 3;
            }
            state.DrawCentered(
                "Ref. " + (state.Document.ReferenceNo ?? "") + "   ·   " + state.Document.GeneratedAt.ToString("dd.MM.yyyy HH:mm"),
                refFont, XBrushes.DimGray);
            state.Y += 8;
            state.DrawRule();
            state.Y += 10;
        }

        private static void DrawMetaBlock(RenderState state)
        {
            if (state.Document.Meta == null || state.Document.Meta.Count == 0) return;
            var labelFont = new XFont("Times New Roman", 9, XFontStyle.Bold);
            var valueFont = new XFont("Times New Roman", 9, XFontStyle.Regular);
            var colWidth = state.ContentWidth / 2.0;
            var rowH = 13.0;
            var i = 0;
            while (i < state.Document.Meta.Count)
            {
                state.EnsureSpace(rowH + 2);
                for (var c = 0; c < 2 && i < state.Document.Meta.Count; c++, i++)
                {
                    var item = state.Document.Meta[i];
                    var x = state.Left + c * colWidth;
                    state.Gfx.DrawString((item.Label ?? "") + ":", labelFont, XBrushes.SaddleBrown, x, state.Y);
                    state.Gfx.DrawString(item.Value ?? "", valueFont, XBrushes.Black, x + 78, state.Y);
                }
                state.Y += rowH;
            }
            state.Y += 6;
            state.DrawRule();
            state.Y += 12;
        }

        private static void DrawSection(RenderState state, ReportSection section)
        {
            var headingFont = new XFont("Times New Roman", 12, XFontStyle.Bold);
            state.EnsureSpace(28);
            state.Gfx.DrawString(section.Heading ?? "Section", headingFont, XBrushes.DarkRed, state.Left, state.Y);
            state.Y += 16;
            state.DrawRule(0.6);
            state.Y += 10;

            if (section.KeyValues != null)
            {
                var lf = new XFont("Times New Roman", 9, XFontStyle.Bold);
                var vf = new XFont("Times New Roman", 9, XFontStyle.Regular);
                foreach (var kv in section.KeyValues)
                {
                    state.EnsureSpace(14);
                    state.Gfx.DrawString((kv.Label ?? "") + ":", lf, XBrushes.SaddleBrown, state.Left, state.Y);
                    DrawWrapped(state, kv.Value ?? "", vf, XBrushes.Black, state.Left + 130, state.ContentWidth - 130);
                }
                state.Y += 4;
            }

            if (section.Paragraphs != null)
            {
                var body = new XFont("Times New Roman", 9.5, XFontStyle.Regular);
                foreach (var p in section.Paragraphs)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;
                    DrawWrapped(state, p.Trim(), body, XBrushes.Black, state.Left, state.ContentWidth);
                    state.Y += 6;
                }
            }

            if (section.MonospaceLines != null && section.MonospaceLines.Count > 0)
            {
                var mono = new XFont("Courier New", 6.6, XFontStyle.Regular);
                foreach (var line in section.MonospaceLines)
                {
                    state.EnsureSpace(10);
                    state.Gfx.DrawString(line ?? "", mono, XBrushes.Black, state.Left, state.Y);
                    state.Y += 8.2;
                }
                state.Y += 4;
            }

            if (section.Table != null && section.Table.Headers != null && section.Table.Headers.Count > 0)
                DrawTable(state, section.Table);

            if (section.ImageBytes != null && section.ImageBytes.Length > 0)
                DrawImage(state, section);

            state.Y += 10;
        }

        private static void DrawImage(RenderState state, ReportSection section)
        {
            try
            {
                var ms = new MemoryStream(section.ImageBytes);
                state.RetainStream(ms);
                var img = XImage.FromStream(ms);
                state.RetainImage(img);

                var maxW = state.ContentWidth;
                var maxH = section.ImageMaxHeight > 0 ? section.ImageMaxHeight : 420;
                var scale = Math.Min(maxW / img.PixelWidth, maxH / img.PixelHeight);
                var w = img.PixelWidth * scale;
                var h = img.PixelHeight * scale;
                state.EnsureSpace(h + 8);
                var x = state.Left + (state.ContentWidth - w) / 2.0;
                state.Gfx.DrawImage(img, x, state.Y, w, h);
                state.Y += h + 6;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[LetterheadPdf.DrawImage] " + ex.Message);
                state.EnsureSpace(20);
                state.Gfx.DrawString("(Chart image unavailable)", new XFont("Times New Roman", 9, XFontStyle.Italic), XBrushes.DimGray, state.Left, state.Y);
                state.Y += 14;
            }
        }

        private static void DrawTable(RenderState state, ReportTable table)
        {
            var headerFont = new XFont("Times New Roman", 8, XFontStyle.Bold);
            var cellFont = new XFont("Times New Roman", 8, XFontStyle.Regular);
            var weights = table.ColumnWeights;
            if (weights == null || weights.Length != table.Headers.Count)
            {
                weights = new float[table.Headers.Count];
                for (var i = 0; i < weights.Length; i++) weights[i] = 1f;
            }
            double weightSum = 0;
            foreach (var w in weights) weightSum += w;
            var colWidths = new double[weights.Length];
            for (var i = 0; i < weights.Length; i++)
                colWidths[i] = state.ContentWidth * (weights[i] / weightSum);

            const double rowPad = 3;
            Action drawHeader = () =>
            {
                state.EnsureSpace(18);
                var x = state.Left;
                var h = 14.0;
                state.Gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(235, 220, 190)), state.Left, state.Y - 10, state.ContentWidth, h);
                for (var i = 0; i < table.Headers.Count; i++)
                {
                    state.Gfx.DrawString(table.Headers[i] ?? "", headerFont, XBrushes.DarkRed, x + rowPad, state.Y);
                    x += colWidths[i];
                }
                state.Y += 8;
                state.DrawRule(0.5);
                state.Y += 8;
            };

            drawHeader();
            foreach (var row in table.Rows ?? new List<string[]>())
            {
                if (row == null) continue;
                state.EnsureSpace(16, drawHeader);
                var x = state.Left;
                for (var i = 0; i < table.Headers.Count; i++)
                {
                    var text = i < row.Length ? (row[i] ?? "") : "";
                    if (text.Length > 42) text = text.Substring(0, 39) + "...";
                    state.Gfx.DrawString(text, cellFont, XBrushes.Black, x + rowPad, state.Y);
                    x += colWidths[i];
                }
                state.Y += 12;
            }
            state.Y += 4;
        }

        private static void DrawWrapped(RenderState state, string text, XFont font, XBrush brush, double x, double maxWidth)
        {
            if (string.IsNullOrEmpty(text))
            {
                state.Y += font.Height + 2;
                return;
            }
            var words = text.Replace("\r", "").Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var line = "";
            foreach (var word in words)
            {
                var test = string.IsNullOrEmpty(line) ? word : line + " " + word;
                var size = state.Gfx.MeasureString(test, font);
                if (size.Width > maxWidth && !string.IsNullOrEmpty(line))
                {
                    state.EnsureSpace(font.Height + 3);
                    state.Gfx.DrawString(line, font, brush, x, state.Y);
                    state.Y += font.Height + 2;
                    line = word;
                }
                else line = test;
            }
            if (!string.IsNullOrEmpty(line))
            {
                state.EnsureSpace(font.Height + 3);
                state.Gfx.DrawString(line, font, brush, x, state.Y);
                state.Y += font.Height + 2;
            }
        }

        private static void NumberPages(PdfDocument pdf, OfficialReportDocument document)
        {
            var font = new XFont("Times New Roman", 8, XFontStyle.Italic);
            var total = pdf.PageCount;
            for (var i = 0; i < total; i++)
            {
                var page = pdf.Pages[i];
                using (var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append))
                {
                    var label = string.Format("Page {0} of {1}  ·  {2}  ·  Confidential counselling document",
                        i + 1, total, document.NativeName ?? "Native");
                    var size = gfx.MeasureString(label, font);
                    gfx.DrawString(label, font, XBrushes.DimGray,
                        (page.Width.Point - size.Width) / 2.0,
                        page.Height.Point - 28);
                }
            }
        }

        private sealed class RenderState
        {
            public PdfDocument Pdf { get; private set; }
            public XImage Letterhead { get; private set; }
            public OfficialReportDocument Document { get; private set; }
            public PdfPage Page { get; private set; }
            public XGraphics Gfx { get; private set; }
            public double Y { get; set; }
            public double Left { get { return MarginLeft; } }
            public double ContentWidth { get { return Page.Width.Point - MarginLeft - MarginRight; } }
            public double BottomLimit { get { return Page.Height.Point - MarginBottom; } }

            private readonly List<IDisposable> _retained = new List<IDisposable>();

            public RenderState(PdfDocument pdf, XImage letterhead, OfficialReportDocument document)
            {
                Pdf = pdf;
                Letterhead = letterhead;
                Document = document;
            }

            public void RetainStream(MemoryStream ms)
            {
                if (ms != null) _retained.Add(ms);
            }

            public void RetainImage(XImage img)
            {
                if (img != null) _retained.Add(img);
            }

            public void DisposeRetained()
            {
                for (var i = _retained.Count - 1; i >= 0; i--)
                {
                    try { _retained[i].Dispose(); } catch { }
                }
                _retained.Clear();
                if (Gfx != null)
                {
                    try { Gfx.Dispose(); } catch { }
                    Gfx = null;
                }
            }

            public void EnsurePage()
            {
                if (Page == null) NewPage();
            }

            public void NewPage()
            {
                if (Gfx != null) Gfx.Dispose();
                Page = Pdf.AddPage();
                Page.Size = PageSize.A4;
                Gfx = XGraphics.FromPdfPage(Page);
                Gfx.DrawImage(Letterhead, 0, 0, Page.Width.Point, Page.Height.Point);
                Y = MarginTop;
            }

            public void EnsureSpace(double needed, Action onNewPage = null)
            {
                if (Y + needed <= BottomLimit) return;
                NewPage();
                if (onNewPage != null) onNewPage();
            }

            public void DrawCentered(string text, XFont font, XBrush brush)
            {
                EnsureSpace(font.Height + 4);
                var size = Gfx.MeasureString(text ?? "", font);
                Gfx.DrawString(text ?? "", font, brush, Left + (ContentWidth - size.Width) / 2.0, Y);
                Y += font.Height + 2;
            }

            public void DrawRule(double width = 1.0)
            {
                EnsureSpace(6);
                var pen = new XPen(XColor.FromArgb(155, 108, 66), width);
                Gfx.DrawLine(pen, Left, Y, Left + ContentWidth, Y);
            }
        }
    }
}
