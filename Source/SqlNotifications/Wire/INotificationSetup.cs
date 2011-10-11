using System.Reflection;
using LandauMedia.Tracker;

namespace LandauMedia.Wire
{
    public interface INotificationSetup
    {
        INotificationSetup ForDatabase(string connectionString);

        INotificationSetup WithNotificationsOfAssembly(Assembly aseembly);

        INotificationSetup UseDefaultChangeTracking();

        INotificationSetup UseDefaultTimestampBased();

        INotificationSetup WithVersionStorage(IVersionStorage storage);

        NotificationTracker Build();
    }
}