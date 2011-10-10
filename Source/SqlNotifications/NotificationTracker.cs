using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Krowiorsch.Dojo.Wire;

namespace Krowiorsch.Dojo
{
    public class NotificationTracker
    {
        readonly string _connectionString;
        readonly IPublishingNotifications _publishing;
        readonly IEnumerable<INotification> _notificationTypes;

        Thread _workerThread;

        IEnumerable<Tracker> _trackers;
        IDictionary<INotification, long> _trackingIds = new Dictionary<INotification, long>(); 

        public NotificationTracker(string connectionString, IPublishingNotifications publishing, IEnumerable<INotification> notificationTypes)
        {
            _connectionString = connectionString;
            _publishing = publishing;
            _notificationTypes = notificationTypes;
        }

        public void Start()
        {
            _trackers = _notificationTypes.Select(n => new Tracker(_connectionString, n, _publishing)).ToList();
            _trackingIds = _trackers.ToDictionary(t => t.Notification, t => t.GetInitialId());


            _workerThread = new Thread(StartTracking);
            _workerThread.Start();
        }

        private void StartTracking()
        {
            // Prepare
            foreach (var tracker in _trackers)
            {
                tracker.Prepare();
            }

            while (true)
            {
                foreach (var tracker in _trackers)
                {
                    var newId = tracker.TrackOnce(_trackingIds[tracker.Notification]);
                    _trackingIds[tracker.Notification] = newId;
                }

                Thread.Sleep(1000);
            }
        }
    }
}