using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class PatientInfoTests
    {
        [Fact]
        public void PatientInfo_DefaultValues()
        {
            var p = new PatientInfo();
            Assert.Equal(string.Empty, p.Name);
            Assert.Equal(string.Empty, p.Id);
            Assert.Equal(string.Empty, p.DateOfBirth);
            Assert.Equal(string.Empty, p.Sex);
            Assert.Equal(string.Empty, p.Physician);
            Assert.Equal(string.Empty, p.Conclusion);
        }

        [Fact]
        public void PatientInfo_SetProperties()
        {
            var p = new PatientInfo
            {
                Name = "John Doe",
                Id = "P12345",
                DateOfBirth = "1990-01-15",
                Sex = "Male",
                Physician = "Dr. Smith",
                Conclusion = "Normal differential"
            };
            Assert.Equal("John Doe", p.Name);
            Assert.Equal("P12345", p.Id);
            Assert.Equal("1990-01-15", p.DateOfBirth);
            Assert.Equal("Male", p.Sex);
            Assert.Equal("Dr. Smith", p.Physician);
            Assert.Equal("Normal differential", p.Conclusion);
        }
    }
}
