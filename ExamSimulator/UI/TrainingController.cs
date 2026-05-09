using ExamSimulator.Services;
using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Models;
using ExamSimulator.Domain.Structs;

namespace ExamSimulator.UI
{
    public class TrainingController
    {
        private readonly TrainingService _trainingService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly SettingsController _settingsController;
        private readonly StatisticsService _statsService; 

        public TrainingController(TrainingService trainingService, IRepository<Topic> topicRepository, SettingsController settingsController, StatisticsService statsService)
        {
            _trainingService = trainingService;
            _topicRepository = topicRepository;
            _settingsController = settingsController;
            _statsService = statsService;
        }

        public void StartTraining()
        {
            List<Topic> topics = _topicRepository.GetAll();
            if (topics.Count == 0)
            {
                Console.WriteLine("Немає доступних тем.");
                return;
            }

            Console.WriteLine("\nДоступні теми:");
            for (int i = 0; i < topics.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {topics[i].Name}");
            }

            Console.Write("Оберіть номер теми: ");
            string topicInput = Console.ReadLine();

            int topicIndex = -1;
            if (int.TryParse(topicInput, out int parsedTopicIndex))
            {
                topicIndex = parsedTopicIndex - 1;
            }

            if (topicIndex < 0 || topicIndex >= topics.Count)
            {
                Console.WriteLine("Невірний вибір теми.");
                return;
            }

            Topic selectedTopic = topics[topicIndex];

            if (selectedTopic.Tests.Count == 0)
            {
                Console.WriteLine("У цій темі ще немає доступних тестів.");
                return;
            }

            Console.WriteLine($"\nДоступні тести у темі '{selectedTopic.Name}':");
            for (int i = 0; i < selectedTopic.Tests.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {selectedTopic.Tests[i].Title}");
            }

            Console.Write("Оберіть номер тесту: ");
            string testInput = Console.ReadLine();

            int testIndex = -1;
            if (int.TryParse(testInput, out int parsedTestIndex))
            {
                testIndex = parsedTestIndex - 1;
            }

            if (testIndex < 0 || testIndex >= selectedTopic.Tests.Count)
            {
                Console.WriteLine("Невірний вибір тесту.");
                return;
            }

            Test selectedTest = selectedTopic.Tests[testIndex];
            RunSession(selectedTopic, selectedTest); 
        }

        private void PrintTopics(List<Topic> topics)
        {
            Console.WriteLine("\nДоступні теми:");
            for (int i = 0; i < topics.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {topics[i].Name}");
            }
        }

        private int GetTopicSelection(int maxIndex)
        {
            Console.Write("Оберіть номер теми: ");
            string input = Console.ReadLine();
            int index;

            if (!int.TryParse(input, out index) || index < 1 || index > maxIndex)
            {
                Console.WriteLine("Невірний вибір.");
                return -1;
            }

            return index - 1;
        }

        private void RunSession(Topic topic, Test test)
        {
            AppConfig config = _settingsController.GetCurrentConfig();
            
            Func<Question, bool> allQuestionsFilter = delegate (Question q) { return true; };
            
            List<Question> questions = _trainingService.GenerateSession(test, allQuestionsFilter, config.QuestionsPerSession);

            if (questions.Count == 0)
            {
                Console.WriteLine("У цьому тесті немає запитань.");
                return;
            }

            double totalScore = 0.0;

            for (int i = 0; i < questions.Count; i++)
            {
                totalScore += AskQuestion(questions[i], i + 1, config.ShowCorrectAnswersAfterQuestion);
            }

            PrintSessionResult(totalScore, questions.Count, config.PassingScorePercentage);

            _statsService.RecordSessionResult(topic, totalScore, questions.Count);
        }

        private void PrintSessionResult(double totalScore, int maxScore, double passingPercentage)
        {
            Console.WriteLine($"\n--- РЕЗУЛЬТАТИ ТРЕНУВАННЯ ---");
            Console.WriteLine($"Ваш бал: {totalScore} з {maxScore}");

            double percentage = 0.0;
            if (maxScore > 0)
            {
                percentage = (totalScore / maxScore) * 100.0;
            }

            Console.WriteLine($"Успішність: {percentage:F1}% (Мінімум для складання: {passingPercentage}%)");

            if (percentage >= passingPercentage)
            {
                Console.WriteLine("Вітаємо! Тест успішно складено.");
            }
            else
            {
                Console.WriteLine("На жаль, тест не складено. Потрібно ще потренуватися!");
            }
        }

        private double AskQuestion(Question question, int questionNumber, bool showCorrect)
        {
            Console.WriteLine($"\nЗапитання {questionNumber}: {question.Text}");
            _trainingService.ShuffleOptions(question);
            PrintOptions(question);

            Console.Write("Ваша відповідь (якщо декілька, введіть через кому): ");
            string answer = Console.ReadLine();

            List<string> userAnswers = ExtractUserAnswers(question, answer);  
            
            double score = question.CalculateScore(userAnswers);

            Console.WriteLine($"Нараховано балів: {score}");

            PrintFeedback(question, score, showCorrect);

            return score;
        }

        private List<string> ExtractUserAnswers(Question question, string answer)
        {
            List<string> userAnswers = new List<string>();

            if (question is SingleChoiceQuestion)
            {
                SingleChoiceQuestion sq = (SingleChoiceQuestion)question;
                userAnswers = ConvertInputToAnswerTexts(answer, sq.Options);
            }
            else if (question is MultipleChoiceQuestion)
            {
                MultipleChoiceQuestion mq = (MultipleChoiceQuestion)question;
                userAnswers = ConvertInputToAnswerTexts(answer, mq.Options);
            }
            else if (question is OpenQuestion)
            {
                if (answer != null)
                {
                    userAnswers.Add(answer.Trim());
                }
            }

            return userAnswers;
        }

        private void PrintFeedback(Question question, double score, bool showCorrect)
        {
            if (score >= 1.0)
            {
                Console.WriteLine("Вірно!");
                return;
            }

            Console.WriteLine("Ви зробили помилку або відповіли неповністю.");

            if (showCorrect)
            {
                PrintCorrectAnswerForQuestion(question);
            }
        }

        private void PrintCorrectAnswerForQuestion(Question question)
        {
            Console.WriteLine("Правильна відповідь:");

            if (question is SingleChoiceQuestion)
            {
                SingleChoiceQuestion sq = (SingleChoiceQuestion)question;
                PrintCorrectOptionsList(sq.Options);
            }
            else if (question is MultipleChoiceQuestion)
            {
                MultipleChoiceQuestion mq = (MultipleChoiceQuestion)question;
                PrintCorrectOptionsList(mq.Options);
            }
            else if (question is OpenQuestion)
            {
                OpenQuestion oq = (OpenQuestion)question;
                Console.WriteLine("- " + oq.CorrectAnswerText);
            }
        }

        private void PrintCorrectOptionsList(List<AnswerOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].IsCorrect)
                {
                    Console.WriteLine("- " + options[i].Text);
                }
            }
        }

        private void PrintOptions(Question question)
        {
            if (question is SingleChoiceQuestion)
            {
                SingleChoiceQuestion single = (SingleChoiceQuestion)question;
                for (int j = 0; j < single.Options.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {single.Options[j].Text}");
                }
            }
            else if (question is MultipleChoiceQuestion)
            {
                MultipleChoiceQuestion multiple = (MultipleChoiceQuestion)question;
                for (int j = 0; j < multiple.Options.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {multiple.Options[j].Text}");
                }
            }
        }

        private List<string> ParseAnswers(string input)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] parts = input.Split(',');
                for (int j = 0; j < parts.Length; j++)
                {
                    result.Add(parts[j].Trim());
                }
            }
            return result;
        }

        private List<string> ConvertInputToAnswerTexts(string input, List<AnswerOption> displayedOptions)
        {
            List<string> result = new List<string>();
            
            if (input == null || input.Trim() == "")
            {
                return result;
            }

            string[] parts = input.Split(',');

            for (int i = 0; i < parts.Length; i++)
            {
                string trimmedPart = parts[i].Trim();
                int index;

                if (int.TryParse(trimmedPart, out index))
                {
                    if (index >= 1 && index <= displayedOptions.Count)
                    {
                        result.Add(displayedOptions[index - 1].Text);
                    }
                }
                else
                {
                    result.Add(trimmedPart);
                }
            }
            return result;
        }
    }
}