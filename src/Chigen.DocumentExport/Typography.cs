namespace Chigen.DocumentExport;

/// <summary>
/// Central catalog of the font sizes used by the PDF and DOCX generators.
/// The PDF generator consumes point sizes directly; the DOCX generator
/// converts them to OpenXML half-points via <see cref="HalfPoints"/>.
/// </summary>
internal static class Typography
{
    /// <summary>The font family used for all report text.</summary>
    public const string Family = "Calibri";

    // Sizes shared by both renderers
    public const int LetterheadTitlePt = 12;
    public const int LetterheadSubtitlePt = 10;
    public const int ReportTitlePt = 16;
    public const int SubtitlePt = 11;
    public const int AddressPt = 9;
    public const int FooterPt = 8;

    // PDF-only roles
    public const int PdfSectionHeaderPt = 10;
    public const int PdfBodyPt = 10;
    public const int PdfTableHeaderPt = 9;
    public const int PdfTablePt = 8;

    // DOCX-only roles (rendered one point larger than their PDF counterparts)
    public const int DocxSectionHeaderPt = 11;
    public const int DocxTablePt = 9;

    /// <summary>Converts a point size to the half-point string used by OpenXML.</summary>
    public static string HalfPoints(int points) => (points * 2).ToString();
}
