using System.Text.Json.Serialization;

namespace Chigen.Core.Models
{
    public class DocumentTemplate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Default";

        [JsonPropertyName("headerFormat")]
        public string HeaderFormat { get; set; } = "";

        [JsonPropertyName("showLetterhead")]
        public bool ShowLetterhead { get; set; } = true;

        [JsonPropertyName("showPatientInfo")]
        public bool ShowPatientInfo { get; set; } = true;

        [JsonPropertyName("showPatientId")]
        public bool ShowPatientId { get; set; } = true;

        [JsonPropertyName("showPatientName")]
        public bool ShowPatientName { get; set; } = true;

        [JsonPropertyName("showPatientDob")]
        public bool ShowPatientDob { get; set; } = true;

        [JsonPropertyName("showPatientSex")]
        public bool ShowPatientSex { get; set; } = true;

        [JsonPropertyName("showPatientDiagnosis")]
        public bool ShowPatientDiagnosis { get; set; } = true;

        [JsonPropertyName("showPatientAddress")]
        public bool ShowPatientAddress { get; set; } = true;

        [JsonPropertyName("showPatientPhysician")]
        public bool ShowPatientPhysician { get; set; } = true;

        [JsonPropertyName("showPatientWard")]
        public bool ShowPatientWard { get; set; } = true;

        [JsonPropertyName("showPatientPaymentMethod")]
        public bool ShowPatientPaymentMethod { get; set; } = true;

        [JsonPropertyName("showReferenceRanges")]
        public bool ShowReferenceRanges { get; set; } = true;

        [JsonPropertyName("showConclusion")]
        public bool ShowConclusion { get; set; } = true;

        [JsonPropertyName("showFooter")]
        public bool ShowFooter { get; set; } = true;
    }
}
