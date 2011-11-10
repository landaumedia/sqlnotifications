namespace LandauMedia.Tracker
{
    public class TrackerOptions
    {
        public TrackerOptions()
        {
            InitializationOptions = InitializationOptions.InitializeToCurrent;
            BucketSize = 1000;
        }

        public InitializationOptions InitializationOptions { get; set; }

        public int BucketSize { get; set; }

        /// <summary>
        /// gibt die TrackerOptions mit DefaultSetting zurück
        /// </summary>
        public static TrackerOptions Default
        {
            get
            {
                return new TrackerOptions();
            }
        }
    }
}