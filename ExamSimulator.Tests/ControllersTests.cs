using ExamSimulator.UI;
using ExamSimulator.Services;
using ExamSimulator.Data.Repositories;
using ExamSimulator.Domain.Models;
using ExamSimulator.Domain.Enums;
using ExamSimulator.Data.Storage;
using ExamSimulator.Domain.Structs;

namespace ExamSimulator.Tests
{
    [TestFixture]
    public class ControllersTests
    {
        private TextReader _originalInput;
        private TextWriter _originalOutput;
        private StringWriter _consoleOutput;

        [SetUp]
        public void Setup()
        {
            _originalInput = Console.In;
            _originalOutput = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetIn(_originalInput);
            Console.SetOut(_originalOutput);
            _consoleOutput.Dispose();
        }

        [Test]
        public void Editor_ManageContent_InputZero_ExitsMenuImmediately()
        {
            StringReader simulatedKeyboard = new StringReader("0\n");
            Console.SetIn(simulatedKeyboard);

            GenericRepository<Topic> repo = new GenericRepository<Topic>();
            FakeDataStorage<Topic> storage = new FakeDataStorage<Topic>();
            ContentManagerService service = new ContentManagerService(repo, storage, "dummy.json");
            EditorController controller = new EditorController(service);

            controller.ManageContent();

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("Редактор контенту"));
        }

        [Test]
        public void Editor_ManageContent_AddTopic_SuccessfullyCreatesTopicAndExits()
        {
            StringReader simulatedKeyboard = new StringReader("1\nМоя супер тема\n0\n");
            Console.SetIn(simulatedKeyboard);

            GenericRepository<Topic> repo = new GenericRepository<Topic>();
            FakeDataStorage<Topic> storage = new FakeDataStorage<Topic>();
            ContentManagerService service = new ContentManagerService(repo, storage, "dummy.json");
            EditorController controller = new EditorController(service);

            controller.ManageContent();

            Assert.That(repo.GetAll().Count, Is.EqualTo(1));
            Assert.That(repo.GetAll()[0].Name, Is.EqualTo("Моя супер тема"));
        }
        
        [Test]
        public void Editor_ManageContent_DeleteTopic_SuccessfullyRemovesTopic()
        {
            GenericRepository<Topic> repo = new GenericRepository<Topic>();
            FakeDataStorage<Topic> storage = new FakeDataStorage<Topic>();
            ContentManagerService service = new ContentManagerService(repo, storage, "dummy.json");
            
            Topic topic = new Topic { Name = "Тема для видалення" };
            repo.Add(topic);

            EditorController controller = new EditorController(service);

            StringReader simulatedKeyboard = new StringReader("2\n1\n0\n");
            Console.SetIn(simulatedKeyboard);

            controller.ManageContent();

            Assert.That(repo.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
        public void Settings_ManageSettings_ExitImmediately_DoesNotCrash()
        {
            StringReader simulatedKeyboard = new StringReader("0\n");
            Console.SetIn(simulatedKeyboard);

            ConfigManager configManager = new ConfigManager("dummy_settings.json");
            SettingsController controller = new SettingsController(configManager); 

            controller.ManageSettings();

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("Налаштування")); 
        }

        [Test]
        public void Settings_ManageSettings_TurnOnShowCorrectAnswers_UpdatesConfig()
        {
            StringReader simulatedKeyboard = new StringReader("2\ny\n0\n");
            Console.SetIn(simulatedKeyboard);

            ConfigManager configManager = new ConfigManager("dummy_settings.json");
            SettingsController controller = new SettingsController(configManager); 
            
            AppConfig config = controller.GetCurrentConfig();
            config.ShowCorrectAnswersAfterQuestion = false;

            controller.ManageSettings();

            AppConfig updatedConfig = controller.GetCurrentConfig();
            Assert.That(updatedConfig.ShowCorrectAnswersAfterQuestion, Is.True);
        }

        [Test]
        public void Training_StartTraining_PerfectScore_PrintsSuccessMessage()
        {
            GenericRepository<Topic> topicRepo = new GenericRepository<Topic>();
            Topic topic = new Topic { Name = "Основи ООП" };
            Test test = new Test { Title = "Перший тест" };
            
            SingleChoiceQuestion question = new SingleChoiceQuestion { Text = "Що таке поліморфізм?", Difficulty = DifficultyLevel.Medium };
            question.Options.Add(new AnswerOption { Text = "Це добре", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "Це погано", IsCorrect = false });
            
            test.Questions.Add(question);
            topic.Tests.Add(test);
            topicRepo.Add(topic);

            TrainingService trainingService = new TrainingService();
            GenericRepository<UserStatistics> statsRepo = new GenericRepository<UserStatistics>();
            FakeDataStorage<UserStatistics> fakeStatsStorage = new FakeDataStorage<UserStatistics>();
            StatisticsService statsService = new StatisticsService(statsRepo, fakeStatsStorage, "dummy.json");

            ConfigManager configManager = new ConfigManager("dummy_settings.json");
            SettingsController settingsController = new SettingsController(configManager);
            
            AppConfig config = settingsController.GetCurrentConfig();
            config.QuestionsPerSession = 5;
            config.PassingScorePercentage = 60;

            TrainingController controller = new TrainingController(trainingService, topicRepo, settingsController, statsService);

            StringReader simulatedKeyboard = new StringReader("1\n1\nЦе добре\n0\n");
            Console.SetIn(simulatedKeyboard);

            controller.StartTraining();

            string output = _consoleOutput.ToString();
            
            Assert.That(output, Does.Contain("Вітаємо! Тест успішно складено"));
            Assert.That(output, Does.Contain("Нараховано балів: 1"));

            List<UserStatistics> allStats = statsRepo.GetAll();
            Assert.That(allStats.Count, Is.EqualTo(1));
            Assert.That(allStats[0].TotalScore, Is.EqualTo(1.0));
        }

        [Test]
        public void Training_StartTraining_WrongAnswer_PrintsFailureMessageAndShowsHint()
        {
            GenericRepository<Topic> topicRepo = new GenericRepository<Topic>();
            Topic topic = new Topic { Name = "Основи ООП" };
            Test test = new Test { Title = "Перший тест" };
            
            SingleChoiceQuestion question = new SingleChoiceQuestion { Text = "Що таке поліморфізм?", Difficulty = DifficultyLevel.Medium };
            question.Options.Add(new AnswerOption { Text = "Правильно", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "Неправильно", IsCorrect = false });
            
            test.Questions.Add(question);
            topic.Tests.Add(test);
            topicRepo.Add(topic);

            TrainingService trainingService = new TrainingService();
            GenericRepository<UserStatistics> statsRepo = new GenericRepository<UserStatistics>();
            FakeDataStorage<UserStatistics> fakeStatsStorage = new FakeDataStorage<UserStatistics>();
            StatisticsService statsService = new StatisticsService(statsRepo, fakeStatsStorage, "dummy.json");

            ConfigManager configManager = new ConfigManager("dummy_settings.json");
            SettingsController settingsController = new SettingsController(configManager);
            
            AppConfig config = settingsController.GetCurrentConfig();
            config.QuestionsPerSession = 5;
            config.PassingScorePercentage = 60;
            config.ShowCorrectAnswersAfterQuestion = true;

            TrainingController controller = new TrainingController(trainingService, topicRepo, settingsController, statsService);

            StringReader simulatedKeyboard = new StringReader("1\n1\nНеправильно\n0\n");
            Console.SetIn(simulatedKeyboard);

            controller.StartTraining();

            string output = _consoleOutput.ToString();
            
            Assert.That(output, Does.Contain("Ви зробили помилку"));
            Assert.That(output, Does.Contain("Правильна відповідь:"));
            Assert.That(output, Does.Contain("- Правильно"));
            
            List<UserStatistics> allStats = statsRepo.GetAll();
            Assert.That(allStats[0].TotalScore, Is.EqualTo(0.0));
        }
    }
}