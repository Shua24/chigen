using System.Text.Json;
using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.Tests.Services
{
    public class TemplateServiceTests : IDisposable
    {
        private static readonly string ConfigFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chigen");

        public TemplateServiceTests()
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        [Fact]
        public void LoadLetterhead_ReturnsDefault_WhenFileMissing()
        {
            var letterheadPath = Path.Combine(ConfigFolder, "letterhead.json");
            if (File.Exists(letterheadPath)) File.Delete(letterheadPath);

            var config = TemplateService.LoadLetterhead();
            Assert.Equal("My Hospital", config.InstitutionName);
        }

        [Fact]
        public void SaveAndLoadLetterhead_PersistsCorrectly()
        {
            var original = new LetterheadConfig
            {
                InstitutionName = "Test Hospital",
                Department = "Test Lab",
                Address = "123 Test St",
                Phone = "555-TEST",
                Email = "test@lab.com",
                LogoPath = @"C:\test\logo.png",
                LogoPlacement = LogoPlacement.Top,
                FooterText = "Test footer"
            };

            TemplateService.SaveLetterhead(original);
            var loaded = TemplateService.LoadLetterhead();

            Assert.Equal(original.InstitutionName, loaded.InstitutionName);
            Assert.Equal(original.Department, loaded.Department);
            Assert.Equal(original.Address, loaded.Address);
            Assert.Equal(original.Phone, loaded.Phone);
            Assert.Equal(original.Email, loaded.Email);
            Assert.Equal(original.LogoPath, loaded.LogoPath);
            Assert.Equal(original.LogoPlacement, loaded.LogoPlacement);
            Assert.Equal(original.FooterText, loaded.FooterText);
        }

        [Fact]
        public void LoadTemplate_ReturnsDefault_WhenFileMissing()
        {
            var templatePath = Path.Combine(ConfigFolder, "templates.json");
            if (File.Exists(templatePath)) File.Delete(templatePath);

            var template = TemplateService.LoadTemplate();
            Assert.Equal("Default", template.Name);
            Assert.True(template.ShowLetterhead);
        }

        [Fact]
        public void SaveAndLoadTemplate_PersistsCorrectly()
        {
            var original = new DocumentTemplate
            {
                Name = "Compact",
                ShowLetterhead = false,
                ShowPatientInfo = false,
                ShowReferenceRanges = false,
                ShowConclusion = false,
                ShowFooter = false
            };

            TemplateService.SaveTemplate(original);
            var loaded = TemplateService.LoadTemplate("Compact");

            Assert.Equal("Compact", loaded.Name);
            Assert.False(loaded.ShowLetterhead);
            Assert.False(loaded.ShowPatientInfo);
            Assert.False(loaded.ShowReferenceRanges);
            Assert.False(loaded.ShowConclusion);
            Assert.False(loaded.ShowFooter);
        }

        [Fact]
        public void LoadTemplate_WithSpecificName_ReturnsCorrectTemplate()
        {
            var t1 = new DocumentTemplate { Name = "A", HeaderFormat = "Format A" };
            var t2 = new DocumentTemplate { Name = "B", HeaderFormat = "Format B" };

            TemplateService.SaveTemplate(t1);
            TemplateService.SaveTemplate(t2);

            var loaded = TemplateService.LoadTemplate("A");
            Assert.Equal("Format A", loaded.HeaderFormat);
        }

        [Fact]
        public void LoadTemplate_WithUnknownName_ReturnsDefault()
        {
            var loaded = TemplateService.LoadTemplate("NonExistent");
            Assert.Equal("Default", loaded.Name);
        }

        [Fact]
        public void SaveTemplate_UpdatesExisting()
        {
            var t = new DocumentTemplate { Name = "Updatable", HeaderFormat = "V1" };
            TemplateService.SaveTemplate(t);

            t.HeaderFormat = "V2";
            TemplateService.SaveTemplate(t);

            var loaded = TemplateService.LoadTemplate("Updatable");
            Assert.Equal("V2", loaded.HeaderFormat);
        }

        [Fact]
        public void LoadHotkeyMappings_ReturnsDefaults_WhenFileMissing()
        {
            var hotkeyPath = Path.Combine(ConfigFolder, "hotkeys.json");
            if (File.Exists(hotkeyPath)) File.Delete(hotkeyPath);

            var mappings = TemplateService.LoadHotkeyMappings();

            Assert.NotEmpty(mappings);
            var pbMappings = mappings.Where(m => m.Mode == CounterMode.PeripheralBlood).ToList();
            var bmMappings = mappings.Where(m => m.Mode == CounterMode.BoneMarrow).ToList();
            Assert.Equal(12, pbMappings.Count);
            Assert.Equal(17, bmMappings.Count);
        }

        [Fact]
        public void SaveAndLoadHotkeyMappings_PersistsCorrectly()
        {
            var original = new List<HotkeyMappingEntry>
            {
                new() { CellTypeId = "neutrophil", Key = "Q", Mode = CounterMode.PeripheralBlood },
                new() { CellTypeId = "monoblast", Key = "Z", Mode = CounterMode.BoneMarrow }
            };

            TemplateService.SaveHotkeyMappings(original);
            var loaded = TemplateService.LoadHotkeyMappings();

            var neutrophil = loaded.Find(m => m.CellTypeId == "neutrophil");
            Assert.NotNull(neutrophil);
            Assert.Equal("Q", neutrophil!.Key);
            Assert.Equal(CounterMode.PeripheralBlood, neutrophil.Mode);

            var monoblast = loaded.Find(m => m.CellTypeId == "monoblast");
            Assert.NotNull(monoblast);
            Assert.Equal("Z", monoblast!.Key);
            Assert.Equal(CounterMode.BoneMarrow, monoblast.Mode);
        }

        [Fact]
        public void LoadLetterhead_ReturnsDefault_WhenFileCorrupt()
        {
            var letterheadPath = Path.Combine(ConfigFolder, "letterhead.json");
            File.WriteAllText(letterheadPath, "not valid json");

            var config = TemplateService.LoadLetterhead();
            Assert.Equal("My Hospital", config.InstitutionName);
        }

        [Fact]
        public void LoadTemplate_ReturnsDefault_WhenFileCorrupt()
        {
            var templatePath = Path.Combine(ConfigFolder, "templates.json");
            File.WriteAllText(templatePath, "not valid json");

            var template = TemplateService.LoadTemplate();
            Assert.Equal("Default", template.Name);
        }

        [Fact]
        public void SaveTemplate_CreatesFileWhenNotExists()
        {
            var templatePath = Path.Combine(ConfigFolder, "templates.json");
            if (File.Exists(templatePath)) File.Delete(templatePath);

            var t = new DocumentTemplate { Name = "NewTemplate" };
            TemplateService.SaveTemplate(t);

            Assert.True(File.Exists(templatePath));
            var loaded = TemplateService.LoadTemplate("NewTemplate");
            Assert.Equal("NewTemplate", loaded.Name);
        }

        public void Dispose()
        {
            try
            {
                var testFiles = new[]
                {
                    Path.Combine(ConfigFolder, "letterhead.json"),
                    Path.Combine(ConfigFolder, "templates.json"),
                    Path.Combine(ConfigFolder, "hotkeys.json")
                };
                foreach (var f in testFiles)
                {
                    if (File.Exists(f)) File.Delete(f);
                }
            }
            catch { }
        }
    }
}
