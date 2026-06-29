using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Chigen.Core.Models;

namespace Chigen.DocumentExport
{
    public class DirectPdfGenerator
    {
        private readonly LetterheadConfig _letterhead;
        private readonly DocumentTemplate _template;

        private static readonly XFont TitleFont = new("Calibri", 16, XFontStyleEx.Bold);
        private static readonly XFont SubtitleFont = new("Calibri", 11, XFontStyleEx.Regular);
        private static readonly XFont HeaderFont = new("Calibri", 10, XFontStyleEx.Bold);
        private static readonly XFont BodyFont = new("Calibri", 10, XFontStyleEx.Regular);
        private static readonly XFont SmallFont = new("Calibri", 9, XFontStyleEx.Regular);
        private static readonly XFont TableHeaderFont = new("Calibri", 9, XFontStyleEx.Bold);
        private static readonly XFont TableFont = new("Calibri", 8, XFontStyleEx.Regular);
        private static readonly XFont LetterheadTitleFont = new("Calibri", 12, XFontStyleEx.Bold);
        private static readonly XFont LetterheadSubtitleFont = new("Calibri", 10, XFontStyleEx.Regular);

        public DirectPdfGenerator(LetterheadConfig letterhead, DocumentTemplate template)
        {
            _letterhead = letterhead;
            _template = template;
        }

        public string Create(string outputPath, CounterState counterState, PatientInfo patient, SpecimenInfo specimen)
        {
            using var doc = new PdfDocument();
            doc.Info.Title = "Hematology Laboratory Report";
            doc.Info.Author = _letterhead.InstitutionName;

            var page = doc.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);

            double y = 20;

            y = DrawLetterhead(gfx, y);
            y = DrawReportTitle(gfx, y);
            y = DrawPatientInfo(gfx, y, patient, specimen);
            y = DrawDifferentialTable(gfx, y, counterState);
            y = DrawConclusion(gfx, y, patient.Conclusion);
            y = DrawRecommendations(gfx, y, patient.Recommendations);
            DrawFooter(gfx, y);

            doc.Save(outputPath);
            return outputPath;
        }

        private double DrawLetterhead(XGraphics gfx, double y)
        {
            if (!_template.ShowLetterhead) return y;

            if (_letterhead.LogoPlacement == Core.Models.LogoPlacement.Side)
                return DrawLetterheadSide(gfx, y);
            else
                return DrawLetterheadTop(gfx, y);
        }

        private double DrawLetterheadTop(XGraphics gfx, double y)
        {
            var logoPath = _letterhead.LogoPath;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                try
                {
                    using var img = XImage.FromFile(logoPath);
                    double maxW = 300, maxH = 120;
                    double scale = Math.Min(maxW / img.PointWidth, maxH / img.PointHeight);
                    double w = img.PointWidth * scale;
                    double h = img.PointHeight * scale;
                    double x = (gfx.PageSize.Width - w) / 2;
                    gfx.DrawImage(img, x, y, w, h);
                    y += h + 6;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to load letterhead logo from '{logoPath}': {ex.Message}", ex);
                }
            }

            gfx.DrawString(_letterhead.InstitutionName, LetterheadTitleFont, XBrushes.Black,
                new XRect(0, y, gfx.PageSize.Width, 18), XStringFormats.TopCenter);
            y += 18;

            if (!string.IsNullOrEmpty(_letterhead.Department))
            {
                gfx.DrawString(_letterhead.Department, LetterheadSubtitleFont, XBrushes.Black,
                    new XRect(0, y, gfx.PageSize.Width, 14), XStringFormats.TopCenter);
                y += 14;
            }

            var addressLine = string.Join(", ", new[] { _letterhead.Address, _letterhead.Phone, _letterhead.Email }.Where(s => !string.IsNullOrEmpty(s)));
            if (!string.IsNullOrEmpty(addressLine))
            {
                gfx.DrawString(addressLine, SmallFont, XBrushes.Gray,
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
            double textLeft, textWidth;

            var logoPath = _letterhead.LogoPath;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                try
                {
                    using var img = XImage.FromFile(logoPath);
                    double maxW = 120, maxH = 120;
                    double scale = Math.Min(maxW / img.PointWidth, maxH / img.PointHeight);
                    logoWidth = img.PointWidth * scale;
                    logoHeight = img.PointHeight * scale;
                    logoX = 20;
                    gfx.DrawImage(img, logoX, y, logoWidth, logoHeight);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to load letterhead logo from '{logoPath}': {ex.Message}", ex);
                }
            }

            textLeft = (logoWidth > 0) ? logoX + logoWidth + 12 : 20;
            textWidth = gfx.PageSize.Width - textLeft - 20;

            double textY = y;

            gfx.DrawString(_letterhead.InstitutionName, LetterheadTitleFont, XBrushes.Black,
                new XRect(textLeft, textY, textWidth, 18), XStringFormats.TopLeft);
            textY += 18;

            if (!string.IsNullOrEmpty(_letterhead.Department))
            {
                gfx.DrawString(_letterhead.Department, LetterheadSubtitleFont, XBrushes.Black,
                    new XRect(textLeft, textY, textWidth, 14), XStringFormats.TopLeft);
                textY += 14;
            }

            var addressLine = string.Join(", ", new[] { _letterhead.Address, _letterhead.Phone, _letterhead.Email }.Where(s => !string.IsNullOrEmpty(s)));
            if (!string.IsNullOrEmpty(addressLine))
            {
                gfx.DrawString(addressLine, SmallFont, XBrushes.Gray,
                    new XRect(textLeft, textY, textWidth, 12), XStringFormats.TopLeft);
                textY += 12;
            }

            y = Math.Max(y + logoHeight + 6, textY + 6);

            if (!string.IsNullOrEmpty(addressLine))
            {
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

        private double DrawReportTitle(XGraphics gfx, double y)
        {
            gfx.DrawString("HEMATOLOGY LABORATORY REPORT", SubtitleFont, XBrushes.Black,
                new XRect(0, y, gfx.PageSize.Width, 18), XStringFormats.TopCenter);
            y += 18;
            return y;
        }

        private double DrawPatientInfo(XGraphics gfx, double y, PatientInfo patient, SpecimenInfo specimen)
        {
            if (!_template.ShowPatientInfo) return y;

            double col0 = 20, col1 = 100, col2 = 190, col3 = 210, col4 = 300;
            double rowH = 14;
            double left = 20;
            double tableW = gfx.PageSize.Width - 40;

            var rows = new List<(string, string, string, string)>();
            string leftId = _template.ShowPatientId ? patient.Id : "";
            string leftDob = _template.ShowPatientDob ? patient.DateOfBirth : "";

            if (_template.ShowPatientId && _template.ShowPatientDob)
                rows.Add(("Patient ID:", leftId, "Date of Birth:", leftDob));
            else if (_template.ShowPatientId)
                rows.Add(("Patient ID:", leftId, "", ""));
            else if (_template.ShowPatientDob)
                rows.Add(("Date of Birth:", leftDob, "", ""));

            if (_template.ShowPatientName || _template.ShowPatientPhysician)
                rows.Add(("Patient Name:", _template.ShowPatientName ? patient.Name : "", "Physician:", _template.ShowPatientPhysician ? patient.Physician : ""));
            else if (_template.ShowPatientName)
                rows.Add(("Patient Name:", patient.Name, "", ""));
            else if (_template.ShowPatientPhysician)
                rows.Add(("Physician:", patient.Physician, "", ""));

            if (_template.ShowPatientSex || _template.ShowPatientWard)
                rows.Add(("Sex:", _template.ShowPatientSex ? patient.Sex : "", "Ward:", _template.ShowPatientWard ? patient.Ward : ""));
            else if (_template.ShowPatientSex)
                rows.Add(("Sex:", patient.Sex, "", ""));
            else if (_template.ShowPatientWard)
                rows.Add(("Ward:", patient.Ward, "", ""));

            if (_template.ShowPatientDiagnosis || _template.ShowPatientPaymentMethod)
                rows.Add(("Diagnosis:", _template.ShowPatientDiagnosis ? patient.Diagnosis : "", "Payment Method:", _template.ShowPatientPaymentMethod ? patient.PaymentMethod : ""));
            else if (_template.ShowPatientDiagnosis)
                rows.Add(("Diagnosis:", patient.Diagnosis, "", ""));
            else if (_template.ShowPatientPaymentMethod)
                rows.Add(("Payment Method:", patient.PaymentMethod, "", ""));

            if (_template.ShowPatientAddress)
                rows.Add(("Address:", patient.Address, "Collection Date:", specimen.CollectionDate));

            rows.Add(("", "", "Specimen:", specimen.Type));
            rows.Add(("", "", "Received:", specimen.ReceivedDate));

            int rowCount = rows.Count;
            gfx.DrawRectangle(XPens.Black, XBrushes.White, left, y, tableW, rowH * rowCount);

            for (int i = 0; i < rowCount; i++)
                DrawPatientInfoRow(gfx, rows[i].Item1, rows[i].Item2, rows[i].Item3, rows[i].Item4, y + rowH * i, col0, col1, col2, col3, col4, rowH);

            y += rowH * rowCount + 6;
            return y;
        }

        private static void DrawPatientInfoRow(XGraphics gfx, string label1, string value1, string label2, string value2, double y,
            double col0, double col1, double col2, double col3, double col4, double rowH)
        {
            if (!string.IsNullOrEmpty(label1))
                gfx.DrawString(label1, TableHeaderFont, XBrushes.Black, new XRect(col0, y, col1 - col0, rowH), XStringFormats.CenterLeft);
            if (!string.IsNullOrEmpty(value1))
                gfx.DrawString(value1, TableFont, XBrushes.Black, new XRect(col1, y, col2 - col1, rowH), XStringFormats.CenterLeft);
            if (!string.IsNullOrEmpty(label2))
                gfx.DrawString(label2, TableHeaderFont, XBrushes.Black, new XRect(col3, y, col4 - col3, rowH), XStringFormats.CenterLeft);
            if (!string.IsNullOrEmpty(value2))
                gfx.DrawString(value2, TableFont, XBrushes.Black, new XRect(col4, y, gfx.PageSize.Width - col4 - 20, rowH), XStringFormats.CenterLeft);
        }

        private double DrawDifferentialTable(XGraphics gfx, double y, CounterState counterState)
        {
            double tableLeft = 20;
            double tableW = gfx.PageSize.Width - 40;
            double rowH = 16;

            gfx.DrawString("CELL DIFFERENTIAL COUNT", HeaderFont, XBrushes.Black,
                new XRect(0, y, gfx.PageSize.Width, 16), XStringFormats.TopCenter);
            y += 18;

            double[] colWidths;
            string[] headers;
            if (_template.ShowReferenceRanges)
            {
                colWidths = [tableW * 0.40, tableW * 0.18, tableW * 0.18, tableW * 0.24];
                headers = ["Cell Type", "Count", "%", "Ref Range"];
            }
            else
            {
                colWidths = [tableW * 0.50, tableW * 0.25, tableW * 0.25];
                headers = ["Cell Type", "Count", "%"];
            }

            double[] colX = new double[colWidths.Length];
            double cx = tableLeft;
            for (int i = 0; i < colWidths.Length; i++)
            {
                colX[i] = cx;
                cx += colWidths[i];
            }

            gfx.DrawRectangle(XBrushes.LightSteelBlue, tableLeft, y, tableW, rowH);

            for (int i = 0; i < colWidths.Length; i++)
            {
                if (i > 0)
                    gfx.DrawLine(XPens.White, colX[i], y, colX[i], y + rowH);

                gfx.DrawString(headers[i], TableHeaderFont, XBrushes.Black,
                    new XRect(colX[i], y, colWidths[i], rowH), XStringFormats.Center);
            }
            y += rowH;

            foreach (var entry in counterState.Entries.OrderBy(e => SortKey(e.CellType.Key)))
            {
                if (entry.Count == 0) continue;

                gfx.DrawRectangle(XPens.LightGray, XBrushes.White, tableLeft, y, tableW, rowH);
                gfx.DrawString(entry.CellType.Name, TableFont, XBrushes.Black,
                    new XRect(colX[0] + 4, y, colWidths[0] - 4, rowH), XStringFormats.CenterLeft);
                gfx.DrawString(entry.Count.ToString(), TableFont, XBrushes.Black,
                    new XRect(colX[1], y, colWidths[1], rowH), XStringFormats.Center);
                gfx.DrawString(entry.Percentage.ToString("F1") + "%", TableFont, XBrushes.Black,
                    new XRect(colX[2], y, colWidths[2], rowH), XStringFormats.Center);
                if (_template.ShowReferenceRanges)
                    gfx.DrawString(entry.CellType.ReferenceRange, TableFont, XBrushes.DimGray,
                        new XRect(colX[3], y, colWidths[3], rowH), XStringFormats.Center);

                gfx.DrawLine(XPens.LightGray, tableLeft, y + rowH, tableLeft + tableW, y + rowH);
                y += rowH;
            }

            gfx.DrawRectangle(new XPen(XColors.Black, 2), XBrushes.LightSteelBlue, tableLeft, y, tableW, rowH);
            gfx.DrawString("TOTAL", TableHeaderFont, XBrushes.Black,
                new XRect(colX[0] + 4, y, colWidths[0] - 4, rowH), XStringFormats.CenterLeft);
            gfx.DrawString(counterState.Total.ToString(), TableFont, XBrushes.Black,
                new XRect(colX[1], y, colWidths[1], rowH), XStringFormats.Center);
            gfx.DrawString("100%", TableFont, XBrushes.Black,
                new XRect(colX[2], y, colWidths[2], rowH), XStringFormats.Center);
            y += rowH + 6;

            return y;
        }

        private double DrawConclusion(XGraphics gfx, double y, string Conclusion)
        {
            if (!_template.ShowConclusion) return y;

            y += 2;
            gfx.DrawString("Conclusion / Interpretation:", TableHeaderFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 14), XStringFormats.TopLeft);
            y += 14;

            var commentText = string.IsNullOrEmpty(Conclusion) ? "No abnormalities detected." : Conclusion;
            gfx.DrawString(commentText, TableFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 12), XStringFormats.TopLeft);
            y += 14;

            return y;
        }

        private double DrawRecommendations(XGraphics gfx, double y, string recommendations)
        {
            if (string.IsNullOrEmpty(recommendations)) return y;

            y += 2;
            gfx.DrawString("Recommendations:", TableHeaderFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 14), XStringFormats.TopLeft);
            y += 14;

            gfx.DrawString(recommendations, TableFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 12), XStringFormats.TopLeft);
            y += 14;

            return y;
        }

        private void DrawFooter(XGraphics gfx, double y)
        {
            if (!_template.ShowFooter) return;

            y += 6;
            gfx.DrawLine(XPens.Black, 20, y, gfx.PageSize.Width - 20, y);
            y += 4;

            gfx.DrawString($"Report generated: {DateTime.Now:yyyy-MM-dd HH:mm}", TableFont, XBrushes.Gray,
                new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
            y += 10;

            if (!string.IsNullOrEmpty(_letterhead.FooterText))
            {
                gfx.DrawString(_letterhead.FooterText, TableFont, XBrushes.Gray,
                    new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
                y += 10;
            }

            y += 4;
            gfx.DrawString("___________________________________", TableFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString("Pathologist Signature", TableFont, XBrushes.Black,
                new XRect(20, y, gfx.PageSize.Width - 40, 10), XStringFormats.TopLeft);
        }

        public static string SortKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "ZZ";
            if (key.Length == 1 && key[0] >= '0' && key[0] <= '9')
                return $"0{key}";
            return $"1{key}";
        }
    }
}
