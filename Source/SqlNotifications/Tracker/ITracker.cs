using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public interface ITracker
    {
        INotificationSetup NotificationSetup { get; }
        INotification Notification { get; }

        void TrackingChanges();
        void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions trackerOptions);
    }
}