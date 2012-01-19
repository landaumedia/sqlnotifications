using System.Collections.Generic;

namespace LandauMedia.Tracker.TimestampBased
{
    public interface ILookupTable
    {
        void Add(object key);

        void AddRange(IEnumerable<object> key);

        bool Contains(object key);
    }
}