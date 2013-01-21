using System;
using System.Collections.Generic;

namespace LandauMedia.State
{
    public class InMemoryHoldStates : IHoldStates
    {
        readonly IDictionary<string, State> _data = new Dictionary<string, State>();
        readonly Func<string, string, string> _keyCreation;

        public InMemoryHoldStates(Func<string, string, string> keyCreation)
        {
            _keyCreation = keyCreation;
        }

        public InMemoryHoldStates()
            : this(CreateKey)
        {
        }

        public void Put(string id, State state)
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