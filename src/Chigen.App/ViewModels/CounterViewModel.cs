using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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
                    return "(not set)";
                var display = PatientName;
                if (!string.IsNullOrEmpty(PatientId))
                    display += $" (ID: {PatientId})";
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

        private string _statusText = "Ready";
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

        private bool _hasPatientInfo = true;
        public bool HasPatientInfo
        {
            get => _hasPatientInfo;
            set { _hasPatientInfo = value; OnPropertyChanged(); }
        }

        public void RefreshTemplateSettings()
        {
            var template = TemplateService.LoadTemplate();
            HasPatientInfo = template.ShowPatientInfo
                && (template.ShowPatientId || template.ShowPatientName || template.ShowPatientDob
                    || template.ShowPatientSex || template.ShowPatientDiagnosis || template.ShowPatientAddress
                    || template.ShowPatientPhysician || template.ShowPatientWard || template.ShowPatientPaymentMethod);
        }

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand GenerateDocxCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public CounterViewModel()
        {
            IncrementCommand = new RelayCommand<string>(IncrementCount);
            DecrementCommand = new RelayCommand<string>(DecrementCount);
            UndoCommand = new RelayCommand(HandleUndo);
            ResetCommand = new RelayCommand(HandleReset);
            GenerateDocxCommand = new RelayCommand(HandleGenerateDocx);
            ExportPdfCommand = new RelayCommand(HandleExportPdf);

            RefreshTemplateSettings();
            LoadCellTypes(CounterMode.PeripheralBlood);
            UpdateState();
        }

        public void HandleKeyPress(string key)
        {
            if (_counterService.TryCount(key))
            {
                UpdateState();
                var entry = _counterService.State.Entries.FirstOrDefault(e => e.CellType.Key == key);
                if (entry != null)
                    StatusText = $"Counted: {entry.CellType.Name}";
            }
        }

        public void HandleUndo()
        {
            if (_counterService.TryUndo())
            {
                UpdateState();
                StatusText = "Undo last count";
            }
        }

        public void HandleReset()
        {
            var result = MessageBox.Show("Reset all counts?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _counterService.Reset();
                UpdateState();
                StatusText = "Counter reset";
            }
        }

        public void HandleGenerateDocx()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    DefaultExt = ".docx",
                    FileName = $"Differential_Report_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var letterhead = TemplateService.LoadLetterhead();
                    var template = TemplateService.LoadTemplate();
                    var generator = new DocxGenerator(letterhead, template);
                    var specimen = new SpecimenInfo
                    {
                        Type = CurrentMode == CounterMode.PeripheralBlood ? "Peripheral Blood" : "Bone Marrow Aspirate"
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
                    StatusText = $"DOCX saved: {saveDialog.FileName}";
                    MessageBox.Show($"Document saved to:\n{saveDialog.FileName}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to generate document:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void HandleExportPdf()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF Document (*.pdf)|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = $"Differential_Report_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var letterhead = TemplateService.LoadLetterhead();
                    var template = TemplateService.LoadTemplate();
                    var specimen = new SpecimenInfo
                    {
                        Type = CurrentMode == CounterMode.PeripheralBlood ? "Peripheral Blood" : "Bone Marrow Aspirate"
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

                    StatusText = $"PDF saved: {saveDialog.FileName}";
                    string method = PdfMethod == PdfConversionMethod.WordInterop ? "via Word" : "direct";
                    MessageBox.Show($"PDF saved to:\n{saveDialog.FileName}\n(Generated {method})", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to export PDF:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SwitchMode(CounterMode mode)
        {
            _counterService.LoadCellTypes(mode);
            ApplySavedHotkeys();
            RefreshEntries();
            StatusText = mode == CounterMode.PeripheralBlood ? "Switched to Peripheral Blood mode" : "Switched to Bone Marrow mode";
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
                StatusText = $"Counted: {cellTypeId}";
            }
        }

        public void DecrementCount(string? cellTypeId)
        {
            if (cellTypeId == null) return;
            if (_counterService.TryUndoManual(cellTypeId))
            {
                UpdateState();
                StatusText = $"Decremented: {cellTypeId}";
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
