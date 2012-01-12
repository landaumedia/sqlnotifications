namespace LandauMedia.Storage
{
    /// <summary>
    /// Interface for accessing a storage
    /// </summary>
    public interface IVersionStorage
    {
        /// <summary>
        /// stores the <paramref name="version"/> in the storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="version"></param>
        void Store(string key, ulong version);

        /// <summary>
        /// Read the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ulong Load(string key);

        /// <summary>
        /// check for a key in the storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exist(string key);
    }
}