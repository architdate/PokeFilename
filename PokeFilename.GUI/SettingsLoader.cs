using System.IO;
using System.Windows.Forms;
using PokeFilename.API;
using System.Text.Json;

namespace PokeFilename
{
    public static class SettingsLoader
    {
        private static readonly string WorkingDirectory = Application.StartupPath;
        private static readonly string ConfigPath = Path.Combine(WorkingDirectory, "namer.json");

        public static EntityNamerSettings GetSettings() => GetSettings<EntityNamerSettings>(ConfigPath);
        public static void SetSettings(EntityNamerSettings obj) => SetSettings(obj, ConfigPath);

        private static T GetSettings<T>(string configPath) where T : new()
        {
            if (!File.Exists(configPath))
                return new T();

            try
            {
                var text = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<T>(text)!;
            }
            catch
            {
                return new T();
            }
        }

        private static readonly JsonSerializerOptions options = new() { WriteIndented = true };
        private static void SetSettings<T>(T cfg, string path)
        {
            try
            {
                string output = JsonSerializer.Serialize(cfg, options);
                File.WriteAllText(path, output);
            }
            catch
            {
                // ignored
            }
        }
    }
}
