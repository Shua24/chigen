using PdfSharp.Fonts;

namespace Chigen.DocumentExport;

public sealed class ChigenFontResolver : IFontResolver
{
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Map Calibri to Arial
        if (familyName.Equals("Calibri", StringComparison.OrdinalIgnoreCase))
            return new FontResolverInfo("Arial", isBold, isItalic);

        return new FontResolverInfo(familyName, isBold, isItalic);
    }

    public byte[] GetFont(string faceName)
    {
        var fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        var file = faceName.Replace(" ", "") switch
        {
            "Arial" => Path.Combine(fontsDir, "arial.ttf"),
            "ArialBold" => Path.Combine(fontsDir, "arialbd.ttf"),
            "ArialItalic" => Path.Combine(fontsDir, "ariali.ttf"),
            "ArialBoldItalic" => Path.Combine(fontsDir, "arialbi.ttf"),
            _ => null
        };

        if (file != null && File.Exists(file))
            return File.ReadAllBytes(file);

        throw new InvalidOperationException($"Font '{faceName}' not found.");
    }
}
