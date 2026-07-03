using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Chigen.Core.Models;
using Chigen.Core.Services;
using Chigen.DocumentExport;
using CommunityToolkit.Mvvm.Input;

namespace Chigen.App.ViewModels
{
    public class CounterViewModel : INotifyPropertyChanged
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

        private string _patientName = "";
        public string PatientName
        {
            get => _patientName;
            set { _patientName = value; OnPropertyChanged(); OnPropertyChanged(nameof(PatientDisplay)); }
        }

        private string _patientId = "";
        public string PatientId
        {
            get => _patientId;
            set { _patientId = value; OnPropertyChanged(); OnPropertyChanged(nameof(PatientDisplay)); }
        }

        private string _patientDob = "";
        public string PatientDob
        {
            get => _patientDob;
            set { _patientDob = value; OnPropertyChanged(); }
        }

        private string _patientSex = "";
        public string PatientSex
        {
            get => _patientSex;
            set { _patientSex = value; OnPropertyChanged(); }
        }

        private string _patientDiagnosis = "";
        public string PatientDiagnosis
        {
            get => _patientDiagnosis;
            set { _patientDiagnosis = value; OnPropertyChanged(); }
        }

        private string _patientAddress = "";
        public string PatientAddress
        {
            get => _patientAddress;
            set { _patientAddress = value; OnPropertyChanged(); }
        }

        private string _physician = "";
        public string Physician
        {
            get => _physician;
            set { _physician = value; OnPropertyChanged(); }
        }

        private string _ward = "";
        public string Ward
        {
            get => _ward;
            set { _ward = value; OnPropertyChanged(); }
        }

        private string _paymentMethod = "";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(); }
        }

        private string _Conclusion = "";
        public string Conclusion
        {
            get => _Conclusion;
            set { _Conclusion = value; OnPropertyChanged(); }
        }

        private string _Recommendations = "";
        public string Recommendations
        {
            get => _Recommendations;
            set { _Recommendations = value; OnPropertyChanged(); }
        }

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

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set { _totalCount = value; OnPropertyChanged(); }
        }

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

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            set { _canUndo = value; OnPropertyChanged(); }
        }

        private bool _canExport;
        public bool CanExport
        {
            get => _canExport;
            set { _canExport = value; OnPropertyChanged(); }
        }

        private bool _isWordAvailable = true;
        public bool IsWordAvailable
        {
            get => _isWordAvailable;
            set { _isWordAvailable = value; OnPropertyChanged(); }
        }

        private PdfConversionMethod _pdfMethod = PdfConversionMethod.DirectPdfGenerator;
        public PdfConversionMethod PdfMethod
        {
            get => _pdfMethod;
            set { _pdfMethod = value; OnPropertyChanged(); }
        }

        public ICommand IncrementCommand => new RelayCommand<string>(IncrementCount);
        public ICommand DecrementCommand => new RelayCommand<string>(DecrementCount);
        public ICommand GenerateDocxCommand => new RelayCommand(HandleGenerateDocx);
        public ICommand ExportPdfCommand => new RelayCommand(HandleExportPdf);
        public ICommand ResetCommand => new RelayCommand(HandleReset);
        public ICommand UndoCommand => new RelayCommand(HandleUndo);

        private bool _hasPatientInfo = true;
        public bool HasPatientInfo
        {
            get => _hasPatientInfo;
            set { _hasPatientInfo = value; OnPropertyChanged(); }
        }

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
                    var letterhead = TemplateService.LoadLetterhead();
                    var template = TemplateService.LoadTemplate();
                    if (template.ShowLetterhead && string.IsNullOrEmpty(letterhead.LogoPath))
                        throw new InvalidOperationException(TranslationService.GetString("LetterheadLogoRequired"));
                    var generator = new DocxGenerator(letterhead, template);
                    var specimen = new SpecimenInfo
                    {
                        Type = CurrentMode == CounterMode.PeripheralBlood ? TranslationService.GetString("SpecimenTypePeripheralBlood") : TranslationService.GetString("SpecimenTypeBoneMarrowAspirate")
                    };
                    var patient = new PatientInfo
                    {
                        Name = PatientName,
                        Id = PatientId,
                        DateOfBirth = PatientDob,
                        Sex = PatientSex,
                        Diagnosis = PatientDiagnosis,
                        Address = PatientAddress,
                        Physician = Physician,
                        Ward = Ward,
                        PaymentMethod = PaymentMethod,
                        Conclusion = Conclusion,
                        Recommendations = Recommendations
                    };
                    generator.Create(saveDialog.FileName, _counterService.State, patient, specimen);
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
                    var letterhead = TemplateService.LoadLetterhead();
                    var template = TemplateService.LoadTemplate();
                    if (template.ShowLetterhead && string.IsNullOrEmpty(letterhead.LogoPath))
                        throw new InvalidOperationException(TranslationService.GetString("LetterheadLogoRequired"));
                    var specimen = new SpecimenInfo
                    {
                        Type = CurrentMode == CounterMode.PeripheralBlood ? TranslationService.GetString("SpecimenTypePeripheralBlood") : TranslationService.GetString("SpecimenTypeBoneMarrowAspirate")
                    };
                    var patient = new PatientInfo
                    {
                        Name = PatientName,
                        Id = PatientId,
                        DateOfBirth = PatientDob,
                        Sex = PatientSex,
                        Diagnosis = PatientDiagnosis,
                        Address = PatientAddress,
                        Physician = Physician,
                        Ward = Ward,
                        PaymentMethod = PaymentMethod,
                        Conclusion = Conclusion,
                        Recommendations = Recommendations
                    };

                    PdfConverter.Convert(saveDialog.FileName, _counterService.State, patient, specimen, letterhead, template, PdfMethod);

                    StatusText = $"{TranslationService.GetString("StatusPdfSaved")}{saveDialog.FileName}";
                    string method = PdfMethod == PdfConversionMethod.WordInterop
                        ? TranslationService.GetString("ViaWord")
                        : TranslationService.GetString("DirectMethod");
                    MessageBox.Show(
                        $"{TranslationService.GetString("PdfSavedMsg")}{saveDialog.FileName}{TranslationService.GetString("GeneratedVia")}{method})",
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

        public Views.PatientInfo GetPatientInfo()
        {
            return new Views.PatientInfo
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

        public void SetPatientInfo(Views.PatientInfo info)
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}






