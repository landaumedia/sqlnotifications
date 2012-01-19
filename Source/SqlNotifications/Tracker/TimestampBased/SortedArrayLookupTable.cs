using System;
using System.Collections.Generic;
using System.Linq;
using C5;

namespace LandauMedia.Tracker.TimestampBased
{
    public class SortedArrayLookupTable : ILookupTable
    {
        readonly SortedArray<int> _intArray = new SortedArray<int>();
        readonly SortedArray<long> _longArray = new SortedArray<long>();
        readonly SortedArray<Guid> _guidArray = new SortedArray<Guid>();
        readonly SortedArray<string> _stringArray = new SortedArray<string>();

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

            if (key is string)
            {
                _stringArray.Add((string)key);
                return;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }

        public void AddRange(IEnumerable<object> keys)
        {
            if (keys.First() is int)
            {
                _intArray.AddAll(keys.Cast<int>());
                return;
            }

            if (keys.First() is long)
            {
                _longArray.AddAll(keys.Cast<long>());
                return;
            }

            if (keys.First() is Guid)
            {
                _guidArray.AddAll(keys.Cast<Guid>());
                return;
            }

            if (keys.First() is string)
            {
                _stringArray.AddAll(keys.Cast<string>());
                return;
            }
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

            if (key is string)
            {
                return _stringArray.IndexOf((string)key) >= 0;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }
    }
}