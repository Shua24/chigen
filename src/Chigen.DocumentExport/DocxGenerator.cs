using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Chigen.Core.Models;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;

namespace Chigen.DocumentExport;

public class DocxGenerator
{
    private readonly ReportDocument _doc;

    public DocxGenerator(ReportDocument doc) => _doc = doc;

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
        AddLetterheadImage(mainPart, body, JustificationValues.Center, null);

        body.Append(new Paragraph(
            new Run(
                new RunProperties(new FontSize { Val = "24" }),
                new Text(_doc.InstitutionName) { Space = SpaceProcessingModeValues.Preserve }
            )
        )
        {
            ParagraphProperties = new ParagraphProperties(
                new Justification { Val = JustificationValues.Center },
                new SpacingBetweenLines { After = "0" }
            )
        });

        if (!string.IsNullOrEmpty(_doc.Department))
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "20" }),
                    new Text(_doc.Department) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "0" }
                )
            });
        }

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "18" }),
                    new Text(_doc.AddressLine) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new ParagraphBorders(
                        new BottomBorder { Val = BorderValues.Single, Size = 24, Space = 1 }
                    ),
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "20" }
                )
            });
        }
        else
        {
            body.Append(new Paragraph(
                new ParagraphProperties(
                    new ParagraphBorders(
                        new BottomBorder { Val = BorderValues.Single, Size = 24, Space = 1 }
                    ),
                    new SpacingBetweenLines { After = "20" }
                )
            ));
        }
    }

    private void AddLetterheadSide(MainDocumentPart mainPart, Body body)
    {
        var table = new Table();
        var tblProps = new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.None },
                new BottomBorder { Val = BorderValues.None },
                new LeftBorder { Val = BorderValues.None },
                new RightBorder { Val = BorderValues.None },
                new InsideHorizontalBorder { Val = BorderValues.None },
                new InsideVerticalBorder { Val = BorderValues.None }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        );
        table.Append(tblProps);

        var row = new TableRow();
        row.Append(new TableRowProperties(new TableRowHeight { Val = 1, HeightType = HeightRuleValues.Auto }));

        long imageEmuW = 0, imageEmuH = 0;
        string? imageRelationshipId = AddLetterheadImagePart(mainPart, out imageEmuW, out imageEmuH);

        var logoCell = new TableCell();
        logoCell.Append(new TableCellProperties(
            new TableCellWidth { Width = "2000", Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        ));

        if (imageRelationshipId != null)
        {
            var imageParagraph = new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center })
            );
            var run = new Run();
            var drawing = new Drawing();
            var inline = new DW.Inline
            {
                DistanceFromTop = 0, DistanceFromBottom = 0,
                DistanceFromLeft = 0, DistanceFromRight = 0,
                Extent = new DW.Extent { Cx = imageEmuW, Cy = imageEmuH },
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
                                new A.Blip { Embed = imageRelationshipId, CompressionState = A.BlipCompressionValues.Print },
                                new A.Stretch(new A.FillRectangle())
                            ),
                            new A.Pictures.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0, Y = 0 },
                                    new A.Extents { Cx = imageEmuW, Cy = imageEmuH }
                                ),
                                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                            )
                        )
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                )
            };
            drawing.Append(inline);
            run.Append(drawing);
            imageParagraph.Append(run);
            logoCell.Append(imageParagraph);
        }
        row.Append(logoCell);

        var textCell = new TableCell();
        textCell.Append(new TableCellProperties(
            new TableCellWidth { Width = "5500", Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        ));

        textCell.Append(new Paragraph(
            new Run(
                new RunProperties(new FontSize { Val = "24" }),
                new Text(_doc.InstitutionName) { Space = SpaceProcessingModeValues.Preserve }
            )
        )
        {
            ParagraphProperties = new ParagraphProperties(
                new Justification { Val = JustificationValues.Left },
                new SpacingBetweenLines { After = "0" }
            )
        });

        if (!string.IsNullOrEmpty(_doc.Department))
        {
            textCell.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "20" }),
                    new Text(_doc.Department) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Left },
                    new SpacingBetweenLines { After = "0" }
                )
            });
        }

        if (!string.IsNullOrEmpty(_doc.AddressLine))
        {
            textCell.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "18" }),
                    new Text(_doc.AddressLine) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Left },
                    new SpacingBetweenLines { After = "0" }
                )
            });
        }

        row.Append(textCell);
        table.Append(row);
        body.Append(table);

        body.Append(new Paragraph(
            new ParagraphProperties(
                new ParagraphBorders(
                    new BottomBorder { Val = BorderValues.Single, Size = 24, Space = 1 }
                ),
                new SpacingBetweenLines { After = "20" }
            )
        ));
    }

    private string? AddLetterheadImagePart(MainDocumentPart mainPart, out long emuW, out long emuH)
    {
        emuW = 0; emuH = 0;
        if (!_doc.HasLogo) return null;

        try
        {
            var ext = Path.GetExtension(_doc.LogoPath).ToLowerInvariant();
            var imageType = ext switch
            {
                ".png" => ImagePartType.Png,
                ".gif" => ImagePartType.Gif,
                ".bmp" => ImagePartType.Bmp,
                ".tiff" or ".tif" => ImagePartType.Tiff,
                ".ico" => ImagePartType.Icon,
                ".svg" => ImagePartType.Svg,
                _ => ImagePartType.Jpeg,
            };

            var imagePart = mainPart.AddImagePart(imageType);
            using var stream = File.OpenRead(_doc.LogoPath);
            imagePart.FeedData(stream);
            var relationshipId = mainPart.GetIdOfPart(imagePart);

            using var img = PdfSharp.Drawing.XImage.FromFile(_doc.LogoPath);
            double maxW = 300, maxH = 120;
            double scale = Math.Min(maxW / img.PointWidth, maxH / img.PointHeight);
            if (scale > 1) scale = 1;
            emuW = (long)(img.PointWidth * scale * 12700);
            emuH = (long)(img.PointHeight * scale * 12700);

            return relationshipId;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load letterhead logo from '{_doc.LogoPath}': {ex.Message}", ex);
        }
    }

    private void AddLetterheadImage(MainDocumentPart mainPart, Body body, JustificationValues justification, string? existingRelationshipId)
    {
        if (!_doc.HasLogo) return;

        string? imageRelationshipId = existingRelationshipId;
        long emuW = 0, emuH = 0;

        if (imageRelationshipId == null)
            imageRelationshipId = AddLetterheadImagePart(mainPart, out emuW, out emuH);

        if (imageRelationshipId == null) return;

        var imageParagraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = justification },
                new SpacingBetweenLines { After = "40" }
            )
        );
        var run = new Run();
        var drawing = new Drawing();
        var inline = new DW.Inline
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
                            new A.Blip { Embed = imageRelationshipId, CompressionState = A.BlipCompressionValues.Print },
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
        };
        drawing.Append(inline);
        run.Append(drawing);
        imageParagraph.Append(run);
        body.Append(imageParagraph);
    }

    private void AddReportTitle(Body body)
    {
        body.Append(new Paragraph(
            new Run(new Text(_doc.ReportTitle) { Space = SpaceProcessingModeValues.Preserve })
        )
        {
            ParagraphProperties = new ParagraphProperties(
                new Justification { Val = JustificationValues.Center },
                new SpacingBetweenLines { After = "200" }
            )
        });
    }

    private void AddPatientInfo(Body body)
    {
        if (!_doc.ShowPatientInfo) return;

        // Section header
        body.Append(new Paragraph(
            new Run(
                new RunProperties(new Bold(), new FontSize { Val = "22" }),
                new Text(_doc.PatientInfoSectionLabel) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));

        var table = new Table();
        var tblProps = new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new RightBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Space = 0 }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        );
        table.Append(tblProps);

        // Pair fields for two-column layout
        for (int i = 0; i < _doc.PatientFields.Count; i += 2)
        {
            var f1 = _doc.PatientFields[i];
            var f2 = i + 1 < _doc.PatientFields.Count ? _doc.PatientFields[i + 1] : null;

            var row = new TableRow();
            row.Append(CreateCell($"{f1.Label} {f1.Value}", false, 2500));
            row.Append(CreateCell(f2 != null ? $"{f2.Label} {f2.Value}" : "", false, 2500));
            table.Append(row);
        }

        body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));
        body.Append(table);
    }

    private void AddDifferentialTable(Body body)
    {
        if (_doc.Rows.Count == 0) return;

        var table = new Table();
        var tblProps = new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new RightBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Space = 0 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Space = 0 }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        );
        table.Append(tblProps);

        // Header row
        var headerRow = new TableRow();
        headerRow.Append(CreateHeaderCell(_doc.ColCellType));
        headerRow.Append(CreateHeaderCell(_doc.ColCount));
        headerRow.Append(CreateHeaderCell(_doc.ColPercent));
        if (_doc.ShowReferenceRanges)
            headerRow.Append(CreateHeaderCell(_doc.ColRefRange));
        table.Append(headerRow);

        // Data rows
        foreach (var row in _doc.Rows)
        {
            var dataRow = new TableRow();
            dataRow.Append(CreateCell(row.CellName, false, 2500));
            dataRow.Append(CreateCell(row.Count.ToString(), false, 1000));
            dataRow.Append(CreateCell(row.Percentage.ToString("F1") + "%", false, 1000));
            if (_doc.ShowReferenceRanges)
                dataRow.Append(CreateCell(row.ReferenceRange, false, 1500));
            table.Append(dataRow);
        }

        // Total row
        var totalRow = new TableRow();
        totalRow.Append(CreateCell(_doc.TotalLabel, true, 2500));
        totalRow.Append(CreateCell(_doc.TotalCount.ToString(), true, 1000));
        totalRow.Append(CreateCell("100%", true, 1000));
        if (_doc.ShowReferenceRanges)
            totalRow.Append(CreateCell("", false, 1500));
        table.Append(totalRow);

        body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));
        body.Append(table);
    }

    private void AddConclusion(Body body)
    {
        if (!_doc.ShowConclusion) return;

        body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));
        body.Append(new Paragraph(
            new Run(
                new RunProperties(new Bold(), new FontSize { Val = "18" }),
                new Text(_doc.ConclusionSectionLabel) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));
        body.Append(new Paragraph(
            new ParagraphProperties(new SpacingBetweenLines { Before = "20", After = "20" }),
            new Run(
                new RunProperties(new FontSize { Val = "18" }),
                new Text(_doc.ConclusionText) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));
    }

    private void AddRecommendations(Body body)
    {
        if (!_doc.ShowRecommendations) return;

        body.Append(new Paragraph(
            new Run(
                new RunProperties(new Bold(), new FontSize { Val = "18" }),
                new Text(_doc.RecommendationsSectionLabel) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));
        body.Append(new Paragraph(
            new ParagraphProperties(new SpacingBetweenLines { Before = "20", After = "20" }),
            new Run(
                new RunProperties(new FontSize { Val = "18" }),
                new Text(_doc.RecommendationsText) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));
    }

    private void AddFooter(Body body)
    {
        if (!_doc.ShowFooter) return;

        body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));

        var borderProps = new ParagraphProperties(
            new ParagraphBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Space = 1 }
            )
        );

        body.Append(new Paragraph(
            borderProps,
            new Run(
                new RunProperties(new FontSize { Val = "16" }),
                new Text(_doc.GeneratedAt) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));

        if (!string.IsNullOrEmpty(_doc.FooterText))
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "16" }),
                    new Text(_doc.FooterText) { Space = SpaceProcessingModeValues.Preserve }
                )
            ));
        }

        body.Append(new Paragraph(
            new ParagraphProperties(new SpacingBetweenLines { Before = "0", After = "0" }),
            new Run(
                new RunProperties(new FontSize { Val = "16" }),
                new Text("___________________________________") { Space = SpaceProcessingModeValues.Preserve }
            )
        ));

        body.Append(new Paragraph(
            new ParagraphProperties(new SpacingBetweenLines { Before = "0", After = "0" }),
            new Run(
                new RunProperties(new FontSize { Val = "16" }),
                new Text(_doc.SignatureLabel) { Space = SpaceProcessingModeValues.Preserve }
            )
        ));
    }

    private static TableCell CreateCell(string text, bool bold, int width)
    {
        var runProps = new RunProperties(new FontSize { Val = "18" });
        if (bold) runProps.Append(new Bold());

        var props = new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        );

        return new TableCell(
            props,
            new Paragraph(
                new ParagraphProperties(
                    new SpacingBetweenLines { Before = "20", After = "20" }
                ),
                new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve })
            )
        );
    }

    private static TableCell CreateHeaderCell(string text)
    {
        var props = new TableCellProperties(
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new Shading { Val = ShadingPatternValues.Clear, Fill = "D9E2F3" }
        );
        return new TableCell(
            props,
            new Paragraph(
                new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { Before = "20", After = "20" }
                ),
                new Run(
                    new RunProperties(new Bold(), new FontSize { Val = "18" }),
                    new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
        );
    }
}
