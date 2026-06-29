using System.Text.Json;
using Chigen.Core.Models;

namespace Chigen.Core.Services
{
    public class TemplateService
    {
        private static readonly string ConfigFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chigen");

        private static readonly string LetterheadPath = Path.Combine(ConfigFolder, "letterhead.json");
        private static readonly string TemplatePath = Path.Combine(ConfigFolder, "templates.json");
        private static readonly string CellConfigPath = Path.Combine(ConfigFolder, "cell_config.json");
        private static readonly string HotkeyPath = Path.Combine(ConfigFolder, "hotkeys.json");

        static TemplateService()
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        public static LetterheadConfig LoadLetterhead()
        {
            if (File.Exists(LetterheadPath))
            {
                try
                {
                    var json = File.ReadAllText(LetterheadPath);
                    return JsonSerializer.Deserialize<LetterheadConfig>(json) ?? new LetterheadConfig();
                }
                catch
                {
                    return new LetterheadConfig();
                }
            }
            return new LetterheadConfig();
        }

        public static void SaveLetterhead(LetterheadConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LetterheadPath, json);
        }

        public static DocumentTemplate LoadTemplate(string name = "Default")
        {
            if (File.Exists(TemplatePath))
            {
                try
                {
                    var json = File.ReadAllText(TemplatePath);
                    var templates = JsonSerializer.Deserialize<List<DocumentTemplate>>(json);
                    return templates?.FirstOrDefault(t => t.Name == name) ?? new DocumentTemplate();
                }
                catch
                {
                    return new DocumentTemplate();
                }
            }
            return new DocumentTemplate();
        }

        public static void SaveTemplate(DocumentTemplate template)
        {
            List<DocumentTemplate> templates = [];
            if (File.Exists(TemplatePath))
            {
                try
                {
                    var json = File.ReadAllText(TemplatePath);
                    templates = JsonSerializer.Deserialize<List<DocumentTemplate>>(json) ?? [];
                }
                catch { }
            }

            var existing = templates.FirstOrDefault(t => t.Name == template.Name);
            if (existing != null)
                templates.Remove(existing);
            templates.Add(template);

            File.WriteAllText(TemplatePath, JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static List<HotkeyMappingEntry> LoadHotkeyMappings()
        {
            if (!File.Exists(HotkeyPath))
            {
                return BuildDefaultMappings();
            }

            try
            {
                var json = File.ReadAllText(HotkeyPath);
                return JsonSerializer.Deserialize<List<HotkeyMappingEntry>>(json) ?? BuildDefaultMappings();
            }
            catch
            {
                return BuildDefaultMappings();
            }
        }

        public static void SaveHotkeyMappings(List<HotkeyMappingEntry> mappings)
        {
            var json = JsonSerializer.Serialize(mappings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(HotkeyPath, json);
        }

        private static List<HotkeyMappingEntry> BuildDefaultMappings()
        {
            var entries = new List<HotkeyMappingEntry>();

            foreach (var ct in CellTypeProvider.GetDefaultPeripheralBloodTypes())
            {
                entries.Add(new HotkeyMappingEntry
                {
                    CellTypeId = ct.Id,
                    Key = ct.Key,
                    Mode = CounterMode.PeripheralBlood
                });
            }

            foreach (var ct in CellTypeProvider.GetDefaultBoneMarrowTypes())
            {
                entries.Add(new HotkeyMappingEntry
                {
                    CellTypeId = ct.Id,
                    Key = ct.Key,
                    Mode = CounterMode.BoneMarrow
                });
            }

            return entries;
        }
    }
}
