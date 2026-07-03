# Chigen â€” Project Plan

A desktop application for cell differential counting with automated medical document (DOCX/PDF) generation. Fully offline.

---

## 1. Tech Stack & Architecture

| Layer | Technology |
|---|---|
| UI Framework | **WPF** with MVVM (`CommunityToolkit.Mvvm`) |
| Core Logic | .NET 8 Class Library |
| Document Generation | **OpenXML SDK** (`DocumentFormat.OpenXml`) |
| PDF Export | **Microsoft Word Interop** (SaveAs PDF) |
| Data / Config | Local **JSON files** (no database, no network) |

**Solution layout:**

```
Chigen/
â”œâ”€â”€ Chigen.sln
â”œâ”€â”€ docs/
â”‚   â”œâ”€â”€ plan.md
â”‚   â”œâ”€â”€ code.md
â”‚   â””â”€â”€ appearance.md
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ Chigen.App/                    # WPF App (net8.0-windows)
â”‚   â”‚   â”œâ”€â”€ App.xaml / App.xaml.cs
â”œâ”€â”€ Translations.cs             # i18n: bindable UI strings resource
â”‚   â”‚   â”œâ”€â”€ MainWindow.xaml / .cs
â”‚   â”‚   â”œâ”€â”€ ViewModels/
â”‚   â”‚   â”‚   â”œâ”€â”€ CounterViewModel.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ SettingsViewModel.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ HotkeySettingsViewModel.cs
â”‚   â”‚   â”‚   â””â”€â”€ HotkeySettingsItem.cs
â”‚   â”‚   â””â”€â”€ Views/
â”‚   â”‚       â”œâ”€â”€ PatientInfoWindow.xaml / .cs
â”‚   â”‚       â”œâ”€â”€ SettingsWindow.xaml / .cs
â”‚   â”‚       â”œâ”€â”€ HotkeySettingsWindow.xaml / .cs
â”‚   â”‚       â”œâ”€â”€ ConclusionWindow.xaml / .cs
â”‚   â”‚       â””â”€â”€ RecommendationsWindow.xaml / .cs
â”‚   â”œâ”€â”€ Chigen.Core/                   # Business Logic (net8.0)
â”‚   â”‚   â”œâ”€â”€ Models/
â”‚   â”‚   â”‚   â”œâ”€â”€ CellType.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ CellCountEntry.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ CounterState.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ CounterMode.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ UndoAction.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ PatientInfo.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ SpecimenInfo.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ DocumentTemplate.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ LetterheadConfig.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ HotkeyMappingEntry.cs
â”‚   â”‚   â”‚   â””â”€â”€ KeyConverter.cs
â”‚   â”‚   â””â”€â”€ Services/
â”‚   â”‚       â”œâ”€â”€ CellTypeProvider.cs
â”‚   â”‚       â”œâ”€â”€ CounterService.cs
â”‚   â”‚       â””â”€â”€ TemplateService.cs
â”‚   â”œâ”€â”€ Chigen.DocumentExport/         # Document Generation (net8.0)
â”‚   â”‚   â”œâ”€â”€ DocxGenerator.cs
â”‚   â”‚   â”œâ”€â”€ DirectPdfGenerator.cs
â”‚   â”‚   â””â”€â”€ PdfConverter.cs
â”‚   â””â”€â”€ Chigen.Tests/                  # xUnit Tests (net8.0)
â”‚       â”œâ”€â”€ Models/
â”‚       â”œâ”€â”€ Services/
â”‚       â””â”€â”€ DocumentExport/
â””â”€â”€ README.md
```

---

## 2. Cell Differential Counter

### Hotkey Mapping

| Key | PB Mode | BM Mode (base) |
|-----|---------|----------------|
| `1` | Neutrophil (Seg) | Neutrophil (Seg) |
| `2` | Band Neutrophil | Band Neutrophil |
| `3` | Lymphocyte | Lymphocyte |
| `4` | Monocyte | Monocyte |
| `5` | Eosinophil | Eosinophil |
| `6` | Basophil | Basophil |
| `7` | Metamyelocyte | Metamyelocyte |
| `8` | Myelocyte | Myelocyte |
| `9` | Promyelocyte | Promyelocyte |
| `0` | Blast | Blast |

**BM mode** adds secondary cell types (NRBC, Megakaryocyte, Plasma cell, Lymphoblast, Monoblast, Erythroblast variants, etc.) navigable via **Shift+key** or **arrow keys** to scroll between cell groups.

All mappings are stored in JSON and fully configurable.

### Other Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Backspace` | Undo last count |
| `R` | Reset all counts (with confirmation) |
| `Ctrl+G` | Generate DOCX |
| `Ctrl+P` | Export PDF |
| `Ctrl+Z` | Undo |
| `Ctrl+Q` | Exit application |
| `Tab` / `Shift+Tab` | Navigate cell groups (BM mode) |

### Features (current)

- **Indonesian language support** â€” full UI and document translation (English/Indonesia toggle in Settings)
- Real-time count display with running total and percentages
- Per-cell-type reference ranges (configurable)
- Mode toggle: Peripheral Blood / Bone Marrow
- Undo stack (unlimited depth)
- Manual adjustment via +/- buttons on each cell row
- Configurable target total (100, 200, or custom)
- Visual alert when target total is reached

### UI Layout (Main Window)

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Chigen â€” Cell Differential Counter                      â”‚
â”‚ Status: Ready                                           â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ [Patient: John Doe (ID: P12345)]  Mode: [â— PB  â—‹ BM]   â”‚
â”‚                                                 2026-06-30â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Key â”‚ Cell Type          â”‚ Count â”‚   %  â”‚ Ref Rangeâ”‚ +/- â”‚
â”‚â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”¤
â”‚  1  â”‚ Neutrophil         â”‚  45   â”‚ 45%  â”‚ 40-80    â”‚ âˆ’ + â”‚
â”‚  2  â”‚ Band Neutrophil    â”‚   5   â”‚  5%  â”‚ 0-10     â”‚ âˆ’ + â”‚
â”‚  3  â”‚ Lymphocyte         â”‚  30   â”‚ 30%  â”‚ 20-40    â”‚ âˆ’ + â”‚
â”‚  4  â”‚ Monocyte           â”‚   8   â”‚  8%  â”‚ 2-10     â”‚ âˆ’ + â”‚
â”‚  5  â”‚ Eosinophil         â”‚   4   â”‚  4%  â”‚ 1-6      â”‚ âˆ’ + â”‚
â”‚  6  â”‚ Basophil           â”‚   1   â”‚  1%  â”‚ 0-2      â”‚ âˆ’ + â”‚
â”‚  7  â”‚ Metamyelocyte      â”‚   3   â”‚  3%  â”‚ 0-5      â”‚ âˆ’ + â”‚
â”‚  8  â”‚ Myelocyte          â”‚   2   â”‚  2%  â”‚ 0-3      â”‚ âˆ’ + â”‚
â”‚  9  â”‚ Promyelocyte       â”‚   1   â”‚  1%  â”‚ 0-2      â”‚ âˆ’ + â”‚
â”‚  0  â”‚ Blast              â”‚   1   â”‚  1%  â”‚ 0-1      â”‚ âˆ’ + â”‚
â”‚â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”¤
â”‚  â€“  â”‚ TOTAL              â”‚ 100   â”‚ 100% â”‚          â”‚     â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ [Generate DOCX] [Export PDF] [Undo] [Reset]             â”‚
â”‚ [Conclusion] [Recommendations] [Hotkeys] [Settings] [Exit] â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Ready  |  Keys: 0-9,A-Z=Count | F5,F6=Select |         â”‚
â”‚         Del=Undo | Ctrl+Q=Exit                          â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

The patient badge (green highlight) is hidden via `Visibility` binding when all Patient Info Fields are disabled in settings. The mode selector and date remain visible.

---

## 3. Document Generator (Letterhead + Template System)

### Letterhead Configuration (JSON)

```json
{
  "institutionName": "City General Hospital",
  "department": "Hematology Laboratory",
  "address": "123 Main Street, Cityville",
  "phone": "+1-555-0100",
  "email": "lab@cghospital.com",
  "logoPath": "C:\\Chigen\\logos\\hospital_logo.png",
  "footerText": "Confidential â€” For clinical use only"
}
```

### Document Sections

1. **Header/Letterhead** â€” Institution name, department, address, optional logo. Thick 3pt bottom border ("letterhead rule") painted directly on the address line. Toggled by `ShowLetterhead`.
2. **Report Title** â€” "HEMATOLOGY LABORATORY REPORT" as standalone heading (not part of letterhead).
3. **Patient Info** â€” Dynamic table with per-field toggles: Patient ID, Name, DOB, Sex, Diagnosis, Address, Physician, Ward, Payment Method. Each field is independently toggleable via Settings. Disabled fields are omitted; rows with both sides disabled are skipped entirely.
4. **Specimen Info** â€” Type (PB/BM), collection date, received date
5. **Differential Table** â€” Formatted table with key, cell type, count, percentage, reference range (range column toggleable via `ShowReferenceRanges`). Data cells use 8pt font.
6. **Conclusion** â€” Free-text interpretation section (toggleable via `ShowConclusion`), 9pt heading / 8pt body.
7. **Recommendations** â€” Free-text section (only shown when non-empty), same font as Conclusion.
8. **Footer** â€” Generation date, footer text, signature line (toggleable via `ShowFooter`), all at 8pt.

### Template System

- Pre-defined templates for PB and BM stored as JSON under `Templates/`
- Placeholders: `{{PatientName}}`, `{{Date}}`, `{{DifferentialTable}}`, `{{Conclusion}}`, etc.
- Users can save/load custom templates via the Settings window

---

## 4. Export Engine

### DOCX Generation

- Uses `DocumentFormat.OpenXml` to build a `.docx` from scratch
- Applies professional formatting: fonts (Calibri), borders, alignment, cell shading for headers
- Inserts letterhead image if configured
- Replaces all `{{placeholders}}` with live data

### PDF Conversion

- Opens the generated DOCX via Microsoft Word Interop (`Microsoft.Office.Interop.Word`)
- Calls `Document.SaveAs(...)` with `WdSaveFormat.wdFormatPDF`
- Cleans up Word process after export
- **Requirement:** Microsoft Word must be installed on the target machine

### Export Flow

```
User clicks [Generate DOCX] or [Export PDF]
       â”‚
       â–¼
Validate: ShowLetterhead && empty LogoPath?
       â”‚
       â”œâ”€â”€ Yes â”€â”€â–º Show error: "The letterhead logo is required.
       â”‚                    Go to settings to set the logo."
       â”‚
       â–¼  No
Load LetterheadConfig + DocumentTemplate via TemplateService
       â”‚
       â”œâ”€â”€ DocxGenerator.Create(...) â”€â”€â–º .docx file on disk
       â”‚                                     â”‚
       â”‚                               [Export PDF clicked?]
       â”‚                                     â”‚
       â”‚                                    Yes
       â”‚                                     â”‚
       â”‚                                     â–¼
       â”‚                              PdfConverter.Convert(docxPath)
       â”‚                              â””â”€ InvokeWordMethod helpers
       â”‚                                 unwrap TargetInvocationException
       â”‚                                 â†’ clear "Word.{method} failed:" msg
       â”‚                                     â”‚
       â”‚                                     â–¼
       â”‚                               .pdf file on disk
       â”‚
       â–¼
Open file / folder / prompt user
```

---

## 5. Data Flow

```
Keypress [1-0] / Click
       â”‚
       â–¼
CounterService.Update(cellTypeId)
   â”€â”€ Update CounterState (increment, recalc percentages)
   â”€â”€ Push to undo stack
       â”‚
       â–¼
CounterViewModel (ObservableObject)
   â”€â”€ ObservableCollection<CellCountEntry> auto-updates UI
       â”‚
       â–¼
User triggers export (HandleGenerateDocx / HandleExportPdf)
       â”‚
       â–¼
Build PatientInfo + SpecimenInfo from ViewModel state
Load LetterheadConfig + DocumentTemplate via TemplateService
       â”‚
       â”œâ”€â”€ DocxGenerator.Create(...) â”€â”€â–º .docx
       â””â”€â”€ PdfConverter.Convert(...)  â”€â”€â–º .pdf
            â””â”€â”€ DirectPdfGenerator (fallback) or Word Interop
```

---

## 6. Edge Cases & Constraints

| Concern | Solution |
|---|---|
| No internet | All NuGet resolved at build time. No runtime HTTP calls. |
| Word not installed | Detect at startup; disable PDF button + show user message; offer alternative fallback (future: MigraDoc integration) |
| Keyboard conflicts | `PreviewKeyDown` event; mark event as `Handled` for mapped keys |
| Zero total (before any count) | Lock export buttons until count > 0 |
| Very large differential (200+ cells) | Percentages calc on running total; no upper limit |
| Missing letterhead config | Ship sensible defaults; prompt on first launch |
| Logo not configured | Throws `InvalidOperationException`: "The letterhead logo is required. Go to settings to set the logo." when `ShowLetterhead` is enabled but `LogoPath` is empty |
| Template JSON corrupt | Validate on load; fall back to embedded default; notify user |
| Multiple instances | Single-instance app (mutex) to avoid file conflicts |
| Undo after export | Undo stack preserved; re-export uses latest state |

---

## 7. Implementation Order

| Step | Task | Est. Effort |
|------|------|-------------|
| 1 | Scaffold solution: `dotnet new sln`, create 3 projects, add NuGet references | 1h |
| 2 | **Core Models** â€” `CellType`, `CounterState`, `DifferentialResult`, `PatientInfo`, `LetterheadConfig`, `DocumentTemplate` | 2h |
| 3 | **CounterService** â€” key-to-cell mapping, increment/decrement/undo/reset, percentage calculation, undo stack | 3h |
| 4 | **TemplateService** â€” JSON serialization/deserialization, load/save templates & letterhead, validation | 2h |
| 5 | **WPF Shell** â€” MainWindow, App.xaml, `CounterViewModel`, basic counter display with `ItemsControl` | 4h |
| 6 | **PB/BM modes** â€” mode toggle UI, extended cell list, Tab/Shift+Tab group navigation | 3h |
| 7 | **Keyboard handling** â€” `PreviewKeyDown` wiring, modifier keys, undo/reset shortcuts | 2h |
| 8 | **DocxGenerator** â€” build differential table, insert letterhead, replace placeholders, save .docx | 4h |
| 9 | **PdfConverter** â€” Word Interop automation, `SaveAs PDF`, process cleanup, error handling | 3h |
| 10 | **ExportService** â€” coordinate export flow, file dialogs, progress indicator | 2h |
| 11 | **Settings Window** â€” letterhead editor, key remapping, default mode, template management, per-field patient info toggles | 4h |
| 12 | **Error handling, edge cases** â€” Word missing, zero state, corrupt files, single-instance | 2h |
| 13 | **Offline validation** â€” disconnect network, test full workflow end-to-end | 1h |
| 14 | **Publish** â€” self-contained single-file `.exe` (`dotnet publish`) | 1h |

**Total: ~34 hours**

---

## 8. NuGet Packages

| Package | Purpose |
|---|---|
| `DocumentFormat.OpenXml` (latest 2.x / 3.x) | Create and manipulate DOCX files |
| `CommunityToolkit.Mvvm` (latest) | MVVM framework (ObservableObject, RelayCommand) |
| `Microsoft.Office.Interop.Word` | PDF conversion via Word (already installed with Office; NuGet for PIA embedding) |
| `System.Text.Json` (built-in) | JSON config serialization |

No package requires a network connection at runtime.

---

## 9. Future Enhancements (Post-MVP)

- Direct PDF generation via **MigraDoc/PDFsharp** (no Word dependency)
- Cumulative statistics / history log (store past differentials locally)
- Barcode/QR code scanning for patient ID
- Print directly from app
~~- Multi-language support (labels via JSON resource files)~~ **Implemented** via `TranslationService` + WPF resource bindings (see `i18n.md`)
- Automated flagging (highlight values outside reference range)
- Bone marrow aspirate report with additional sections (megakaryocytes, iron stores, etc.)

