using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using Chigen.Core.Models;

namespace Chigen.DocumentExport;

public class DirectPdfGenerator
{
    private readonly ReportDocument _doc;

    private readonly XFont TitleFont = new("Calibri", 16, XFontStyleEx.Bold);
    private readonly XFont SubtitleFont = new("Calibri", 11, XFontStyleEx.Regular);
    private readonly XFont HeaderFont = new("Calibri", 10, XFontStyleEx.Bold);
    private readonly XFont BodyFont = new("Calibri", 10, XFontStyleEx.Regular);
    private readonly XFont SmallFont = new("Calibri", 9, XFontStyleEx.Regular);
    private readonly XFont TableHeaderFont = new("Calibri", 9, XFontStyleEx.Bold);
    private readonly XFont TableFont = new("Calibri", 8, XFontStyleEx.Regular);
    private readonly XFont LetterheadTitleFont = new("Calibri", 12, XFontStyleEx.Bold);
    private readonly XFont LetterheadSubtitleFont = new("Calibri", 10, XFontStyleEx.Regular);

    private const double PageTopMargin = 20;
    private const double PageBottomMargin = 60;
    private const double PageLeftMargin = 20;
    private const double PageRightMargin = 20;

    private PdfDocument? _currentDocument;
    private PdfPage? _currentPage;
    private XGraphics? _currentGraphics;
    private double _pageWidth;
    private double _pageHeight;

    public DirectPdfGenerator(ReportDocument doc) => _doc = doc;

    public string Create(string outputPath)
    {
        using var doc = new PdfDocument();
        doc.Info.Title = _doc.ReportTitle;
        doc.Info.Author = _doc.InstitutionName;

        _currentDocument = doc;
        _currentPage = doc.AddPage();
        _pageWidth = _currentPage.Width;
        _pageHeight = _currentPage.Height;
        _currentGraphics = XGraphics.FromPdfPage(_currentPage);

        double y = PageTopMargin;
        y = DrawLetterhead(_currentGraphics, y);
        y = DrawReportTitle(_currentGraphics, y);
        y = DrawPatientInfo(_currentGraphics, y);
        y = DrawDifferentialTable(_currentGraphics, y);
        y = DrawConclusion(_currentGraphics, y);
        y = DrawRecommendations(_currentGraphics, y);
        DrawFooter(_currentGraphics, y);

        _currentGraphics.Dispose();
        doc.Save(outputPath);
        return outputPath;
    }

    private void EnsureSpace(ref XGraphics gfx, ref double y, double requiredHeight)
    {
        if (y + requiredHeight > _pageHeight - PageBottomMargin)
        {
            // Start a new page
            gfx.Dispose();
            _currentPage = _currentDocument!.AddPage();
            gfx = XGraphics.FromPdfPage(_currentPage);
            _currentGraphics = gfx;
            y = PageTopMargin;
        }
    }

    private double DrawLetterhead(XGraphics gfx, double y)
    {
        if (!_doc.ShowLetterhead) return y;
        return _doc.LogoPlacement == LogoPlacement.Side
            ? DrawLetterheadSide(gfx, y)
            : DrawLetterheadTop(gfx, y);
    }

    private double DrawLetterheadTop(XGraphics gfx, double y)
    {
        if (_doc.HasLogo)
        {
            try
            {
                using var img = XImage.FromFile(_doc.LogoPath);
                double maxW = 300, maxH = 120;
                double scale = Math.Min(maxW / img.PointWidth, maxH / img.PointHeight);
                double w = img.PointWidth * scale;
                double h = img.PointHeight * scale;
                gfx.DrawImage(img, (gfx.PageSize.Width - w) / 2, y, w, h);
                y += h + 6;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to load letterhead logo from '{_doc.LogoPath}': {ex.Message}", ex);
            }
        }

        gfx.DrawString(_doc.InstitutionName, LetterheadTitleFont, XBrushes.Black,
            new XRect(0, y, gfx.PageSize.Width, 18), XStringFormats.TopCenter);
        y += 18;

        if (!string.IsNullOrEmpty(_doc.Department))
        {
            gfx.DrawString(_doc.Department, LetterheadSubtitleFont, XBrushes.Black,
                new XRect(0, y, gfx.PageSize.Width, 14), XStringFormats.TopCenter);
            y += 14;
        }

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            gfx.DrawString(_doc.AddressLine, SmallFont, XBrushes.Gray,
                new XRect(0, y, gfx.PageSize.Width, 12), XStringFormats.TopCenter);
            y += 12;
            gfx.DrawLine(new XPen(XColors.Black, 3), 20, y - 1, gfx.PageSize.Width - 20, y - 1);
            y += 4;
        }
        else
        {
            gfx.DrawLine(new XPen(XColors.Black, 3), 20, y + 2, gfx.PageSize.Width - 20, y + 2);
            y += 6;
        }
        return y;
    }

    private double DrawLetterheadSide(XGraphics gfx, double y)
    {
        double logoWidth = 0, logoHeight = 0, logoX = 0;
        if (_doc.HasLogo)
        {
            try
            {
                using var img = XImage.FromFile(_doc.LogoPath);
                double maxDim = 60;
                double scale = Math.Min(maxDim / img.PointWidth, maxDim / img.PointHeight);
                logoWidth = img.PointWidth * scale;
                logoHeight = img.PointHeight * scale;
                logoX = 40;
                gfx.DrawImage(img, logoX, y + 2, logoWidth, logoHeight);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to load letterhead logo from '{_doc.LogoPath}': {ex.Message}", ex);
            }
        }

        double textX = (logoWidth > 0) ? logoX + logoWidth + 10 : 40;
        double textWidth = gfx.PageSize.Width - textX - 40;
        double lineHeight = 0;

        gfx.DrawString(_doc.InstitutionName, LetterheadTitleFont, XBrushes.Black,
            new XRect(textX, y, textWidth, 18), XStringFormats.TopLeft);
        lineHeight = 18;

        if (!string.IsNullOrEmpty(_doc.Department))
        {
            gfx.DrawString(_doc.Department, LetterheadSubtitleFont, XBrushes.Black,
                new XRect(textX, y + lineHeight, textWidth, 14), XStringFormats.TopLeft);
            lineHeight += 14;
        }

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            gfx.DrawString(_doc.AddressLine, SmallFont, XBrushes.Gray,
                new XRect(textX, y + lineHeight, textWidth, 12), XStringFormats.TopLeft);
            lineHeight += 12;
        }

        y += Math.Max(logoHeight, lineHeight) + 4;
        gfx.DrawLine(new XPen(XColors.Black, 3), 20, y, gfx.PageSize.Width - 20, y);
        y += 6;
        return y;
    }

    private double DrawReportTitle(XGraphics gfx, double y)
    {
        y += 8;
        gfx.DrawString(_doc.ReportTitle, TitleFont, XBrushes.Black,
            new XRect(0, y, gfx.PageSize.Width, 22), XStringFormats.TopCenter);
        y += 22;
        gfx.DrawString(_doc.Subtitle, SubtitleFont, XBrushes.Black,
            new XRect(0, y, gfx.PageSize.Width, 16), XStringFormats.TopCenter);
        y += 20;
        gfx.DrawLine(XPens.Black, 20, y, gfx.PageSize.Width - 20, y);
        y += 6;
        return y;
    }

    private double DrawPatientInfo(XGraphics gfx, double y)
    {
        if (!_doc.ShowPatientInfo) return y;

        y += 2;
        gfx.DrawString(_doc.PatientInfoSectionLabel, HeaderFont, XBrushes.Black,
            new XRect(20, y, gfx.PageSize.Width - 40, 14), XStringFormats.TopLeft);
        y += 14;

        var pairs = new List<(string, string, string, string)>();
        for (int i = 0; i < _doc.PatientFields.Count; i += 2)
        {
            var f1 = _doc.PatientFields[i];
            var f2 = i + 1 < _doc.PatientFields.Count ? _doc.PatientFields[i + 1] : null;
            pairs.Add((f1.Label, f1.Value, f2?.Label ?? "", f2?.Value ?? ""));
        }

        double left = 20;
        double mid = gfx.PageSize.Width / 2;

        foreach (var (l1, v1, l2, v2) in pairs)
        {
            gfx.DrawString($"{l1} {v1}", BodyFont, XBrushes.Black,
                new XRect(left, y, mid - left, 12), XStringFormats.TopLeft);
            if (!string.IsNullOrEmpty(l2))
                gfx.DrawString($"{l2} {v2}", BodyFont, XBrushes.Black,
                    new XRect(mid, y, gfx.PageSize.Width - mid - 20, 12), XStringFormats.TopLeft);
            y += 12;
        }

        y += 4;
        gfx.DrawLine(XPens.Black, 20, y, gfx.PageSize.Width - 20, y);
        y += 6;
        return y;
    }

    private double DrawDifferentialTable(XGraphics gfx, double y)
    {
        if (_doc.Rows.Count == 0) return y;

        double tableLeft = 20;
        double tableW = gfx.PageSize.Width - 40;
        double rowH = 16;

        int colCount = _doc.ShowReferenceRanges ? 4 : 3;
        double[] colWidths = _doc.ShowReferenceRanges
            ? [tableW * 0.40, tableW * 0.18, tableW * 0.18, tableW * 0.24]
            : [tableW * 0.50, tableW * 0.25, tableW * 0.25];
        string[] headers = _doc.ShowReferenceRanges
            ? [_doc.ColCellType, _doc.ColCount, _doc.ColPercent, _doc.ColRefRange]
            : [_doc.ColCellType, _doc.ColCount, _doc.ColPercent];

        double[] colX = new double[colCount];
        double cx = tableLeft;
        for (int i = 0; i < colCount; i++) { colX[i] = cx; cx += colWidths[i]; }

        // Draw header row
        EnsureSpace(ref gfx, ref y, rowH);
        gfx.DrawRectangle(XBrushes.LightSteelBlue, tableLeft, y, tableW, rowH);
        for (int i = 0; i < colCount; i++)
        {
            if (i > 0) gfx.DrawLine(XPens.White, colX[i], y, colX[i], y + rowH);
            gfx.DrawString(headers[i], TableHeaderFont, XBrushes.Black,
                new XRect(colX[i], y, colWidths[i], rowH), XStringFormats.Center);
        }
        y += rowH;

        // Draw data rows
        foreach (var row in _doc.Rows)
        {
            EnsureSpace(ref gfx, ref y, rowH);
            gfx.DrawRectangle(XPens.LightGray, XBrushes.White, tableLeft, y, tableW, rowH);
            gfx.DrawString(row.CellName, TableFont, XBrushes.Black,
                new XRect(colX[0] + 4, y, colWidths[0] - 4, rowH), XStringFormats.CenterLeft);
            gfx.DrawString(row.Count.ToString(), TableFont, XBrushes.Black,
                new XRect(colX[1], y, colWidths[1], rowH), XStringFormats.Center);
            gfx.DrawString(row.Percentage.ToString("F1") + "%", TableFont, XBrushes.Black,
                new XRect(colX[2], y, colWidths[2], rowH), XStringFormats.Center);
            if (_doc.ShowReferenceRanges)
                gfx.DrawString(row.ReferenceRange, TableFont, XBrushes.DimGray,
                    new XRect(colX[3], y, colWidths[3], rowH), XStringFormats.Center);
            gfx.DrawLine(XPens.LightGray, tableLeft, y + rowH, tableLeft + tableW, y + rowH);
            y += rowH;
        }

        // Draw total row
        EnsureSpace(ref gfx, ref y, rowH);
        gfx.DrawRectangle(new XPen(XColors.Black, 2), XBrushes.LightSteelBlue, tableLeft, y, tableW, rowH);
        gfx.DrawString(_doc.TotalLabel, TableHeaderFont, XBrushes.Black,
            new XRect(colX[0] + 4, y, colWidths[0] - 4, rowH), XStringFormats.CenterLeft);
        gfx.DrawString(_doc.TotalCount.ToString(), TableFont, XBrushes.Black,
            new XRect(colX[1], y, colWidths[1], rowH), XStringFormats.Center);
        gfx.DrawString("100%", TableFont, XBrushes.Black,
            new XRect(colX[2], y, colWidths[2], rowH), XStringFormats.Center);
        y += rowH + 6;
        return y;
    }

    private double DrawConclusion(XGraphics gfx, double y)
    {
        if (!_doc.ShowConclusion) return y;

        // Measure wrapped text height
        var textFormatter = new XTextFormatter(gfx);
        var availableWidth = gfx.PageSize.Width - 40;
        var measureRect = new XRect(20, 0, availableWidth, double.MaxValue);
        var textSize = gfx.MeasureString(_doc.ConclusionText, TableFont);
        var estimatedLines = Math.Ceiling(textSize.Width / availableWidth);
        var wrappedHeight = estimatedLines * (TableFont.Height + 2);
        var requiredHeight = 14 + wrappedHeight + 14;

        EnsureSpace(ref gfx, ref y, requiredHeight);

        y += 2;
        gfx.DrawString(_doc.ConclusionSectionLabel, TableHeaderFont, XBrushes.Black,
            new XRect(20, y, availableWidth, 14), XStringFormats.TopLeft);
        y += 14;

        var textRect = new XRect(20, y, availableWidth, wrappedHeight);
        textFormatter.DrawString(_doc.ConclusionText, TableFont, XBrushes.Black, textRect, XStringFormats.TopLeft);
        y += wrappedHeight + 4;

        return y;
    }

    private double DrawRecommendations(XGraphics gfx, double y)
    {
        if (!_doc.ShowRecommendations) return y;

        // Measure wrapped text height
        var textFormatter = new XTextFormatter(gfx);
        var availableWidth = gfx.PageSize.Width - 40;
        var measureRect = new XRect(20, 0, availableWidth, double.MaxValue);
        var textSize = gfx.MeasureString(_doc.RecommendationsText, TableFont);
        var estimatedLines = Math.Ceiling(textSize.Width / availableWidth);
        var wrappedHeight = estimatedLines * (TableFont.Height + 2);
        var requiredHeight = 14 + wrappedHeight + 14;

        EnsureSpace(ref gfx, ref y, requiredHeight);

        y += 2;
        gfx.DrawString(_doc.RecommendationsSectionLabel, TableHeaderFont, XBrushes.Black,
            new XRect(20, y, availableWidth, 14), XStringFormats.TopLeft);
        y += 14;

        var textRect = new XRect(20, y, availableWidth, wrappedHeight);
        textFormatter.DrawString(_doc.RecommendationsText, TableFont, XBrushes.Black, textRect, XStringFormats.TopLeft);
        y += wrappedHeight + 4;

        return y;
    }

    private void DrawFooter(XGraphics gfx, double y)
    {
        if (!_doc.ShowFooter) return;

        var footerHeight = 6 + 4 + 10 + 10 + 4 + 12 + 10;
        EnsureSpace(ref gfx, ref y, footerHeight);

        y += 6;
        gfx.DrawLine(XPens.Black, 20, y, gfx.PageSize.Width - 20, y);
        y += 4;
        gfx.DrawString(_doc.GeneratedAt, TableFont, XBrushes.Gray,
            new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
        y += 10;
        if (!string.IsNullOrEmpty(_doc.FooterText))
        {
            gfx.DrawString(_doc.FooterText, TableFont, XBrushes.Gray,
                new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
            y += 10;
        }
        y += 4;
        gfx.DrawString("___________________________________", TableFont, XBrushes.Black,
            new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
        y += 12;
        gfx.DrawString(_doc.SignatureLabel, TableFont, XBrushes.Black,
            new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
    }
}
