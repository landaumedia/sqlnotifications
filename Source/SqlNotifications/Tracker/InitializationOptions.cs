namespace LandauMedia.Tracker
{
    /// <summary>
    /// defines how the tracker ist initialized
    /// </summary>
    public enum InitializationOptions
    {
        /// <summary>
        /// start with version 0
        /// </summary>
        InitializeWithZero,


        /// <summary>
        /// start with current version
        /// </summary>
        InitializeToCurrent,


        /// <summary>
        /// start with currentversion, if no version in Database
        /// </summary>
        InitializeToCurrentIfNotSet,

        /// <summary>
        /// start with 0 if no rowversion set
        /// </summary>
        InitializeToZeroIfNotSet
    }
}