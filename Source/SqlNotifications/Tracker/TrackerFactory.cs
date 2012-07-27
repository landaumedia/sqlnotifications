using System;
using LandauMedia.Tracker.ChangeOnlyTimestampBased;
using LandauMedia.Tracker.ChangeTrackingBased;
using LandauMedia.Tracker.TimestampBased;

namespace LandauMedia.Tracker
{
    public class TrackerFactory
    {
        public static ITracker BuildByName(string name)
        {
            switch(name.ToLower())
            {
                case "changetracking":
                    return new ChangeTrackingBasedTracker();
                case "timestamp":
                    return new TimestampBasedTracker();
                case "changeonlytimestamp":
                    return new ChangeOnlyTimestampBasedTracker();
                default:
                    throw new ArgumentOutOfRangeException("name");
            }
        } 
    }
}