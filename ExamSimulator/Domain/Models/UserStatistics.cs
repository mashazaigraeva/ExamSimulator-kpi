using ExamSimulator.Domain.Interfaces;

namespace ExamSimulator.Domain.Models
{
    public class UserStatistics : IEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public DateTime SessionDate { get; set; } = DateTime.Now;
        public double TotalScore { get; set; }
        public int MaxPossibleScore { get; set; }
    }
}