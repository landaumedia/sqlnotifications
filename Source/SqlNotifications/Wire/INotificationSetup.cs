using System.Reflection;
using LandauMedia.Storage;
using LandauMedia.Tracker;

namespace LandauMedia.Wire
{
    public interface INotificationSetup
    {
        INotificationSetup Database(string connectionString);

        INotificationSetup WithNotificationsOfAssembly(Assembly aseembly);

        INotificationSetup UseDefaultChangeTracking();

        INotificationSetup UseDefaultTimestampBased();

        INotificationSetup WithVersionStorage(IVersionStorage storage);

        NotificationTracker Build();
    }
}