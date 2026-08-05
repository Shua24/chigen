using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using Chigen.Core.Models;

namespace Chigen.DocumentExport;

/// <summary>
/// Generates a PDF rendering of a <see cref="ReportDocument"/> using PdfSharp.
/// </summary>
public class DirectPdfGenerator
{
    private const double RowHeight = 16;
    private const double SectionHeaderHeight = 14;
    private const double FooterHeight = 56;
    private const double GapSmall = 4;
    private const double GapMedium = 6;
    private const double LogoGap = 6;
    private const double SideColumnX = 40;

    private static readonly XFont TitleFont = new(Typography.Family, Typography.ReportTitlePt, XFontStyleEx.Bold);
    private static readonly XFont SubtitleFont = new(Typography.Family, Typography.SubtitlePt, XFontStyleEx.Regular);
    private static readonly XFont HeaderFont = new(Typography.Family, Typography.PdfSectionHeaderPt, XFontStyleEx.Bold);
    private static readonly XFont BodyFont = new(Typography.Family, Typography.PdfBodyPt, XFontStyleEx.Regular);
    private static readonly XFont SmallFont = new(Typography.Family, Typography.AddressPt, XFontStyleEx.Regular);
    private static readonly XFont TableHeaderFont = new(Typography.Family, Typography.PdfTableHeaderPt, XFontStyleEx.Bold);
    private static readonly XFont TableFont = new(Typography.Family, Typography.PdfTablePt, XFontStyleEx.Regular);
    private static readonly XFont LetterheadTitleFont = new(Typography.Family, Typography.LetterheadTitlePt, XFontStyleEx.Bold);
    private static readonly XFont LetterheadSubtitleFont = new(Typography.Family, Typography.LetterheadSubtitlePt, XFontStyleEx.Regular);

    private readonly ReportDocument _doc;

    /// <summary>Creates a generator for the given report.</summary>
    public DirectPdfGenerator(ReportDocument doc) => _doc = doc;

    /// <summary>Renders the report to a PDF file at <paramref name="outputPath"/> and returns the path.</summary>
    public string Create(string outputPath)
    {
        using var doc = new PdfDocument();
        doc.Info.Title = _doc.ReportTitle;
        doc.Info.Author = _doc.InstitutionName;

        var cursor = new PdfPageCursor(doc);
        DrawLetterhead(cursor);
        DrawReportTitle(cursor);
        DrawPatientInfo(cursor);
        DrawDifferentialTable(cursor);
        if (_doc.ShowConclusion)
            DrawTextSection(cursor, _doc.ConclusionSectionLabel, _doc.ConclusionText);
        if (_doc.ShowRecommendations)
            DrawTextSection(cursor, _doc.RecommendationsSectionLabel, _doc.RecommendationsText);
        DrawFooter(cursor);
        cursor.Finish();

        doc.Save(outputPath);
        return outputPath;
    }

    private void DrawLetterhead(PdfPageCursor cursor)
    {
        if (!_doc.ShowLetterhead) return;
        if (_doc.LogoPlacement == LogoPlacement.Side)
            DrawLetterheadSide(cursor);
        else
            DrawLetterheadTop(cursor);
    }

    private void DrawLetterheadTop(PdfPageCursor cursor)
    {
        if (_doc.HasLogo)
        {
            using var logo = ImageMetrics.LoadScaled(_doc.LogoPath, ImageMetrics.LogoMaxWidth, ImageMetrics.LogoMaxHeight);
            cursor.Graphics.DrawImage(logo.Image, (cursor.PageWidth - logo.Width) / 2, cursor.Y, logo.Width, logo.Height);
            cursor.Advance(logo.Height + LogoGap);
        }

        cursor.WriteCentered(_doc.InstitutionName, LetterheadTitleFont, XBrushes.Black, 18);
        if (!string.IsNullOrEmpty(_doc.Department))
            cursor.WriteCentered(_doc.Department, LetterheadSubtitleFont, XBrushes.Black, 14);

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            cursor.WriteCentered(_doc.AddressLine, SmallFont, XBrushes.Gray, 12);
            cursor.DrawHLine(new XPen(XColors.Black, 3), cursor.Y - 1);
            cursor.Advance(GapSmall);
        }
        else
        {
            cursor.DrawHLine(new XPen(XColors.Black, 3), cursor.Y + 2);
            cursor.Advance(GapMedium);
        }
    }

    private void DrawLetterheadSide(PdfPageCursor cursor)
    {
        double logoWidth = 0, logoHeight = 0, logoX = 0;
        if (_doc.HasLogo)
        {
            using var logo = ImageMetrics.LoadScaled(_doc.LogoPath, ImageMetrics.SideLogoMaxDimension, ImageMetrics.SideLogoMaxDimension);
            logoWidth = logo.Width;
            logoHeight = logo.Height;
            logoX = SideColumnX;
            cursor.Graphics.DrawImage(logo.Image, logoX, cursor.Y + 2, logoWidth, logoHeight);
        }

        double textX = logoWidth > 0 ? logoX + logoWidth + 10 : SideColumnX;
        double textWidth = cursor.PageWidth - textX - PdfPageCursor.RightMargin;
        double lineHeight = 0;

        cursor.Graphics.DrawString(_doc.InstitutionName, LetterheadTitleFont, XBrushes.Black,
            new XRect(textX, cursor.Y, textWidth, 18), XStringFormats.TopLeft);
        lineHeight = 18;

        if (!string.IsNullOrEmpty(_doc.Department))
        {
            cursor.Graphics.DrawString(_doc.Department, LetterheadSubtitleFont, XBrushes.Black,
                new XRect(textX, cursor.Y + lineHeight, textWidth, 14), XStringFormats.TopLeft);
            lineHeight += 14;
        }

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            cursor.Graphics.DrawString(_doc.AddressLine, SmallFont, XBrushes.Gray,
                new XRect(textX, cursor.Y + lineHeight, textWidth, 12), XStringFormats.TopLeft);
            lineHeight += 12;
        }

        cursor.Advance(Math.Max(logoHeight, lineHeight) + GapSmall);
        cursor.DrawHLine(new XPen(XColors.Black, 3), cursor.Y);
        cursor.Advance(GapMedium);
    }

    private void DrawReportTitle(PdfPageCursor cursor)
    {
        cursor.Advance(8);
        cursor.WriteCentered(_doc.ReportTitle, TitleFont, XBrushes.Black, 22);
        cursor.WriteCentered(_doc.Subtitle, SubtitleFont, XBrushes.Black, 16);
        cursor.Advance(GapSmall);
        cursor.DrawHLine(XPens.Black, cursor.Y);
        cursor.Advance(GapMedium);
    }

    private void DrawPatientInfo(PdfPageCursor cursor)
    {
        if (!_doc.ShowPatientInfo) return;

        cursor.Advance(2);
        cursor.WriteLeft(_doc.PatientInfoSectionLabel, HeaderFont, XBrushes.Black, SectionHeaderHeight);

        var pairs = new List<(string, string, string, string)>();
        for (int i = 0; i < _doc.PatientFields.Count; i += 2)
        {
            var f1 = _doc.PatientFields[i];
            var f2 = i + 1 < _doc.PatientFields.Count ? _doc.PatientFields[i + 1] : null;
            pairs.Add((f1.Label, f1.Value, f2?.Label ?? "", f2?.Value ?? ""));
        }

        double left = cursor.ContentLeft;
        double mid = cursor.PageWidth / 2;

        foreach (var (l1, v1, l2, v2) in pairs)
        {
            cursor.Graphics.DrawString($"{l1} {v1}", BodyFont, XBrushes.Black,
                new XRect(left, cursor.Y, mid - left, 12), XStringFormats.TopLeft);
            if (!string.IsNullOrEmpty(l2))
                cursor.Graphics.DrawString($"{l2} {v2}", BodyFont, XBrushes.Black,
                    new XRect(mid, cursor.Y, cursor.PageWidth - mid - PdfPageCursor.RightMargin, 12), XStringFormats.TopLeft);
            cursor.Advance(12);
        }

        cursor.Advance(GapSmall);
        cursor.DrawHLine(XPens.Black, cursor.Y);
        cursor.Advance(GapMedium);
    }

    private void DrawDifferentialTable(PdfPageCursor cursor)
    {
        if (_doc.Rows.Count == 0) return;

        double tableLeft = cursor.ContentLeft;
        double tableW = cursor.ContentWidth;
        double rowH = RowHeight;

        int colCount = _doc.ShowReferenceRanges ? 4 : 3;
        double[] colWidths = _doc.ShowReferenceRanges
            ? [tableW * 0.40, tableW * 0.18, tableW * 0.18, tableW * 0.24]
            : [tableW * 0.50, tableW * 0.25, tableW * 0.25];
        string[] headers = _doc.ShowReferenceRanges
            ? [_doc.ColCellType, _doc.ColCount, _doc.ColPercent, _doc.ColRefRange]
            : [_doc.ColCellType, _doc.ColCount, _doc.ColPercent];

        double[] colX = new double[colCount];
        double cx = tableLeft;
        for (int i = 0; i < colCount; i++)
        {
            colX[i] = cx;
            cx += colWidths[i];
        }

        void DrawCell(int col, string text, XFont font, XBrush brush, XStringFormat format, double leftPadding = 0)
            => cursor.Graphics.DrawString(text, font, brush,
                new XRect(colX[col] + leftPadding, cursor.Y, colWidths[col] - leftPadding, rowH), format);

        // Header row
        cursor.EnsureSpace(rowH);
        cursor.Graphics.DrawRectangle(XBrushes.LightSteelBlue, tableLeft, cursor.Y, tableW, rowH);
        for (int i = 0; i < colCount; i++)
        {
            if (i > 0) cursor.Graphics.DrawLine(XPens.White, colX[i], cursor.Y, colX[i], cursor.Y + rowH);
            DrawCell(i, headers[i], TableHeaderFont, XBrushes.Black, XStringFormats.Center);
        }
        cursor.Advance(rowH);

        // Data rows
        foreach (var row in _doc.Rows)
        {
            cursor.EnsureSpace(rowH);
            cursor.Graphics.DrawRectangle(XPens.LightGray, XBrushes.White, tableLeft, cursor.Y, tableW, rowH);
            DrawCell(0, row.CellName, TableFont, XBrushes.Black, XStringFormats.CenterLeft, 4);
            DrawCell(1, row.Count.ToString(), TableFont, XBrushes.Black, XStringFormats.Center);
            DrawCell(2, row.Percentage.ToString("F1") + "%", TableFont, XBrushes.Black, XStringFormats.Center);
            if (_doc.ShowReferenceRanges)
                DrawCell(3, row.ReferenceRange, TableFont, XBrushes.DimGray, XStringFormats.Center);
            cursor.Graphics.DrawLine(XPens.LightGray, tableLeft, cursor.Y + rowH, tableLeft + tableW, cursor.Y + rowH);
            cursor.Advance(rowH);
        }

        // Total row
        cursor.EnsureSpace(rowH);
        cursor.Graphics.DrawRectangle(new XPen(XColors.Black, 2), XBrushes.LightSteelBlue, tableLeft, cursor.Y, tableW, rowH);
        DrawCell(0, _doc.TotalLabel, TableHeaderFont, XBrushes.Black, XStringFormats.CenterLeft, 4);
        DrawCell(1, _doc.TotalCount.ToString(), TableFont, XBrushes.Black, XStringFormats.Center);
        DrawCell(2, "100%", TableFont, XBrushes.Black, XStringFormats.Center);
        cursor.Advance(rowH + GapMedium);
    }

    private void DrawTextSection(PdfPageCursor cursor, string sectionLabel, string text)
    {
        var availableWidth = cursor.ContentWidth;

        // Calculate actual wrapped height by simulating XTextFormatter's line-breaking behavior
        var lines = new List<string>();
        var paragraphs = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var paragraph in paragraphs)
        {
            if (string.IsNullOrEmpty(paragraph))
            {
                lines.Add("");
                continue;
            }

            var words = paragraph.Split(new[] { ' ' }, StringSplitOptions.None);
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testWidth = cursor.Graphics.MeasureString(testLine, TableFont).Width;

                if (testWidth > availableWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
        }

        var lineCount = Math.Max(1, lines.Count);
        var wrappedHeight = lineCount * TableFont.Height;
        var requiredHeight = SectionHeaderHeight + wrappedHeight + SectionHeaderHeight;

        cursor.EnsureSpace(requiredHeight);

        cursor.Advance(2);
        cursor.WriteLeft(sectionLabel, TableHeaderFont, XBrushes.Black, SectionHeaderHeight);

        var textFormatter = new XTextFormatter(cursor.Graphics);
        var textRect = new XRect(cursor.ContentLeft, cursor.Y, availableWidth, wrappedHeight);
        textFormatter.DrawString(text, TableFont, XBrushes.Black, textRect, XStringFormats.TopLeft);
        cursor.Advance(wrappedHeight + GapSmall);
    }

    private void DrawFooter(PdfPageCursor cursor)
    {
        if (!_doc.ShowFooter) return;

        cursor.EnsureSpace(FooterHeight);

        cursor.Advance(GapMedium);
        cursor.DrawHLine(XPens.Black, cursor.Y);
        cursor.Advance(GapSmall);
        cursor.WriteLeft(_doc.GeneratedAt, TableFont, XBrushes.Gray, 10);
        if (!string.IsNullOrEmpty(_doc.FooterText))
            cursor.WriteLeft(_doc.FooterText, TableFont, XBrushes.Gray, 10);
        cursor.Advance(GapSmall);
        cursor.WriteLeft("___________________________________", TableFont, XBrushes.Black, 10);
        cursor.Advance(12);
        cursor.WriteLeft(_doc.SignatureLabel, TableFont, XBrushes.Black, 10);
    }
}
