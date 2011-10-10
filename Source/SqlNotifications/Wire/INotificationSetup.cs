using System.Reflection;
using Krowiorsch.Dojo;
using Krowiorsch.Dojo.Wire;

namespace LandauMedia.Wire
{
    public interface INotificationSetup
    {
        INotificationSetup ForDatabase(string connectionString);

        INotificationSetup WithNotificationsOfAssembly(Assembly aseembly);

        INotificationSetup UseChangeTracking();

        INotificationSetup UseTimestampBased();

        NotificationTracker Build();
    }
}