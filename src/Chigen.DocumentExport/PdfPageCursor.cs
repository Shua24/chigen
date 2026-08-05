using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Chigen.DocumentExport;

/// <summary>
/// Tracks the vertical writing position and current page of a PDF under construction,
/// and transparently starts a new page when content would overflow the bottom margin.
/// </summary>
internal sealed class PdfPageCursor
{
    public const double TopMargin = 20;
    public const double BottomMargin = 60;
    public const double LeftMargin = 20;
    public const double RightMargin = 20;

    private readonly PdfDocument _document;
    private PdfPage _page;
    private XGraphics _graphics;
    private double _y;

    public PdfPageCursor(PdfDocument document)
    {
        _document = document;
        _page = document.AddPage();
        _graphics = XGraphics.FromPdfPage(_page);
        _y = TopMargin;
    }

    /// <summary>The graphics context of the current page.</summary>
    public XGraphics Graphics => _graphics;

    /// <summary>The current vertical writing position, in points from the top of the page.</summary>
    public double Y => _y;

    /// <summary>Width of the current page, in points.</summary>
    public double PageWidth => _page.Width.Point;

    /// <summary>Height of the current page, in points.</summary>
    public double PageHeight => _page.Height.Point;

    /// <summary>The x-coordinate where the content area begins.</summary>
    public double ContentLeft => LeftMargin;

    /// <summary>The width of the area between the left and right margins.</summary>
    public double ContentWidth => PageWidth - LeftMargin - RightMargin;

    /// <summary>
    /// Starts a new page if the remaining space cannot fit <paramref name="requiredHeight"/>
    /// points of content, resetting the cursor to the top margin.
    /// </summary>
    public void EnsureSpace(double requiredHeight)
    {
        if (_y + requiredHeight <= PageHeight - BottomMargin) return;
        _graphics.Dispose();
        _page = _document.AddPage();
        _graphics = XGraphics.FromPdfPage(_page);
        _y = TopMargin;
    }

    /// <summary>Moves the cursor down by <paramref name="height"/> points.</summary>
    public void Advance(double height) => _y += height;

    /// <summary>
    /// Draws <paramref name="text"/> centered across the full page width at the current
    /// cursor position and advances the cursor by <paramref name="height"/> points.
    /// </summary>
    public void WriteCentered(string text, XFont font, XBrush brush, double height)
    {
        Graphics.DrawString(text, font, brush, new XRect(0, _y, PageWidth, height), XStringFormats.TopCenter);
        _y += height;
    }

    /// <summary>
    /// Draws <paramref name="text"/> left-aligned within the content area at the current
    /// cursor position and advances the cursor by <paramref name="height"/> points.
    /// </summary>
    public void WriteLeft(string text, XFont font, XBrush brush, double height)
    {
        Graphics.DrawString(text, font, brush, new XRect(ContentLeft, _y, ContentWidth, height), XStringFormats.TopLeft);
        _y += height;
    }

    /// <summary>Draws a horizontal rule across the content area at the given y-position.</summary>
    public void DrawHLine(XPen pen, double y)
        => Graphics.DrawLine(pen, ContentLeft, y, PageWidth - RightMargin, y);

    /// <summary>Releases the current page's graphics context.</summary>
    public void Finish() => _graphics.Dispose();
}
