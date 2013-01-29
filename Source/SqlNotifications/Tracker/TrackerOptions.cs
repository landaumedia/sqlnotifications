using System;

namespace LandauMedia.Tracker
{
    public class TrackerOptions
    {
        public TrackerOptions()
        {
            InitializationOptions = InitializationOptions.InitializeToCurrent;
            BucketSize = 1000;
            FetchInterval = TimeSpan.FromSeconds(1);
            Throttling = TimeSpan.FromMilliseconds(250);
        }

        public InitializationOptions InitializationOptions { get; set; }

        /// <summary>
        /// gibt die grösse an, in der die Daten abgerufen werden
        /// </summary>
        public int BucketSize { get; set; }

        /// <summary>
        /// Gibt das Interval an, in dem nach neuen Daten gefragt wird.
        /// </summary>
        public TimeSpan FetchInterval { get; set; }

        public TimeSpan Throttling { get; set; }

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