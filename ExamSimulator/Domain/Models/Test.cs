using ExamSimulator.Domain.Interfaces;

namespace ExamSimulator.Domain.Models
{
    public class Test : IEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}