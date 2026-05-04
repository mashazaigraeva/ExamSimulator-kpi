namespace ExamSimulator.Data.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        void Add(T entity);
        void Remove(Guid id);
        T GetById(Guid id);
        List<T> GetAll();
        void LoadData(List<T> data);
    }
}