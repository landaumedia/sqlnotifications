using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LandauMedia.Infrastructure
{
    public class DictionaryCounter : IPerformanceCounter
    {
        readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();

        public int ByName(string name)
        {
            return _counters.ContainsKey(name) ? _counters[name] : 0;
        }

        public IDictionary<string, int> All()
        {
            return _counters;
        }

        public void Inc(string name)
        {
            _counters.AddOrUpdate(name, s => 1, (s, i) => i + 1);
        }
    }
}