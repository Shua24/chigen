using PdfSharp.Drawing;

namespace Chigen.DocumentExport;

/// <summary>
/// Loads report images and computes the size they should be rendered at.
/// </summary>
internal static class ImageMetrics
{
    /// <summary>Maximum width of the letterhead logo in the centered (top) layout, in points.</summary>
    public const double LogoMaxWidth = 300;

    /// <summary>Maximum height of the letterhead logo in the centered (top) layout, in points.</summary>
    public const double LogoMaxHeight = 120;

    /// <summary>Maximum width/height of the letterhead logo in the side layout, in points.</summary>
    public const double SideLogoMaxDimension = 60;

    /// <summary>EMU (English Metric Units) per point, used by the DOCX generator for image extents.</summary>
    public const double EmuPerPoint = 12700;

    /// <summary>
    /// Loads the image at <paramref name="path"/> and returns it scaled to fit within
    /// <paramref name="maxWidth"/> by <paramref name="maxHeight"/> points, preserving aspect ratio.
    /// When <paramref name="clampAtOriginalSize"/> is true, images smaller than the bounding
    /// box are never scaled up.
    /// </summary>
    public static ScaledImage LoadScaled(string path, double maxWidth, double maxHeight, bool clampAtOriginalSize = false)
    {
        try
        {
            var image = XImage.FromFile(path);
            double scale = Math.Min(maxWidth / image.PointWidth, maxHeight / image.PointHeight);
            if (clampAtOriginalSize && scale > 1) scale = 1;
            return new ScaledImage(image, image.PointWidth * scale, image.PointHeight * scale);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load letterhead logo from '{path}': {ex.Message}", ex);
        }
    }
}

/// <summary>An image together with the size it should be rendered at.</summary>
internal sealed class ScaledImage : IDisposable
{
    public XImage Image { get; }
    public double Width { get; }
    public double Height { get; }

    public ScaledImage(XImage image, double width, double height)
    {
        Image = image;
        Width = width;
        Height = height;
    }

    public void Dispose() => Image.Dispose();
}
