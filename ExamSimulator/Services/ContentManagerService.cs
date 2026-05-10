using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Services
{
    public class ContentManagerService
    {
        private readonly IRepository<Topic> _topicRepository;
        private readonly IDataStorage<Topic> _topicStorage;
        private readonly string _storagePath;

        public ContentManagerService(IRepository<Topic> topicRepository, IDataStorage<Topic> topicStorage, string storagePath)
        {
            _topicRepository = topicRepository;
            _topicStorage = topicStorage;
            _storagePath = storagePath;
        }

        public List<Topic> GetAllTopics()
        {
            return _topicRepository.GetAll();
        }

        public bool CreateTopic(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            Topic newTopic = new Topic { Name = name };
            _topicRepository.Add(newTopic);
            SaveChanges();
            return true;
        }

        public bool DeleteTopic(Guid topicId)
        {
            Topic existing = _topicRepository.GetById(topicId);
            if (existing != null)
            {
                _topicRepository.Remove(topicId);
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteTest(Guid topicId, Guid testId)
        {
            Topic topic = _topicRepository.GetById(topicId);
            if (topic == null) return false;

            for (int i = 0; i < topic.Tests.Count; i++)
            {
                if (topic.Tests[i].Id == testId)
                {
                    topic.Tests.RemoveAt(i);
                    SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeleteQuestion(Guid topicId, Guid testId, Guid questionId)
        {
            Topic topic = _topicRepository.GetById(topicId);
            if (topic == null)
            {
                return false;
            } 

            for (int i = 0; i < topic.Tests.Count; i++)
            {
                if (topic.Tests[i].Id != testId)
                {
                    continue;
                } 

                Test test = topic.Tests[i];
                for (int j = 0; j < test.Questions.Count; j++)
                {
                    if (test.Questions[j].Id == questionId)
                    {
                        test.Questions.RemoveAt(j);
                        SaveChanges();
                        return true;
                    }
                }
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
            SaveChanges();
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
                        SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveChanges()
        {
            _topicStorage.SaveToFile(_topicRepository.GetAll(), _storagePath);
        }
    }
}