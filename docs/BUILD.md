# Building Chigen from Source

## Prerequisites

- **Windows** (7, 8, 10, or 11) -- required because the app is WPF-based
- **.NET 8 SDK** -- download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
- Optional: **Visual Studio 2022** (any edition) or **JetBrains Rider** for IDE support

## Quick start

```bash
git clone <repo-url>
cd Chigen
dotnet restore
dotnet build
```

This produces four assemblies under `src/*/bin/Debug/net8.0/` (and one under `net8.0-windows/` for the WPF project).

## Run the app

```bash
dotnet run --project src/Chigen.App/Chigen.App.csproj
```

Or open `Chigen.sln` in Visual Studio and press **F5**.

## Run the tests

```bash
dotnet test src/Chigen.Tests/Chigen.Tests.csproj
```

All 118 tests should pass. Test framework: **xUnit** with `Microsoft.NET.Test.Sdk` and `coverlet.collector` for code coverage.

## Publish a standalone executable

To produce a single `.exe` that runs on any Windows machine without the .NET runtime installed:

```bash
dotnet publish src/Chigen.App/Chigen.App.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --output ./publish
```

The output will be in `./publish/`. You can distribute the entire folder.

### Trimmed publish (smaller size)

```bash
dotnet publish src/Chigen.App/Chigen.App.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --p:PublishTrimmed=true \
  --output ./publish-trimmed
```

## Project dependencies

```
Chigen.App --> Chigen.Core
Chigen.App --> Chigen.DocumentExport
Chigen.DocumentExport --> Chigen.Core
Chigen.Tests --> Chigen.Core
Chigen.Tests --> Chigen.DocumentExport
```

## Target frameworks

| Project | Framework |
|---------|-----------|
| `Chigen.App` | `net8.0-windows` (WPF) |
| `Chigen.Core` | `net8.0` |
| `Chigen.DocumentExport` | `net8.0` |
| `Chigen.Tests` | `net8.0` |

All projects use `<ImplicitUsings>enable</ImplicitUsings>` and `<Nullable>enable</Nullable>`.

## NuGet packages

| Package | Version | Used in |
|---------|---------|---------|
| `DocumentFormat.OpenXml` | 3.5.1 | Core, DocumentExport |
| `CommunityToolkit.Mvvm` | 8.4.2 | App |
| `Microsoft.Office.Interop.Word` | 15.0.4797.1004 | DocumentExport |
| `PdfSharp` | 6.2.4 | DocumentExport |
| `xunit` | 2.5.3 | Tests |
| `xunit.runner.visualstudio` | 2.5.3 | Tests |
| `Microsoft.NET.Test.Sdk` | 17.8.0 | Tests |
| `coverlet.collector` | 6.0.0 | Tests |

## Notes

- The WPF project (`Chigen.App`) **must** be built on Windows. Cross-platform builds are not supported.
- PDF export via Word Interop requires Microsoft Word to be installed at runtime. The app auto-detects this and falls back to direct PDF generation via PdfSharp if Word is unavailable.
- All configuration files (letterhead, templates, hotkeys) are JSON files stored at `%LOCALAPPDATA%\Chigen\`. Defaults are used if the files are missing or corrupt.
