using ExamSimulator.Data.Interfaces;
using ExamSimulator.Data.Repositories;
using ExamSimulator.Data.Storage;
using ExamSimulator.Domain.Models;
using ExamSimulator.Services;
using ExamSimulator.UI;

namespace ExamSimulator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string topicsFilePath = "data/topics.json";
            string configFilePath = "data/appsettings.json";
            string statsFilePath = "data/statistics.json";

            IDataStorage<Topic> topicStorage = new JsonDataStorage<Topic>();
            IDataStorage<UserStatistics> statsStorage = new JsonDataStorage<UserStatistics>();
            ConfigManager configManager = new ConfigManager(configFilePath);

            IRepository<Topic> topicRepository = new GenericRepository<Topic>();
            IRepository<UserStatistics> statsRepository = new GenericRepository<UserStatistics>();

            topicRepository.LoadData(topicStorage.LoadFromFile(topicsFilePath));
            statsRepository.LoadData(statsStorage.LoadFromFile(statsFilePath));

            TrainingService trainingService = new TrainingService();
            StatisticsService statsService = new StatisticsService(statsRepository, statsStorage, statsFilePath);
            ContentManagerService contentManager = new ContentManagerService(topicRepository, topicStorage, topicsFilePath);

            SettingsController settingsController = new SettingsController(configManager);           
            EditorController editorController = new EditorController(contentManager);
            TrainingController trainingController = new TrainingController(trainingService, topicRepository, settingsController, statsService);
            ConsoleUI ui = new ConsoleUI(trainingController, editorController, settingsController, statsService);

            ui.Run();
        }
    }
}