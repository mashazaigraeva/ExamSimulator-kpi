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
                Console.WriteLine("Немає доступних тем для тестування.");
                return;
            }

            PrintTopics(topics);
            int topicIndex = GetTopicSelection(topics.Count);

            if (topicIndex == -1)
            {
                return; 
            }

            Topic selectedTopic = topics[topicIndex];
            RunSession(selectedTopic);
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

        private void RunSession(Topic topic)
        {
            AppConfig config = _settingsController.GetCurrentConfig();
            
            Func<Question, bool> allQuestionsFilter = delegate (Question q) { return true; };

            List<Question> questions = _trainingService.GenerateSession(topic, allQuestionsFilter, config.QuestionsPerSession);

            if (questions.Count == 0)
            {
                Console.WriteLine("У цій темі немає запитань.");
                return;
            }

            double totalScore = 0.0;

            for (int i = 0; i < questions.Count; i++)
            {
                totalScore += AskQuestion(questions[i], i + 1, config.ShowCorrectAnswersAfterQuestion);
            }

            Console.WriteLine($"\nТренування завершено! Ваш бал: {totalScore} з {questions.Count}");
            _statsService.RecordSessionResult(topic, totalScore, questions.Count);
        }

        private double AskQuestion(Question question, int questionNumber, bool showCorrect)
        {
            Console.WriteLine($"\nЗапитання {questionNumber}: {question.Text}");
            _trainingService.ShuffleOptions(question);
            PrintOptions(question);

            Console.Write("Ваша відповідь (якщо декілька, введіть через кому): ");
            string answer = Console.ReadLine();

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
            
            double score = question.CalculateScore(userAnswers);

            Console.WriteLine($"Нараховано балів: {score}");

            if (showCorrect && score < 1.0)
            {
                Console.WriteLine("Ви зробили помилку або відповіли неповністю.");
            }

            return score;
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