namespace ExamSimulator.Data.Interfaces
{
    public interface IDataStorage<T>
    {
        void SaveToFile(List<T> data, string filePath);
        List<T> LoadFromFile(string filePath);
    }
}