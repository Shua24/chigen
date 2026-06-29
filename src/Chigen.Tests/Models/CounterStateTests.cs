using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class CounterStateTests
    {
        private static CellType MakeCellType(string id, string key) =>
            new() { Id = id, Name = id, Key = key };

        [Fact]
        public void Total_ZeroWhenNoEntries()
        {
            var state = new CounterState();
            Assert.Equal(0, state.Total);
        }

        [Fact]
        public void Total_SumOfEntryCounts()
        {
            var state = new CounterState
            {
                Entries =
                [
                    new CellCountEntry(MakeCellType("a", "1")) { Count = 10 },
                    new CellCountEntry(MakeCellType("b", "2")) { Count = 20 },
                    new CellCountEntry(MakeCellType("c", "3")) { Count = 30 }
                ]
            };
            Assert.Equal(60, state.Total);
        }

        [Fact]
        public void Mode_DefaultsToPeripheralBlood()
        {
            var state = new CounterState();
            Assert.Equal(CounterMode.PeripheralBlood, state.Mode);
        }

        [Fact]
        public void UndoStack_IsInitialized()
        {
            var state = new CounterState();
            Assert.NotNull(state.UndoStack);
            Assert.Empty(state.UndoStack);
        }

        [Fact]
        public void RecalculatePercentages_ComputesCorrectly()
        {
            var state = new CounterState
            {
                Entries =
                [
                    new CellCountEntry(MakeCellType("a", "1")) { Count = 25 },
                    new CellCountEntry(MakeCellType("b", "2")) { Count = 25 },
                    new CellCountEntry(MakeCellType("c", "3")) { Count = 50 }
                ]
            };
            state.RecalculatePercentages();

            Assert.Equal(25.0, state.Entries[0].Percentage);
            Assert.Equal(25.0, state.Entries[1].Percentage);
            Assert.Equal(50.0, state.Entries[2].Percentage);
        }

        [Fact]
        public void RecalculatePercentages_RoundsToOneDecimal()
        {
            var state = new CounterState
            {
                Entries =
                [
                    new CellCountEntry(MakeCellType("a", "1")) { Count = 1 },
                    new CellCountEntry(MakeCellType("b", "2")) { Count = 3 }
                ]
            };
            state.RecalculatePercentages();

            Assert.Equal(25.0, state.Entries[0].Percentage);
            Assert.Equal(75.0, state.Entries[1].Percentage);
        }

        [Fact]
        public void RecalculatePercentages_SkipsWhenTotalIsZero()
        {
            var state = new CounterState
            {
                Entries =
                [
                    new CellCountEntry(MakeCellType("a", "1")) { Percentage = 10.0 },
                    new CellCountEntry(MakeCellType("b", "2")) { Percentage = 20.0 }
                ]
            };
            state.RecalculatePercentages();

            Assert.Equal(10.0, state.Entries[0].Percentage);
            Assert.Equal(20.0, state.Entries[1].Percentage);
        }

        [Fact]
        public void ResetAll_ClearsEntriesAndUndoStack()
        {
            var state = new CounterState
            {
                Entries =
                [
                    new CellCountEntry(MakeCellType("a", "1")) { Count = 10, Percentage = 100.0 }
                ]
            };
            state.UndoStack.Push(new UndoAction { CellTypeId = "a", CellName = "A" });

            state.ResetAll();

            Assert.Equal(0, state.Entries[0].Count);
            Assert.Equal(0, state.Entries[0].Percentage);
            Assert.Empty(state.UndoStack);
        }
    }
}
