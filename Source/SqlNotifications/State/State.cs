using System.Collections.Generic;

namespace LandauMedia.State
{
    public class State
    {
        /// <summary>
        /// specifies the Type for the State
        /// </summary>
        public string StateType { get; set; }

        /// <summary>
        /// State of the object in a Dictionary
        /// </summary>
        public IDictionary<string, object> Data { get; set; }
    }
}