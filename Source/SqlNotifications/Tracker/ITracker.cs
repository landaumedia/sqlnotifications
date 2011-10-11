using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public interface ITracker
    {
        INotificationSetup NotificationSetup { get; }
        void TrackingChanges();
        void Prepare(string connectionString, INotificationSetup notificationSetup, IVersionStorage storage, TrackerOptions trackerOptions);
    }
}