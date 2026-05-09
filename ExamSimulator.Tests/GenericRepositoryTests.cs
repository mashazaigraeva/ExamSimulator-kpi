using ExamSimulator.Data.Repositories;
using ExamSimulator.Domain.Models;

namespace ExamSimulator.Tests
{
    public class GenericRepositoryTests
    {
        private GenericRepository<Topic> _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new GenericRepository<Topic>();
        }

        [Test]
        public void Add_ValidEntity_IncreasesCount()
        {
            Topic topic = new Topic { Name = "Алгоритми" };
            
            _repository.Add(topic);
            
            Assert.That(_repository.GetAll().Count, Is.EqualTo(1));
            Assert.That(_repository.GetAll()[0].Name, Is.EqualTo("Алгоритми"));
        }

        [Test]
        public void Remove_ExistingEntity_RemovesFromList()
        {
            Topic topic = new Topic { Name = "Математика" };
            _repository.Add(topic);
            
            _repository.Remove(topic.Id);
            
            Assert.That(_repository.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
        public void GetById_ExistingId_ReturnsCorrectEntity()
        {
            Topic topic1 = new Topic { Name = "Тема 1" };
            Topic topic2 = new Topic { Name = "Тема 2" };
            _repository.Add(topic1);
            _repository.Add(topic2);
            
            Topic result = _repository.GetById(topic2.Id);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Тема 2"));
        }

        [Test]
        public void LoadData_OverwritesExistingData()
        {
            _repository.Add(new Topic { Name = "Стара тема" });
            
            List<Topic> newData = new List<Topic>();
            newData.Add(new Topic { Name = "Нова тема 1" });
            newData.Add(new Topic { Name = "Нова тема 2" });
            
            _repository.LoadData(newData);
            
            Assert.That(_repository.GetAll().Count, Is.EqualTo(2));
            Assert.That(_repository.GetAll()[0].Name, Is.EqualTo("Нова тема 1"));
        }

        [Test]
        public void Add_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(delegate () 
            { 
                _repository.Add(null); 
            });
        }

        [Test]
        public void GetById_NonExistingId_ReturnsNull()
        {
            Guid randomId = Guid.NewGuid();

            Topic result = _repository.GetById(randomId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Remove_NonExistingId_DoesNotChangeCountAndDoesNotThrow()
        {
            Topic topic = new Topic { Name = "Єдина тема" };
            _repository.Add(topic);
            Guid randomId = Guid.NewGuid(); 

            _repository.Remove(randomId);

            Assert.That(_repository.GetAll().Count, Is.EqualTo(1));
            Assert.That(_repository.GetAll()[0].Name, Is.EqualTo("Єдина тема"));
        }

        [Test]
        public void LoadData_NullList_ClearsExistingDataAndDoesNotThrow()
        {
            _repository.Add(new Topic { Name = "Стара тема" });

            _repository.LoadData(null);

            Assert.That(_repository.GetAll().Count, Is.EqualTo(0));
        }
    }
}