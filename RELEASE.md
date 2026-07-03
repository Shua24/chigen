# Chigen v1.0.0

**Cell Differential Counter & Report Generator** — a desktop app for hematology labs. Count white blood cells with your keyboard and generate ready-to-print DOCX/PDF reports in seconds.

---

## What it does

Instead of clicking a hand tally counter and writing reports by hand, you press number keys — each key maps to a cell type. Chigen tracks every count, calculates percentages in real time, and produces a professional report when you're done.

## Features

- **Keyboard-driven counting** — keys `1–9` for common cells, `A–E` for bone marrow
- **Peripheral Blood and Bone Marrow modes**
- **Real-time counts, percentages, and reference ranges**
- **Undo** (Delete/Backspace) and **Reset**
- **Patient info form** with toggle-able fields
- **Customizable hotkey mappings**
- **Professional DOCX and PDF reports** with your lab's letterhead
- **Built-in PDF generator** (no Word required) + higher-quality Word-based export
- **Conclusion & Recommendations** text fields
- **English / Bahasa Indonesia** — switch anytime, no restart
- **Single-instance protection**

## Requirements

- Windows 10 or 11 (64-bit)
- .NET 8.0 is included — no extra downloads
- Microsoft Word is *optional* (enables higher-quality PDF export; without it Chigen uses its built-in PDF generator)

## Download options

### Option 1: Installer (recommended)
Download `Chigen-Setup-1.0.xxxx.exe` and run it. The installer places Chigen in `%ProgramFiles%\Chigen` and adds a Start Menu shortcut. Uninstall via Windows **Add or Remove Programs**.

> **⚠️ SmartScreen notice:** The installer is unsigned. Windows may show **"Windows protected your PC"** when you run it.
> Click **More info** → **Run anyway** to proceed.
>
> If you see **"Windows could not find the file specified"**, Windows Defender may have quarantined extracted files.
> Temporarily disable Real-time Protection before installing, or use the **Portable ZIP** instead.
>
> You can also right-click the downloaded `.exe` → **Properties** → check **Unblock** → **OK** before running.

### Option 2: Portable ZIP (no install)
Download `Chigen-Portable-xxxx.zip`, extract to any folder, and run `Chigen.exe`. No admin rights needed. No SmartScreen warning.

## Quick start

1. **Launch Chigen** — enter patient details or skip
2. **Press `1`** for Neutrophil, **`2`** for Lymphocyte, **`3`** for Monocyte…
3. **Toggle Peripheral Blood / Bone Marrow** at the top
4. Click **Generate DOCX** or **Export PDF** when done
5. Open **Settings** to set up your letterhead or switch language

## Keyboard shortcuts

| Key | Action |
|-----|--------|
| `0–9` | Count cell |
| `A–E` | Count (Bone Marrow mode) |
| `Delete` / `Backspace` | Undo |
| `R` | Reset |
| `Ctrl+G` | Generate DOCX |
| `Ctrl+P` | Export PDF |

All mappings are configurable in **Hotkeys**.

## Language

Open **Settings** → pick **English** or **Bahasa Indonesia**. The UI and all generated reports switch immediately. Your choice is remembered the next time you open Chigen.

## Changelog (v1.0.0)

First public release.
