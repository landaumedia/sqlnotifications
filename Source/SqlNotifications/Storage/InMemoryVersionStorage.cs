using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LandauMedia.Storage
{
    public class InMemoryVersionStorage : IVersionStorage
    {
        // use concurrent Dictionary to be threadsave
        readonly IDictionary<string, ulong> _storage = new ConcurrentDictionary<string, ulong>();

        public void Store(string key, ulong version)
        {
            if(_storage.ContainsKey(key))
                _storage[key] = version;
            else
                _storage.Add(key, version);
        }

        public ulong Load(string key)
        {
            return _storage.ContainsKey(key) ? _storage[key] : 0;
        }

        public bool Exist(string key)
        {
            return _storage.ContainsKey(key);
        }
    }
}