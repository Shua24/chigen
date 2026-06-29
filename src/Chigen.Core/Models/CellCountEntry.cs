using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chigen.Core.Models
{
    public class CellCountEntry : INotifyPropertyChanged
    {
        public CellType CellType { get; }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _percentage;
        public double Percentage
        {
            get => _percentage;
            set
            {
                if (Math.Abs(_percentage - value) > 0.01)
                {
                    _percentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public CellCountEntry(CellType cellType)
        {
            CellType = cellType;
        }

        public void Reset()
        {
            Count = 0;
            Percentage = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
