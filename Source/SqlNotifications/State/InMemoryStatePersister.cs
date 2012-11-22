using System;
using System.Collections.Generic;

namespace LandauMedia.State
{
    public class InMemoryStatePersister : IStatePersister
    {
        readonly IDictionary<string, State> _data = new Dictionary<string, State>();
        readonly Func<string, string, string> _keyCreation;

        public InMemoryStatePersister(Func<string, string, string> keyCreation)
        {
            _keyCreation = keyCreation;
        }

        public InMemoryStatePersister()
            :this(CreateKey)
        {
        }

        public void Save(string id, State state)
        {
            var key = _keyCreation(id, state.StateType);

            if (_data.ContainsKey(key))
                _data[key] = state;
            else
                _data.Add(key, state);
        }

        public State GetForTypeOrNull(string id, string type)
        {
            var key = _keyCreation(id, type);

            return _data.ContainsKey(key) ? _data[key] : null;
        }

        static string CreateKey(string id, string type)
        {
            return type + "/" + id;
        }
    }
}