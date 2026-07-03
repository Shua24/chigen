using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chigen.Core.Services
{
    public static class TranslationService
    {
        private static readonly Dictionary<string, string> _en = new();
        private static readonly Dictionary<string, string> _id = new();

        private static string _currentLanguage = "";
        private static Dictionary<string, string> _current = _en;

        public static event Action? LanguageChanged;

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage == value) return;
                _currentLanguage = value;
                _current = value switch
                {
                    "id" => _id,
                    _ => _en,
                };
                LanguageChanged?.Invoke();
            }
        }

        public static string GetString(string key)
        {
            return _current.TryGetValue(key, out var val) ? val : key;
        }


        static TranslationService()
        {
            LoadEnglish();
            LoadIndonesian();
            CurrentLanguage = "en";
        }

        private static void LoadEnglish()
        {
            // MainWindow
            _en["MainTitle"] = "Chigen — Cell Differential Counter";
            _en["PatientLabel"] = "Patient:";
            _en["Edit"] = "Edit";
            _en["Mode"] = "Mode:";
            _en["PeripheralBlood"] = "Peripheral Blood";
            _en["BoneMarrow"] = "Bone Marrow";
            _en["Total"] = "TOTAL";
            _en["GenerateDocx"] = "Generate DOCX";
            _en["ExportPdf"] = "Export PDF";
            _en["UndoDel"] = "Undo (Del)";
            _en["Reset"] = "Reset";
            _en["Conclusion"] = "Conclusion";
            _en["Recommendations"] = "Recommendations";
            _en["Hotkeys"] = "Hotkeys";
            _en["Settings"] = "Settings";
            _en["Exit"] = "Exit";
            _en["KeysHint"] = "Keys: ";
            _en["Keys0to9"] = "0-9,A-Z=Count  |  ";
            _en["DelUndo"] = "Del=Undo  |  ";
            _en["RReset"] = "R=Reset  |  ";
            _en["CtrlG"] = "Ctrl+G=DOCX  |  ";
            _en["CtrlP"] = "Ctrl+P=PDF";
            _en["StatusReady"] = "Ready";
            _en["StatusSwitchedPb"] = "Switched to Peripheral Blood mode";
            _en["StatusSwitchedBm"] = "Switched to Bone Marrow mode";
            _en["StatusCounted"] = "Counted: ";
            _en["StatusDecremented"] = "Decremented: ";
            _en["StatusReset"] = "Counts reset";
            _en["ResetConfirm"] = "Reset all counts?";
            _en["ResetConfirmCaption"] = "Reset";
            _en["StatusDocxSaved"] = "DOCX saved: ";
            _en["StatusPdfSaved"] = "PDF saved: ";
            _en["DocxSavedMsg"] = "Document saved to:\n";
            _en["PdfSavedMsg"] = "PDF saved to:\n";
            _en["ExportComplete"] = "Export Complete";
            _en["ExportError"] = "Failed to export";
            _en["Error"] = "Error";
            _en["GeneratedVia"] = "\n(Generated ";
            _en["ViaWord"] = "via Word";
            _en["DirectMethod"] = "direct";
            _en["PatientNotSet"] = "(not set)";
            _en["PatientDisplayFmt"] = " (ID: {0})";
            _en["LetterheadLogoRequired"] = "If you toggled the letterhead on, the letterhead logo is required. Go to settings to set the logo.";

            // SettingsWindow
            _en["SettingsTitle"] = "Settings — Letterhead Configuration";
            _en["LetterheadConfig"] = "Letterhead Configuration";
            _en["InstitutionName"] = "Institution Name:";
            _en["Department"] = "Department:";
            _en["Address"] = "Address:";
            _en["Phone"] = "Phone:";
            _en["Email"] = "Email:";
            _en["Logo"] = "Logo:";
            _en["LogoPlacement"] = "Logo Placement:";
            _en["FooterText"] = "Footer Text:";
            _en["Browse"] = "Browse...";
            _en["UseLetterhead"] = "Use letterhead in exported documents";
            _en["PatientInfoFields"] = "Patient Info Fields";
            _en["ShowPatientId"] = "Patient ID";
            _en["ShowPatientName"] = "Patient Name";
            _en["ShowPatientDob"] = "Date of Birth";
            _en["ShowPatientSex"] = "Sex";
            _en["ShowPatientDiagnosis"] = "Diagnosis";
            _en["ShowPatientAddress"] = "Address";
            _en["ShowPatientPhysician"] = "Physician";
            _en["ShowPatientWard"] = "Ward";
            _en["ShowPatientPayment"] = "Payment Method";
            _en["Save"] = "Save";
            _en["Cancel"] = "Cancel";
            _en["SavedMessage"] = "All settings saved. You may continue to do the cell differentiation process.";
            _en["SavedCaption"] = "Saved";
            _en["Language"] = "Language:";
            _en["LogoTop"] = "Top (centered above text)";
            _en["LogoSide"] = "Side (aligned left of text)";

            // PatientInfoWindow
            _en["PatientInfoTitle"] = "Patient Information";
            _en["EnterPatientDetails"] = "Enter Patient Details";
            _en["PatientName"] = "Patient Name:";
            _en["PatientId"] = "Patient ID:";
            _en["DateOfBirth"] = "Date of Birth:";
            _en["Sex"] = "Sex:";
            _en["Diagnosis"] = "Diagnosis:";
            _en["PatientAddress"] = "Address:";
            _en["Physician"] = "Physician:";
            _en["Ward"] = "Ward:";
            _en["PaymentMethod"] = "Payment Method:";
            _en["Start"] = "Start";
            _en["Skip"] = "Skip";
            _en["PatientInfoHelp1"] = "Fill in patient details and press Start to begin differential counting.";
            _en["PatientInfoHelp2"] = "Hotkeys (0-9, A-Z) will be available on the counter screen.";

            // ConclusionWindow
            _en["ConclusionTitle"] = "Conclusion / Interpretation";
            _en["Ok"] = "OK";

            // RecommendationsWindow
            _en["RecommendationsTitle"] = "Recommendations";

            // HotkeySettingsWindow
            _en["HotkeySettingsTitle"] = "Hotkey Configuration";
            _en["ConfigureHotkeys"] = "Configure Cell Hotkeys";
            _en["HotkeyHelpText"] = "Select a cell type and press a key (0-9 or A-Z) to assign. Conflicts are highlighted in red.";
            _en["TabPeripheralBlood"] = "Peripheral Blood";
            _en["TabBoneMarrow"] = "Bone Marrow";
            _en["ColKey"] = "Key";
            _en["ColCellType"] = "Cell Type";
            _en["ColConflict"] = "Conflict";
            _en["ConflictText"] = "? Conflict";

            // Export messages
            _en["DocxFilter"] = "Word Documents|*.docx";
            _en["PdfFilter"] = "PDF Files|*.pdf";
            _en["SaveDocxTitle"] = "Save Word Document";
            _en["SavePdfTitle"] = "Save PDF Document";
            // Report strings
            _en["ReportTitle"] = "Hematology Laboratory Report";
            _en["CellDifferentialCount"] = "CELL DIFFERENTIAL COUNT";
            _en["PatientInfoSection"] = "Patient Information";
            _en["PatientNameLabel"] = "Name:";
            _en["PatientIdLabel"] = "Patient ID:";
            _en["DateOfBirthLabel"] = "Date of Birth:";
            _en["SexLabel"] = "Sex:";
            _en["DiagnosisLabel"] = "Diagnosis:";
            _en["PatientAddressLabel"] = "Address:";
            _en["PhysicianLabel"] = "Physician:";
            _en["WardLabel"] = "Ward:";
            _en["PaymentMethodLabel"] = "Payment Method:";
            _en["SpecimenTypeLabel"] = "Specimen Type:";
            _en["SampleCollectionDate"] = "Sample Collection Date:";
            _en["ColCount"] = "Count";
            _en["ColPercent"] = "%";
            _en["ColRefRange"] = "Ref. Range";
            _en["ConclusionInterpretation"] = "Conclusion / Interpretation:";
            _en["NoAbnormalities"] = "No abnormalities detected.";
            _en["RecommendationsLabel"] = "Recommendations:";
            _en["ReportGenerated"] = "Report generated:";
            _en["PathologistSignature"] = "Pathologist Signature";
            _en["report_PatientId"] = "Patient ID";
            _en["report_PatientName"] = "Patient Name";
            _en["report_DateOfBirth"] = "Date of Birth";
            _en["report_Physician"] = "Physician";
            _en["report_Sex"] = "Sex";
            _en["report_Ward"] = "Ward";
            _en["report_Diagnosis"] = "Diagnosis";
            _en["report_PaymentMethod"] = "Payment Method";
            _en["report_Address"] = "Address";
            _en["report_CollectionDate"] = "Collection Date";
            _en["report_Specimen"] = "Specimen";
            _en["report_Received"] = "Received";
            _en["SpecimenTypePeripheralBlood"] = "Peripheral Blood";
            _en["SpecimenTypeBoneMarrowAspirate"] = "Bone Marrow Aspirate";
        }

        private static void LoadIndonesian()
        {
            // MainWindow
            _id["MainTitle"] = "Chigen — Penghitung Diferensial Sel";
            _id["PatientLabel"] = "Pasien:";
            _id["Edit"] = "Ubah";
            _id["Mode"] = "Mode:";
            _id["PeripheralBlood"] = "Darah Tepi";
            _id["BoneMarrow"] = "Sumsum Tulang";
            _id["Total"] = "TOTAL";
            _id["GenerateDocx"] = "Buat DOCX";
            _id["ExportPdf"] = "Ekspor PDF";
            _id["UndoDel"] = "Urungkan (Del)";
            _id["Reset"] = "Setel Ulang";
            _id["Conclusion"] = "Kesimpulan";
            _id["Recommendations"] = "Rekomendasi";
            _id["Hotkeys"] = "Tombol Pintas";
            _id["Settings"] = "Pengaturan";
            _id["Exit"] = "Keluar";
            _id["KeysHint"] = "Tombol: ";
            _id["Keys0to9"] = "0-9,A-Z=Hitungan  |  ";
            _id["DelUndo"] = "Del=Urungkan  |  ";
            _id["RReset"] = "R=Setel Ulang  |  ";
            _id["CtrlG"] = "Ctrl+G=DOCX  |  ";
            _id["CtrlP"] = "Ctrl+P=PDF";
            _id["StatusReady"] = "Siap";
            _id["StatusSwitchedPb"] = "Berpindah ke mode Darah Tepi";
            _id["StatusSwitchedBm"] = "Berpindah ke mode Sumsum Tulang";
            _id["StatusCounted"] = "Terhitung: ";
            _id["StatusDecremented"] = "Dikurangi: ";
            _id["StatusReset"] = "Hitungan diatur ulang";
            _id["ResetConfirm"] = "Atur ulang semua hitungan?";
            _id["ResetConfirmCaption"] = "Setel Ulang";
            _id["StatusDocxSaved"] = "DOCX tersimpan: ";
            _id["StatusPdfSaved"] = "PDF tersimpan: ";
            _id["DocxSavedMsg"] = "Dokumen tersimpan ke:\n";
            _id["PdfSavedMsg"] = "PDF tersimpan ke:\n";
            _id["ExportComplete"] = "Ekspor Selesai";
            _id["ExportError"] = "Gagal mengekspor";
            _id["Error"] = "Kesalahan";
            _id["GeneratedVia"] = "\n(Dibuat ";
            _id["ViaWord"] = "melalui Word";
            _id["DirectMethod"] = "langsung";
            _id["PatientNotSet"] = "(belum diisi)";
            _id["PatientDisplayFmt"] = " (ID: {0})";
            _id["LetterheadLogoRequired"] = "Jika kop surat diaktifkan, logo kop surat diperlukan. Buka Pengaturan untuk mengatur logo.";

            // SettingsWindow
            _id["SettingsTitle"] = "Pengaturan — Konfigurasi Kop Surat";
            _id["LetterheadConfig"] = "Konfigurasi Kop Surat";
            _id["InstitutionName"] = "Nama Institusi:";
            _id["Department"] = "Departemen:";
            _id["Address"] = "Alamat:";
            _id["Phone"] = "Telepon:";
            _id["Email"] = "Email:";
            _id["Logo"] = "Logo:";
            _id["LogoPlacement"] = "Posisi Logo:";
            _id["FooterText"] = "Teks Kaki:";
            _id["Browse"] = "Telusuri...";
            _id["UseLetterhead"] = "Gunakan kop surat di dokumen yang diekspor";
            _id["PatientInfoFields"] = "Bidang Info Pasien";
            _id["ShowPatientId"] = "ID Pasien";
            _id["ShowPatientName"] = "Nama Pasien";
            _id["ShowPatientDob"] = "Tanggal Lahir";
            _id["ShowPatientSex"] = "Jenis Kelamin";
            _id["ShowPatientDiagnosis"] = "Diagnosis";
            _id["ShowPatientAddress"] = "Alamat";
            _id["ShowPatientPhysician"] = "Dokter";
            _id["ShowPatientWard"] = "Ruang Rawat";
            _id["ShowPatientPayment"] = "Metode Pembayaran";
            _id["Save"] = "Simpan";
            _id["Cancel"] = "Batal";
            _id["SavedMessage"] = "Semua pengaturan telah disimpan. Anda dapat melanjutkan proses diferensiasi sel.";
            _id["SavedCaption"] = "Tersimpan";
            _id["Language"] = "Bahasa:";
            _id["LogoTop"] = "Atas (terpusat di atas teks)";
            _id["LogoSide"] = "Samping (rata kiri teks)";

            // PatientInfoWindow
            _id["PatientInfoTitle"] = "Informasi Pasien";
            _id["EnterPatientDetails"] = "Masukkan Data Pasien";
            _id["PatientName"] = "Nama Pasien:";
            _id["PatientId"] = "ID Pasien:";
            _id["DateOfBirth"] = "Tanggal Lahir:";
            _id["Sex"] = "Jenis Kelamin:";
            _id["Diagnosis"] = "Diagnosis:";
            _id["PatientAddress"] = "Alamat:";
            _id["Physician"] = "Dokter:";
            _id["Ward"] = "Ruang Rawat:";
            _id["PaymentMethod"] = "Metode Pembayaran:";
            _id["Start"] = "Mulai";
            _id["Skip"] = "Lewati";
            _id["PatientInfoHelp1"] = "Isi data pasien lalu tekan Mulai untuk memulai penghitungan diferensial.";
            _id["PatientInfoHelp2"] = "Tombol pintas (0-9, A-Z) akan tersedia di layar penghitungan.";

            // ConclusionWindow
            _id["ConclusionTitle"] = "Kesimpulan / Interpretasi";
            _id["Ok"] = "OK";

            // RecommendationsWindow
            _id["RecommendationsTitle"] = "Rekomendasi";

            // HotkeySettingsWindow
            _id["HotkeySettingsTitle"] = "Konfigurasi Tombol Pintas";
            _id["ConfigureHotkeys"] = "Atur Tombol Pintas Sel";
            _id["HotkeyHelpText"] = "Pilih jenis sel dan tekan tombol (0-9 atau A-Z) untuk menetapkannya. Konflik ditandai merah.";
            _id["TabPeripheralBlood"] = "Darah Tepi";
            _id["TabBoneMarrow"] = "Sumsum Tulang";
            _id["ColKey"] = "Tombol";
            _id["ColCellType"] = "Jenis Sel";
            _id["ColConflict"] = "Konflik";
            _id["ConflictText"] = "? Konflik";

            // Export messages
            _id["DocxFilter"] = "Dokumen Word|*.docx";
            _id["PdfFilter"] = "Berkas PDF|*.pdf";
            _id["SaveDocxTitle"] = "Simpan Dokumen Word";
            _id["SavePdfTitle"] = "Simpan Dokumen PDF";
            // Report strings
            _id["ReportTitle"] = "Laporan Laboratorium Hematologi";
            _id["CellDifferentialCount"] = "HITUNGAN DIFERENSIAL SEL";
            _id["PatientInfoSection"] = "Informasi Pasien";
            _id["PatientNameLabel"] = "Nama:";
            _id["PatientIdLabel"] = "ID Pasien:";
            _id["DateOfBirthLabel"] = "Tanggal Lahir:";
            _id["SexLabel"] = "Jenis Kelamin:";
            _id["DiagnosisLabel"] = "Diagnosis:";
            _id["PatientAddressLabel"] = "Alamat:";
            _id["PhysicianLabel"] = "Dokter:";
            _id["WardLabel"] = "Ruang Rawat:";
            _id["PaymentMethodLabel"] = "Metode Pembayaran:";
            _id["SpecimenTypeLabel"] = "Jenis Sampel:";
            _id["SampleCollectionDate"] = "Tanggal Pengambilan Sampel:";
            _id["ColCount"] = "Jumlah";
            _id["ColPercent"] = "%";
            _id["ColRefRange"] = "Rentang Rujukan";
            _id["ConclusionInterpretation"] = "Kesimpulan / Interpretasi:";
            _id["NoAbnormalities"] = "Tidak ditemukan kelainan.";
            _id["RecommendationsLabel"] = "Rekomendasi:";
            _id["ReportGenerated"] = "Laporan dibuat:";
            _id["PathologistSignature"] = "Tanda Tangan Dokter Patologi";
            _id["report_PatientId"] = "ID Pasien";
            _id["report_PatientName"] = "Nama Pasien";
            _id["report_DateOfBirth"] = "Tanggal Lahir";
            _id["report_Physician"] = "Dokter";
            _id["report_Sex"] = "Jenis Kelamin";
            _id["report_Ward"] = "Ruang Rawat";
            _id["report_Diagnosis"] = "Diagnosis";
            _id["report_PaymentMethod"] = "Metode Pembayaran";
            _id["report_Address"] = "Alamat";
            _id["report_CollectionDate"] = "Tanggal Pengambilan";
            _id["report_Specimen"] = "Sampel";
            _id["report_Received"] = "Diterima";
            _id["SpecimenTypePeripheralBlood"] = "Darah Tepi";
            _id["SpecimenTypeBoneMarrowAspirate"] = "Aspirasi Sumsum Tulang";
        }
    }
}
