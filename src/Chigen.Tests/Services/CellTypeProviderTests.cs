using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.Tests.Services
{
    public class CellTypeProviderTests
    {
        [Fact]
        public void GetDefaultPeripheralBloodTypes_ReturnsTwelveTypes()
        {
            var types = CellTypeProvider.GetDefaultPeripheralBloodTypes();
            Assert.Equal(12, types.Count);
        }

        [Fact]
        public void GetDefaultPeripheralBloodTypes_AllHavePeripheralBloodMode()
        {
            var types = CellTypeProvider.GetDefaultPeripheralBloodTypes();
            Assert.All(types, t => Assert.Equal(CounterMode.PeripheralBlood, t.Mode));
        }

        [Theory]
        [InlineData("neutrophil", "1")]
        [InlineData("band", "2")]
        [InlineData("lymphocyte", "3")]
        [InlineData("monocyte", "4")]
        [InlineData("eosinophil", "5")]
        [InlineData("basophil", "6")]
        [InlineData("metamyelocyte", "7")]
        [InlineData("myelocyte", "8")]
        [InlineData("promyelocyte", "9")]
        [InlineData("monoblast", "Q")]
        [InlineData("lymphoblast", "W")]
        [InlineData("other", "O")]
        public void GetDefaultPeripheralBloodTypes_HasCorrectMapping(string id, string key)
        {
            var types = CellTypeProvider.GetDefaultPeripheralBloodTypes();
            var ct = types.Find(t => t.Id == id);
            Assert.NotNull(ct);
            Assert.Equal(key, ct!.Key);
        }

        [Fact]
        public void GetDefaultBoneMarrowTypes_ReturnsSeventeenTypes()
        {
            var types = CellTypeProvider.GetDefaultBoneMarrowTypes();
            Assert.Equal(17, types.Count);
        }

        [Fact]
        public void GetDefaultBoneMarrowTypes_AllHaveBoneMarrowMode()
        {
            var types = CellTypeProvider.GetDefaultBoneMarrowTypes();
            Assert.All(types, t => Assert.Equal(CounterMode.BoneMarrow, t.Mode));
        }

        [Theory]
        [InlineData("neutrophil", "Myeloid")]
        [InlineData("lymphocyte", "Lymphoid")]
        [InlineData("monocyte", "Monocytic")]
        [InlineData("nrbC", "Erythroid")]
        [InlineData("megakaryocyte", "Megakaryocytic")]
        [InlineData("monocytePrecursor", "Monocytic")]
        [InlineData("monoblast", "Blast")]
        [InlineData("lymphoblast", "Blast")]
        [InlineData("other", "Other")]
        public void GetDefaultBoneMarrowTypes_HasCorrectGroup(string id, string group)
        {
            var types = CellTypeProvider.GetDefaultBoneMarrowTypes();
            var ct = types.Find(t => t.Id == id);
            Assert.NotNull(ct);
            Assert.Equal(group, ct!.Group);
        }

        [Fact]
        public void GetDefaultBoneMarrowTypes_ContainsExtraTypes()
        {
            var types = CellTypeProvider.GetDefaultBoneMarrowTypes();
            var ids = types.Select(t => t.Id).ToList();
            Assert.Contains("nrbC", ids);
            Assert.Contains("megakaryocyte", ids);
            Assert.Contains("plasmaCell", ids);
            Assert.Contains("monocytePrecursor", ids);
            Assert.Contains("lymphoblast", ids);
            Assert.Contains("other", ids);
            Assert.Contains("erythroblast", ids);
        }
    }
}
