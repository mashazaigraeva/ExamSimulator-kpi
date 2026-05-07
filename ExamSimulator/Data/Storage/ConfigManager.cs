using System.Text.Json;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Data.Storage
{
    public class ConfigManager
    {
        private readonly string _filePath;

        public ConfigManager(string filePath)
        {
            _filePath = filePath;
        }

        public AppConfig LoadConfig()
        {
            if (!File.Exists(_filePath))
            {
                return new AppConfig();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                AppConfig config = JsonSerializer.Deserialize<AppConfig>(json);
                
                if (config != null)
                {
                    return config;
                }
                
                return new AppConfig();
            }
            catch (Exception)
            {
                return new AppConfig();
            }
        }

        public void SaveConfig(AppConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                };
                
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Помилка збереження конфігурації: {ex.Message}", ex);
            }
        }
    }
}