using ExamSimulator.Data.Storage;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.UI
{
    public class SettingsController
    {
        private readonly ConfigManager _configManager;
        private AppConfig _currentConfig;

        public SettingsController(ConfigManager configManager)
        {
            _configManager = configManager;
            _currentConfig = _configManager.LoadConfig();
        }

        public AppConfig GetCurrentConfig()
        {
            return _currentConfig;
        }

        public void ManageSettings()
        {
            string choice = "";
            while (choice != "0")
            {
                PrintSettingsMenu();
                choice = Console.ReadLine();
                ProcessSettingsChoice(choice);
            }
        }

        private void PrintSettingsMenu()
        {
            Console.WriteLine("\n--- Налаштування ---");
            Console.WriteLine($"1. Кількість питань у сесії: {_currentConfig.QuestionsPerSession}");
            Console.WriteLine($"2. Показувати правильні відповіді: {_currentConfig.ShowCorrectAnswersAfterQuestion}");
            Console.WriteLine("0. Повернутися назад");
            Console.Write("Що бажаєте змінити? ");
        }

        private void ProcessSettingsChoice(string choice)
        {
            if (choice == "1")
            {
                ChangeQuestionsAmount();
            }
            else if (choice == "2")
            {
                _currentConfig.ShowCorrectAnswersAfterQuestion = !_currentConfig.ShowCorrectAnswersAfterQuestion;
                _configManager.SaveConfig(_currentConfig);
                Console.WriteLine("Налаштування змінено.");
            }
        }

        private void ChangeQuestionsAmount()
        {
            Console.Write("Введіть нову кількість питань (більше 0): ");
            string amountStr = Console.ReadLine();
            int amount;

            if (int.TryParse(amountStr, out amount) && amount > 0)
            {
                _currentConfig.QuestionsPerSession = amount;
                _configManager.SaveConfig(_currentConfig);
                Console.WriteLine("Налаштування збережено.");
            }
            else
            {
                Console.WriteLine("Невірне значення.");
            }
        }
    }
}