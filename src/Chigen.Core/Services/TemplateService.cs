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
        private static readonly string ConfigFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chigen");

        private static readonly string ConfigPath = Path.Combine(ConfigFolder, "config.json");

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
                    return JsonSerializer.Deserialize<AppConfigV1>(json) ?? new AppConfigV1();
                }
                catch { }
            }
            return new AppConfigV1();
        }

        private static void SaveConfig(AppConfigV1 config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
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
