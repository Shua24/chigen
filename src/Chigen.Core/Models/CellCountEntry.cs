using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chigen.Core.Models
{
    public partial class CellCountEntry : ObservableObject
    {
        public CellType CellType { get; }

        [ObservableProperty]
        private int _count;

        [ObservableProperty]
        private double _percentage;

        public CellCountEntry(CellType cellType)
        {
            CellType = cellType;
        }

        public void Reset()
        {
            Count = 0;
            Percentage = 0;
        }
    }
}
