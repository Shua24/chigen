namespace Chigen.Core.Models
{
    public class CounterState
    {
        public List<CellCountEntry> Entries { get; set; } = [];
        public int Total => Entries.Sum(e => e.Count);
        public CounterMode Mode { get; set; } = CounterMode.PeripheralBlood;
        public Stack<UndoAction> UndoStack { get; } = new();

        public void RecalculatePercentages()
        {
            int total = Total;
            if (total == 0) return;
            foreach (var entry in Entries)
            {
                entry.Percentage = Math.Round((double)entry.Count / total * 100, 1);
            }
        }

        public void ResetAll()
        {
            foreach (var entry in Entries)
            {
                entry.Reset();
            }
            UndoStack.Clear();
        }
    }
}
