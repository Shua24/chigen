using System.Text.Json;
using Chigen.Core.Models;

namespace Chigen.Core.Services
{
    public class AppConfigV1
    {
        public LetterheadConfig Letterhead { get; set; } = new();
        public DocumentTemplate Template { get; set; } = new();
        public List<HotkeyMappingEntry> Hotkeys { get; set; } = [];
        public string Language { get; set; } = "en";
        public string Theme { get; set; } = "Light";
    }

    public class TemplateService
    {
        // Allow tests to override the config directory
        internal static string? TestConfigFolderOverride { get; set; }

        private static string ConfigFolder =>
            TestConfigFolderOverride ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Chigen");

        private static string ConfigPath => Path.Combine(ConfigFolder, "config.json");

        static TemplateService()
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        private static AppConfigV1 LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    var json = File.ReadAllText(ConfigPath);
                    var cfg = JsonSerializer.Deserialize<AppConfigV1>(json);
                    if (cfg != null)
                    {
                        cfg.Hotkeys ??= [];
                        return cfg;
                    }
                }
                catch { }
            }

            // Migrate from legacy files if they exist
            var config = new AppConfigV1();
            bool hasLegacyData = false;

            var legacyLetterheadPath = Path.Combine(ConfigFolder, "letterhead.json");
            if (File.Exists(legacyLetterheadPath))
            {
                try
                {
                    var json = File.ReadAllText(legacyLetterheadPath);
                    var letterhead = JsonSerializer.Deserialize<LetterheadConfig>(json);
                    if (letterhead != null)
                    {
                        config.Letterhead = letterhead;
                        hasLegacyData = true;
                    }
                }
                catch { }
            }

            var legacyTemplatesPath = Path.Combine(ConfigFolder, "templates.json");
            if (File.Exists(legacyTemplatesPath))
            {
                try
                {
                    var json = File.ReadAllText(legacyTemplatesPath);
                    var templates = JsonSerializer.Deserialize<List<DocumentTemplate>>(json);
                    if (templates != null && templates.Count > 0)
                    {
                        config.Template = templates[0];
                        hasLegacyData = true;
                    }
                }
                catch { }
            }

            var legacyHotkeysPath = Path.Combine(ConfigFolder, "hotkeys.json");
            if (File.Exists(legacyHotkeysPath))
            {
                try
                {
                    var json = File.ReadAllText(legacyHotkeysPath);
                    var hotkeys = JsonSerializer.Deserialize<List<HotkeyMappingEntry>>(json);
                    if (hotkeys != null && hotkeys.Count > 0)
                    {
                        config.Hotkeys = hotkeys;
                        hasLegacyData = true;
                    }
                }
                catch { }
            }

            // Persist migrated config
            if (hasLegacyData)
            {
                SaveConfig(config);
            }

            config.Hotkeys ??= [];
            return config;
        }

        private static void SaveConfig(AppConfigV1 config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            var tempPath = Path.Combine(ConfigFolder, $"config.tmp.{Guid.NewGuid():N}");

            try
            {
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, ConfigPath, overwrite: true);
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
                throw;
            }
        }

        public static LetterheadConfig LoadLetterhead() => LoadConfig().Letterhead;
        public static void SaveLetterhead(LetterheadConfig config)
        {
            var cfg = LoadConfig();
            cfg.Letterhead = config;
            SaveConfig(cfg);
        }

        public static DocumentTemplate LoadTemplate(string name = "Default") => LoadConfig().Template;
        public static void SaveTemplate(DocumentTemplate template)
        {
            var cfg = LoadConfig();
            cfg.Template = template;
            SaveConfig(cfg);
        }

        public static string LoadLanguage() => LoadConfig().Language;
        public static void SaveLanguage(string lang)
        {
            var cfg = LoadConfig();
            cfg.Language = lang;
            SaveConfig(cfg);
        }

        public static string LoadTheme() => LoadConfig().Theme;
        public static void SaveTheme(string theme)
        {
            var cfg = LoadConfig();
            cfg.Theme = theme;
            SaveConfig(cfg);
        }

        public static List<HotkeyMappingEntry> LoadHotkeyMappings()
        {
            var cfg = LoadConfig();
            if (cfg.Hotkeys.Count > 0) return cfg.Hotkeys;
            return BuildDefaultMappings();
        }

        public static void SaveHotkeyMappings(List<HotkeyMappingEntry> mappings)
        {
            var cfg = LoadConfig();
            cfg.Hotkeys = mappings;
            SaveConfig(cfg);
        }

        private static List<HotkeyMappingEntry> BuildDefaultMappings()
        {
            var entries = new List<HotkeyMappingEntry>();
            foreach (var ct in CellTypeProvider.GetDefaultPeripheralBloodTypes())
            {
                entries.Add(new HotkeyMappingEntry { CellTypeId = ct.Id, Key = ct.Key, Mode = CounterMode.PeripheralBlood });
            }
            foreach (var ct in CellTypeProvider.GetDefaultBoneMarrowTypes())
            {
                entries.Add(new HotkeyMappingEntry { CellTypeId = ct.Id, Key = ct.Key, Mode = CounterMode.BoneMarrow });
            }
            return entries;
        }
    }
}
