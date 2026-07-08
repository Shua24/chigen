# Chigen Codebase Documentation

## Overview

Chigen is a fully offline WPF desktop application for cell differential counting in hematology. It allows laboratory professionals to tally different white blood cell types via keyboard hotkeys, with mode toggling between Peripheral Blood (PB) and Bone Marrow (BM) panels, and generates professional medical documents (DOCX and PDF).

---

## Solution Structure

```
Chigen/
+-- Chigen.sln
+-- docs/
|   +-- plan.md
|   +-- appearance.md
|   +-- code.md               (this file)
+-- src/
    +-- Chigen.App/            (WPF, net8.0-windows)
    +-- Chigen.Core/           (Class Library, net8.0)
    +-- Chigen.DocumentExport/ (Class Library, net8.0)
    +-- Chigen.Tests/          (xUnit Test Project, net8.0)
```

### Project Dependencies

```
Chigen.App --> Chigen.Core
Chigen.App --> Chigen.DocumentExport
Chigen.DocumentExport --> Chigen.Core
Chigen.Tests --> Chigen.Core
Chigen.Tests --> Chigen.DocumentExport
```

---

## Chigen.Core (src/Chigen.Core/)

Business logic and domain models. No WPF dependency.

### Namespace: `Chigen.Core.Models`

#### `CellType`
Represents a type of blood cell that can be counted.

| Property       | Type          | Default               | Description                           |
|----------------|---------------|-----------------------|---------------------------------------|
| `Id`           | `string`      | `""`                  | Unique identifier (e.g. "neutrophil") |
| `Name`         | `string`      | `""`                  | Display name (e.g. "Neutrophil (Seg)")|
| `Key`          | `string`      | `""`                  | Keyboard shortcut, uses `KeyConverter`|
| `ReferenceRange`| `string`     | `""`                  | Display string (e.g. "40-80")         |
| `ReferenceLow` | `double`      | `0`                   | Lower reference bound                 |
| `ReferenceHigh`| `double`      | `0`                   | Upper reference bound                 |
| `Group`        | `string`      | `"default"`           | Cell lineage group (e.g. "Myeloid")   |
| `Mode`         | `CounterMode` | `PeripheralBlood`     | Which mode this cell type belongs to  |

#### `CounterMode` (enum)
| Value             | Description          |
|-------------------|----------------------|
| `PeripheralBlood` | Peripheral blood mode|
| `BoneMarrow`      | Bone marrow mode     |

#### `CellCountEntry` : `INotifyPropertyChanged`
A tally entry for a single cell type.

| Property      | Type       | Description                        |
|---------------|------------|------------------------------------|
| `CellType`    | `CellType` | The associated cell type (readonly)|
| `Count`       | `int`      | Current tally, raises PropertyChanged|
| `Percentage`  | `double`   | Computed percentage, raises PropertyChanged|

| Method   | Description                   |
|----------|-------------------------------|
| `Reset()`| Sets Count and Percentage to 0|

#### `CounterState`
Holds the complete state of the differential counter.

| Property        | Type                        | Description                  |
|-----------------|-----------------------------|------------------------------|
| `Entries`       | `List<CellCountEntry>`      | The list of cell count entries|
| `Total`         | `int` (computed)            | Sum of all entry counts      |
| `Mode`          | `CounterMode`               | Current counter mode         |
| `UndoStack`     | `Stack<UndoAction>`         | Stack of undo actions        |

| Method                    | Description                              |
|---------------------------|------------------------------------------|
| `RecalculatePercentages()`| Recomputes each entry's percentage       |
| `ResetAll()`              | Resets all entries and clears undo stack |

#### `UndoAction`
Records a single count action for undo purposes.

| Property     | Type     | Description            |
|--------------|----------|------------------------|
| `CellTypeId` | `string` | The cell type ID       |
| `CellName`   | `string` | Display name           |

#### `PatientInfo`
Patient demographic and clinical information.

| Property       | Type     | Default |
|----------------|----------|---------|
| `Id`           | `string` | `""`    |
| `Name`         | `string` | `""`    |
| `Sex`          | `string` | `""`    |
| `DateOfBirth`  | `string` | `""`    |
| `Diagnosis`    | `string` | `""`    |
| `Address`      | `string` | `""`    |
| `Physician`    | `string` | `""`    |
| `Ward`         | `string` | `""`    |
| `PaymentMethod`| `string` | `""`    |
| `Conclusion`     | `string` | `""`    |
| `Recommendations`| `string` | `""`    |

#### `SpecimenInfo`
Specimen collection information.

| Property         | Type     | Default                        |
|------------------|----------|--------------------------------|
| `Type`           | `string` | `"Peripheral Blood"`           |
| `CollectionDate` | `string` | Today (yyyy-MM-dd)             |
| `ReceivedDate`   | `string` | Today (yyyy-MM-dd)             |

#### `DocumentTemplate`
Controls which sections appear in generated documents.

| Property            | Type     | Default |
|---------------------|----------|---------|
| `Name`              | `string` | `"Default"` |
| `HeaderFormat`      | `string` | `""`    |
| `ShowLetterhead`    | `bool`   | `true`  |
| `ShowPatientInfo`   | `bool`   | `true`  |
| `ShowPatientId`     | `bool`   | `true`  |
| `ShowPatientName`   | `bool`   | `true`  |
| `ShowPatientDob`    | `bool`   | `true`  |
| `ShowPatientSex`    | `bool`   | `true`  |
| `ShowPatientDiagnosis`| `bool` | `true`  |
| `ShowPatientAddress`| `bool`   | `true`  |
| `ShowPatientPhysician`| `bool` | `true`  |
| `ShowPatientWard`   | `bool`   | `true`  |
| `ShowPatientPaymentMethod`| `bool`| `true` |
| `ShowReferenceRanges`| `bool`  | `true`  |
| `ShowConclusion`      | `bool`   | `true`  |
| `ShowFooter`        | `bool`   | `true`  |

#### `LetterheadConfig`
Institution letterhead settings.

| Property          | Type     | Default                                 |
|-------------------|----------|-----------------------------------------|
| `InstitutionName` | `string` | `"My Hospital"`                         |
| `Department`      | `string` | `"Hematology Laboratory"`               |
| `Address`         | `string` | `""`                                    |
| `Phone`           | `string` | `""`                                    |
| `Email`           | `string` | `""`                                    |
| `LogoPath`        | `string`         | `""`                                        |
| `LogoPlacement`   | `LogoPlacement`  | `Side` (enum: `Top`, `Side`)              |
| `FooterText`      | `string`         | `"Confidential -- For clinical use only"`   |

#### `HotkeyMappingEntry`
Maps a keyboard key to a cell type for a specific mode.

| Property     | Type          | Default               |
|--------------|---------------|-----------------------|
| `CellTypeId` | `string`      | `""`                  |
| `Key`        | `string`      | `""` (uses `KeyConverter`) |
| `Mode`       | `CounterMode` | `PeripheralBlood`     |

#### `KeyConverter` : `JsonConverter<string>`
Custom JSON converter that handles both numeric and string key values. Reads `1` (int) as `"1"` and `"A"` (string) as `"A"`. Always writes as string.

---

### Namespace: `Chigen.Core.Services`

#### `CellTypeProvider` (static)
Provides default cell type definitions.

| Method                              | Returns              | Description                  |
|-------------------------------------|----------------------|------------------------------|
| `GetDefaultPeripheralBloodTypes()`  | `List<CellType>`     | 10 PB cell types (keys 0-9)  |
| `GetDefaultBoneMarrowTypes()`       | `List<CellType>`     | 15 BM cell types (keys 0-9, A-E)|

**PB Cell Types:** Neutrophil, Band, Lymphocyte, Monocyte, Eosinophil, Basophil, Metamyelocyte, Myelocyte, Promyelocyte, Blast.

**BM Cell Types:** Same 10 as PB plus NRBC, Megakaryocyte, Plasma Cell, Monoblast, Erythroblast. Each has a `Group` property (Myeloid, Lymphoid, Monocytic, Erythroid, Blast, Megakaryocytic).

#### `CounterService`
Core counting logic. Maintains key-to-cell-type mappings and the counter state.

| Method                              | Description                                       |
|-------------------------------------|---------------------------------------------------|
| `Constructor()`                     | Loads default Peripheral Blood cell types         |
| `LoadCellTypes(CounterMode)`        | Loads cell types for given mode                   |
| `TryCount(string key)`              | Increments count for cell type mapped to key      |
| `TryUndo()`                         | Pops last undo action and decrements              |
| `Reset()`                           | Resets all counts and undo stack                  |
| `TryCountManual(string cellTypeId)` | Increments count by cell type ID                  |
| `TryUndoManual(string cellTypeId)`  | Decrements count by cell type ID                  |
| `GetCounts()`                       | Returns `Dictionary<string, int>` of counts       |
| `GetEntriesForMode(CounterMode)`    | Filters entries matching the mode                 |
| `ApplyHotkeyMappings(List<...>)`    | Applies custom key mappings from config           |
| `GetAllCellTypes()`                 | Returns all loaded cell types                     |

**Key behavior:**
- `TryCount` with null/empty/invalid key returns `false`
- `TryUndo` on empty stack returns `false`
- Count never goes below zero
- Percentages recalculated after every count operation, rounded to 1 decimal
- Switching modes clears all counts and the undo stack

#### `TemplateService` (static)
Manages JSON configuration files stored in `%LOCALAPPDATA%\Chigen\`.

| Method                                  | Description                             |
|-----------------------------------------|-----------------------------------------|
| `LoadLetterhead()`                      | Loads `letterhead.json`                 |
| `SaveLetterhead(LetterheadConfig)`      | Saves to `letterhead.json`              |
| `LoadTemplate(string name = "Default")` | Loads from `templates.json` by name     |
| `SaveTemplate(DocumentTemplate)`        | Saves/updates a template                |
| `LoadHotkeyMappings()`                  | Loads from `hotkeys.json` or builds defaults |
| `SaveHotkeyMappings(List<...>)`         | Saves to `hotkeys.json`                 |

**Config files:**
- `letterhead.json` -- `LetterheadConfig`
- `templates.json` -- `List<DocumentTemplate>`
- `hotkeys.json` -- `List<HotkeyMappingEntry>`

**Error handling:** Returns defaults for missing or corrupt files.

---

## Chigen.DocumentExport (src/Chigen.DocumentExport/)

Document generation engine for DOCX and PDF output.

### Namespace: `Chigen.DocumentExport`

#### `PdfConversionMethod` (enum)
| Value               | Description                      |
|---------------------|----------------------------------|
| `Unavailable`       | No PDF conversion available      |
| `WordInterop`       | Use Microsoft Word COM interop   |
| `DirectPdfGenerator`| Use PdfSharp direct generation   |

#### `DocxGenerator`
Builds a DOCX file using OpenXML SDK.

| Method                                                        | Description                  |
|---------------------------------------------------------------|------------------------------|
| `Create(string outputPath, CounterState, PatientInfo, SpecimenInfo)` | Creates the DOCX file |
| `AddLetterhead(CounterState.PatientInfo, LetterheadConfig?)` | Adds institution name, department, address, logo, and thick bottom border (letterhead rule) |
| `AddReportTitle()` | Adds "HEMATOLOGY LABORATORY REPORT" as standalone heading after letterhead |
| `AddPatientInfoTable(CounterState.PatientInfo, DocumentTemplate)` | Adds dynamic patient info rows |
| `AddDifferentialTable(CounterState)` | Adds the main cell count table with 8pt data cells |
| `AddFooter(CounterState.PatientInfo)` | Adds timestamp, footer text, signature line at 8pt |

**Document sections (controlled by `DocumentTemplate`):**
1. **Letterhead** -- Institution name, department, address, optional logo image. Logo can be placed centered above text (`Top`) or left-aligned beside text (`Side`), controlled by `LetterheadConfig.LogoPlacement`. A thick 3pt bottom border ("letterhead rule") is painted directly on the last letterhead element (address line) using the border painter approach.
2. **Report Title** -- "HEMATOLOGY LABORATORY REPORT" rendered as a standalone heading, separate from the letterhead. Implemented as `AddReportTitle` (DOCX) / `DrawReportTitle` (PDF).
3. **Patient Info** -- Dynamic table where each field can be individually toggled via settings (`ShowPatientId`, `ShowPatientName`, `ShowPatientDob`, `ShowPatientSex`, `ShowPatientDiagnosis`, `ShowPatientAddress`, `ShowPatientPhysician`, `ShowPatientWard`, `ShowPatientPaymentMethod`). Fields are paired in rows (Patient ID/Date of Birth, Patient Name/Physician, Sex/Ward, Diagnosis/Payment Method, Address/Collection Date). Disabled fields are omitted from the table; when both paired fields are disabled, the row is skipped entirely.
4. **Differential Count Table** -- Header row (Cell Type, Count, %, Ref Range), data rows sorted by key, TOTAL row. Table data cells use 8pt font.
5. **Conclusion** -- Free-text interpretation section
6. **Recommendations** -- Free-text recommendations section (rendered below Conclusion; only shown when non-empty)
7. **Footer** -- Generation date, footer text, signature line. All elements use 8pt font.

**Sort key logic:** Numeric keys (0-9) sort before alpha keys (A-Z). `"1"` -> `"01"`, `"A"` -> `"1A"`.

#### `DirectPdfGenerator`
Generates PDF directly using PdfSharp (no Word dependency). Mirrors the DOCX layout.

| Method                                                        | Description                  |
|---------------------------------------------------------------|------------------------------|
| `Create(string outputPath, CounterState, PatientInfo, SpecimenInfo)` | Creates the PDF file |
| `DrawLetterhead(CounterState.PatientInfo, LetterheadConfig?)` | Draws institution name, department, address, logo, and thick bottom border (letterhead rule) |
| `DrawReportTitle()` | Draws "HEMATOLOGY LABORATORY REPORT" as standalone heading after letterhead |
| `DrawPatientInfoTable(CounterState.PatientInfo, DocumentTemplate)` | Draws dynamic patient info rows |
| `DrawDifferentialTable(CounterState)` | Draws the main cell count table with 8pt data cells |
| `DrawFooter(CounterState.PatientInfo)` | Draws timestamp, footer text, signature line at 8pt |

Same document structure as `DocxGenerator` (Letterhead, Report Title, Patient Info, Differential Table, Conclusion, Recommendations, Footer) but uses PdfSharp drawing primitives. Filters out zero-count entries from the differential table.

#### `PdfConverter`
Facilitates PDF conversion via Word Interop or fallback to `DirectPdfGenerator`.

| Method                                                                          | Description                        |
|---------------------------------------------------------------------------------|------------------------------------|
| `CheckAvailability()`                                                           | Detects if Word is installed       |
| `Convert(string outputPdfPath, CounterState, PatientInfo, SpecimenInfo, LetterheadConfig, DocumentTemplate, PdfConversionMethod)` | Converts to PDF using specified method |
| `InvokeWordMethod(object target, string methodName, params object?[] args)`     | Calls a Word COM method via reflection; unwraps `TargetInvocationException` -> `InvalidOperationException` with `"Word.{methodName} failed: {inner}"` |
| `InvokeGetProperty(object target, string propertyName)`                         | Gets a Word COM property via reflection; same error unwrapping |

---

## Chigen.App (src/Chigen.App/)

WPF application layer. Uses CommunityToolkit.Mvvm for MVVM.

### Namespace: `Chigen.App`

#### `App`
Application entry point. Enforces single-instance via named mutex.

#### `MainWindow`
Main application window. Handles keyboard input for cell counting and triggers document generation.
The patient summary badge (column 0 of the info bar) is bound to `HasPatientInfo` via `BooleanToVisibilityConverter` -- it hides when all patient fields are disabled in settings.

**Action buttons** are defined in a 9-column Grid row with `Column="0"` through `Column="8"`. Column 8 is the **Exit** button (`Background="#D9534F"`) which calls `Close()` via `Exit_Click` handler. **Ctrl+Q** is bound in `Window_PreviewKeyDown` and shown in the status bar as `Ctrl+Q=Exit`.

**Window dimensions:** Default `Width="1100" Height="750"`, `MinWidth="960"`.

### Namespace: `Chigen.App.ViewModels`

#### `CounterViewModel`
Main view model connecting the UI to `CounterService`. Exposes `ObservableCollection<CellCountEntry>`, patient info properties, and relay commands for all user actions.

**Logo validation**: Both `HandleGenerateDocx()` and `HandleExportPdf()` check that `LogoPath` is non-empty when `ShowLetterhead` is enabled. If empty, they throw `InvalidOperationException("The letterhead logo is required. Go to settings to set the logo.")`.

**Error unwrapping**: Both catch handlers unwrap `TargetInvocationException` to show the inner exception's message, giving actionable error text instead of the generic "Exception has been thrown by the target of an invocation".

| Member                  | Type   | Description                                      |
|-------------------------|--------|--------------------------------------------------|
| `HasPatientInfo`        | `bool` | Whether patient info section is enabled; hides the patient summary bar in main window when `false` |
| `RefreshTemplateSettings()` | `void` | Reloads template settings from disk and updates `HasPatientInfo` |

#### `SettingsViewModel`
Manages letterhead configuration editing and document template settings.

| Property                  | Type   | Description                                      |
|---------------------------|--------|--------------------------------------------------|
| `UseLetterhead`           | `bool` | Toggle to include/exclude letterhead in documents|
| `ShowPatientId`           | `bool` | Toggle to include/exclude Patient ID in documents|
| `ShowPatientName`         | `bool` | Toggle to include/exclude Patient Name           |
| `ShowPatientDob`          | `bool` | Toggle to include/exclude Date of Birth          |
| `ShowPatientSex`          | `bool` | Toggle to include/exclude Sex                    |
| `ShowPatientDiagnosis`    | `bool` | Toggle to include/exclude Diagnosis              |
| `ShowPatientAddress`      | `bool` | Toggle to include/exclude Address                |
| `ShowPatientPhysician`    | `bool` | Toggle to include/exclude Physician              |
| `ShowPatientWard`         | `bool` | Toggle to include/exclude Ward                   |
| `ShowPatientPaymentMethod`| `bool` | Toggle to include/exclude Payment Method         |

#### `HotkeySettingsViewModel`
Manages hotkey remapping for both PB and BM modes.

#### `HotkeySettingsItem`
Individual hotkey mapping item with conflict detection.

### Namespace: `Chigen.App.Views`

#### `PatientInfoWindow`
Patient data entry dialog. Each field (Patient Name, ID, DOB, Sex, Diagnosis, Address, Physician, Ward, Payment Method) is wrapped in a named `Grid` container whose `Visibility` is toggled individually based on the corresponding `DocumentTemplate` setting (`ShowPatientName`, `ShowPatientId`, etc.). The dialog is skipped entirely when all patient fields are disabled (`HasPatientInfo == false`).

#### `SettingsWindow`
Letterhead configuration and document template settings. Contains:
- **Letterhead fields** -- Institution name, department, address, phone, email, logo, logo placement, footer text
- **"Use letterhead"** checkbox -- toggles letterhead in generated documents
- **Patient Info Fields** (bordered section) -- per-field checkboxes for Patient ID, Name, DOB, Sex, Diagnosis, Address, Physician, Ward, Payment Method

#### `HotkeySettingsWindow`, `ConclusionWindow`, `RecommendationsWindow`
Dialog windows for hotkey configuration, Conclusion entry, and Recommendations entry.

---

## Chigen.Tests (src/Chigen.Tests/)

xUnit test project covering business logic and document generation utilities.

### Test Structure

```
Chigen.Tests/
+-- Models/
|   +-- CellTypeTests.cs
|   +-- CounterModeTests.cs
|   +-- CellCountEntryTests.cs
|   +-- CounterStateTests.cs
|   +-- PatientInfoTests.cs
|   +-- SpecimenInfoTests.cs
|   +-- DocumentTemplateTests.cs
|   +-- LetterheadConfigTests.cs
|   +-- UndoActionTests.cs
|   +-- HotkeyMappingEntryTests.cs
|   +-- KeyConverterTests.cs
+-- Services/
|   +-- CellTypeProviderTests.cs
|   +-- CounterServiceTests.cs
|   +-- CounterServiceModeTests.cs
|   +-- TemplateServiceTests.cs
+-- DocumentExport/
    +-- SortKeyTests.cs
    +-- PdfConversionMethodTests.cs
```

### Test Categories

#### Model Tests (11 files, ~30 tests)
- **CellTypeTests** -- Default values and property assignment
- **CounterModeTests** -- Enum integer values
- **CellCountEntryTests** -- Constructor, property get/set, Reset(), PropertyChanged events, INotifyPropertyChanged implementation
- **CounterStateTests** -- Total computation, percentage calculation (including edge cases: zero total, rounding), mode default, undo stack, ResetAll()
- **PatientInfoTests** -- Default values and property assignment
- **SpecimenInfoTests** -- Default values (type, dates), property assignment
- **DocumentTemplateTests** -- Default values and property assignment
- **LetterheadConfigTests** -- Default values and property assignment
- **UndoActionTests** -- Default values and property assignment
- **HotkeyMappingEntryTests** -- Default values and property assignment
- **KeyConverterTests** -- JSON deserialization (numeric and string keys), serialization, HotkeyMappingEntry integration

#### Service Tests (4 files, ~45 tests)
- **CellTypeProviderTests** -- PB returns 10 types, BM returns 15 types, correct modes, correct key mappings, correct groups, extra BM types
- **CounterServiceTests** -- Constructor loads PB by default, TryCount (valid/invalid/null/empty keys), undo stack behavior, TryCountManual, TryUndoManual, reset, count dictionary, total computation, percentage recalculation, mode switching, ApplyHotkeyMappings
- **CounterServiceModeTests** -- BM mode cell types, alpha keys, numeric keys in BM mode, mode switching clears counts and undo stack
- **TemplateServiceTests** -- File I/O: load/save letterhead, load/save templates, template by name, overwrite existing, default when missing, corrupt file handling, hotkey mappings, build defaults when missing

#### Document Export Tests (2 files, ~10 tests)
- **SortKeyTests** -- Numeric keys zero-prefixed, alpha keys one-prefixed, empty/null returns "ZZ", numeric sorts before alpha
- **PdfConversionMethodTests** -- Enum integer values

### Running Tests

```bash
dotnet test src/Chigen.Tests/Chigen.Tests.csproj
```

All 118 tests should pass.

---

## NuGet Packages

| Package                          | Version | Project(s)           |
|----------------------------------|---------|----------------------|
| `DocumentFormat.OpenXml`         | 3.5.1   | Core, DocumentExport |
| `CommunityToolkit.Mvvm`          | 8.4.2   | App                  |
| `Microsoft.Office.Interop.Word`  | 15.0... | DocumentExport       |
| `PdfSharp`                       | 6.2.4   | DocumentExport       |
| `xunit`                          | 2.5.3   | Tests                |
| `xunit.runner.visualstudio`      | 2.5.3   | Tests                |
| `Microsoft.NET.Test.Sdk`         | 17.8.0  | Tests                |
| `coverlet.collector`             | 6.0.0   | Tests                |

---

## Architecture Notes

- **MVVM Pattern**: App uses CommunityToolkit.Mvvm (RelayCommand) with manual `INotifyPropertyChanged`
- **Single-Instance**: Named mutex prevents multiple app instances
- **JSON Config**: All persistence via JSON files in `%LOCALAPPDATA%\Chigen\`
- **PDF Fallback**: Detects Word at startup; falls back to PdfSharp if Word unavailable
- **Keyboard Handling**: WPF `PreviewKeyDown` event captures numeric (0-9) and alpha (A-Z) keys for counting, plus Ctrl+Q for Exit
- **Error unwrapping**: PDF export via Word Interop uses `InvokeWordMethod`/`InvokeGetProperty` helpers that unwrap `TargetInvocationException` into `InvalidOperationException` with a `"Word.{method} failed:"` prefix, so the real COM error surfaces instead of the generic reflection wrapper. Both DOCX and PDF catch handlers in `CounterViewModel` also unwrap `TargetInvocationException` to show the inner exception.
- **Single Line Spacing**: DOCX documents use `DocDefaults(ParagraphPropertiesDefault(SpacingBetweenLines Line="240" LineRule=Auto Before=0 After=0))` for compact layout
- **Logo validation**: When `ShowLetterhead` is enabled, `LogoPath` must be non-empty; export throws `"The letterhead logo is required. Go to settings to set the logo."` otherwise
- **Font Sizes**: Letterhead uses 12pt/10pt/9pt hierarchy; table data cells use 8pt; footer uses 8pt -- all sized to fit the report on a single page
- **Mode Switching**: PB mode (10 cell types) vs BM mode (15 cell types) with separate hotkey mappings

---

## Release & Versioning

Artifact versions are driven by the CI workflow (`.github/workflows/dotnet-desktop.yml`) and the installer script (`installer/setup.iss`).

### Where each version is set

| Artifact | Source | Current pattern |
|---|---|---|
| **Portable ZIP filename** | Workflow `Create portable ZIP` step | `Chigen-Portable-${{ github.run_number }}.zip` |
| **Installer filename** | `setup.iss` line 10, overridden by workflow `/DMyAppVersion=` | `Chigen-Setup-1.0.{run_number}.exe` |
| **Installer internal version** | `setup.iss` line 7, overridden by workflow | `1.0.{run_number}` |
| **Release tag** | Workflow `tag_name` field | `v{run_number}` |
| **Release name** | Workflow `name` field | `Chigen v{run_number}` |
| **App binary version** (FileVersion, ProductVersion) | Not set | Add `<Version>` to `Chigen.App.csproj` |

`${{ github.run_number }}` is GitHub\x27s auto-incrementing counter. It increases by 1 every workflow run.

### How to change the version scheme

**Option A \u2014 Tag-driven (recommended)**
Set a manual version at the top of the workflow:
```yaml
env:
  APP_VERSION: 1.1.0
```
Then use `${{ env.APP_VERSION }}` in filenames, installer args, and tags.

**Option B \u2014 Hybrid (major.minor manual, patch auto)**
Keep `1.0.{run_number}` but bump the major/minor part in `setup.iss` and the workflow when needed.

**Option C \u2014 Date-based**
Use a date-derived version like `2026.07.04` passed as a workflow variable.

### Adding version to the app binary
To embed the version into `Chigen.exe`\x27s file properties:
```xml
<!-- src/Chigen.App/Chigen.App.csproj -->
<PropertyGroup>
  <Version>1.0.0</Version>
  <FileVersion>1.0.0</FileVersion>
</PropertyGroup>
```
Then pass from the workflow:
```bash
dotnet publish ... -p:Version=1.0.${{ github.run_number }}
```

---

## Simplification Notes

### MVVM Source Generators

All ViewModels and observable models now use **CommunityToolkit.Mvvm source generators** (=8.4.2) instead of manual INotifyPropertyChanged. The [ObservableProperty] attribute generates the property, backing field, and change notification from a partial class extending ObservableObject. The [RelayCommand] attribute generates ICommand properties from methods. This eliminated ~270 lines of boilerplate across 8 files.

**Converted classes:**

| File | Before | After | Change |
|---|---|---|---|
| Chigen.Core/Models/CellCountEntry.cs | 70 lines, manual INPC | 20 lines, [ObservableProperty] | Removed OnPropertyChanged, backing fields, event |
| Chigen.App/ViewModels/HotkeySettingsItem.cs | 70 lines, manual INPC | 15 lines, [ObservableProperty] | Same |
| Chigen.App/ViewModels/CounterViewModel.cs | 480 lines | 375 lines | 9 [ObservableProperty] fields, 4 [RelayCommand] methods |
| Chigen.App/ViewModels/SettingsViewModel.cs | 180 lines | 130 lines | 19 pass-through wrappers ? 1 [ObservableProperty] field |
| Chigen.App/ViewModels/HotkeySettingsViewModel.cs | 160 lines | 120 lines | 2 [ObservableProperty] fields |
| Chigen.App/Translations.cs | manual INPC | ObservableObject base | Uses OnPropertyChanged() instead of direct event invocation |

### KeyToBinding Deduplication

KeyBindingHelper.KeyToBinding(Key) was extracted from 2 identical 40-line switch statements into src/Chigen.App/KeyBindingHelper.cs. Both MainWindow.xaml.cs and HotkeySettingsWindow.xaml.cs now call the shared method.

### PatientInfo Merge

Chigen.Core.Models.PatientInfo already had all fields the Chigen.App.Views.PatientInfo duplicate had, plus Conclusion and Recommendations. Removed the duplicate Chigen.App.Views.PatientInfo class and updated PatientInfoWindow.xaml.cs and CounterViewModel.cs to reference Core.Models.PatientInfo directly.

### TemplateService � Single Config File

Replaced 7 separate JSON files (letterhead.json, 	emplates.json, cell_config.json, hotkeys.json, language.json, 	heme.json) with a single %LOCALAPPDATA%\Chigen\config.json carrying an AppConfigV1 container object. All existing Load/Save method signatures preserved. Backward compat: first load creates defaults; no migration needed from the old multi-file format (old files are ignored).

### Dead Code Removed

- Empty src/Chigen.App/Converters/ directory
- Outdated docs/plan.md and docs/appearance.md (superseded by code.md)
- AssemblyInfo.cs collapsed to a single [assembly: ThemeInfo(...)] line
