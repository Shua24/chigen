using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class UndoActionTests
    {
        [Fact]
        public void UndoAction_DefaultValues()
        {
            var u = new UndoAction();
            Assert.Equal(string.Empty, u.CellTypeId);
            Assert.Equal(string.Empty, u.CellName);
        }

        [Fact]
        public void UndoAction_SetProperties()
        {
            var u = new UndoAction
            {
                CellTypeId = "neutrophil",
                CellName = "Neutrophil (Seg)"
            };
            Assert.Equal("neutrophil", u.CellTypeId);
            Assert.Equal("Neutrophil (Seg)", u.CellName);
        }
    }
}
