using System.Text.Json.Serialization;

namespace Chigen.Core.Models
{
    public class HotkeyMappingEntry
    {
        [JsonPropertyName("cellTypeId")]
        public string CellTypeId { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        [JsonConverter(typeof(KeyConverter))]
        public string Key { get; set; } = "";

        [JsonPropertyName("mode")]
        public CounterMode Mode { get; set; } = CounterMode.PeripheralBlood;
    }
}
