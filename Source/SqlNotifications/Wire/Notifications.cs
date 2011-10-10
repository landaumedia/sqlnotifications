using Krowiorsch.Dojo.Wire;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public static class Notifications
    {
        public static INotificationSetup WireUp()
        {
            return new StandardNotificationSetup();
        }
    }
}