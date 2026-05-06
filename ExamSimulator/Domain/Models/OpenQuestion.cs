namespace ExamSimulator.Domain.Models
{
    public class OpenQuestion : Question
    {
        public string CorrectAnswerText { get; set; }

        public override double CalculateScore(List<string> userAnswers)
        {
            if (userAnswers == null || userAnswers.Count == 0)
            {
                return 0.0;
            }

            string answerText = userAnswers[0];
            
            if (string.Equals(answerText, CorrectAnswerText, StringComparison.OrdinalIgnoreCase))
            {
                return 1.0;
            }

            return 0.0;
        }
    }
}