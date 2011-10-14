namespace LandauMedia.Tracker
{
    public class TrackerOptions
    {
        public TrackerOptions()
        {
            InitializeToCurrentVersion = true;
            BucketSize = 1000;
        }

        /// <summary>
        /// if set the tracker starts the current version
        /// </summary>
        public bool InitializeToCurrentVersion { get; set; }


        public int BucketSize { get; set; }
    }
}