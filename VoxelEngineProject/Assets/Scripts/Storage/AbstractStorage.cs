using System.Collections.Generic;

namespace Storage
{
    public abstract class AbstractStorage
    {
        protected List<IData> accesses;

        protected AbstractStorage()
        {
            accesses = new List<IData>();
        }

        protected T GetAccess<T>() where T : IData
        {
            foreach (var access in accesses)
            {
                if (access is T)
                    return (T)access;
            }

            return default(T);
        }

    }
}
