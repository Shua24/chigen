using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class LetterheadConfigTests
    {
        [Fact]
        public void LetterheadConfig_DefaultValues()
        {
            var c = new LetterheadConfig();
            Assert.Equal("My Hospital", c.InstitutionName);
            Assert.Equal("Hematology Laboratory", c.Department);
            Assert.Equal(string.Empty, c.Address);
            Assert.Equal(string.Empty, c.Phone);
            Assert.Equal(string.Empty, c.Email);
            Assert.Equal(string.Empty, c.LogoPath);
            Assert.Equal(LogoPlacement.Side, c.LogoPlacement);
            Assert.Equal("Confidential — For clinical use only", c.FooterText);
        }

        [Fact]
        public void LetterheadConfig_SetProperties()
        {
            var c = new LetterheadConfig
            {
                InstitutionName = "City Hospital",
                Department = "Pathology",
                Address = "123 Main St",
                Phone = "555-0100",
                Email = "lab@hospital.com",
                LogoPath = @"C:\logos\logo.png",
                LogoPlacement = LogoPlacement.Top,
                FooterText = "Confidential"
            };
            Assert.Equal("City Hospital", c.InstitutionName);
            Assert.Equal("Pathology", c.Department);
            Assert.Equal("123 Main St", c.Address);
            Assert.Equal("555-0100", c.Phone);
            Assert.Equal("lab@hospital.com", c.Email);
            Assert.Equal(@"C:\logos\logo.png", c.LogoPath);
            Assert.Equal(LogoPlacement.Top, c.LogoPlacement);
            Assert.Equal("Confidential", c.FooterText);
        }
    }
}
