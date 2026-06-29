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

                InvokeWordMethod(word, "Quit");
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
                        InvokeWordMethod(word, "Visible", false);
                        InvokeWordMethod(word, "DisplayAlerts", 0);

                        var doc = InvokeGetProperty(word, "Documents");
                        if (doc == null) throw new InvalidOperationException("Could not access Word Documents collection.");

                        var openedDoc = InvokeWordMethod(doc, "Open", tempDocx);
                        if (openedDoc == null) throw new InvalidOperationException("Could not open document in Word.");

                        InvokeWordMethod(openedDoc, "SaveAs2", outputPdfPath, 17);
                        InvokeWordMethod(openedDoc, "Close");
                    }
                    finally
                    {
                        try { InvokeWordMethod(word, "Quit"); } catch { }
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
        private static object? InvokeWordMethod(object target, string methodName, params object?[] args)
        {
            try
            {
                var flags = System.Reflection.BindingFlags.InvokeMethod;
                return target.GetType().InvokeMember(methodName, flags, null, target, args);
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Word.{methodName} failed: {ex.InnerException.Message}", ex.InnerException);
            }
        }

        private static object? InvokeGetProperty(object target, string propertyName)
        {
            try
            {
                var flags = System.Reflection.BindingFlags.GetProperty;
                return target.GetType().InvokeMember(propertyName, flags, null, target, null);
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Word.{propertyName} failed: {ex.InnerException.Message}", ex.InnerException);
            }
        }
    }
}
