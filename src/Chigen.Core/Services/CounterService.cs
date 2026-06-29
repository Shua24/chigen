using Chigen.Core.Models;

namespace Chigen.Core.Services
{
    public class CounterService
    {
        private readonly Dictionary<string, CellType> _keyMap = [];
        private readonly Dictionary<string, CellType> _idMap = [];

        public CounterState State { get; } = new();

        public CounterService()
        {
            LoadCellTypes(CounterMode.PeripheralBlood);
        }

        public void LoadCellTypes(CounterMode mode)
        {
            var cellTypes = mode == CounterMode.PeripheralBlood
                ? CellTypeProvider.GetDefaultPeripheralBloodTypes()
                : CellTypeProvider.GetDefaultBoneMarrowTypes();

            _keyMap.Clear();
            _idMap.Clear();
            State.Entries.Clear();
            State.Mode = mode;

            foreach (var ct in cellTypes)
            {
                _idMap[ct.Id] = ct;
                if (!string.IsNullOrEmpty(ct.Key) && !_keyMap.ContainsKey(ct.Key))
                    _keyMap[ct.Key] = ct;
                State.Entries.Add(new CellCountEntry(ct));
            }

            State.ResetAll();
        }

        public bool TryCount(string key)
        {
            if (string.IsNullOrEmpty(key) || !_keyMap.TryGetValue(key, out var cellType))
                return false;

            var entry = State.Entries.First(e => e.CellType.Id == cellType.Id);
            entry.Count++;
            State.UndoStack.Push(new UndoAction { CellTypeId = cellType.Id, CellName = cellType.Name });
            State.RecalculatePercentages();
            return true;
        }

        public bool TryUndo()
        {
            if (State.UndoStack.Count == 0)
                return false;

            var last = State.UndoStack.Pop();
            var entry = State.Entries.First(e => e.CellType.Id == last.CellTypeId);
            if (entry.Count > 0)
                entry.Count--;
            State.RecalculatePercentages();
            return true;
        }

        public void Reset()
        {
            State.ResetAll();
        }

        public bool TryCountManual(string cellTypeId)
        {
            if (!_idMap.TryGetValue(cellTypeId, out var cellType))
                return false;

            var entry = State.Entries.First(e => e.CellType.Id == cellType.Id);
            entry.Count++;
            State.UndoStack.Push(new UndoAction { CellTypeId = cellType.Id, CellName = cellType.Name });
            State.RecalculatePercentages();
            return true;
        }

        public bool TryUndoManual(string cellTypeId)
        {
            if (!_idMap.TryGetValue(cellTypeId, out var cellType))
                return false;

            var entry = State.Entries.First(e => e.CellType.Id == cellType.Id);
            if (entry.Count > 0)
            {
                entry.Count--;
                State.RecalculatePercentages();
                return true;
            }
            return false;
        }

        public Dictionary<string, int> GetCounts()
        {
            return State.Entries.ToDictionary(e => e.CellType.Id, e => e.Count);
        }

        public List<CellCountEntry> GetEntriesForMode(CounterMode mode)
        {
            return State.Entries.Where(e => e.CellType.Mode == mode || e.CellType.Mode == CounterMode.PeripheralBlood).ToList();
        }

        public void ApplyHotkeyMappings(List<HotkeyMappingEntry> mappings)
        {
            foreach (var mapping in mappings)
            {
                if (_idMap.TryGetValue(mapping.CellTypeId, out var cellType))
                {
                    cellType.Key = mapping.Key;
                }
            }

            _keyMap.Clear();
            foreach (var ct in _idMap.Values)
            {
                if (!string.IsNullOrEmpty(ct.Key) && !_keyMap.ContainsKey(ct.Key))
                {
                    _keyMap[ct.Key] = ct;
                }
            }
        }

        public List<CellType> GetAllCellTypes()
        {
            return _idMap.Values.ToList();
        }
    }
}
