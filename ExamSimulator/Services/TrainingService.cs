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

        public List<Question> GenerateSession(Topic topic, Func<Question, bool> filterDelegate, int maxQuestions)
        {
            if (topic == null || topic.Tests == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            List<Question> filteredQuestions = new List<Question>();

            for (int i = 0; i < topic.Tests.Count; i++)
            {
                CollectQuestionsFromTest(topic.Tests[i], filterDelegate, filteredQuestions);
            }

            ShuffleQuestions(filteredQuestions);

            List<Question> finalSession = new List<Question>();
            int limit;
            if (filteredQuestions.Count < maxQuestions)
            {
               limit = filteredQuestions.Count;
            }
            else
            {
                limit = maxQuestions;
            }

            for (int k = 0; k < limit; k++)
            {
                finalSession.Add(filteredQuestions[k]);
            }

            return finalSession;
        }

        private void CollectQuestionsFromTest(Test test, Func<Question, bool> filter, List<Question> resultList)
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
        }

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