using ExamSimulator.Domain.Interfaces;
using ExamSimulator.Domain.Enums;
using System.Text.Json.Serialization;

namespace ExamSimulator.Domain.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "QuestionType")]
    [JsonDerivedType(typeof(SingleChoiceQuestion), "Single")]
    [JsonDerivedType(typeof(MultipleChoiceQuestion), "Multiple")]
    [JsonDerivedType(typeof(OpenQuestion), "Open")]
    public abstract class Question : IEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public string Text { get; set; }
        public DifficultyLevel Difficulty { get; set; }

        public abstract double CalculateScore(List<string> userAnswers);
    }
}