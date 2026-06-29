using System.Text.Json.Serialization;

namespace Chigen.Core.Models
{
    public class CellType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        [JsonConverter(typeof(KeyConverter))]
        public string Key { get; set; } = "";

        [JsonPropertyName("referenceRange")]
        public string ReferenceRange { get; set; } = string.Empty;

        [JsonPropertyName("referenceLow")]
        public double ReferenceLow { get; set; }

        [JsonPropertyName("referenceHigh")]
        public double ReferenceHigh { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; } = "default";

        [JsonPropertyName("mode")]
        public CounterMode Mode { get; set; } = CounterMode.PeripheralBlood;
    }
}
