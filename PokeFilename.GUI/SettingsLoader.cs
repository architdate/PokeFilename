using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using PokeFilename.API;

namespace PokeFilename
{
    public static class SettingsLoader
    {
        public static readonly string WorkingDirectory = Application.StartupPath;
        public static readonly string ConfigPath = Path.Combine(WorkingDirectory, "namer.json");

        public static EntityNamerSettings GetSettings() => GetSettings<EntityNamerSettings>(ConfigPath);
        public static void SetSettings(EntityNamerSettings obj) => SetSettings(obj, ConfigPath);

        private static T GetSettings<T>(string configPath) where T : new()
        {
            if (!File.Exists(configPath))
                return new T();

            try
            {
                var lines = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<T>(lines) ?? new T();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return new T();
            }
        }

        private static void SetSettings<T>(T cfg, string path)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    NullValueHandling = NullValueHandling.Ignore,
                };
                var text = JsonConvert.SerializeObject(cfg, settings);
                File.WriteAllText(path, text);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
            }
        }
    }
}
