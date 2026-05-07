using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Services
{
    public class ContentManagerService
    {
        private readonly IRepository<Topic> _topicRepository;

        public ContentManagerService(IRepository<Topic> topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public bool CreateTopic(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            Topic newTopic = new Topic { Name = name };
            _topicRepository.Add(newTopic);
            return true;
        }

        public bool DeleteTopic(Guid topicId)
        {
            Topic existing = _topicRepository.GetById(topicId);
            if (existing != null)
            {
                _topicRepository.Remove(topicId);
                return true;
            }
            return false;
        }

        public bool AddTestToTopic(Guid topicId, string testTitle)
        {
            if (string.IsNullOrEmpty(testTitle))
            {
                return false;
            }

            Topic topic = _topicRepository.GetById(topicId);
            if (topic == null)
            {
                return false;
            }

            Test newTest = new Test { Title = testTitle };
            topic.Tests.Add(newTest);
            return true;
        }

        public bool AddQuestionToTest(Guid topicId, Guid testId, Question question)
        {
            if (question == null)
            {
                return false;
            }

            Topic topic = _topicRepository.GetById(topicId);
            if (topic != null)
            {
                for (int i = 0; i < topic.Tests.Count; i++)
                {
                    if (topic.Tests[i].Id == testId)
                    {
                        topic.Tests[i].Questions.Add(question);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}