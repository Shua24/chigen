namespace Chigen.Core.Models
{
    // Dates already handled in SpecimenInfo. No need to add date here.
    public class PatientInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        // public string ClinicalHistory { get; set; } = string.Empty;
        public string Physician { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Conclusion { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
    }
}
