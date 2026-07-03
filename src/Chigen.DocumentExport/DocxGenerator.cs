using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Drawing;
using Chigen.Core.Models;
using Chigen.Core.Services;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;

namespace Chigen.DocumentExport
{
    public class DocxGenerator
    {
        private readonly LetterheadConfig _letterhead;
        private readonly DocumentTemplate _template;

        public DocxGenerator(LetterheadConfig letterhead, DocumentTemplate template)
        {
            _letterhead = letterhead;
            _template = template;
        }

        public string Create(string outputPath, CounterState counterState, PatientInfo patient, SpecimenInfo specimen)
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
            AddPatientInfo(body, patient, specimen);
            AddDifferentialTable(body, counterState);
            AddConclusion(body, patient.Conclusion);
            AddRecommendations(body, patient.Recommendations);
            AddFooter(body);

            mainPart.Document.Body = body;
            mainPart.Document.Save();
            return outputPath;
        }

        private void AddLetterhead(MainDocumentPart mainPart, Body body)
        {
            if (!_template.ShowLetterhead) return;

            if (_letterhead.LogoPlacement == Core.Models.LogoPlacement.Side)
            {
                AddLetterheadSide(mainPart, body);
            }
            else
            {
                AddLetterheadTop(mainPart, body);
            }
        }

        private void AddLetterheadTop(MainDocumentPart mainPart, Body body)
        {
            AddLetterheadImage(mainPart, body, JustificationValues.Center, null);

            body.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "24" }),
                    new Text(_letterhead.InstitutionName) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "0" }
                )
            });

            if (!string.IsNullOrEmpty(_letterhead.Department))
            {
                body.Append(new Paragraph(
                    new Run(
                        new RunProperties(new FontSize { Val = "20" }),
                        new Text(_letterhead.Department) { Space = SpaceProcessingModeValues.Preserve }
                    )
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center },
                        new SpacingBetweenLines { After = "0" }
                    )
                });
            }

            var addressLine = string.Join(", ", new[] { _letterhead.Address, _letterhead.Phone, _letterhead.Email }.Where(s => !string.IsNullOrEmpty(s)));
            if (!string.IsNullOrEmpty(addressLine))
            {
                body.Append(new Paragraph(
                    new Run(
                        new RunProperties(new FontSize { Val = "18" }),
                        new Text(addressLine) { Space = SpaceProcessingModeValues.Preserve }
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

            long imageEmuW = 0;
            long imageEmuH = 0;
            string? imageRelationshipId = AddLetterheadImagePart(mainPart, out imageEmuW, out imageEmuH);

            var logoCell = new TableCell();
            var logoCellProps = new TableCellProperties(
                new TableCellWidth { Width = "2000", Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            );
            logoCell.Append(logoCellProps);

            if (imageRelationshipId != null)
            {
                var imageParagraph = new Paragraph(
                    new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center }
                    )
                );

                var run = new Run();
                var drawing = new Drawing();
                var inline = new DW.Inline
                {
                    DistanceFromTop = 0,
                    DistanceFromBottom = 0,
                    DistanceFromLeft = 0,
                    DistanceFromRight = 0,
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
            var textCellProps = new TableCellProperties(
                new TableCellWidth { Width = "5500", Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            );
            textCell.Append(textCellProps);

            textCell.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "24" }),
                    new Text(_letterhead.InstitutionName) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Left },
                    new SpacingBetweenLines { After = "0" }
                )
            });

            if (!string.IsNullOrEmpty(_letterhead.Department))
            {
                textCell.Append(new Paragraph(
                    new Run(
                        new RunProperties(new FontSize { Val = "20" }),
                        new Text(_letterhead.Department) { Space = SpaceProcessingModeValues.Preserve }
                    )
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification { Val = JustificationValues.Left },
                        new SpacingBetweenLines { After = "0" }
                    )
                });
            }

            var addressLine = string.Join(", ", new[] { _letterhead.Address, _letterhead.Phone, _letterhead.Email }.Where(s => !string.IsNullOrEmpty(s)));
            if (!string.IsNullOrEmpty(addressLine))
            {
                textCell.Append(new Paragraph(
                    new Run(
                        new RunProperties(new FontSize { Val = "18" }),
                        new Text(addressLine) { Space = SpaceProcessingModeValues.Preserve }
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

        private void AddReportTitle(Body body)
        {
            body.Append(new Paragraph(
                new Run(new Text(TranslationService.GetString("ReportTitle")) { Space = SpaceProcessingModeValues.Preserve })
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "200" }
                )
            });
        }

        private string? AddLetterheadImagePart(MainDocumentPart mainPart, out long emuW, out long emuH)
        {
            emuW = 0;
            emuH = 0;
            var logoPath = _letterhead.LogoPath;
            if (string.IsNullOrEmpty(logoPath) || !File.Exists(logoPath))
                return null;

            try
            {
                var ext = Path.GetExtension(logoPath).ToLowerInvariant();
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
                using var stream = File.OpenRead(logoPath);
                imagePart.FeedData(stream);
                var relationshipId = mainPart.GetIdOfPart(imagePart);

                using var img = XImage.FromFile(logoPath);
                double maxW = 300;
                double maxH = 120;
                double scale = Math.Min(maxW / img.PointWidth, maxH / img.PointHeight);
                if (scale > 1) scale = 1;
                double finalW = img.PointWidth * scale;
                double finalH = img.PointHeight * scale;
                emuW = (long)(finalW * 12700);
                emuH = (long)(finalH * 12700);

                return relationshipId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to load letterhead logo from '{logoPath}': {ex.Message}", ex);
            }
        }

        private void AddLetterheadImage(MainDocumentPart mainPart, Body body, JustificationValues justification, string? existingRelationshipId)
        {
            var logoPath = _letterhead.LogoPath;
            if (string.IsNullOrEmpty(logoPath) || !File.Exists(logoPath))
                return;

            string? imageRelationshipId = existingRelationshipId;
            long emuW = 0, emuH = 0;

            if (imageRelationshipId == null)
            {
                imageRelationshipId = AddLetterheadImagePart(mainPart, out emuW, out emuH);
            }

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
                DistanceFromTop = 0,
                DistanceFromBottom = 0,
                DistanceFromLeft = 0,
                DistanceFromRight = 0,
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

        private void AddPatientInfo(Body body, PatientInfo patient, SpecimenInfo specimen)
        {
            if (!_template.ShowPatientInfo) return;

            var rows = new List<(string, string, string, string)>();
            string leftId = _template.ShowPatientId ? patient.Id : "";
            string leftDob = _template.ShowPatientDob ? patient.DateOfBirth : "";

            if (_template.ShowPatientId && _template.ShowPatientDob)
                rows.Add((TranslationService.GetString("report_PatientId"), leftId, TranslationService.GetString("report_DateOfBirth"), leftDob));
            else if (_template.ShowPatientId)
                rows.Add((TranslationService.GetString("report_PatientId"), leftId, "", ""));
            else if (_template.ShowPatientDob)
                rows.Add((TranslationService.GetString("report_DateOfBirth"), leftDob, "", ""));

            if (_template.ShowPatientName || _template.ShowPatientPhysician)
                rows.Add((TranslationService.GetString("report_PatientName"), _template.ShowPatientName ? patient.Name : "", TranslationService.GetString("report_Physician"), _template.ShowPatientPhysician ? patient.Physician : ""));
            else if (_template.ShowPatientName)
                rows.Add((TranslationService.GetString("report_PatientName"), patient.Name, "", ""));
            else if (_template.ShowPatientPhysician)
                rows.Add((TranslationService.GetString("report_Physician"), patient.Physician, "", ""));

            if (_template.ShowPatientSex || _template.ShowPatientWard)
                rows.Add((TranslationService.GetString("report_Sex"), _template.ShowPatientSex ? patient.Sex : "", TranslationService.GetString("report_Ward"), _template.ShowPatientWard ? patient.Ward : ""));
            else if (_template.ShowPatientSex)
                rows.Add((TranslationService.GetString("report_Sex"), patient.Sex, "", ""));
            else if (_template.ShowPatientWard)
                rows.Add((TranslationService.GetString("report_Ward"), patient.Ward, "", ""));

            if (_template.ShowPatientDiagnosis || _template.ShowPatientPaymentMethod)
                rows.Add((TranslationService.GetString("report_Diagnosis"), _template.ShowPatientDiagnosis ? patient.Diagnosis : "", TranslationService.GetString("report_PaymentMethod"), _template.ShowPatientPaymentMethod ? patient.PaymentMethod : ""));
            else if (_template.ShowPatientDiagnosis)
                rows.Add((TranslationService.GetString("report_Diagnosis"), patient.Diagnosis, "", ""));
            else if (_template.ShowPatientPaymentMethod)
                rows.Add((TranslationService.GetString("report_PaymentMethod"), patient.PaymentMethod, "", ""));

            if (_template.ShowPatientAddress)
                rows.Add((TranslationService.GetString("report_Address"), patient.Address, TranslationService.GetString("report_CollectionDate"), specimen.CollectionDate));

            rows.Add(("", "", TranslationService.GetString("report_Specimen"), specimen.Type));
            rows.Add(("", "", TranslationService.GetString("report_Received"), specimen.ReceivedDate));

            var table = new Table();
            var props = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 6 },
                    new BottomBorder { Val = BorderValues.Single, Size = 6 },
                    new LeftBorder { Val = BorderValues.Single, Size = 6 },
                    new RightBorder { Val = BorderValues.Single, Size = 6 }
                ),
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(props);

            foreach (var (l1, v1, l2, v2) in rows)
                AddInfoRow2(table, l1, v1, l2, v2);

            body.Append(table);
            body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));
        }

        private static void AddInfoRow2(Table table, string label1, string value1, string label2, string value2)
        {
            var row = new TableRow();
            row.Append(
                CreateCell(string.IsNullOrEmpty(label1) ? "" : label1 + ":", true, 1500),
                CreateCell(value1, false, 2000),
                CreateCell(string.IsNullOrEmpty(label2) ? "" : label2 + ":", true, 1500),
                CreateCell(value2, false, 2000)
            );
            table.Append(row);
        }

        private static TableCell CreateCell(string text, bool bold, int width)
        {
            var runProps = new RunProperties(
                new FontSize { Val = "16" }
            );
            if (bold) runProps.Append(new Bold());

            var cell = new TableCell(
                new TableCellProperties(
                    new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ),
                new Paragraph(
                    new ParagraphProperties(new SpacingBetweenLines { Before = "0", After = "0" }),
                    new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve })
                )
            );
            return cell;
        }

        private void AddDifferentialTable(Body body, CounterState counterState)
        {
            body.Append(new Paragraph(
                new Run(
                    new RunProperties(new FontSize { Val = "20" }),
                    new Text(TranslationService.GetString("CellDifferentialCount")) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { Before = "40", After = "40" }
                )
            });

            var table = new Table();
            var tblProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 6 },
                    new BottomBorder { Val = BorderValues.Single, Size = 6 },
                    new LeftBorder { Val = BorderValues.Single, Size = 6 },
                    new RightBorder { Val = BorderValues.Single, Size = 6 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                ),
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.Append(tblProps);

            var headerRow = new TableRow();
            headerRow.Append(
                CreateHeaderCell(TranslationService.GetString("ColCellType")),
                CreateHeaderCell(TranslationService.GetString("ColCount")),
                CreateHeaderCell(TranslationService.GetString("ColPercent"))
            );
            if (_template.ShowReferenceRanges)
                headerRow.Append(CreateHeaderCell(TranslationService.GetString("ColRefRange")));

            table.Append(headerRow);

            foreach (var entry in counterState.Entries.OrderBy(e => SortKey(e.CellType.Key)))
            {
                var row = new TableRow();
                row.Append(
                    CreateCell(entry.CellType.Name, false, 2500),
                    CreateCell(entry.Count.ToString(), false, 1000),
                    CreateCell(entry.Percentage.ToString("F1") + "%", false, 1000)
                );
                if (_template.ShowReferenceRanges)
                    row.Append(CreateCell(entry.CellType.ReferenceRange, false, 1500));
                table.Append(row);
            }

            var totalRow = new TableRow();
            totalRow.Append(
                CreateCell(TranslationService.GetString("Total"), true, 2500),
                CreateCell(counterState.Total.ToString(), true, 1000),
                CreateCell("100%", true, 1000)
            );
            if (_template.ShowReferenceRanges)
                totalRow.Append(CreateCell("", false, 1500));
            table.Append(totalRow);

            body.Append(table);
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
                    new Run(new RunProperties(new Bold(), new FontSize { Val = "18" }), new Text(text) { Space = SpaceProcessingModeValues.Preserve })
                )
            );
        }

        private void AddConclusion(Body body, string Conclusion)
        {
            if (!_template.ShowConclusion) return;

            body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));
            body.Append(new Paragraph(
                new Run(new RunProperties(new Bold(), new FontSize { Val = "18" }), new Text(TranslationService.GetString("ConclusionInterpretation")) { Space = SpaceProcessingModeValues.Preserve })
            ));

            var commentText = string.IsNullOrEmpty(Conclusion) ? TranslationService.GetString("NoAbnormalities") : Conclusion;

            body.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines { Before = "20", After = "20" }),
                new Run(new RunProperties(new FontSize { Val = "18" }), new Text(commentText) { Space = SpaceProcessingModeValues.Preserve })
            ));
        }

        private void AddRecommendations(Body body, string recommendations)
        {
            if (string.IsNullOrEmpty(recommendations)) return;

            body.Append(new Paragraph(
                new Run(new RunProperties(new Bold(), new FontSize { Val = "18" }), new Text(TranslationService.GetString("RecommendationsLabel")) { Space = SpaceProcessingModeValues.Preserve })
            ));

            body.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines { Before = "20", After = "20" }),
                new Run(new RunProperties(new FontSize { Val = "18" }), new Text(recommendations) { Space = SpaceProcessingModeValues.Preserve })
            ));
        }

        private void AddFooter(Body body)
        {
            if (!_template.ShowFooter) return;

            body.Append(new Paragraph(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve })));

            var borderProps = new ParagraphProperties(
                new ParagraphBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4, Space = 1 }
                )
            );

            body.Append(new Paragraph(
                borderProps,
                new Run(new RunProperties(new FontSize { Val = "16" }), new Text($"{TranslationService.GetString("ReportGenerated")} {DateTime.Now:yyyy-MM-dd HH:mm}") { Space = SpaceProcessingModeValues.Preserve })
            ));

            if (!string.IsNullOrEmpty(_letterhead.FooterText))
            {
                body.Append(new Paragraph(
                    new Run(new RunProperties(new FontSize { Val = "16" }), new Text(_letterhead.FooterText) { Space = SpaceProcessingModeValues.Preserve })
                ));
            }

            body.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines { Before = "0", After = "0" }),
                new Run(new RunProperties(new FontSize { Val = "16" }), new Text("___________________________________") { Space = SpaceProcessingModeValues.Preserve })
            ));

            body.Append(new Paragraph(
                new ParagraphProperties(new SpacingBetweenLines { Before = "0", After = "0" }),
                new Run(new RunProperties(new FontSize { Val = "16" }), new Text(TranslationService.GetString("PathologistSignature")) { Space = SpaceProcessingModeValues.Preserve })
            ));
        }

        private static string SortKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "ZZ";
            if (key.Length == 1 && key[0] >= '0' && key[0] <= '9')
                return $"0{key}";
            return $"1{key}";
        }
    }
}
