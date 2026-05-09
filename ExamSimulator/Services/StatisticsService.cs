using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Services
{
    public class StatisticsService
    {
        private readonly IRepository<UserStatistics> _statsRepository;
        private readonly IDataStorage<UserStatistics> _statsStorage;
        private readonly string _storagePath;

        public StatisticsService(IRepository<UserStatistics> statsRepository, IDataStorage<UserStatistics> statsStorage, string storagePath)
        {
            _statsRepository = statsRepository;
            _statsStorage = statsStorage;
            _storagePath = storagePath;
        }

        public void RecordSessionResult(Topic topic, double score, int maxScore)
        {
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            UserStatistics stat = new UserStatistics
            {
                TopicId = topic.Id,
                TopicName = topic.Name,
                SessionDate = DateTime.Now,
                TotalScore = score,
                MaxPossibleScore = maxScore
            };

            _statsRepository.Add(stat);
            SaveChanges();
        }

        public double CalculateAverageScorePercentage(string topicName)
        {
            List<UserStatistics> allStats = _statsRepository.GetAll();
            double totalPercentage = 0.0;
            int sessionsCount = 0;

            for (int i = 0; i < allStats.Count; i++)
            {
                UserStatistics current = allStats[i];
                
                if (current.TopicName == topicName)
                {
                    totalPercentage += current.CalculatePercentage();
                    sessionsCount++;
                }
            }

            if (sessionsCount == 0)
            {
                return 0.0;
            }

            return totalPercentage / sessionsCount;
        }

        public void ShowStatistics()
        {
            List<UserStatistics> history = GetFullHistory();

            if (history.Count == 0)
            {
                Console.WriteLine("\nІсторія порожня. Пройдіть хоча б один тест!");
                return;
            }

            Console.WriteLine("\n--- Ваші результати ---");
            Console.WriteLine("№   | Тема                 | Дата       | Бал   | %");
            Console.WriteLine("--------------------------------------------------");

            for (int i = 0; i < history.Count; i++)
            {
                UserStatistics stat = history[i];
                double percentage = stat.CalculatePercentage();
                string dateStr = stat.SessionDate.ToShortDateString();

                Console.WriteLine($"{i + 1,-3} | {stat.TopicName,-20} | {stat.SessionDate.ToShortDateString()} | {stat.TotalScore}/{stat.MaxPossibleScore} | {percentage:F1}%");
            }

            Console.WriteLine("\n--- Середня успішність по темах ---");
            
            List<string> processedTopics = new List<string>();
            
            for (int i = 0; i < history.Count; i++)
            {
                bool alreadyProcessed = false;
                for (int j = 0; j < processedTopics.Count; j++)
                {
                    if (processedTopics[j] == history[i].TopicName)
                    {
                        alreadyProcessed = true;
                        break;
                    }
                }

                if (!alreadyProcessed)
                {
                    processedTopics.Add(history[i].TopicName);
                    double avg = CalculateAverageScorePercentage(history[i].TopicName);
                    Console.WriteLine($"- {history[i].TopicName}: {avg:F1}%");
                }
            }
        }
        public List<UserStatistics> GetFullHistory()
        {
            return _statsRepository.GetAll();
        }

        private void SaveChanges()
        {
            _statsStorage.SaveToFile(_statsRepository.GetAll(), _storagePath);
        }
    }
}