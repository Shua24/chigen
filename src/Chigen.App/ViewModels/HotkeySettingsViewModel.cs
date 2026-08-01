using System.Collections.ObjectModel;
using Chigen.Core.Models;
using Chigen.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chigen.App.ViewModels
{
    public partial class HotkeySettingsViewModel : ObservableObject
    {
        public ObservableCollection<HotkeySettingsItem> PbItems { get; } = [];
        public ObservableCollection<HotkeySettingsItem> BmItems { get; } = [];

        private HotkeySettingsItem? _selectedItem;
        public HotkeySettingsItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != null)
                    _selectedItem.IsListening = false;

                _selectedItem = value;
                OnPropertyChanged();

                if (_selectedItem != null)
                {
                    _selectedItem.IsListening = true;
                    StatusText = "Press a key (0-9 or A-Z) to assign...";
                }
                else
                {
                    StatusText = "";
                }
            }
        }

        [ObservableProperty]
        private string _statusText = "";

        [ObservableProperty]
        private int _selectedTab;

        public HotkeySettingsViewModel()
        {
            LoadFromSaved();
        }

        public void LoadFromSaved()
        {
            PbItems.Clear();
            BmItems.Clear();

            var pbCells = CellTypeProvider.GetDefaultPeripheralBloodTypes();
            var bmCells = CellTypeProvider.GetDefaultBoneMarrowTypes();
            var saved = TemplateService.LoadHotkeyMappings();

            foreach (var cell in pbCells)
            {
                var savedMapping = saved.FirstOrDefault(s => s.CellTypeId == cell.Id && s.Mode == CounterMode.PeripheralBlood);
                var item = new HotkeySettingsItem
                {
                    CellTypeId = cell.Id,
                    CellName = cell.Name,
                    Group = cell.Group,
                    Key = savedMapping?.Key ?? cell.Key
                };
                PbItems.Add(item);
            }

            foreach (var cell in bmCells)
            {
                var savedMapping = saved.FirstOrDefault(s => s.CellTypeId == cell.Id && s.Mode == CounterMode.BoneMarrow);
                var item = new HotkeySettingsItem
                {
                    CellTypeId = cell.Id,
                    CellName = cell.Name,
                    Group = cell.Group,
                    Key = savedMapping?.Key ?? cell.Key
                };
                BmItems.Add(item);
            }

            RecalcConflicts();
        }

        public void HandleKeyPress(string key)
        {
            if (SelectedItem == null || string.IsNullOrEmpty(key)) return;

            var items = SelectedTab == 0 ? PbItems : BmItems;

            var conflict = items.FirstOrDefault(i =>
                i != SelectedItem && i.Key == key);

            if (conflict != null)
            {
                StatusText = $"Warning: Key \"{key}\" is already assigned to \"{conflict.CellName}\". Confirm to reassign.";
                SelectedItem.Key = key;
            }
            else
            {
                SelectedItem.Key = key;
                StatusText = $"Key \"{key}\" assigned to \"{SelectedItem.CellName}\".";
            }

            SelectedItem.IsListening = false;
            SelectedItem = null;
            RecalcConflicts();
        }

        public void CancelSelection()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsListening = false;
                SelectedItem = null;
                StatusText = "";
            }
        }

        private void RecalcConflicts()
        {
            RecalcConflictsFor(PbItems);
            RecalcConflictsFor(BmItems);
        }

        private static void RecalcConflictsFor(ObservableCollection<HotkeySettingsItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].HasConflict = items.Any(other =>
                    other != items[i] && !string.IsNullOrEmpty(other.Key) && other.Key == items[i].Key);
            }
        }

        public void Save()
        {
            var mappings = new List<HotkeyMappingEntry>();
            foreach (var item in PbItems)
            {
                mappings.Add(new HotkeyMappingEntry
                {
                    CellTypeId = item.CellTypeId,
                    Key = item.Key,
                    Mode = CounterMode.PeripheralBlood
                });
            }
            foreach (var item in BmItems)
            {
                mappings.Add(new HotkeyMappingEntry
                {
                    CellTypeId = item.CellTypeId,
                    Key = item.Key,
                    Mode = CounterMode.BoneMarrow
                });
            }
            TemplateService.SaveHotkeyMappings(mappings);
        }
    }
}
