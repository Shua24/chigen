using Chigen.Core.Models;
using Chigen.Core.Services;

namespace Chigen.Tests.Services
{
    public class TemplateServiceTests : IDisposable
    {
        private static readonly string ConfigFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chigen");

        private static readonly string ConfigPath = Path.Combine(ConfigFolder, "config.json");

        public TemplateServiceTests()
        {
            Directory.CreateDirectory(ConfigFolder);
            // Start each test with no config file
            if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
        }

        [Fact]
        public void LoadLetterhead_ReturnsDefault_WhenFileMissing()
        {
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
        public void SaveAndLoadLanguage_PersistsCorrectly()
        {
            TemplateService.SaveLanguage("id");
            Assert.Equal("id", TemplateService.LoadLanguage());
        }

        [Fact]
        public void SaveAndLoadTheme_PersistsCorrectly()
        {
            TemplateService.SaveTheme("Dark");
            Assert.Equal("Dark", TemplateService.LoadTheme());
        }

        [Fact]
        public void LoadHotkeyMappings_ReturnsDefaults_WhenEmpty()
        {
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
        public void LoadConfig_ReturnsDefault_WhenFileCorrupt()
        {
            File.WriteAllText(ConfigPath, "not valid json");

            var config = TemplateService.LoadLetterhead();
            Assert.Equal("My Hospital", config.InstitutionName);
        }

        [Fact]
        public void SaveConfig_CreatesFileWhenNotExists()
        {
            var t = new DocumentTemplate { Name = "NewTemplate" };
            TemplateService.SaveTemplate(t);

            Assert.True(File.Exists(ConfigPath));
            var loaded = TemplateService.LoadTemplate("NewTemplate");
            Assert.Equal("NewTemplate", loaded.Name);
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
            }
            catch { }
        }
    }
}
