using ExamSimulator.Domain.Models;

namespace ExamSimulator.Services
{
    public class TrainingService
    {
        private readonly Random _random;

        public TrainingService()
        {
            _random = new Random();
        }

        public List<Question> GenerateSession(Test test, Func<Question, bool> filter, int limit)
        {
            List<Question> sessionQuestions = new List<Question>();
            
            for (int i = 0; i < test.Questions.Count; i++)
            {
                if (sessionQuestions.Count >= limit)
                {
                    break;
                }

                if (filter(test.Questions[i]))
                {
                    sessionQuestions.Add(test.Questions[i]);
                }
            }

            return sessionQuestions;
        }

        /*private void CollectQuestionsFromTest(Test test, Func<Question, bool> filter, List<Question> resultList)
        {
            if (test.Questions == null)
            {
                return;
            } 

            for (int j = 0; j < test.Questions.Count; j++)
            {
                if (filter(test.Questions[j]))
                {
                    resultList.Add(test.Questions[j]);
                }
            }
        }*/

        public void ShuffleOptions(Question question)
        {
            if (question is SingleChoiceQuestion)
            {
                SingleChoiceQuestion single = (SingleChoiceQuestion)question;
                ShuffleQuestions(single.Options);
            }
            else if (question is MultipleChoiceQuestion)
            {
                MultipleChoiceQuestion multiple = (MultipleChoiceQuestion)question;
                ShuffleQuestions(multiple.Options);
            }
        }

        public double EvaluateAnswer(Question question, List<string> userAnswers)
        {
            if (question == null) 
            {
                throw new ArgumentNullException(nameof(question));
            }
            
            return question.CalculateScore(userAnswers);
        }

        private void ShuffleQuestions<T>(List<T> list)
        {
            if (list == null)
            {
                return;
            }

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}