using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Services
{
    public class StatisticsService
    {
        private readonly IRepository<UserStatistics> _statsRepository;

        public StatisticsService(IRepository<UserStatistics> statsRepository)
        {
            _statsRepository = statsRepository;
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
        }

        public double CalculateAverageScorePercentage(Guid topicId)
        {
            List<UserStatistics> allStats = _statsRepository.GetAll();
            double totalPercentage = 0.0;
            int sessionsCount = 0;

            for (int i = 0; i < allStats.Count; i++)
            {
                UserStatistics current = allStats[i];
                
                if (current.TopicId == topicId && current.MaxPossibleScore > 0)
                {
                    double sessionPercentage = (current.TotalScore / current.MaxPossibleScore) * 100.0;
                    totalPercentage += sessionPercentage;
                    sessionsCount++;
                }
            }

            if (sessionsCount == 0)
            {
                return 0.0;
            }

            return totalPercentage / sessionsCount;
        }

        public List<UserStatistics> GetFullHistory()
        {
            return _statsRepository.GetAll();
        }
    }
}