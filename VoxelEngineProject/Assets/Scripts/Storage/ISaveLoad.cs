
namespace Storage
{
    public interface ISaveLoad : IData
    {
        void Save<T>(string path, T obj);
        T Load<T>(string path);
    }
}
