using System.Runtime.Versioning;
using Chigen.Core.Models;

namespace Chigen.DocumentExport
{
    public enum PdfConversionMethod
    {
        Unavailable,
        WordInterop,
        DirectPdfGenerator
    }

    public class PdfConverter
    {
        [SupportedOSPlatform("windows")]
        public static PdfConversionMethod CheckAvailability()
        {
            try
            {
                var type = Type.GetTypeFromProgID("Word.Application");
                if (type == null) return PdfConversionMethod.DirectPdfGenerator;

                var word = Activator.CreateInstance(type);
                if (word == null) return PdfConversionMethod.DirectPdfGenerator;

                word.GetType().InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
                return PdfConversionMethod.WordInterop;
            }
            catch
            {
                return PdfConversionMethod.DirectPdfGenerator;
            }
        }

        [SupportedOSPlatform("windows")]
        public static string Convert(
            string outputPdfPath,
            CounterState counterState,
            PatientInfo patient,
            SpecimenInfo specimen,
            LetterheadConfig letterhead,
            DocumentTemplate template,
            PdfConversionMethod method)
        {
            if (method == PdfConversionMethod.WordInterop)
            {
                var tempDocx = Path.Combine(Path.GetTempPath(), $"chigen_pdf_{Guid.NewGuid()}.docx");
                try
                {
                    var docxGen = new DocxGenerator(letterhead, template);
                    docxGen.Create(tempDocx, counterState, patient, specimen);

                    var wordType = Type.GetTypeFromProgID("Word.Application");
                    if (wordType == null)
                    {
                        var pdfGen = new DirectPdfGenerator(letterhead, template);
                        return pdfGen.Create(outputPdfPath, counterState, patient, specimen);
                    }

                    var word = Activator.CreateInstance(wordType);
                    if (word == null)
                    {
                        var pdfGen = new DirectPdfGenerator(letterhead, template);
                        return pdfGen.Create(outputPdfPath, counterState, patient, specimen);
                    }

                    try
                    {
                        word.GetType().InvokeMember("Visible", System.Reflection.BindingFlags.SetProperty, null, word, ["false"]);
                        word.GetType().InvokeMember("DisplayAlerts", System.Reflection.BindingFlags.SetProperty, null, word, [0]);

                        var doc = word.GetType().InvokeMember("Documents", System.Reflection.BindingFlags.GetProperty, null, word, null);
                        if (doc == null) throw new InvalidOperationException("Could not access Word Documents collection.");

                        var openedDoc = doc.GetType().InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, doc, [tempDocx]);
                        if (openedDoc == null) throw new InvalidOperationException("Could not open document in Word.");

                        openedDoc.GetType().InvokeMember("SaveAs2", System.Reflection.BindingFlags.InvokeMethod, null, openedDoc, [outputPdfPath, 17]);
                        openedDoc.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, openedDoc, null);
                    }
                    finally
                    {
                        word.GetType().InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
                    }

                    return outputPdfPath;
                }
                finally
                {
                    try { File.Delete(tempDocx); } catch { }
                }
            }
            else
            {
                var pdfGen = new DirectPdfGenerator(letterhead, template);
                return pdfGen.Create(outputPdfPath, counterState, patient, specimen);
            }
        }
    }
}
