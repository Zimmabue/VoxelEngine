
using Storage.Accesses;

namespace Storage
{
    public class StorageManager : AbstractStorage
    {

        #region Singleton
        private static StorageManager _instance = null;
        public static StorageManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StorageManager();

                return _instance;
            }
        }
        #endregion

        private StorageManager() : base()
        {
            accesses.Add(new XmlAccess());
            accesses.Add(new DataAccess());
        }

        public static IXmlAccess GetXmlAccess()
        {
            return Instance.GetAccess<IXmlAccess>();
        }

        public static IDataAccess GetDataAccess()
        {
            return Instance.GetAccess<IDataAccess>();
        }

    }
}
