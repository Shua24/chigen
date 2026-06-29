using Chigen.DocumentExport;

namespace Chigen.Tests.DocumentExport
{
    public class PdfConversionMethodTests
    {
        [Fact]
        public void Enum_HasExpectedValues()
        {
            Assert.Equal(0, (int)PdfConversionMethod.Unavailable);
            Assert.Equal(1, (int)PdfConversionMethod.WordInterop);
            Assert.Equal(2, (int)PdfConversionMethod.DirectPdfGenerator);
        }
    }
}
