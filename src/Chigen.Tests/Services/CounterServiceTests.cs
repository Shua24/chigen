using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.Tests.Services
{
    public class CounterServiceTests : IDisposable
    {
        private readonly CounterService _service = new();

        [Fact]
        public void Constructor_LoadsPeripheralBloodByDefault()
        {
            Assert.Equal(CounterMode.PeripheralBlood, _service.State.Mode);
            Assert.Equal(12, _service.State.Entries.Count);
        }

        [Fact]
        public void TryCount_WithValidKey_IncrementsCount()
        {
            var result = _service.TryCount("1");
            Assert.True(result);
            Assert.Equal(1, _service.State.Entries[0].Count);
        }

        [Fact]
        public void TryCount_WithInvalidKey_ReturnsFalse()
        {
            var result = _service.TryCount("Z");
            Assert.False(result);
        }

        [Fact]
        public void TryCount_WithEmptyKey_ReturnsFalse()
        {
            var result = _service.TryCount("");
            Assert.False(result);
        }

        [Fact]
        public void TryCount_WithNullKey_ReturnsFalse()
        {
            var result = _service.TryCount(null!);
            Assert.False(result);
        }

        [Fact]
        public void TryCount_PushesUndoAction()
        {
            _service.TryCount("1");
            Assert.Single(_service.State.UndoStack);
            Assert.Equal("neutrophil", _service.State.UndoStack.Peek().CellTypeId);
        }

        [Fact]
        public void TryUndo_AfterCount_DecrementsAndRemovesFromStack()
        {
            _service.TryCount("1");
            _service.TryCount("1");

            var result = _service.TryUndo();
            Assert.True(result);
            Assert.Equal(1, _service.State.Entries[0].Count);
            Assert.Single(_service.State.UndoStack);
        }

        [Fact]
        public void TryUndo_WithEmptyStack_ReturnsFalse()
        {
            var result = _service.TryUndo();
            Assert.False(result);
        }

        [Fact]
        public void TryUndo_DoesNotGoBelowZero()
        {
            _service.TryCount("1");
            _service.TryUndo();
            _service.TryUndo();

            Assert.Equal(0, _service.State.Entries[0].Count);
        }

        [Fact]
        public void Reset_ClearsAllCountsAndUndoStack()
        {
            _service.TryCount("1");
            _service.TryCount("2");

            _service.Reset();

            Assert.All(_service.State.Entries, e => Assert.Equal(0, e.Count));
            Assert.Empty(_service.State.UndoStack);
        }

        [Fact]
        public void TryCountManual_WithValidId_IncrementsCount()
        {
            var result = _service.TryCountManual("neutrophil");
            Assert.True(result);
            Assert.Equal(1, _service.State.Entries[0].Count);
        }

        [Fact]
        public void TryCountManual_WithInvalidId_ReturnsFalse()
        {
            var result = _service.TryCountManual("nonexistent");
            Assert.False(result);
        }

        [Fact]
        public void TryUndoManual_WithValidId_DecrementsCount()
        {
            _service.TryCountManual("neutrophil");
            _service.TryCountManual("neutrophil");

            var result = _service.TryUndoManual("neutrophil");
            Assert.True(result);
            Assert.Equal(1, _service.State.Entries[0].Count);
        }

        [Fact]
        public void TryUndoManual_WithInvalidId_ReturnsFalse()
        {
            var result = _service.TryUndoManual("nonexistent");
            Assert.False(result);
        }

        [Fact]
        public void TryUndoManual_DoesNotGoBelowZero()
        {
            var result = _service.TryUndoManual("neutrophil");
            Assert.False(result);
            Assert.Equal(0, _service.State.Entries[0].Count);
        }

        [Fact]
        public void GetCounts_ReturnsDictionary()
        {
            _service.TryCount("1");
            _service.TryCount("3");

            var counts = _service.GetCounts();
            Assert.Equal(1, counts["neutrophil"]);
            Assert.Equal(1, counts["lymphocyte"]);
            Assert.Equal(0, counts["monocyte"]);
        }

        [Fact]
        public void Total_ReflectsAllCounts()
        {
            _service.TryCount("1");
            _service.TryCount("2");
            _service.TryCount("3");

            Assert.Equal(3, _service.State.Total);
        }

        [Fact]
        public void RecalculatePercentages_UpdatesAfterCount()
        {
            _service.TryCount("1");
            _service.TryCount("1");

            var neutrophil = _service.State.Entries[0];
            Assert.Equal(100.0, neutrophil.Percentage);
        }

        [Fact]
        public void MultipleCounts_ProduceCorrectPercentages()
        {
            _service.TryCount("1");
            _service.TryCount("1");
            _service.TryCount("2");

            var neutrophil = _service.State.Entries[0];
            var band = _service.State.Entries[1];

            Assert.Equal(66.7, neutrophil.Percentage, 1);
            Assert.Equal(33.3, band.Percentage, 1);
        }

        [Fact]
        public void LoadCellTypes_SwitchesToBoneMarrow()
        {
            _service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.Equal(CounterMode.BoneMarrow, _service.State.Mode);
            Assert.Equal(17, _service.State.Entries.Count);
        }

        [Fact]
        public void LoadCellTypes_ResetsState()
        {
            _service.TryCount("1");
            _service.LoadCellTypes(CounterMode.BoneMarrow);

            Assert.All(_service.State.Entries, e => Assert.Equal(0, e.Count));
            Assert.Empty(_service.State.UndoStack);
        }

        [Fact]
        public void GetEntriesForMode_ReturnsMatchingEntries()
        {
            _service.LoadCellTypes(CounterMode.BoneMarrow);
            var entries = _service.GetEntriesForMode(CounterMode.BoneMarrow);

            Assert.All(entries, e => Assert.True(
                e.CellType.Mode == CounterMode.BoneMarrow || e.CellType.Mode == CounterMode.PeripheralBlood));
        }

        [Fact]
        public void GetAllCellTypes_ReturnsAllLoaded()
        {
            var types = _service.GetAllCellTypes();
            Assert.Equal(12, types.Count);
        }

        [Fact]
        public void ApplyHotkeyMappings_ChangesKeys()
        {
            var mappings = new List<HotkeyMappingEntry>
            {
                new() { CellTypeId = "neutrophil", Key = "Q", Mode = CounterMode.PeripheralBlood }
            };

            _service.ApplyHotkeyMappings(mappings);

            var result = _service.TryCount("Q");
            Assert.True(result);
            Assert.Equal(1, _service.State.Entries[0].Count);
        }

        [Fact]
        public void ApplyHotkeyMappings_OldKeyNoLongerWorks()
        {
            var mappings = new List<HotkeyMappingEntry>
            {
                new() { CellTypeId = "neutrophil", Key = "Q", Mode = CounterMode.PeripheralBlood }
            };

            _service.ApplyHotkeyMappings(mappings);

            var result = _service.TryCount("1");
            Assert.False(result);
        }

        public void Dispose()
        {
            _service.Reset();
        }
    }
}
