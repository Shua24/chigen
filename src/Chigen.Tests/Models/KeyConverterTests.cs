using System.Text.Json;
using Chigen.Core.Models;

namespace Chigen.Tests.Models
{
    public class KeyConverterTests
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [Fact]
        public void Deserialize_StringKey_ReadsAsString()
        {
            var json = """{"id":"test","key":"A"}""";
            var ct = JsonSerializer.Deserialize<CellType>(json, Options);
            Assert.Equal("A", ct!.Key);
        }

        [Fact]
        public void Deserialize_NumericKey_ReadsAsString()
        {
            var json = """{"id":"test","key":1}""";
            var ct = JsonSerializer.Deserialize<CellType>(json, Options);
            Assert.Equal("1", ct!.Key);
        }

        [Fact]
        public void Serialize_StringKey_WritesAsString()
        {
            var ct = new CellType { Id = "test", Key = "A" };
            var json = JsonSerializer.Serialize(ct, Options);
            Assert.Contains("\"key\":\"A\"", json);
        }

        [Fact]
        public void Serialize_NumericKeyString_WritesAsString()
        {
            var ct = new CellType { Id = "test", Key = "5" };
            var json = JsonSerializer.Serialize(ct, Options);
            Assert.Contains("\"key\":\"5\"", json);
        }

        [Fact]
        public void Deserialize_HotkeyMappingWithNumericKey()
        {
            var json = """{"cellTypeId":"neutrophil","key":1}""";
            var h = JsonSerializer.Deserialize<HotkeyMappingEntry>(json, Options);
            Assert.Equal("1", h!.Key);
        }

        [Fact]
        public void Deserialize_HotkeyMappingWithAlphaKey()
        {
            var json = """{"cellTypeId":"nrbC","key":"A"}""";
            var h = JsonSerializer.Deserialize<HotkeyMappingEntry>(json, Options);
            Assert.Equal("A", h!.Key);
        }
    }
}
