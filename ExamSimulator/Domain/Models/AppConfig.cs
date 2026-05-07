namespace ExamSimulator.Domain.Models
{
    public class AppConfig
    {
        public int QuestionsPerSession { get; set; } = 5;
        public bool ShowCorrectAnswersAfterQuestion { get; set; } = true;
        public double PassingScorePercentage { get; set; } = 60.0;
    }
}