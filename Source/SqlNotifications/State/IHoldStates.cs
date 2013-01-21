namespace LandauMedia.State
{
    public interface IHoldStates
    {
        /// <summary>
        /// saves the state
        /// </summary>
        void Put(string id, State state);

        /// <summary>
        /// return the state or null
        /// </summary>
        State GetForTypeOrNull(string id, string type);
    }
}