using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    /// <summary>
    /// Interface for a trackersystem. A trackersystem tracks databasechanges with a specific technique.
    /// </summary>
    public interface ITracker
    {
        /// <summary>
        /// Gets the notification setup.
        /// </summary>
        INotificationSetup NotificationSetup { get; }


        /// <summary>
        /// the Notification that will be called on databasechanges.
        /// </summary>
        INotification Notification { get; }

        /// <summary>
        /// start tracking changes. the call is syncron and will block until aborted
        /// </summary>
        void TrackingChanges();


        /// <summary>
        /// Prepares the tracker.
        /// </summary>
        /// <param name="connectionString">connectionstring for the Datanase</param>
        /// <param name="notificationSetup">Setup descibes the information the consumer wanted the track</param>
        /// <param name="notification">the notificationobject that should be called on changes</param>
        /// <param name="storage">the versionstorage that is needed for tracking</param>
        /// <param name="trackerOptions">options for tracking</param>
        void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions trackerOptions);
    }
}