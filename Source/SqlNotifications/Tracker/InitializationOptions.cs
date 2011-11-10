namespace LandauMedia.Tracker
{
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
        InitializeToCurrentIfNotSet
    }
}