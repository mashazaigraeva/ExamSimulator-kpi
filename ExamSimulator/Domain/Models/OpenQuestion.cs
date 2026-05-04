public class OpenQuestion : Question
{
    public string CorrectAnswerText { get; set; }

    public override double CalculateScore(List<string> userAnswers)
    {
        return 0.0;
    }
}