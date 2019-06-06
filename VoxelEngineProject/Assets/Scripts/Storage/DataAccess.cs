using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Storage.Accesses
{
    class DataAccess : IDataAccess
    {
        public T Load<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default(T);
            }
            using(FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

        public void Save<T>(string path, T obj)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
            }
        }
    }
}
