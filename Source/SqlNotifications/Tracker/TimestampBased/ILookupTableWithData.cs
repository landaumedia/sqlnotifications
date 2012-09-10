using System.Collections.Generic;

namespace LandauMedia.Tracker.TimestampBased
{
    public interface ILookupTableWithPayload
    {
        void Add(object key, object[] data);

        void AddRange(IEnumerable<object> key, IDictionary<object, object[]> data);

        bool Contains(object key);

        object[] GetPayload(object key);
        void SetPayload(object key, object[] payload);
    }
}