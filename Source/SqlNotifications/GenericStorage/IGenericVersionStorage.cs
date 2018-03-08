namespace LandauMedia.GenericStorage
{
    /// <summary>
    /// Interface for accessing a storage
    /// </summary>
    public interface IGenericVersionStorage
    {
        /// <summary>
        /// stores the <paramref name="version"/> in the storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="version"></param>
        void Store(string key, string version);

        /// <summary>
        /// Read the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Load(string key);

        /// <summary>
        /// check for a key in the storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exist(string key);


        /// <summary>
        /// reset the complete Storage
        /// </summary>
        void Reset();
    }
}