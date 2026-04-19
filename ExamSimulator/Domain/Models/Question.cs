public abstract class Question : IEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string Text { get; set; }
    public DifficultyLevel Difficulty { get; set; }

    public abstract double CalculateScore(List<string> userAnswers);
}