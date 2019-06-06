using System.Xml.Serialization;
using System.IO;

namespace Storage.Accesses
{
    class XmlAccess : IXmlAccess
    {
        public T Load<T>(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(T));
                return (T)xml.Deserialize(stream);
            }
        }

        public void Save<T>(string path, T obj)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(stream, obj);
            }
        }
    }
}
