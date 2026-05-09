using ExamSimulator.Services;
using ExamSimulator.Domain.Models;
using ExamSimulator.Data.Repositories;
using ExamSimulator.Data.Interfaces;
using ExamSimulator.Domain.Enums;

namespace ExamSimulator.Tests
{
    public class ServicesTests
    {
        private GenericRepository<Topic> _topicRepo; 
        private ContentManagerService _contentService;
        private GenericRepository<UserStatistics> _statsRepo;
        private StatisticsService _statsService;

        [SetUp] 
        public void Setup()
        {
            _topicRepo = new GenericRepository<Topic>(); 
            FakeDataStorage<Topic> fakeTopicStorage = new FakeDataStorage<Topic>();
            _contentService = new ContentManagerService(_topicRepo, fakeTopicStorage, "dummy_topics.json");

            _statsRepo = new GenericRepository<UserStatistics>();
            FakeDataStorage<UserStatistics> fakeStatsStorage = new FakeDataStorage<UserStatistics>();
            _statsService = new StatisticsService(_statsRepo, fakeStatsStorage, "dummy_stats.json");
        }

        [Test]
        public void CreateTopic_ValidName_ReturnsTrueAndAddsToRepo()
        {
            string topicName = "Тестова тема";

            bool result = _contentService.CreateTopic(topicName);

            Assert.That(result, Is.True);
            Assert.That(_topicRepo.GetAll().Count, Is.EqualTo(1));
            Assert.That(_topicRepo.GetAll()[0].Name, Is.EqualTo(topicName));
        }

        [Test]
        public void CreateTopic_EmptyName_ReturnsFalse()
        {            
            string topicName = "";

            bool result = _contentService.CreateTopic(topicName);

            Assert.That(result, Is.False);
            Assert.That(_topicRepo.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
        public void AddTestToTopic_ExistingTopic_ReturnsTrue()
        {
            Topic topic = new Topic { Name = "Математика" };
            _topicRepo.Add(topic); 

            bool result = _contentService.AddTestToTopic(topic.Id, "Новий тест");

            Assert.That(result, Is.True);
            Assert.That(_topicRepo.GetById(topic.Id).Tests.Count, Is.EqualTo(1));
            Assert.That(_topicRepo.GetById(topic.Id).Tests[0].Title, Is.EqualTo("Новий тест"));
        }

        [Test]
        public void AddQuestionToTest_ExistingTest_ReturnsTrueAndAddsQuestion()
        {
            Topic topic = new Topic { Name = "Програмування" };
            Test test = new Test { Title = "Основи" };
            topic.Tests.Add(test);
            _topicRepo.Add(topic);

            SingleChoiceQuestion question = new SingleChoiceQuestion { Text = "Що таке int?" };

            bool result = _contentService.AddQuestionToTest(topic.Id, test.Id, question);

            Assert.That(result, Is.True);
            Assert.That(_topicRepo.GetById(topic.Id).Tests[0].Questions.Count, Is.EqualTo(1));
            Assert.That(_topicRepo.GetById(topic.Id).Tests[0].Questions[0].Text, Is.EqualTo("Що таке int?"));
        }

        [Test]
        public void GenerateSession_LimitIsTwo_ReturnsExactlyTwoQuestions()
        {
            TrainingService trainingService = new TrainingService();
            Test test = new Test { Title = "Типи даних" };
            
            test.Questions.Add(new SingleChoiceQuestion { Text = "Питання 1" });
            test.Questions.Add(new SingleChoiceQuestion { Text = "Питання 2" });
            test.Questions.Add(new SingleChoiceQuestion { Text = "Питання 3" });

            Func<Question, bool> allQuestions = delegate (Question q) { return true; };

            List<Question> result = trainingService.GenerateSession(test, allQuestions, 2);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GenerateSession_WithFilter_ReturnsOnlyMatchingQuestions()
        {
            TrainingService trainingService = new TrainingService();
            Topic topic = new Topic { Name = "Алгоритми" };
            Test test = new Test { Title = "Сортування" };
            
            test.Questions.Add(new SingleChoiceQuestion { Text = "Легке", Difficulty = DifficultyLevel.Easy });
            test.Questions.Add(new SingleChoiceQuestion { Text = "Важке 1", Difficulty = DifficultyLevel.Hard });
            test.Questions.Add(new SingleChoiceQuestion { Text = "Важке 2", Difficulty = DifficultyLevel.Hard });
            topic.Tests.Add(test);

            Func<Question, bool> onlyHard = delegate (Question q) 
            { 
                if (q.Difficulty == DifficultyLevel.Hard)
                {
                    return true;
                }
                return false; 
            };

            List<Question> result = trainingService.GenerateSession(test, onlyHard, 10);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Difficulty, Is.EqualTo(DifficultyLevel.Hard));
            Assert.That(result[1].Difficulty, Is.EqualTo(DifficultyLevel.Hard));
        }

        [Test]
        public void GenerateSession_EmptyTopic_ReturnsEmptyList()
        {
            TrainingService trainingService = new TrainingService();
            Test emptyTest = new Test { Title = "Порожній тест" };
            Func<Question, bool> allQuestions = delegate (Question q) { return true; };

            List<Question> result = trainingService.GenerateSession(emptyTest, allQuestions, 5);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void RecordSessionResult_ValidData_AddsToRepo()
        {
            Topic topic = new Topic { Name = "Структури даних" };
            double score = 4.0;
            int maxScore = 5;

            _statsService.RecordSessionResult(topic, score, maxScore);

            List<UserStatistics> allStats = _statsRepo.GetAll();
            Assert.That(allStats.Count, Is.EqualTo(1));
            Assert.That(allStats[0].TopicName, Is.EqualTo("Структури даних"));
            Assert.That(allStats[0].TotalScore, Is.EqualTo(4.0));
            Assert.That(allStats[0].MaxPossibleScore, Is.EqualTo(5));
        }

        [Test]
        public void RecordSessionResult_ZeroScore_SavesCorrectly()
        {
            Topic topic = new Topic { Name = "Складний тест" };
            double score = 0.0;
            int maxScore = 10;

            _statsService.RecordSessionResult(topic, score, maxScore);

            List<UserStatistics> allStats = _statsRepo.GetAll();
            Assert.That(allStats.Count, Is.EqualTo(1));
            Assert.That(allStats[0].TotalScore, Is.EqualTo(0.0));
        }

        [Test]
        public void CalculateAverageScorePercentage_MultipleSessions_ReturnsCorrectAverage()
        {
            Topic topic = new Topic { Name = "ООП" };
            
            _statsService.RecordSessionResult(topic, 5.0, 10);
            
            _statsService.RecordSessionResult(topic, 10.0, 10);
            
            double average = _statsService.CalculateAverageScorePercentage(topic.Name);
            
            Assert.That(average, Is.EqualTo(75.0));
        }
    }

    public class FakeDataStorage<T> : IDataStorage<T>
    {
        public void SaveToFile(List<T> data, string filePath)
        {
        }

        public List<T> LoadFromFile(string filePath)
        {
            return new List<T>();
        }
    }
}