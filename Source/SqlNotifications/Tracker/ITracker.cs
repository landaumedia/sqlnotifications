using Krowiorsch.Dojo.Wire;

namespace LandauMedia.Tracker
{
    public interface ITracker
    {
        INotification Notification { get; }
        void TrackingChanges();
        void Prepare();
    }
}