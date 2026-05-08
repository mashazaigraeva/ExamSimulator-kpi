using ExamSimulator.Services;
using ExamSimulator.Domain.Models;
using ExamSimulator.Domain.Enums;
using ExamSimulator.Domain.Structs;

namespace ExamSimulator.UI
{
    public class EditorController
    {
        private readonly ContentManagerService _contentService;

        public EditorController(ContentManagerService contentService)
        {
            _contentService = contentService;
        }

        public void ManageContent()
        {
            string choice = "";
            while (choice != "0")
            {
                PrintEditorMenu();
                choice = Console.ReadLine();
                ProcessEditorChoice(choice);
            }
        }

        private void PrintEditorMenu()
        {
            Console.WriteLine("\n--- Редактор контенту ---");
            Console.WriteLine("1 - Додати нову тему");
            Console.WriteLine("2 - Видалити тему");
            Console.WriteLine("3 - Показати всі теми");
            Console.WriteLine("4 - Додати новий тест до теми");
            Console.WriteLine("5 - Додати запитання до тесту");
            Console.WriteLine("0 - Повернутися назад");
            Console.Write("Оберіть дію: ");
        }

        private void ProcessEditorChoice(string choice)
        {
            if (choice == "1")
            {
                AddNewTopic();
            }
            else if (choice == "2")
            {
                DeleteTopic();
            }
            else if (choice == "3")
            {
                ShowAllTopics();
            }
            else if (choice == "4")
            {
                AddNewTestToTopic();
            }
            else if (choice == "5")
            {
                AddQuestionToTest();
            }
        }

        private void AddNewTopic()
        {
            Console.Write("Введіть назву нової теми: ");
            string name = Console.ReadLine();

            bool success = _contentService.CreateTopic(name);
            
            if (success)
            {
                Console.WriteLine("Тему успішно додано.");
            }
            else
            {
                Console.WriteLine("Назва не може бути порожньою.");
            }
        }

        private void DeleteTopic()
        {
            List<Topic> topics = _contentService.GetAllTopics();
            if (topics.Count == 0)
            {
                Console.WriteLine("Список тем порожній.");
                return;
            }

            ShowAllTopics();
            Console.Write("Введіть номер теми для видалення: ");
            string input = Console.ReadLine();
            int index;

            if (int.TryParse(input, out index) && index >= 1 && index <= topics.Count)
            {
                Topic topicToDelete = topics[index - 1];
                
                bool success = _contentService.DeleteTopic(topicToDelete.Id);
                
                if (success)
                {
                    Console.WriteLine("Тему видалено.");
                }
                else
                {
                    Console.WriteLine("Помилка під час видалення.");
                }
            }
            else
            {
                Console.WriteLine("Невірний номер.");
            }
        }

        private void ShowAllTopics()
        {
            List<Topic> topics = _contentService.GetAllTopics();
            Console.WriteLine("\nСписок тем:");
            for (int i = 0; i < topics.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {topics[i].Name}");
            }
        }

        private void AddNewTestToTopic()
        {
            List<Topic> topics = _contentService.GetAllTopics();
            
            if (topics.Count == 0)
            {
                Console.WriteLine("Список тем порожній. Спочатку створіть хоча б одну тему.");
                return;
            }

            ShowAllTopics();
            Console.Write("\nВведіть номер теми, до якої бажаєте додати тест: ");
            string input = Console.ReadLine();
            int index;

            if (int.TryParse(input, out index) && index >= 1 && index <= topics.Count)
            {
                Console.Write("Введіть назву нового тесту: ");
                string testName = Console.ReadLine();

                Topic selectedTopic = topics[index - 1];
                bool success = _contentService.AddTestToTopic(selectedTopic.Id, testName);

                if (success)
                {
                    Console.WriteLine("Тест успішно додано до теми.");
                }
                else
                {
                    Console.WriteLine("Помилка: назва тесту не може бути порожньою.");
                }
            }
            else
            {
                Console.WriteLine("Невірний номер теми.");
            }
        }

        private void AddQuestionToTest()
        {
            List<Topic> topics = _contentService.GetAllTopics();
            if (topics.Count == 0) return;

            ShowAllTopics();
            Console.Write("Оберіть номер теми: ");
            int tIndex = int.Parse(Console.ReadLine()) - 1;

            if (tIndex < 0 || tIndex >= topics.Count) return;
            Topic topic = topics[tIndex];

            if (topic.Tests.Count == 0)
            {
                Console.WriteLine("У цій темі ще немає тестів.");
                return;
            }

            ShowTestsInTopic(topic);
            Console.Write("Оберіть номер тесту: ");
            int testIndex = int.Parse(Console.ReadLine()) - 1;

            if (testIndex >= 0 && testIndex < topic.Tests.Count)
            {
                CreateQuestionFlow(topic.Id, topic.Tests[testIndex].Id);
            }
        }

        private void ShowTestsInTopic(Topic topic)
        {
            Console.WriteLine("\nТести в темі:");
            for (int i = 0; i < topic.Tests.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {topic.Tests[i].Title}");
            }
        }

        private void CreateQuestionFlow(Guid topicId, Guid testId)
        {
            Console.WriteLine("\nТип запитання: 1 - Одиночний вибір, 2 - Множинний вибір, 3 - Відкрита відповідь");
            string type = Console.ReadLine();

            Console.Write("Текст запитання: ");
            string text = Console.ReadLine();
            
            Console.WriteLine("Складність: 1 - Easy, 2 - Medium, 3 - Hard");
            Console.Write("Оберіть рівень: ");
            int diffInput = int.Parse(Console.ReadLine());
            
            DifficultyLevel diff = (DifficultyLevel)diffInput;

            Question newQuestion = null;

            if (type == "1")
            {
                newQuestion = CreateSingleChoice(text, diff);
            }
            else if (type == "2")
            {
                newQuestion = CreateMultipleChoice(text, diff);
            }
            else if (type == "3")
            {
                newQuestion = CreateOpenEndedQuestion(text, diff);
            }

            if (newQuestion != null)
            {
                _contentService.AddQuestionToTest(topicId, testId, newQuestion);
                Console.WriteLine("Запитання додано.");
            }
        }

        private SingleChoiceQuestion CreateSingleChoice(string text, DifficultyLevel diff)
        {
            SingleChoiceQuestion q = new SingleChoiceQuestion { Text = text, Difficulty = diff };
            Console.Write("Кількість варіантів: ");
            int count = int.Parse(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                Console.Write($"Варіант {i + 1}: ");
                string optText = Console.ReadLine();
                Console.Write("Це правильна відповідь? (y/n): ");
                string answer = Console.ReadLine();
                bool isCorrect = false;
                if (answer == "y" || answer == "Y")
                {
                    isCorrect = true;
                }
                
                q.Options.Add(new AnswerOption { Text = optText, IsCorrect = isCorrect });
            }
            return q;
        }

        private MultipleChoiceQuestion CreateMultipleChoice(string text, DifficultyLevel diff)
        {
            MultipleChoiceQuestion q = new MultipleChoiceQuestion { Text = text, Difficulty = diff };
            Console.Write("Кількість варіантів: ");
            int count = int.Parse(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                Console.Write($"Варіант {i + 1}: ");
                string optText = Console.ReadLine();
                Console.Write("Правильний? (y/n): ");
                
                string answer = Console.ReadLine();
                bool isCorrect = false;
                if (answer == "y" || answer == "Y")
                {
                    isCorrect = true;
                }

                q.Options.Add(new AnswerOption { Text = optText, IsCorrect = isCorrect });
            }
            return q;
        }

        private OpenQuestion CreateOpenEndedQuestion(string text, DifficultyLevel diff)
        {
            OpenQuestion q = new OpenQuestion { Text = text, Difficulty = diff };
            
            Console.Write("Введіть правильну відповідь (еталон): ");
            q.CorrectAnswerText = Console.ReadLine();
            
            return q;
        }
    }
}