using ExamSimulator.Services;

namespace ExamSimulator.UI
{
    public class ConsoleUI
    {
        private readonly TrainingController _trainingController;
        private readonly EditorController _editorController;
        private readonly SettingsController _settingsController;

        private readonly StatisticsService _statsService;


        public ConsoleUI(TrainingController trainingController, EditorController editorController, SettingsController settingsController, StatisticsService statsService)
        {
            _trainingController = trainingController;
            _editorController = editorController;
            _settingsController = settingsController;
            _statsService = statsService; 
        }


        public void Run()
        {
            try
            {
                string input = "";
                while (input != "0")
                {
                    PrintMainMenu();
                    input = Console.ReadLine();
                    ProcessInput(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Деталі: {ex.Message}");
                Console.WriteLine("Будь ласка, перевірте файли даних у папці 'data' на наявність пошкоджень.");
                Console.WriteLine("Натисніть Enter для завершення роботи...");
                Console.ReadLine();
            }            
        }


        private void PrintMainMenu()
        {
            Console.WriteLine("\n--- Головне меню ---");
            Console.WriteLine("1 - Почати тренування");
            Console.WriteLine("2 - Редактор контенту (Теми та Запитання)");
            Console.WriteLine("3 - Налаштування");
            Console.WriteLine("4 - Переглянути статистику");
            Console.WriteLine("0 - Вихід");
            Console.Write("Оберіть дію: ");
        }


        private void ProcessInput(string input)
        {
            if (input == "1")
            {
                _trainingController.StartTraining();
            }
            else if (input == "2")
            {
                _editorController.ManageContent();
            }
            else if (input == "3")
            {
                _settingsController.ManageSettings();
            }
            else if (input == "4")
            {
                _statsService.ShowStatistics();
            }
            else if (input == "0")
            {
                Console.WriteLine("До побачення!");
            }
            else
            {
                Console.WriteLine("Невідома команда.");
            }
        }
    }
}
