using Krowiorsch.Dojo.Wire;
using LandauMedia.Wire;

namespace Krowiorsch.Dojo
{
    public static class Notifications
    {
        public static INotificationSetup WireUp()
        {
            return new StandardNotificationSetup();
        }
    }
}