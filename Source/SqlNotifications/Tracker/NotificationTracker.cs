using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker
{
    public class NotificationTracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        readonly string _connectionString;
        readonly IEnumerable<INotificationSetup> _notificationTypes;
        readonly IVersionStorage _storage;

        readonly string _defaultTrackingType;

        Thread _workerThread;

        IEnumerable<ITracker> _trackers;

        public NotificationTracker(string connectionString, IEnumerable<INotificationSetup> notificationTypes, string defaultTrackingType, IVersionStorage storage)
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
            Logger.Debug(() => string.Format("Start Tracking with {0} Trackers", _trackers.Count()));

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

        private ITracker BuildAndPrepareTracker(INotificationSetup notificationSetup)
        {
            ITracker tracker = TrackerFactory.BuildByName(
                string.IsNullOrWhiteSpace(notificationSetup.TrackingType) ? _defaultTrackingType : notificationSetup.TrackingType);

            tracker.Prepare(_connectionString, notificationSetup, _storage, new TrackerOptions {InitializeToCurrentVersion = true});
            return tracker;
        }
    }
}