using System.Reflection;
using Krowiorsch.Dojo;

namespace LandauMedia.Wire
{
    public interface INotificationSetup
    {
        INotificationSetup ForDatabase(string connectionString);

        INotificationSetup WithNotificationsOfAssembly(Assembly aseembly);

        INotificationSetup UseDefaultChangeTracking();

        INotificationSetup UseDefaultTimestampBased();

        NotificationTracker Build();
    }
}