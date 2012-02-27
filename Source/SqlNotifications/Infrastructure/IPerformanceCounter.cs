using System.Collections.Generic;

namespace LandauMedia.Infrastructure
{
    public interface IPerformanceCounter
    {
        // query
        int ByName(string name);
        IDictionary<string, int> All();

        // command
        void Inc(string name);
    }
}