using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Chigen.Core.Models;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;

namespace Chigen.DocumentExport;

/// <summary>
/// Generates a DOCX rendering of a <see cref="ReportDocument"/> using OpenXML.
/// </summary>
public class DocxGenerator
{
    private const int NameCellWidthDxa = 2500;
    private const int CountCellWidthDxa = 1000;
    private const int PercentCellWidthDxa = 1000;
    private const int ReferenceRangeCellWidthDxa = 1500;

    private const int LetterheadBorderSize = 24;
    private const int FooterBorderSize = 4;

    private readonly ReportDocument _doc;

    /// <summary>Creates a generator for the given report.</summary>
    public DocxGenerator(ReportDocument doc) => _doc = doc;

    /// <summary>Renders the report to a DOCX file at <paramref name="outputPath"/> and returns the path.</summary>
    public string Create(string outputPath)
    {
        using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = new Body();

        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = new Styles(
            new DocDefaults(
                new ParagraphPropertiesDefault(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
                    )
                )
            )
        );

        AddLetterhead(mainPart, body);
        AddReportTitle(body);
        AddPatientInfo(body);
        AddDifferentialTable(body);
        AddConclusion(body);
        AddRecommendations(body);
        AddFooter(body);

        mainPart.Document.Body = body;
        mainPart.Document.Save();
        return outputPath;
    }

    private void AddLetterhead(MainDocumentPart mainPart, Body body)
    {
        if (!_doc.ShowLetterhead) return;
        if (_doc.LogoPlacement == LogoPlacement.Side)
            AddLetterheadSide(mainPart, body);
        else
            AddLetterheadTop(mainPart, body);
    }

    private void AddLetterheadTop(MainDocumentPart mainPart, Body body)
    {
        AddLetterheadLogoParagraph(mainPart, body, JustificationValues.Center);

        body.Append(TextParagraph(_doc.InstitutionName, Typography.LetterheadTitlePt, justification: JustificationValues.Center, spacingAfter: 0));

        if (!string.IsNullOrEmpty(_doc.Department))
            body.Append(TextParagraph(_doc.Department, Typography.LetterheadSubtitlePt, justification: JustificationValues.Center, spacingAfter: 0));

        if (!string.IsNullOrEmpty(_doc.AddressLine))
            body.Append(TextParagraph(_doc.AddressLine, Typography.AddressPt, justification: JustificationValues.Center, spacingAfter: 20, bottomBorderSize: LetterheadBorderSize));
        else
            body.Append(TextParagraph("", spacingAfter: 20, bottomBorderSize: LetterheadBorderSize));
    }

    private void AddLetterheadSide(MainDocumentPart mainPart, Body body)
    {
        var table = new Table();
        table.Append(new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.None },
                new BottomBorder { Val = BorderValues.None },
                new LeftBorder { Val = BorderValues.None },
                new RightBorder { Val = BorderValues.None },
                new InsideHorizontalBorder { Val = BorderValues.None },
                new InsideVerticalBorder { Val = BorderValues.None }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        ));

        var row = new TableRow();
        row.Append(new TableRowProperties(new TableRowHeight { Val = 1, HeightType = HeightRuleValues.Auto }));

        var logo = AddLetterheadLogoPart(mainPart);

        var logoCell = new TableCell();
        logoCell.Append(new TableCellProperties(
            new TableCellWidth { Width = "2000", Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        ));

        if (logo.RelationshipId != null)
        {
            var imageParagraph = new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center })
            );
            var run = new Run();
            run.Append(BuildInlineDrawing(logo.RelationshipId, logo.EmuW, logo.EmuH));
            imageParagraph.Append(run);
            logoCell.Append(imageParagraph);
        }
        row.Append(logoCell);

        var textCell = new TableCell();
        textCell.Append(new TableCellProperties(
            new TableCellWidth { Width = "5500", Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        ));

        textCell.Append(TextParagraph(_doc.InstitutionName, Typography.LetterheadTitlePt, spacingAfter: 0));
        if (!string.IsNullOrEmpty(_doc.Department))
            textCell.Append(TextParagraph(_doc.Department, Typography.LetterheadSubtitlePt, spacingAfter: 0));
        if (!string.IsNullOrEmpty(_doc.AddressLine))
            textCell.Append(TextParagraph(_doc.AddressLine, Typography.AddressPt, spacingAfter: 0));

        row.Append(textCell);
        table.Append(row);
        body.Append(table);

        body.Append(TextParagraph("", spacingAfter: 20, bottomBorderSize: LetterheadBorderSize));
    }

    private void AddReportTitle(Body body)
    {
        body.Append(TextParagraph(_doc.ReportTitle, justification: JustificationValues.Center, spacingAfter: 200));
    }

    private void AddPatientInfo(Body body)
    {
        if (!_doc.ShowPatientInfo) return;

        body.Append(TextParagraph(_doc.PatientInfoSectionLabel, Typography.DocxSectionHeaderPt, bold: true));

        var table = CreateBorderedTable();
        for (int i = 0; i < _doc.PatientFields.Count; i += 2)
        {
            var f1 = _doc.PatientFields[i];
            var f2 = i + 1 < _doc.PatientFields.Count ? _doc.PatientFields[i + 1] : null;

            var row = new TableRow();
            row.Append(CreateCell($"{f1.Label} {f1.Value}", false, NameCellWidthDxa));
            row.Append(CreateCell(f2 != null ? $"{f2.Label} {f2.Value}" : "", false, NameCellWidthDxa));
            table.Append(row);
        }

        body.Append(EmptyParagraph());
        body.Append(table);
    }

    private void AddDifferentialTable(Body body)
    {
        if (_doc.Rows.Count == 0) return;

        var table = CreateBorderedTable();

        var headerRow = new TableRow();
        headerRow.Append(CreateHeaderCell(_doc.ColCellType));
        headerRow.Append(CreateHeaderCell(_doc.ColCount));
        headerRow.Append(CreateHeaderCell(_doc.ColPercent));
        if (_doc.ShowReferenceRanges)
            headerRow.Append(CreateHeaderCell(_doc.ColRefRange));
        table.Append(headerRow);

        foreach (var row in _doc.Rows)
        {
            var dataRow = new TableRow();
            dataRow.Append(CreateCell(row.CellName, false, NameCellWidthDxa));
            dataRow.Append(CreateCell(row.Count.ToString(), false, CountCellWidthDxa));
            dataRow.Append(CreateCell(row.Percentage.ToString("F1") + "%", false, PercentCellWidthDxa));
            if (_doc.ShowReferenceRanges)
                dataRow.Append(CreateCell(row.ReferenceRange, false, ReferenceRangeCellWidthDxa));
            table.Append(dataRow);
        }

        var totalRow = new TableRow();
        totalRow.Append(CreateCell(_doc.TotalLabel, true, NameCellWidthDxa));
        totalRow.Append(CreateCell(_doc.TotalCount.ToString(), true, CountCellWidthDxa));
        totalRow.Append(CreateCell("100%", true, PercentCellWidthDxa));
        if (_doc.ShowReferenceRanges)
            totalRow.Append(CreateCell("", false, ReferenceRangeCellWidthDxa));
        table.Append(totalRow);

        body.Append(EmptyParagraph());
        body.Append(table);
    }

    private void AddConclusion(Body body)
    {
        if (!_doc.ShowConclusion) return;

        body.Append(EmptyParagraph());
        body.Append(TextParagraph(_doc.ConclusionSectionLabel, Typography.DocxTablePt, bold: true));
        body.Append(TextParagraph(_doc.ConclusionText, Typography.DocxTablePt, spacingBefore: 20, spacingAfter: 20));
    }

    private void AddRecommendations(Body body)
    {
        if (!_doc.ShowRecommendations) return;

        body.Append(TextParagraph(_doc.RecommendationsSectionLabel, Typography.DocxTablePt, bold: true));
        body.Append(TextParagraph(_doc.RecommendationsText, Typography.DocxTablePt, spacingBefore: 20, spacingAfter: 20));
    }

    private void AddFooter(Body body)
    {
        if (!_doc.ShowFooter) return;

        body.Append(EmptyParagraph());

        body.Append(TextParagraph(_doc.GeneratedAt, Typography.FooterPt, topBorderSize: FooterBorderSize));

        if (!string.IsNullOrEmpty(_doc.FooterText))
            body.Append(TextParagraph(_doc.FooterText, Typography.FooterPt));

        body.Append(TextParagraph("___________________________________", Typography.FooterPt, spacingBefore: 0, spacingAfter: 0));
        body.Append(TextParagraph(_doc.SignatureLabel, Typography.FooterPt, spacingBefore: 0, spacingAfter: 0));
    }

    // --- Builders ---

    private readonly record struct LogoPart(string? RelationshipId, long EmuW, long EmuH);

    private static Paragraph EmptyParagraph()
        => new(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve }));

    private static Table CreateBorderedTable()
    {
        var table = new Table();
        table.Append(new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new RightBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Space = 0 }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        ));
        return table;
    }

    private static Drawing BuildInlineDrawing(string relationshipId, long emuW, long emuH)
    {
        var drawing = new Drawing();
        drawing.Append(new DW.Inline
        {
            DistanceFromTop = 0, DistanceFromBottom = 0,
            DistanceFromLeft = 0, DistanceFromRight = 0,
            Extent = new DW.Extent { Cx = emuW, Cy = emuH },
            EffectExtent = new DW.EffectExtent { TopEdge = 0, LeftEdge = 0, BottomEdge = 0, RightEdge = 0 },
            DocProperties = new DW.DocProperties { Id = 1, Name = "Logo" },
            NonVisualGraphicFrameDrawingProperties = new DW.NonVisualGraphicFrameDrawingProperties(
                new A.GraphicFrameLocks { NoChangeAspect = true }
            ),
            Graphic = new A.Graphic(
                new A.GraphicData(
                    new A.Pictures.Picture(
                        new A.Pictures.NonVisualPictureProperties(
                            new A.Pictures.NonVisualDrawingProperties { Id = 0, Name = "Logo.png" },
                            new A.Pictures.NonVisualPictureDrawingProperties()
                        ),
                        new A.Pictures.BlipFill(
                            new A.Blip { Embed = relationshipId, CompressionState = A.BlipCompressionValues.Print },
                            new A.Stretch(new A.FillRectangle())
                        ),
                        new A.Pictures.ShapeProperties(
                            new A.Transform2D(
                                new A.Offset { X = 0, Y = 0 },
                                new A.Extents { Cx = emuW, Cy = emuH }
                            ),
                            new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                        )
                    )
                )
                { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
            )
        });
        return drawing;
    }

    private static TableCell CreateCell(string text, bool bold, int width)
    {
        var props = new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        );
        return new TableCell(props, TextParagraph(text, Typography.DocxTablePt, bold: bold, spacingBefore: 20, spacingAfter: 20));
    }

    private static TableCell CreateHeaderCell(string text)
    {
        var props = new TableCellProperties(
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new Shading { Val = ShadingPatternValues.Clear, Fill = "D9E2F3" }
        );
        return new TableCell(
            props,
            TextParagraph(text, Typography.DocxTablePt, bold: true, justification: JustificationValues.Center, spacingBefore: 20, spacingAfter: 20)
        );
    }

    private static Paragraph TextParagraph(
        string text,
        int? fontSizePt = null,
        bool bold = false,
        JustificationValues justification = default,
        int? spacingBefore = null,
        int? spacingAfter = null,
        int topBorderSize = 0,
        int bottomBorderSize = 0)
    {
        var runProps = new RunProperties();
        if (fontSizePt.HasValue)
            runProps.Append(new FontSize { Val = Typography.HalfPoints(fontSizePt.Value) });
        if (bold)
            runProps.Append(new Bold());

        var paraProps = new ParagraphProperties();
        if (justification != JustificationValues.Left)
            paraProps.Append(new Justification { Val = justification });
        if (spacingBefore.HasValue || spacingAfter.HasValue)
            paraProps.Append(new SpacingBetweenLines
            {
                Before = spacingBefore?.ToString(),
                After = spacingAfter?.ToString()
            });
        if (topBorderSize > 0)
            paraProps.Append(new ParagraphBorders(new TopBorder { Val = BorderValues.Single, Size = (uint)topBorderSize, Space = 1 }));
        if (bottomBorderSize > 0)
            paraProps.Append(new ParagraphBorders(new BottomBorder { Val = BorderValues.Single, Size = (uint)bottomBorderSize, Space = 1 }));

        var run = string.IsNullOrEmpty(text) ? null : new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve });

        var hasParagraphProps = paraProps.ChildElements.Count > 0;

        if (run == null && !hasParagraphProps)
            return new Paragraph();
        if (run == null)
            return new Paragraph(paraProps);
        if (!hasParagraphProps)
            return new Paragraph(run);
        return new Paragraph(paraProps, run);
    }

    private LogoPart AddLetterheadLogoPart(MainDocumentPart mainPart)
    {
        if (!_doc.HasLogo) return new LogoPart(null, 0, 0);

        try
        {
            var imagePart = mainPart.AddImagePart(ImageTypeFor(_doc.LogoPath));
            using (var stream = File.OpenRead(_doc.LogoPath))
                imagePart.FeedData(stream);
            var relationshipId = mainPart.GetIdOfPart(imagePart);

            using var logo = ImageMetrics.LoadScaled(_doc.LogoPath, ImageMetrics.LogoMaxWidth, ImageMetrics.LogoMaxHeight, clampAtOriginalSize: true);
            return new LogoPart(
                relationshipId,
                (long)(logo.Width * ImageMetrics.EmuPerPoint),
                (long)(logo.Height * ImageMetrics.EmuPerPoint));
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"Failed to load letterhead logo from '{_doc.LogoPath}': {ex.Message}", ex);
        }
    }

    private void AddLetterheadLogoParagraph(MainDocumentPart mainPart, Body body, JustificationValues justification)
    {
        if (!_doc.HasLogo) return;

        var logo = AddLetterheadLogoPart(mainPart);
        if (logo.RelationshipId == null) return;

        var imageParagraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = justification },
                new SpacingBetweenLines { After = "40" }
            )
        );
        var run = new Run();
        run.Append(BuildInlineDrawing(logo.RelationshipId, logo.EmuW, logo.EmuH));
        imageParagraph.Append(run);
        body.Append(imageParagraph);
    }

    private static PartTypeInfo ImageTypeFor(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".png" => ImagePartType.Png,
            ".gif" => ImagePartType.Gif,
            ".bmp" => ImagePartType.Bmp,
            ".tiff" or ".tif" => ImagePartType.Tiff,
            ".ico" => ImagePartType.Icon,
            ".svg" => ImagePartType.Svg,
            _ => ImagePartType.Jpeg,
        };
    }
}
