using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TomoAIO.Infrastructure
{
    internal class SaveConfig
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TomoAIO", "config.json");

        public static string? LoadSavePath()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return null;
                var json = File.ReadAllText(ConfigPath);
                var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("savePath").GetString();
            }
            catch { return null; }
        }

        public static void StoreSavePath(string path)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                var json = JsonSerializer.Serialize(new { savePath = path });
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }
    }
}
