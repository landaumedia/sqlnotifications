using System.Collections.Generic;

namespace LandauMedia.State
{
    public class State
    {
        public string StateType { get; set; }

        public IDictionary<string, object> Data { get; set; }
        
    }
}