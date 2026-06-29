using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chigen.App.ViewModels
{
    public class HotkeySettingsItem : INotifyPropertyChanged
    {
        public string CellTypeId { get; set; } = string.Empty;
        public string CellName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;

        private string _key = "";
        public string Key
        {
            get => _key;
            set { _key = value; OnPropertyChanged(); OnPropertyChanged(nameof(KeyDisplay)); OnPropertyChanged(nameof(HasConflict)); }
        }

        public string KeyDisplay => string.IsNullOrEmpty(_key) ? "-" : _key;

        private bool _hasConflict;
        public bool HasConflict
        {
            get => _hasConflict;
            set { _hasConflict = value; OnPropertyChanged(); }
        }

        private bool _isListening;
        public bool IsListening
        {
            get => _isListening;
            set { _isListening = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
