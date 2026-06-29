using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.Tests.Services
{
    public class CounterServiceModeTests
    {
        [Fact]
        public void BoneMarrowMode_HasCorrectTypes()
        {
            var service = new CounterService();
            service.LoadCellTypes(CounterMode.BoneMarrow);

            var ids = service.GetAllCellTypes().Select(t => t.Id).ToList();
            Assert.Contains("nrbC", ids);
            Assert.Contains("megakaryocyte", ids);
            Assert.Contains("plasmaCell", ids);
            Assert.Contains("monocytePrecursor", ids);
            Assert.Contains("monoblast", ids);
            Assert.Contains("lymphoblast", ids);
            Assert.Contains("erythroblast", ids);
        }

        [Fact]
        public void BoneMarrowMode_AlphaKeysWork()
        {
            var service = new CounterService();
            service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.True(service.TryCount("A"));
            Assert.True(service.TryCount("B"));
            Assert.True(service.TryCount("C"));
            Assert.True(service.TryCount("D"));
            Assert.True(service.TryCount("E"));
            Assert.True(service.TryCount("Q"));
            Assert.True(service.TryCount("W"));
        }

        [Fact]
        public void BoneMarrowMode_NumericKeysStillWork()
        {
            var service = new CounterService();
            service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.True(service.TryCount("1"));
        }

        [Fact]
        public void SwitchingModes_ClearsPreviousCounts()
        {
            var service = new CounterService();
            service.TryCount("1");
            service.TryCount("2");
            service.TryCount("3");

            Assert.Equal(3, service.State.Total);

            service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.Equal(0, service.State.Total);
        }

        [Fact]
        public void SwitchingModes_ClearsUndoStack()
        {
            var service = new CounterService();
            service.TryCount("1");
            Assert.NotEmpty(service.State.UndoStack);

            service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.Empty(service.State.UndoStack);
        }
    }
}
