using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class HotkeyMappingEntryTests
    {
        [Fact]
        public void HotkeyMappingEntry_DefaultValues()
        {
            var h = new HotkeyMappingEntry();
            Assert.Equal(string.Empty, h.CellTypeId);
            Assert.Equal("", h.Key);
            Assert.Equal(CounterMode.PeripheralBlood, h.Mode);
        }

        [Fact]
        public void HotkeyMappingEntry_SetProperties()
        {
            var h = new HotkeyMappingEntry
            {
                CellTypeId = "neutrophil",
                Key = "1",
                Mode = CounterMode.BoneMarrow
            };
            Assert.Equal("neutrophil", h.CellTypeId);
            Assert.Equal("1", h.Key);
            Assert.Equal(CounterMode.BoneMarrow, h.Mode);
        }
    }
}
