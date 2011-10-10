using Krowiorsch.Dojo.Wire;

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