using System;
using System.Collections.Generic;
using System.Linq;
using C5;

namespace LandauMedia.Tracker.TimestampBased
{
    public class HashbasedLookupWithPayload : ILookupTableWithPayload
    {
        readonly HashDictionary<int, object[]> _intDictionary = new HashDictionary<int, object[]>();
        readonly HashDictionary<long, object[]> _longDictionary = new HashDictionary<long, object[]>();
        readonly HashDictionary<Guid, object[]> _guidDictionary = new HashDictionary<Guid, object[]>();
        readonly HashDictionary<string, object[]> _stringDictionary = new HashDictionary<string, object[]>();
        
        public void Add(object key, object[] data)
        {
            if (key is int)
            {
                _intDictionary.Add((int)key, data);
                return;
            }

            if (key is long)
            {
                _longDictionary.Add((long)key, data);
                return;
            }

            if (key is Guid)
            {
                _guidDictionary.Add((Guid)key, data);
                return;
            }

            if (key is string)
            {
                _stringDictionary.Add((string)key, data);
                return;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }

        public void AddRange(IEnumerable<object> keys, System.Collections.Generic.IDictionary<object, object[]> data)
        {
            if (!keys.Any())
                return;

            if (keys.First() is int)
            {
                _intDictionary.AddAll(data.Select(pair => new C5.KeyValuePair<int, object[]>((int)pair.Key, pair.Value)));
                return;
            }

            if (keys.First() is long)
            {
                _longDictionary.AddAll(data.Select(pair => new C5.KeyValuePair<long, object[]>((long)pair.Key, pair.Value)));
                return;
            }

            if (keys.First() is Guid)
            {
                _guidDictionary.AddAll(data.Select(pair => new C5.KeyValuePair<Guid, object[]>((Guid)pair.Key, pair.Value)));
                return;
            }

            if (keys.First() is string)
            {
                _stringDictionary.AddAll(data.Select(pair => new C5.KeyValuePair<string, object[]>((string)pair.Key, pair.Value)));
                return;
            }
        }

        public bool Contains(object key)
        {
            if (key is int)
            {
                return _intDictionary.Contains((int)key);
            }

            if (key is long)
            {
                return _longDictionary.Contains((long)key);
            }

            if (key is Guid)
            {
                return _guidDictionary.Contains((Guid)key);
            }

            if (key is string)
            {
                return _stringDictionary.Contains((string)key);
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }

        public object[] GetPayload(object key)
        {
            if (key is int)
            {
                return _intDictionary[(int)key];
            }

            if (key is long)
            {
                return _longDictionary[(long)key];
            }

            if (key is Guid)
            {
                return _guidDictionary[(Guid)key];
            }

            if (key is string)
            {
                return _stringDictionary[(string)key];
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }

        public void SetPayload(object key, object[] payload)
        {
            if (!Contains(key))
            {
                Add(key, payload);
                return;
            }


            if (key is int)
            {
                _intDictionary[(int)key] = payload;
                return;
            }

            if (key is long)
            {
                _longDictionary[(long)key] = payload;
                return;
            }

            if (key is Guid)
            {
                _guidDictionary[(Guid)key] = payload;
                return;
            }

            if (key is string)
            {
                _stringDictionary[(string)key] = payload;
                return;
            }

            throw new NotImplementedException("Dieser Typ ist nicht implementiert");
        }
    }
}