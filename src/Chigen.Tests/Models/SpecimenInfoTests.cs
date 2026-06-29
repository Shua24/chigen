using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class SpecimenInfoTests
    {
        [Fact]
        public void SpecimenInfo_DefaultType()
        {
            var s = new SpecimenInfo();
            Assert.Equal("Peripheral Blood", s.Type);
        }

        [Fact]
        public void SpecimenInfo_CollectionDateDefaultsToToday()
        {
            var s = new SpecimenInfo();
            Assert.Equal(DateTime.Now.ToString("dd-MM-yyyy"), s.CollectionDate);
        }

        [Fact]
        public void SpecimenInfo_ReceivedDateDefaultsToToday()
        {
            var s = new SpecimenInfo();
            Assert.Equal(DateTime.Now.ToString("dd-MM-yyyy"), s.ReceivedDate);
        }

        [Fact]
        public void SpecimenInfo_SetProperties()
        {
            var s = new SpecimenInfo
            {
                Type = "Bone Marrow",
                CollectionDate = DateTime.Now.ToString("dd-MM-yyyy"),
                ReceivedDate = DateTime.Now.ToString("dd-MM-yyyy")
            };
            Assert.Equal("Bone Marrow", s.Type);
            Assert.Equal(DateTime.Now.ToString("dd-MM-yyyy"), s.CollectionDate);
            Assert.Equal(DateTime.Now.ToString("dd-MM-yyyy"), s.ReceivedDate);
        }
    }
}
