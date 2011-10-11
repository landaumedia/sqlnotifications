using System;

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
                default:
                    throw new ArgumentOutOfRangeException("name");
            }
        } 
    }
}