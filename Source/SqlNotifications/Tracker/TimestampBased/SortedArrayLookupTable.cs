using System;
using C5;

namespace LandauMedia.Tracker.TimestampBased
{
    public class SortedArrayLookupTable : ILookupTable
    {
        readonly SortedArray<int> _intArray = new SortedArray<int>();
        readonly SortedArray<long> _longArray = new SortedArray<long>();
        readonly SortedArray<Guid> _guidArray = new SortedArray<Guid>();

        public void Add(object key)
        {
            if (key is int)
            {
                _intArray.Add((int)key);
                return;
            }

            if (key is long)
            {
                _longArray.Add((long)key);
                return;
            }

            if (key is Guid)
            {
                _guidArray.Add((Guid)key);
                return;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }

        public bool Contains(object key)
        {
            if (key is int)
            {
                return _intArray.IndexOf((int)key) >= 0;
                
            }

            if (key is long)
            {
                return _longArray.IndexOf((long)key) >= 0;
            }

            if (key is Guid)
            {
                return _guidArray.IndexOf((Guid)key) >= 0;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }
    }
}