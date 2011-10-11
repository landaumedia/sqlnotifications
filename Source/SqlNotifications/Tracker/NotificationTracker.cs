using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker
{
    public class NotificationTracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        readonly string _connectionString;
        readonly IEnumerable<INotification> _notificationTypes;
        readonly IVersionStorage _storage;

        readonly string _defaultTrackingType;

        Thread _workerThread;

        IEnumerable<ITracker> _trackers;

        public NotificationTracker(string connectionString, IEnumerable<INotification> notificationTypes, string defaultTrackingType, IVersionStorage storage)
        {
            _connectionString = connectionString;
            _notificationTypes = notificationTypes;
            _defaultTrackingType = defaultTrackingType;
            _storage = storage;
        }

        public IDisposable Start()
        {
            _trackers = _notificationTypes.Select(BuildAndPrepareTracker).ToList();

            _workerThread = new Thread(StartTracking);
            _workerThread.Start();

            return Disposable.Create(() => _workerThread.Abort());
        }

        private void StartTracking()
        {
            try
            {
                while (true)
                {
                    foreach (var tracker in _trackers)
                    {
                        tracker.TrackingChanges();
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            {
                Logger.Debug(() => "Tracking Thread aborted");
                return;
            }
        }

        private ITracker BuildAndPrepareTracker(INotification notification)
        {
            ITracker tracker = TrackerFactory.BuildByName(
                string.IsNullOrWhiteSpace(notification.TrackingType) ? _defaultTrackingType : notification.TrackingType);

            tracker.Prepare(_connectionString, notification, _storage, new TrackerOptions {InitializeToCurrentVersion = true});
            return tracker;
        }
    }
}