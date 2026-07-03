using System.ComponentModel;
using System.Runtime.CompilerServices;
using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.App.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private LetterheadConfig _config;
        private DocumentTemplate _template;

        public bool UseLetterhead
        {
            get => _template.ShowLetterhead;
            set { _template.ShowLetterhead = value; OnPropertyChanged(); }
        }

        public bool ShowPatientId
        {
            get => _template.ShowPatientId;
            set { _template.ShowPatientId = value; OnPropertyChanged(); }
        }

        public bool ShowPatientName
        {
            get => _template.ShowPatientName;
            set { _template.ShowPatientName = value; OnPropertyChanged(); }
        }

        public bool ShowPatientDob
        {
            get => _template.ShowPatientDob;
            set { _template.ShowPatientDob = value; OnPropertyChanged(); }
        }

        public bool ShowPatientSex
        {
            get => _template.ShowPatientSex;
            set { _template.ShowPatientSex = value; OnPropertyChanged(); }
        }

        public bool ShowPatientDiagnosis
        {
            get => _template.ShowPatientDiagnosis;
            set { _template.ShowPatientDiagnosis = value; OnPropertyChanged(); }
        }

        public bool ShowPatientAddress
        {
            get => _template.ShowPatientAddress;
            set { _template.ShowPatientAddress = value; OnPropertyChanged(); }
        }

        public bool ShowPatientPhysician
        {
            get => _template.ShowPatientPhysician;
            set { _template.ShowPatientPhysician = value; OnPropertyChanged(); }
        }

        public bool ShowPatientWard
        {
            get => _template.ShowPatientWard;
            set { _template.ShowPatientWard = value; OnPropertyChanged(); }
        }

        public bool ShowPatientPaymentMethod
        {
            get => _template.ShowPatientPaymentMethod;
            set { _template.ShowPatientPaymentMethod = value; OnPropertyChanged(); }
        }

        public string InstitutionName
        {
            get => _config.InstitutionName;
            set { _config.InstitutionName = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get => _config.Department;
            set { _config.Department = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _config.Address;
            set { _config.Address = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _config.Phone;
            set { _config.Phone = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _config.Email;
            set { _config.Email = value; OnPropertyChanged(); }
        }

        public string LogoPath
        {
            get => _config.LogoPath;
            set { _config.LogoPath = value; OnPropertyChanged(); }
        }

        public string FooterText
        {
            get => _config.FooterText;
            set { _config.FooterText = value; OnPropertyChanged(); }
        }

        public LogoPlacement LogoPlacement
        {
            get => _config.LogoPlacement;
            set { _config.LogoPlacement = value; OnPropertyChanged(); }
        }

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set { _selectedLanguage = value; OnPropertyChanged(); }
        }

        public List<KeyValuePair<string, LogoPlacement>> LogoPlacementOptions
        {
            get
            {
                var t = Chigen.Core.Services.TranslationService.GetString;
                return
                [
                    new(t("LogoTop"), LogoPlacement.Top),
                    new(t("LogoSide"), LogoPlacement.Side)
                ];
            }
        }

        public static List<KeyValuePair<string, string>> LanguageOptions { get; } =
        [
            new("English", "en"),
            new("Bahasa Indonesia", "id")
        ];

        public SettingsViewModel()
        {
            _config = TemplateService.LoadLetterhead();
            _template = TemplateService.LoadTemplate();
            _selectedLanguage = TranslationService.CurrentLanguage;
            TranslationService.LanguageChanged += () =>
            {
                OnPropertyChanged(nameof(LogoPlacementOptions));
            };
        }

        public void Save()
        {
            TemplateService.SaveLetterhead(_config);
            TemplateService.SaveTemplate(_template);
            TemplateService.SaveLanguage(SelectedLanguage);
        }

        public void BrowseLogo()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Select Logo Image"
            };
            if (dialog.ShowDialog() == true)
            {
                LogoPath = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
