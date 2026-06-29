using System.Text.Json.Serialization;

namespace Chigen.Core.Models
{
    public enum LogoPlacement
    {
        Top,
        Side
    }

    public class LetterheadConfig
    {
        [JsonPropertyName("institutionName")]
        public string InstitutionName { get; set; } = "My Hospital";

        [JsonPropertyName("department")]
        public string Department { get; set; } = "Hematology Laboratory";

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("logoPath")]
        public string LogoPath { get; set; } = string.Empty;

        [JsonPropertyName("logoPlacement")]
        public LogoPlacement LogoPlacement { get; set; } = LogoPlacement.Side;

        [JsonPropertyName("footerText")]
        public string FooterText { get; set; } = "Confidential — For clinical use only";
    }
}
