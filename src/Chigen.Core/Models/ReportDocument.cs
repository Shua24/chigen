using Chigen.Core.Services;

namespace Chigen.Core.Models;

public class LabelledValue
{
    public string Label { get; init; }
    public string Value { get; init; }

    public LabelledValue(string label, string value)
    {
        Label = label;
        Value = value;
    }
}

public class DifferentialRow
{
    public string CellName { get; init; } = string.Empty;
    public int Count { get; init; }
    public double Percentage { get; init; }
    public string ReferenceRange { get; init; } = string.Empty;
}

public class ReportDocument
{
    public ReportDocument(
        CounterState counterState,
        PatientInfo patient,
        SpecimenInfo specimen,
        LetterheadConfig letterhead,
        DocumentTemplate template,
        string reportTitle,
        string subtitle,
        string patientInfoSectionLabel,
        string conclusionSectionLabel,
        string recommendationsSectionLabel,
        string totalLabel,
        string noAbnormalitiesText,
        string generatedDateLabel,
        string signatureLabel,
        string colCellType, string colCount, string colPercent, string colRefRange,
        string labelPatientId, string labelPatientName, string labelDob, string labelSex,
        string labelDiagnosis, string labelAddress, string labelPhysician, string labelWard, string labelPaymentMethod,
        string labelSpecimenType, string labelCollectionDate, string labelReceivedDate)
    {
        ShowLetterhead = template.ShowLetterhead;
        LogoPath = letterhead.LogoPath;
        LogoPlacement = letterhead.LogoPlacement;
        InstitutionName = letterhead.InstitutionName;
        Department = letterhead.Department;
        AddressLine = string.Join(", ", new[] { letterhead.Address, letterhead.Phone, letterhead.Email }.Where(s => !string.IsNullOrEmpty(s)));
        FooterText = letterhead.FooterText;

        ReportTitle = reportTitle;
        Subtitle = subtitle;

        ShowPatientInfo = template.ShowPatientInfo;
        PatientInfoSectionLabel = patientInfoSectionLabel;
        PatientFields = BuildPatientFields(patient, specimen, template,
            labelPatientId, labelPatientName, labelDob, labelSex,
            labelDiagnosis, labelAddress, labelPhysician, labelWard, labelPaymentMethod,
            labelSpecimenType, labelCollectionDate, labelReceivedDate);

        ShowReferenceRanges = template.ShowReferenceRanges;
        Rows = BuildRows(counterState);
        TotalCount = counterState.Total;
        TotalLabel = totalLabel;
        ColCellType = colCellType;
        ColCount = colCount;
        ColPercent = colPercent;
        ColRefRange = colRefRange;

        ShowConclusion = template.ShowConclusion;
        ConclusionSectionLabel = conclusionSectionLabel;
        ConclusionText = string.IsNullOrEmpty(patient.Conclusion) ? noAbnormalitiesText : patient.Conclusion;

        ShowRecommendations = !string.IsNullOrEmpty(patient.Recommendations);
        RecommendationsSectionLabel = recommendationsSectionLabel;
        RecommendationsText = patient.Recommendations;

        ShowFooter = template.ShowFooter;
        GeneratedAt = $"{generatedDateLabel} {DateTime.Now:yyyy-MM-dd HH:mm}";
        SignatureLabel = signatureLabel;
    }

    // --- Letterhead ---
    public bool ShowLetterhead { get; }
    public bool HasLogo => !string.IsNullOrEmpty(LogoPath) && File.Exists(LogoPath);
    public string LogoPath { get; }
    public LogoPlacement LogoPlacement { get; }
    public string InstitutionName { get; }
    public string Department { get; }
    public string AddressLine { get; }
    public string FooterText { get; }

    // --- Title ---
    public string ReportTitle { get; }
    public string Subtitle { get; }

    // --- Patient Info ---
    public bool ShowPatientInfo { get; }
    public string PatientInfoSectionLabel { get; }
    public List<LabelledValue> PatientFields { get; }

    // --- Table ---
    public bool ShowReferenceRanges { get; }
    public List<DifferentialRow> Rows { get; }
    public int TotalCount { get; }
    public string TotalLabel { get; }
    public string ColCellType { get; }
    public string ColCount { get; }
    public string ColPercent { get; }
    public string ColRefRange { get; }

    // --- Conclusion ---
    public bool ShowConclusion { get; }
    public string ConclusionSectionLabel { get; }
    public string ConclusionText { get; }

    // --- Recommendations ---
    public bool ShowRecommendations { get; }
    public string RecommendationsSectionLabel { get; }
    public string RecommendationsText { get; }

    // --- Footer ---
    public bool ShowFooter { get; }
    public string GeneratedAt { get; }
    public string SignatureLabel { get; }

    private static List<LabelledValue> BuildPatientFields(
        PatientInfo patient, SpecimenInfo specimen, DocumentTemplate template,
        string lpId, string lpName, string lDob, string lSex,
        string lDiag, string lAddr, string lPhys, string lWard, string lPay,
        string lSpecType, string lCollDate, string lRecvDate)
    {
        var fields = new List<LabelledValue>();
        if (template.ShowPatientId)      fields.Add(new LabelledValue(lpId, patient.Id));
        if (template.ShowPatientName)    fields.Add(new LabelledValue(lpName, patient.Name));
        if (template.ShowPatientDob)     fields.Add(new LabelledValue(lDob, patient.DateOfBirth));
        if (template.ShowPatientSex)     fields.Add(new LabelledValue(lSex, patient.Sex));
        if (template.ShowPatientDiagnosis)    fields.Add(new LabelledValue(lDiag, patient.Diagnosis));
        if (template.ShowPatientAddress)      fields.Add(new LabelledValue(lAddr, patient.Address));
        if (template.ShowPatientPhysician)    fields.Add(new LabelledValue(lPhys, patient.Physician));
        if (template.ShowPatientWard)         fields.Add(new LabelledValue(lWard, patient.Ward));
        if (template.ShowPatientPaymentMethod) fields.Add(new LabelledValue(lPay, patient.PaymentMethod));
        fields.Add(new LabelledValue(lSpecType, specimen.Type));
        fields.Add(new LabelledValue(lCollDate, specimen.CollectionDate));
        fields.Add(new LabelledValue(lRecvDate, specimen.ReceivedDate));
        return fields;
    }

    private static List<DifferentialRow> BuildRows(CounterState state)
    {
        return state.Entries
            .Where(e => e.Count > 0)
            .OrderBy(e => CellSortHelper.SortKey(e.CellType.Key))
            .Select(e => new DifferentialRow
            {
                CellName = e.CellType.Name,
                Count = e.Count,
                Percentage = e.Percentage,
                ReferenceRange = e.CellType.ReferenceRange
            })
            .ToList();
    }
}
