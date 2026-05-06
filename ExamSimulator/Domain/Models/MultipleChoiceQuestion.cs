using ExamSimulator.Domain.Structs;

namespace ExamSimulator.Domain.Models
{
    public class MultipleChoiceQuestion : Question
    {
        public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();

        public override double CalculateScore(List<string> userAnswers)
        {
            if (userAnswers == null || userAnswers.Count == 0)
            {
                return 0.0;
            } 

            int correctAnswersCount = CountCorrectOptions();
            if (correctAnswersCount == 0)
            {
                return 0.0;
            } 

            double scorePerCorrect = 1.0 / correctAnswersCount;
            double totalScore = 0.0;

            for (int i = 0; i < userAnswers.Count; i++)
            {
                totalScore += EvaluateSingleAnswer(userAnswers[i], scorePerCorrect);
            }

            if (totalScore < 0.0)
            {
                return 0.0;
            }
            return totalScore;
        }

        private int CountCorrectOptions()
        {
            int count = 0;
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].IsCorrect)
                {
                    count++;
                } 
            }
            return count;
        }

        private double EvaluateSingleAnswer(string userAnswer, double scorePerCorrect)
        {
            bool foundCorrect = false;

            for (int j = 0; j < Options.Count; j++)
            {
                if (Options[j].Text == userAnswer && Options[j].IsCorrect)
                {
                    foundCorrect = true;
                    break;
                }
            }

            if (foundCorrect)
            {
                return scorePerCorrect;
            }
            
            return -scorePerCorrect;
        }
    }
}