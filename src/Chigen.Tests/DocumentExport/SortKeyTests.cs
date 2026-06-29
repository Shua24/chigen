using Chigen.DocumentExport;

namespace Chigen.Tests.DocumentExport
{
    public class SortKeyTests
    {
        [Theory]
        [InlineData("0", "00")]
        [InlineData("1", "01")]
        [InlineData("9", "09")]
        public void NumericKey_ReturnsZeroPrefixed(string input, string expected)
        {
            Assert.Equal(expected, DirectPdfGenerator.SortKey(input));
        }

        [Theory]
        [InlineData("A", "1A")]
        [InlineData("B", "1B")]
        [InlineData("Z", "1Z")]
        public void AlphaKey_ReturnsOnePrefixed(string input, string expected)
        {
            Assert.Equal(expected, DirectPdfGenerator.SortKey(input));
        }

        [Fact]
        public void EmptyKey_ReturnsZZ()
        {
            Assert.Equal("ZZ", DirectPdfGenerator.SortKey(""));
        }

        [Fact]
        public void NullKey_ReturnsZZ()
        {
            Assert.Equal("ZZ", DirectPdfGenerator.SortKey(null!));
        }

        [Fact]
        public void NumericKeysSortBeforeAlphaKeys()
        {
            var numeric = DirectPdfGenerator.SortKey("1");
            var alpha = DirectPdfGenerator.SortKey("A");
            Assert.True(string.Compare(numeric, alpha, StringComparison.Ordinal) < 0);
        }
    }
}
