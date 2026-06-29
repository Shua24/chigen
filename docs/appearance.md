# Customizing Chigen's Appearance

The entire UI is built with **WPF** and styled via **XAML resources** — no separate CSS, no external theme engine. Every color, font, border, and spacing value is defined in one of a few XAML files. To change the look, you edit those values.

---

## Where Styles Are Defined

| File | What It Controls |
|---|---|
| `src/Chigen.App/MainWindow.xaml` (`<Window.Resources>`) | Header bar, action buttons, patient badge, totla bar, status bar |
| `src/Chigen.App/Views/SettingsWindow.xaml` (`<Window.Resources>`) | Settings dialog fields (letterhead, logo placement), buttons |
| `src/Chigen.App/Views/HotkeySettingsWindow.xaml` (`<Window.Resources>`) | Hotkey config list, tabs, conflict warnings |
| `src/Chigen.App/Views/PatientInfoWindow.xaml` (`<Window.Resources>`) | Patient info form fields (each wrapped in a named Grid for per-field visibility toggling) |
| `src/Chigen.App/Views/ConclusionWindow.xaml` | Conclusion dialog (inline styles) |

There is no separate `Styles.xaml` or `Colors.xaml` yet — styles are kept local to each window. If you want a global theme, create a `ResourceDictionary` (see below).

---

## Quick Wins — Change Colors

All of these values are hardcoded hex colors in the XAML. A few `Find & Replace` changes can retheme the entire app.

### Header bar (dark blue)
In `MainWindow.xaml`, search for `#1A3C6E` and replace with your colour.

```xml
<!-- Current -->
<Border Grid.Row="0" Background="#1A3C6E" ...>
```
This colour is used for:
- Top header banner
- Cell-type key badges
- Table header row background (PB/BM tabs in hotkey settings)
- Total bar background
- Status bar background

### Light grey info bars
Search for `#F5F5F5` (patient info row) and `#F0F0F0` (action buttons row).

### Warning / hint banner (yellow)
Search for `#FFF3CD` (the hint banner in hotkey settings and patient info window).

### Table cell shading (blue-grey headers)
In the DOCX generator (`DocxGenerator.cs`), search for `"D9E2F3"` — this is the table header fill colour.

---

## Change Fonts

The app uses **Calibri** by default for both the WPF UI and the generated DOCX/PDF documents.

### UI fonts
Each window defines its own font sizes inline. For a global change, add a resource at `Application.Resources` in `App.xaml`:

```xml
<Application.Resources>
    <Style TargetType="TextBlock">
        <Setter Property="FontFamily" Value="Segoe UI"/>
    </Style>
    <Style TargetType="TextBox">
        <Setter Property="FontFamily" Value="Segoe UI"/>
    </Style>
</Application.Resources>
```

### Document fonts (DOCX / PDF)
Font sizes are set explicitly across different document sections:

| Section | DOCX | PDF |
|---|---|---|
| Letterhead institution | 12pt (`FontSize="24"`) | `LetterheadTitleFont` 12pt Bold |
| Letterhead department | 10pt (`FontSize="20"`) | `LetterheadSubtitleFont` 10pt |
| Letterhead address | 9pt (`FontSize="18"`) | `SmallFont` 9pt |
| Report title | default (inherited) | `SubtitleFont` 11pt |
| Table header | 9pt (`FontSize="18"`, Bold) | `TableHeaderFont` 9pt Bold |
| Table data cells | 8pt (`FontSize="16"`) | `TableFont` 8pt |
| Patient info cells | 8pt (`FontSize="16"`) | `TableHeaderFont` 9pt / `TableFont` 8pt |
| Footer / timestamp | 8pt (`FontSize="16"`) | `TableFont` 8pt |
| Conclusion / Recommendations | 9pt (`FontSize="18"`) | `TableHeaderFont` 9pt / `TableFont` 8pt |

Font family is **Calibri** throughout. To change it, update the `XFont` static fields in `DirectPdfGenerator.cs` or add `RunFonts` to `RunProperties` in `DocxGenerator.cs`:

```csharp
// In DirectPdfGenerator.cs
private static readonly XFont LetterheadTitleFont = new("Georgia", 12, XFontStyleEx.Bold);

// In DocxGenerator.cs
new Run(
    new RunProperties(new RunFonts { Ascii = "Georgia", HighAnsi = "Georgia" }),
    new Text("Hello")
)
```

### Letterhead Logo Placement

The logo position in generated documents is controlled by `LetterheadConfig.LogoPlacement`:

- **Top** (default for backward compatibility): Logo is centered above the institution name, same as the original layout.
- **Side** (new default): Logo is placed on the left side of the letterhead, with the institution name, department, and address aligned to the right. A borderless table (DOCX) or absolute positioning (PDF) is used to achieve the side-by-side layout.

Set this in the Settings window via the "Logo Placement" dropdown, or directly in `letterhead.json`.

### Letterhead Thick Bottom Border

The letterhead has a **thick** bottom rule (3pt single border) separating it from the document body, meeting official letterhead standards ("letterhead rule"). The border is applied using the **border painter approach**:

- **DOCX**: `ParagraphBorders` with `BottomBorder` (single, 24pt = 3pt) is painted directly on the last letterhead paragraph (the address line). No separate empty border paragraph is added.
- **PDF**: A horizontal line is drawn with `new XPen(XColors.Black, 3)` at the bottom of the address line rendering.

This approach saves ~2 lines of vertical space compared to using a separate border paragraph, critical for fitting the report on a single page.

---

### Patient Info Table Layout

The patient information section in generated DOCX and PDF documents uses a dynamic table where each field can be individually toggled in Settings → Patient Info Fields. Fields are paired in rows:

| Left Column | Right Column |
|---|---|
| Patient ID (toggleable) | Date of Birth (toggleable) |
| Patient Name (toggleable) | Physician (toggleable) |
| Sex (toggleable) | Ward (toggleable) |
| Diagnosis (toggleable) | Payment Method (toggleable) |
| Address (toggleable) | Collection Date |
| | Specimen |
| | Received Date |

When both paired fields are disabled, the entire row is skipped. When only one is disabled, the enabled field spans the row. The table has a full border (single, 6pt) in DOCX, and a drawn rectangle in PDF.

### Patient Info Form (Dialog)

Each field in the PatientInfoWindow dialog is wrapped in a named `Grid` container with `x:Name` set to `patientXxxPanel`. Visibility is driven by the corresponding `DocumentTemplate` setting — the field row collapses when its toggle is off. If all fields are disabled, the dialog is skipped entirely on startup and when clicking Edit.

---

## Change the Counter Table Layout

The counter rows are rendered by an `ItemsControl` in `MainWindow.xaml`. Each row is a `DataTemplate` inside `<ItemsControl.ItemTemplate>`:

```xml
<ItemsControl ItemsSource="{Binding Entries}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="#E0E0E0" BorderThickness="0,0,0,1" Padding="4">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="50"/>   <!-- Key badge -->
                        <ColumnDefinition Width="*"/>    <!-- Cell name -->
                        <ColumnDefinition Width="80"/>   <!-- Count -->
                        <ColumnDefinition Width="80"/>   <!-- Percentage -->
                        <ColumnDefinition Width="100"/>  <!-- Ref range -->
                        <ColumnDefinition Width="80"/>   <!-- +/- buttons -->
                    </Grid.ColumnDefinitions>
                    ...
```

Adjust the `Width` values or add/remove columns as needed.

---

## Creating a Global Theme (Resource Dictionary)

1. Create a new file `src/Chigen.App/Themes/Colors.xaml`:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Color x:Key="PrimaryColor">#1A3C6E</Color>
    <Color x:Key="BackgroundLight">#F5F5F5</Color>
    <Color x:Key="BorderColor">#D0D0D0</Color>
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}"/>
    <SolidColorBrush x:Key="BackgroundLightBrush" Color="{StaticResource BackgroundLight}"/>
</ResourceDictionary>
```

2. Merge it in `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/Colors.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

3. Reference colours by key in windows:

```xml
<Border Background="{StaticResource PrimaryBrush}" .../>
```

---

## Adding Dark Mode

The quickest path:
1. Create a `Themes/Dark.xaml` resource dictionary with dark colour values.
2. Add a toggle in the Settings window that swaps the `Source` URI of the merged dictionary at runtime.
3. Either reload the window or use `DynamicResource` bindings so colours update live.

---

## Notes

- WPF uses a **pixel-based layout**. All `Width`, `Height`, `Margin`, `Padding` are in device-independent pixels (1/96 inch).
- The status bar hint text (`0-9,A-Z=Count | F5,F6=Select | Del=Undo | Ctrl+Q=Exit`) is plain text in `MainWindow.xaml` — edit it directly if keybindings change.
- The blue key badges in the counter table are `<Border Background="#1A3C6E" CornerRadius="3">`.
- Row borders in the counter table use `BorderBrush="#E0E0E0" BorderThickness="0,0,0,1"`.
- The Exit button (8th action button) has `Background="#D9534F"` and is fired via `Click` handler calling `Close()`.
- Window default size is `Width="1100" Height="750"` with `MinWidth="960"`.
