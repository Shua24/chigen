using System.ComponentModel;
using System.Runtime.CompilerServices;
using Chigen.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chigen.App
{
    public class Translations : ObservableObject
    {
        public Translations()
        {
            TranslationService.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            var props = typeof(Translations)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
                OnPropertyChanged(prop.Name);
        }

        // -- MainWindow --
        public string MainTitle => TranslationService.GetString("MainTitle");
        public string PatientLabel => TranslationService.GetString("PatientLabel");
        public string Edit => TranslationService.GetString("Edit");
        public string Mode => TranslationService.GetString("Mode");
        public string PeripheralBlood => TranslationService.GetString("PeripheralBlood");
        public string BoneMarrow => TranslationService.GetString("BoneMarrow");
        public string Total => TranslationService.GetString("Total");
        public string GenerateDocx => TranslationService.GetString("GenerateDocx");
        public string ExportPdf => TranslationService.GetString("ExportPdf");
        public string UndoDel => TranslationService.GetString("UndoDel");
        public string Reset => TranslationService.GetString("Reset");
        public string Conclusion => TranslationService.GetString("Conclusion");
        public string Recommendations => TranslationService.GetString("Recommendations");
        public string Hotkeys => TranslationService.GetString("Hotkeys");
        public string Settings => TranslationService.GetString("Settings");
        public string Exit => TranslationService.GetString("Exit");
        public string KeysHint => TranslationService.GetString("KeysHint");
        public string Keys0to9 => TranslationService.GetString("Keys0to9");
        public string DelUndo => TranslationService.GetString("DelUndo");
        public string RReset => TranslationService.GetString("RReset");
        public string CtrlG => TranslationService.GetString("CtrlG");
        public string CtrlP => TranslationService.GetString("CtrlP");

        // -- SettingsWindow --
        public string SettingsTitle => TranslationService.GetString("SettingsTitle");
        public string LetterheadConfig => TranslationService.GetString("LetterheadConfig");
        public string InstitutionName => TranslationService.GetString("InstitutionName");
        public string Department => TranslationService.GetString("Department");
        public string Address => TranslationService.GetString("Address");
        public string Phone => TranslationService.GetString("Phone");
        public string Email => TranslationService.GetString("Email");
        public string Logo => TranslationService.GetString("Logo");
        public string LogoPlacement => TranslationService.GetString("LogoPlacement");
        public string FooterText => TranslationService.GetString("FooterText");
        public string Browse => TranslationService.GetString("Browse");
        public string UseLetterhead => TranslationService.GetString("UseLetterhead");
        public string PatientInfoFields => TranslationService.GetString("PatientInfoFields");
        public string ShowPatientId => TranslationService.GetString("ShowPatientId");
        public string ShowPatientName => TranslationService.GetString("ShowPatientName");
        public string ShowPatientDob => TranslationService.GetString("ShowPatientDob");
        public string ShowPatientSex => TranslationService.GetString("ShowPatientSex");
        public string ShowPatientDiagnosis => TranslationService.GetString("ShowPatientDiagnosis");
        public string ShowPatientAddress => TranslationService.GetString("ShowPatientAddress");
        public string ShowPatientPhysician => TranslationService.GetString("ShowPatientPhysician");
        public string ShowPatientWard => TranslationService.GetString("ShowPatientWard");
        public string ShowPatientPayment => TranslationService.GetString("ShowPatientPayment");
        public string Save => TranslationService.GetString("Save");
        public string Cancel => TranslationService.GetString("Cancel");
        public string Language => TranslationService.GetString("Language");
        public string LogoTop => TranslationService.GetString("LogoTop");
        public string LogoSide => TranslationService.GetString("LogoSide");
        public string Theme => TranslationService.GetString("Theme");
        public string ThemeLightMode => TranslationService.GetString("ThemeLightMode");
        public string ThemeDarkMode => TranslationService.GetString("ThemeDarkMode");

        // -- PatientInfoWindow --
        public string PatientInfoTitle => TranslationService.GetString("PatientInfoTitle");
        public string EnterPatientDetails => TranslationService.GetString("EnterPatientDetails");
        public string PatientName => TranslationService.GetString("PatientName");
        public string PatientId => TranslationService.GetString("PatientId");
        public string DateOfBirth => TranslationService.GetString("DateOfBirth");
        public string Sex => TranslationService.GetString("Sex");
        public string Diagnosis => TranslationService.GetString("Diagnosis");
        public string PatientAddress => TranslationService.GetString("PatientAddress");
        public string Physician => TranslationService.GetString("Physician");
        public string Ward => TranslationService.GetString("Ward");
        public string PaymentMethod => TranslationService.GetString("PaymentMethod");
        public string Start => TranslationService.GetString("Start");
        public string Skip => TranslationService.GetString("Skip");
        public string PatientInfoHelp1 => TranslationService.GetString("PatientInfoHelp1");
        public string PatientInfoHelp2 => TranslationService.GetString("PatientInfoHelp2");

        // -- ConclusionWindow --
        public string ConclusionTitle => TranslationService.GetString("ConclusionTitle");
        public string Ok => TranslationService.GetString("Ok");

        // -- RecommendationsWindow --
        public string RecommendationsTitle => TranslationService.GetString("RecommendationsTitle");

        // -- HotkeySettingsWindow --
        public string HotkeySettingsTitle => TranslationService.GetString("HotkeySettingsTitle");
        public string ConfigureHotkeys => TranslationService.GetString("ConfigureHotkeys");
        public string HotkeyHelpText => TranslationService.GetString("HotkeyHelpText");
        public string TabPeripheralBlood => TranslationService.GetString("TabPeripheralBlood");
        public string TabBoneMarrow => TranslationService.GetString("TabBoneMarrow");
        public string ColKey => TranslationService.GetString("ColKey");
        public string ColCellType => TranslationService.GetString("ColCellType");
        public string ColConflict => TranslationService.GetString("ColConflict");
        public string ConflictText => TranslationService.GetString("ConflictText");

        // -- Export messages --
        public string DocxFilter => TranslationService.GetString("DocxFilter");
        public string PdfFilter => TranslationService.GetString("PdfFilter");
        public string SaveDocxTitle => TranslationService.GetString("SaveDocxTitle");
        public string SavePdfTitle => TranslationService.GetString("SavePdfTitle");
    }
}
