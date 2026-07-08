using CommunityToolkit.Mvvm.ComponentModel;

namespace Chigen.App.ViewModels
{
    public partial class HotkeySettingsItem : ObservableObject
    {
        public string CellTypeId { get; set; } = string.Empty;
        public string CellName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;

        [ObservableProperty]
        private string _key = "";

        public string KeyDisplay => string.IsNullOrEmpty(Key) ? "-" : Key;

        [ObservableProperty]
        private bool _hasConflict;

        [ObservableProperty]
        private bool _isListening;
    }
}
