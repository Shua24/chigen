using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Chigen.Core.Models;
using Chigen.Core.Services;
using Chigen.DocumentExport;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chigen.App.ViewModels
{
    public partial class CounterViewModel : ObservableObject
    {
        private readonly CounterService _counterService = new();

        public ObservableCollection<CellCountEntry> Entries { get; } = [];

        public CounterViewModel()
        {
            _statusText = TranslationService.GetString("StatusReady");
            TranslationService.LanguageChanged += () =>
            {
                OnPropertyChanged(nameof(PatientDisplay));
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(CurrentThemeLabel));
            };
            RefreshTemplateSettings();
            ApplySavedHotkeys();
            RefreshEntries();
            UpdateState();
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PatientDisplay))]
        private string _patientName = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PatientDisplay))]
        private string _patientId = "";

        [ObservableProperty]
        private string _patientDob = "";

        [ObservableProperty]
        private string _patientSex = "";

        [ObservableProperty]
        private string _patientDiagnosis = "";

        [ObservableProperty]
        private string _patientAddress = "";

        [ObservableProperty]
        private string _physician = "";

        [ObservableProperty]
        private string _ward = "";

        [ObservableProperty]
        private string _paymentMethod = "";

        [ObservableProperty]
        private string _Conclusion = "";

        [ObservableProperty]
        private string _Recommendations = "";

        public string PatientDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(PatientName) && string.IsNullOrEmpty(PatientId))
                    return TranslationService.GetString("PatientNotSet");
                var display = PatientName;
                if (!string.IsNullOrEmpty(PatientId))
                    display += string.Format(TranslationService.GetString("PatientDisplayFmt"), PatientId);
                return display;
            }
        }

        public string SpecimenDate => DateTime.Now.ToString("yyyy-MM-dd");

        [ObservableProperty]
        private int _totalCount;

        private CounterMode _currentMode = CounterMode.PeripheralBlood;
        public CounterMode CurrentMode
        {
            get => _currentMode;
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPbMode));
                    OnPropertyChanged(nameof(IsBmMode));
                    SwitchMode(value);
                }
            }
        }

        public bool IsPbMode => CurrentMode == CounterMode.PeripheralBlood;
        public bool IsBmMode => CurrentMode == CounterMode.BoneMarrow;

        public Geometry CurrentThemeIcon
        {
            get
            {
                var theme = TemplateService.LoadTheme();
                var key = theme == "Dark" ? "GeomSun" : "GeomMoon";
                return (Geometry)Application.Current.Resources[key];
            }
        }

        public string CurrentThemeLabel
        {
            get
            {
                var theme = TemplateService.LoadTheme();
                var key = theme == "Dark" ? "ThemeLightMode" : "ThemeDarkMode";
                return TranslationService.GetString(key);
            }
        }

        public void ToggleTheme()
        {
            var currentTheme = TemplateService.LoadTheme();
            var newTheme = currentTheme == "Dark" ? "Light" : "Dark";
            TemplateService.SaveTheme(newTheme);
            App.SetTheme(newTheme);
            OnPropertyChanged(nameof(CurrentThemeIcon));
            OnPropertyChanged(nameof(CurrentThemeLabel));
        }

        [ObservableProperty]
        private string _statusText;

        [ObservableProperty]
        private bool _canUndo;

        [ObservableProperty]
        private bool _canExport;

        public ICommand IncrementCommand => new RelayCommand<string>(IncrementCount);
        public ICommand DecrementCommand => new RelayCommand<string>(DecrementCount);

        [RelayCommand]
        private void GenerateDocx() => HandleGenerateDocx();

        [RelayCommand]
        private void ExportPdf() => HandleExportPdf();

        [RelayCommand]
        private void Reset() => HandleReset();

        [RelayCommand]
        private void Undo() => HandleUndo();

        [ObservableProperty]
        private bool _hasPatientInfo = true;

        public void HandleReset()
        {
            var result = MessageBox.Show(
                TranslationService.GetString("ResetConfirm"),
                TranslationService.GetString("ResetConfirmCaption"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _counterService.Reset();
                RefreshEntries();
                UpdateState();
                StatusText = TranslationService.GetString("StatusReset");
            }
        }

        public void HandleKeyPress(string key)
        {
            if (_counterService.TryCount(key))
            {
                UpdateState();
                StatusText = $"{TranslationService.GetString("StatusCounted")}{key}";
            }
        }

        public void HandleUndo()
        {
            if (_counterService.TryUndo())
            {
                UpdateState();
                StatusText = TranslationService.GetString("StatusDecremented");
            }
        }

        private ReportDocument BuildReportDocument()
        {
            var letterhead = TemplateService.LoadLetterhead();
            var template = TemplateService.LoadTemplate();
            var specimen = new SpecimenInfo
            {
                Type = CurrentMode == CounterMode.PeripheralBlood
                    ? TranslationService.GetString("SpecimenTypePeripheralBlood")
                    : TranslationService.GetString("SpecimenTypeBoneMarrowAspirate")
            };
            var patient = new PatientInfo
            {
                Name = PatientName, Id = PatientId, DateOfBirth = PatientDob,
                Sex = PatientSex, Diagnosis = PatientDiagnosis, Address = PatientAddress,
                Physician = Physician, Ward = Ward, PaymentMethod = PaymentMethod,
                Conclusion = Conclusion, Recommendations = Recommendations
            };
            return new ReportDocument(
                _counterService.State, patient, specimen, letterhead, template,
                reportTitle: TranslationService.GetString("ReportTitle"),
                subtitle: TranslationService.GetString("CellDifferentialCount"),
                patientInfoSectionLabel: TranslationService.GetString("PatientInfoSection"),
                conclusionSectionLabel: TranslationService.GetString("ConclusionInterpretation"),
                recommendationsSectionLabel: TranslationService.GetString("RecommendationsLabel"),
                totalLabel: TranslationService.GetString("Total"),
                noAbnormalitiesText: TranslationService.GetString("NoAbnormalities"),
                generatedDateLabel: TranslationService.GetString("ReportGenerated"),
                signatureLabel: TranslationService.GetString("PathologistSignature"),
                colCellType: TranslationService.GetString("ColCellType"),
                colCount: TranslationService.GetString("ColCount"),
                colPercent: TranslationService.GetString("ColPercent"),
                colRefRange: TranslationService.GetString("ColRefRange"),
                labelPatientId: TranslationService.GetString("report_PatientId"),
                labelPatientName: TranslationService.GetString("report_PatientName"),
                labelDob: TranslationService.GetString("report_DateOfBirth"),
                labelSex: TranslationService.GetString("report_Sex"),
                labelDiagnosis: TranslationService.GetString("report_Diagnosis"),
                labelAddress: TranslationService.GetString("report_Address"),
                labelPhysician: TranslationService.GetString("report_Physician"),
                labelWard: TranslationService.GetString("report_Ward"),
                labelPaymentMethod: TranslationService.GetString("report_PaymentMethod"),
                labelSpecimenType: TranslationService.GetString("report_Specimen"),
                labelCollectionDate: TranslationService.GetString("report_CollectionDate"),
                labelReceivedDate: TranslationService.GetString("report_Received"));
        }

        public void HandleGenerateDocx()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = TranslationService.GetString("DocxFilter"),
                    DefaultExt = ".docx",
                    FileName = $"Differential_Report_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var doc = BuildReportDocument();
                    if (doc.ShowLetterhead && !doc.HasLogo)
                        throw new InvalidOperationException(TranslationService.GetString("LetterheadLogoRequired"));
                    new DocxGenerator(doc).Create(saveDialog.FileName);
                    StatusText = $"{TranslationService.GetString("StatusDocxSaved")}{saveDialog.FileName}";
                    MessageBox.Show(
                        $"{TranslationService.GetString("DocxSavedMsg")}{saveDialog.FileName}",
                        TranslationService.GetString("ExportComplete"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                var msg = ex is System.Reflection.TargetInvocationException && ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;
                StatusText = $"{TranslationService.GetString("ExportError")}: {msg}";
                MessageBox.Show(
                    $"{TranslationService.GetString("ExportError")}:\n{msg}",
                    TranslationService.GetString("Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void HandleExportPdf()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = TranslationService.GetString("PdfFilter"),
                    DefaultExt = ".pdf",
                    FileName = $"Differential_Report_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var doc = BuildReportDocument();
                    if (doc.ShowLetterhead && !doc.HasLogo)
                        throw new InvalidOperationException(TranslationService.GetString("LetterheadLogoRequired"));
                    new DirectPdfGenerator(doc).Create(saveDialog.FileName);
                    StatusText = $"{TranslationService.GetString("StatusPdfSaved")}{saveDialog.FileName}";
                    MessageBox.Show(
                        $"{TranslationService.GetString("PdfSavedMsg")}{saveDialog.FileName}",
                        TranslationService.GetString("ExportComplete"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                var msg = ex is System.Reflection.TargetInvocationException && ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;
                StatusText = $"{TranslationService.GetString("ExportError")}: {msg}";
                MessageBox.Show(
                    $"{TranslationService.GetString("ExportError")}:\n{msg}",
                    TranslationService.GetString("Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SwitchMode(CounterMode mode)
        {
            _counterService.LoadCellTypes(mode);
            ApplySavedHotkeys();
            RefreshEntries();
            StatusText = mode == CounterMode.PeripheralBlood
                ? TranslationService.GetString("StatusSwitchedPb")
                : TranslationService.GetString("StatusSwitchedBm");
        }

        public void LoadCellTypes(CounterMode mode)
        {
            _counterService.LoadCellTypes(mode);
            ApplySavedHotkeys();
            RefreshEntries();
        }

        public void RefreshHotkeys()
        {
            ApplySavedHotkeys();
            RefreshEntries();
        }

        public void RefreshTemplateSettings()
        {
            OnPropertyChanged(nameof(CurrentThemeIcon));
            OnPropertyChanged(nameof(CurrentThemeLabel));
        }

        private void ApplySavedHotkeys()
        {
            var saved = TemplateService.LoadHotkeyMappings();
            _counterService.ApplyHotkeyMappings(saved);
        }

        private void RefreshEntries()
        {
            Entries.Clear();
            foreach (var entry in _counterService.State.Entries)
            {
                Entries.Add(entry);
                entry.PropertyChanged += (_, _) => UpdateState();
            }
        }

        public void IncrementCount(string? cellTypeId)
        {
            if (cellTypeId == null) return;
            if (_counterService.TryCountManual(cellTypeId))
            {
                UpdateState();
                StatusText = $"{TranslationService.GetString("StatusCounted")}{cellTypeId}";
            }
        }

        public void DecrementCount(string? cellTypeId)
        {
            if (cellTypeId == null) return;
            if (_counterService.TryUndoManual(cellTypeId))
            {
                UpdateState();
                StatusText = $"{TranslationService.GetString("StatusDecremented")}{cellTypeId}";
            }
        }

        public Core.Models.PatientInfo GetPatientInfo()
        {
            return new Core.Models.PatientInfo
            {
                Name = PatientName,
                Id = PatientId,
                DateOfBirth = PatientDob,
                Sex = PatientSex,
                Diagnosis = PatientDiagnosis,
                Address = PatientAddress,
                Physician = Physician,
                Ward = Ward,
                PaymentMethod = PaymentMethod
            };
        }

        public void SetPatientInfo(Core.Models.PatientInfo info)
        {
            PatientName = info.Name;
            PatientId = info.Id;
            PatientDob = info.DateOfBirth;
            PatientSex = info.Sex;
            PatientDiagnosis = info.Diagnosis;
            PatientAddress = info.Address;
            Physician = info.Physician;
            Ward = info.Ward;
            PaymentMethod = info.PaymentMethod;
        }

        private void UpdateState()
        {
            TotalCount = _counterService.State.Total;
            CanUndo = _counterService.State.UndoStack.Count > 0;
            CanExport = TotalCount > 0;
        }
    }
}
