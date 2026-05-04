public class SingleChoiceQuestion : Question
{
    public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();

    public override double CalculateScore(List<string> userAnswers)
    {
        return 0.0;
    }
}