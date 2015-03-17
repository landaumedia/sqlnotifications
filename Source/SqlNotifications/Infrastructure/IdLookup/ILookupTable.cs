using System.Collections.Generic;

namespace LandauMedia.Infrastructure.IdLookup
{
    internal interface ILookupTable
    {
        void Add(object key);

        void AddRange(IEnumerable<object> key);

        bool Contains(object key);
    }
}