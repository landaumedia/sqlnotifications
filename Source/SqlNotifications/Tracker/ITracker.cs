using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public interface ITracker
    {
        INotification Notification { get; }
        void TrackingChanges();
        void Prepare(string connectionString, INotification notification, IVersionStorage storage, TrackerOptions trackerOptions);
    }
}