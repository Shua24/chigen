using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class CellTypeTests
    {
        [Fact]
        public void CellType_DefaultValues()
        {
            var ct = new CellType();
            Assert.Equal(string.Empty, ct.Id);
            Assert.Equal(string.Empty, ct.Name);
            Assert.Equal("", ct.Key);
            Assert.Equal(string.Empty, ct.ReferenceRange);
            Assert.Equal(0, ct.ReferenceLow);
            Assert.Equal(0, ct.ReferenceHigh);
            Assert.Equal("default", ct.Group);
            Assert.Equal(CounterMode.PeripheralBlood, ct.Mode);
        }

        [Fact]
        public void CellType_SetProperties()
        {
            var ct = new CellType
            {
                Id = "neutrophil",
                Name = "Neutrophil (Seg)",
                Key = "1",
                ReferenceRange = "40-80",
                ReferenceLow = 40,
                ReferenceHigh = 80,
                Group = "Myeloid",
                Mode = CounterMode.BoneMarrow
            };
            Assert.Equal("neutrophil", ct.Id);
            Assert.Equal("Neutrophil (Seg)", ct.Name);
            Assert.Equal("1", ct.Key);
            Assert.Equal("40-80", ct.ReferenceRange);
            Assert.Equal(40, ct.ReferenceLow);
            Assert.Equal(80, ct.ReferenceHigh);
            Assert.Equal("Myeloid", ct.Group);
            Assert.Equal(CounterMode.BoneMarrow, ct.Mode);
        }
    }
}
