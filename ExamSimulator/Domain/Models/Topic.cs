using ExamSimulator.Domain.Interfaces;

namespace ExamSimulator.Domain.Models
{
    public class Topic : IEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<Test> Tests { get; set; } = new List<Test>();
    }
}