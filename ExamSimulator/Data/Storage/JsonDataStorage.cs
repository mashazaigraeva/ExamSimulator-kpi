using System.Text.Json;
using ExamSimulator.Data.Interfaces;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace ExamSimulator.Data.Storage
{
    public class JsonDataStorage<T> : IDataStorage<T>
    {
        public void SaveToFile(List<T> data, string filePath)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                JsonSerializerOptions options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                };
                
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Помилка збереження у файл {filePath}: {ex.Message}", ex);
            }
        }

        public List<T> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                List<T> result = JsonSerializer.Deserialize<List<T>>(json);
                
                if (result == null)
                {
                    return new List<T>();
                }
                
                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Неправильний формат JSON у файлі {filePath}.", ex);
            }
            catch (Exception ex)
            {
                throw new IOException($"Помилка читання з файлу {filePath}: {ex.Message}", ex);
            }
        }
    }
}