using System.ComponentModel;
using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class CellCountEntryTests
    {
        private readonly CellType _cellType = new()
        {
            Id = "neutrophil",
            Name = "Neutrophil (Seg)",
            Key = "1"
        };

        [Fact]
        public void Constructor_SetsCellType()
        {
            var entry = new CellCountEntry(_cellType);
            Assert.Same(_cellType, entry.CellType);
            Assert.Equal(0, entry.Count);
            Assert.Equal(0, entry.Percentage);
        }

        [Fact]
        public void Count_SetAndGet()
        {
            var entry = new CellCountEntry(_cellType) { Count = 5 };
            Assert.Equal(5, entry.Count);
        }

        [Fact]
        public void Percentage_SetAndGet()
        {
            var entry = new CellCountEntry(_cellType) { Percentage = 25.5 };
            Assert.Equal(25.5, entry.Percentage);
        }

        [Fact]
        public void Reset_SetsCountAndPercentageToZero()
        {
            var entry = new CellCountEntry(_cellType)
            {
                Count = 10,
                Percentage = 50.0
            };
            entry.Reset();
            Assert.Equal(0, entry.Count);
            Assert.Equal(0, entry.Percentage);
        }

        [Fact]
        public void Count_RaisesPropertyChanged()
        {
            var entry = new CellCountEntry(_cellType);
            var changed = new List<string>();
            entry.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

            entry.Count = 3;

            Assert.Contains("Count", changed);
        }

        [Fact]
        public void Count_DoesNotRaisePropertyChanged_WhenSameValue()
        {
            var entry = new CellCountEntry(_cellType);
            var changed = new List<string>();
            entry.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

            entry.Count = 0;

            Assert.DoesNotContain("Count", changed);
        }

        [Fact]
        public void Percentage_RaisesPropertyChanged()
        {
            var entry = new CellCountEntry(_cellType);
            var changed = new List<string>();
            entry.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

            entry.Percentage = 50.0;

            Assert.Contains("Percentage", changed);
        }

        [Fact]
        public void ImplementsINotifyPropertyChanged()
        {
            var entry = new CellCountEntry(_cellType);
            Assert.IsAssignableFrom<INotifyPropertyChanged>(entry);
        }
    }
}
