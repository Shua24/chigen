# Chigen — Project Plan

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
├── Chigen.sln
├── docs/
│   ├── plan.md
│   ├── code.md
│   └── appearance.md
├── src/
│   ├── Chigen.App/                    # WPF App (net8.0-windows)
│   │   ├── App.xaml / App.xaml.cs
│   │   ├── MainWindow.xaml / .cs
│   │   ├── ViewModels/
│   │   │   ├── CounterViewModel.cs
│   │   │   ├── SettingsViewModel.cs
│   │   │   ├── HotkeySettingsViewModel.cs
│   │   │   └── HotkeySettingsItem.cs
│   │   └── Views/
│   │       ├── PatientInfoWindow.xaml / .cs
│   │       ├── SettingsWindow.xaml / .cs
│   │       ├── HotkeySettingsWindow.xaml / .cs
│   │       ├── ConclusionWindow.xaml / .cs
│   │       └── RecommendationsWindow.xaml / .cs
│   ├── Chigen.Core/                   # Business Logic (net8.0)
│   │   ├── Models/
│   │   │   ├── CellType.cs
│   │   │   ├── CellCountEntry.cs
│   │   │   ├── CounterState.cs
│   │   │   ├── CounterMode.cs
│   │   │   ├── UndoAction.cs
│   │   │   ├── PatientInfo.cs
│   │   │   ├── SpecimenInfo.cs
│   │   │   ├── DocumentTemplate.cs
│   │   │   ├── LetterheadConfig.cs
│   │   │   ├── HotkeyMappingEntry.cs
│   │   │   └── KeyConverter.cs
│   │   └── Services/
│   │       ├── CellTypeProvider.cs
│   │       ├── CounterService.cs
│   │       └── TemplateService.cs
│   ├── Chigen.DocumentExport/         # Document Generation (net8.0)
│   │   ├── DocxGenerator.cs
│   │   ├── DirectPdfGenerator.cs
│   │   └── PdfConverter.cs
│   └── Chigen.Tests/                  # xUnit Tests (net8.0)
│       ├── Models/
│       ├── Services/
│       └── DocumentExport/
└── README.md
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
| `Tab` / `Shift+Tab` | Navigate cell groups (BM mode) |

### Features

- Real-time count display with running total and percentages
- Per-cell-type reference ranges (configurable)
- Mode toggle: Peripheral Blood / Bone Marrow
- Undo stack (unlimited depth)
- Manual adjustment via +/- buttons on each cell row
- Configurable target total (100, 200, or custom)
- Visual alert when target total is reached

### UI Layout (Main Window)

```
┌─────────────────────────────────────────────────────────┐
│ Chigen — Cell Differential Counter                      │
│ Status: Ready                                           │
├─────────────────────────────────────────────────────────┤
│ [Patient: John Doe (ID: P12345)]  Mode: [● PB  ○ BM]   │
│                                                 2026-06-30│
├─────────────────────────────────────────────────────────┤
│ Key │ Cell Type          │ Count │   %  │ Ref Range│ +/- │
│─────┼────────────────────┼───────┼──────┼──────────┼─────┤
│  1  │ Neutrophil         │  45   │ 45%  │ 40-80    │ − + │
│  2  │ Band Neutrophil    │   5   │  5%  │ 0-10     │ − + │
│  3  │ Lymphocyte         │  30   │ 30%  │ 20-40    │ − + │
│  4  │ Monocyte           │   8   │  8%  │ 2-10     │ − + │
│  5  │ Eosinophil         │   4   │  4%  │ 1-6      │ − + │
│  6  │ Basophil           │   1   │  1%  │ 0-2      │ − + │
│  7  │ Metamyelocyte      │   3   │  3%  │ 0-5      │ − + │
│  8  │ Myelocyte          │   2   │  2%  │ 0-3      │ − + │
│  9  │ Promyelocyte       │   1   │  1%  │ 0-2      │ − + │
│  0  │ Blast              │   1   │  1%  │ 0-1      │ − + │
│─────┼────────────────────┼───────┼──────┼──────────┼─────┤
│  –  │ TOTAL              │ 100   │ 100% │          │     │
├─────────────────────────────────────────────────────────┤
│ [Generate DOCX] [Export PDF] [Undo] [Reset]             │
│ [Conclusion] [Recommendations] [Hotkeys] [Settings]     │
├─────────────────────────────────────────────────────────┤
│ Ready  |  Keys: 0-9,A-Z=Count | Del=Undo | R=Reset ... │
└─────────────────────────────────────────────────────────┘
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
  "footerText": "Confidential — For clinical use only"
}
```

### Document Sections

1. **Header** — Letterhead (institution name, logo, address, contact) — toggled by `ShowLetterhead`
2. **Patient Info** — Dynamic table with per-field toggles: Patient ID, Name, DOB, Sex, Diagnosis, Address, Physician, Ward, Payment Method. Each field is independently toggleable via Settings. Disabled fields are omitted; rows with both sides disabled are skipped entirely.
3. **Specimen Info** — Type (PB/BM), collection date, received date
4. **Differential Table** — Formatted table with key, cell type, count, percentage, reference range (range column toggleable via `ShowReferenceRanges`)
5. **Conclusion** — Free-text interpretation section (toggleable via `ShowConclusion`)
6. **Recommendations** — Free-text section (only shown when non-empty)
7. **Footer** — Generation date, footer text, signature line (toggleable via `ShowFooter`)

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
       │
       ▼
ExportService.ReadCounterData()
       │
       ▼
DocxGenerator.Create(template, data) ──► .docx file on disk
       │                                     │
       │                               [Export PDF clicked?]
       │                                     │
       │                                    Yes
       │                                     │
       │                                     ▼
       │                              PdfConverter.Convert(docxPath)
       │                                     │
       │                                     ▼
       │                               .pdf file on disk
       │
       ▼
Open file / folder / prompt user
```

---

## 5. Data Flow

```
Keypress [1-0] / Click
       │
       ▼
CounterService.Update(cellTypeId)
   ── Update CounterState (increment, recalc percentages)
   ── Push to undo stack
       │
       ▼
CounterViewModel (ObservableObject)
   ── ObservableCollection<CellCountEntry> auto-updates UI
       │
       ▼
User triggers export (HandleGenerateDocx / HandleExportPdf)
       │
       ▼
Build PatientInfo + SpecimenInfo from ViewModel state
Load LetterheadConfig + DocumentTemplate via TemplateService
       │
       ├── DocxGenerator.Create(...) ──► .docx
       └── PdfConverter.Convert(...)  ──► .pdf
            └── DirectPdfGenerator (fallback) or Word Interop
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
| Image not found (logo) | Silently skip logo in document; log warning |
| Template JSON corrupt | Validate on load; fall back to embedded default; notify user |
| Multiple instances | Single-instance app (mutex) to avoid file conflicts |
| Undo after export | Undo stack preserved; re-export uses latest state |

---

## 7. Implementation Order

| Step | Task | Est. Effort |
|------|------|-------------|
| 1 | Scaffold solution: `dotnet new sln`, create 3 projects, add NuGet references | 1h |
| 2 | **Core Models** — `CellType`, `CounterState`, `DifferentialResult`, `PatientInfo`, `LetterheadConfig`, `DocumentTemplate` | 2h |
| 3 | **CounterService** — key-to-cell mapping, increment/decrement/undo/reset, percentage calculation, undo stack | 3h |
| 4 | **TemplateService** — JSON serialization/deserialization, load/save templates & letterhead, validation | 2h |
| 5 | **WPF Shell** — MainWindow, App.xaml, `CounterViewModel`, basic counter display with `ItemsControl` | 4h |
| 6 | **PB/BM modes** — mode toggle UI, extended cell list, Tab/Shift+Tab group navigation | 3h |
| 7 | **Keyboard handling** — `PreviewKeyDown` wiring, modifier keys, undo/reset shortcuts | 2h |
| 8 | **DocxGenerator** — build differential table, insert letterhead, replace placeholders, save .docx | 4h |
| 9 | **PdfConverter** — Word Interop automation, `SaveAs PDF`, process cleanup, error handling | 3h |
| 10 | **ExportService** — coordinate export flow, file dialogs, progress indicator | 2h |
| 11 | **Settings Window** — letterhead editor, key remapping, default mode, template management, per-field patient info toggles | 4h |
| 12 | **Error handling, edge cases** — Word missing, zero state, corrupt files, single-instance | 2h |
| 13 | **Offline validation** — disconnect network, test full workflow end-to-end | 1h |
| 14 | **Publish** — self-contained single-file `.exe` (`dotnet publish`) | 1h |

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
- Multi-language support (labels via JSON resource files)
- Automated flagging (highlight values outside reference range)
- Bone marrow aspirate report with additional sections (megakaryocytes, iron stores, etc.)
