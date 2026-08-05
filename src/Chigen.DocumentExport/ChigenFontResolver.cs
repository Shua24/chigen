using PdfSharp.Fonts;

namespace Chigen.DocumentExport;

/// <summary>
/// Maps the report's font family (Calibri) to a font that is actually installed
/// on the system (Arial) so that PdfSharp can render it.
/// </summary>
public sealed class ChigenFontResolver : IFontResolver
{
    private const string Arial = "Arial";
    private const string ArialBold = "ArialBold";
    private const string ArialItalic = "ArialItalic";
    private const string ArialBoldItalic = "ArialBoldItalic";

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Map Calibri to Arial
        if (familyName.Equals(Typography.Family, StringComparison.OrdinalIgnoreCase))
            return new FontResolverInfo(Arial, isBold, isItalic);

        return new FontResolverInfo(familyName, isBold, isItalic);
    }

    public byte[] GetFont(string faceName)
    {
        var fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        var file = faceName.Replace(" ", "") switch
        {
            Arial => Path.Combine(fontsDir, "arial.ttf"),
            ArialBold => Path.Combine(fontsDir, "arialbd.ttf"),
            ArialItalic => Path.Combine(fontsDir, "ariali.ttf"),
            ArialBoldItalic => Path.Combine(fontsDir, "arialbi.ttf"),
            _ => null
        };

        if (file != null && File.Exists(file))
            return File.ReadAllBytes(file);

        throw new InvalidOperationException($"Font '{faceName}' not found.");
    }
}
