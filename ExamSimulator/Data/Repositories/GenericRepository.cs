using ExamSimulator.Data.Interfaces;

namespace ExamSimulator.Data.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> items = new List<T>();

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            items.Add(entity);
        }

        public void Remove(Guid id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    items.RemoveAt(i);
                    return;
                }
            }
        }

        public T GetById(Guid id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    return items[i];
                }
            }
            return null;
        }

        public List<T> GetAll()
        {
            List<T> copy = new List<T>();
            for (int i = 0; i < items.Count; i++)
            {
                copy.Add(items[i]);
            }
            return copy;
        }

        public void LoadData(List<T> data)
        {
            items.Clear();
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    items.Add(data[i]);
                }
            }
        }
    }
}