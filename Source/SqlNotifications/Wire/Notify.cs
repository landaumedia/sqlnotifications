using LandauMedia.Tracker;

namespace LandauMedia.Wire
{
    public static class Notify
    {
        public static ITrackerSetup For()
        {
            return new StandardTrackerSetup();
        }
    }
}