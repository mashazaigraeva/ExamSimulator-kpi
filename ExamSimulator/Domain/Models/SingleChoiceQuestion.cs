using ExamSimulator.Domain.Structs;

namespace ExamSimulator.Domain.Models
{
    public class SingleChoiceQuestion : Question
    {
        public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();

        public override double CalculateScore(List<string> userAnswers)
        {
            if (userAnswers == null || userAnswers.Count == 0)
            {
                return 0.0;
            }

            string answerText = userAnswers[0];

            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].IsCorrect)
                {
                    if (Options[i].Text == answerText)
                    {
                        return 1.0;
                    }
                }
            }

            return 0.0;
        }
    }
}