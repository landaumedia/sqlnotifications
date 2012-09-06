using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using LandauMedia.Infrastructure;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker
{
    public class TrackerRunner
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        readonly string _connectionString;
        readonly IEnumerable<INotificationSetup> _notificationTypes;
        readonly IVersionStorage _storage;

        readonly TrackerOptions _defaultTrackerOptions;

        readonly Func<Type, INotification> _factory;
        readonly IPerformanceCounter _counter;

        readonly string _defaultTrackingType;

        Thread _workerThread;

        IEnumerable<ITracker> _trackers;

        public TrackerRunner(string connectionString, IEnumerable<INotificationSetup> notificationTypes, string defaultTrackingType, IVersionStorage storage, Func<Type, INotification> factory, TrackerOptions defaultTrackerOptions, IPerformanceCounter counter)
        {
            _connectionString = connectionString;
            _notificationTypes = notificationTypes;
            _defaultTrackingType = defaultTrackingType;
            _factory = factory;
            _defaultTrackerOptions = defaultTrackerOptions;
            _storage = storage;
            _counter = counter;
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

            IList<Thread> trackerThreads = new List<Thread>();

            try
            {
                foreach (var tracker in _trackers)
                {
                    trackerThreads.Add(new Thread(tracker.TrackingChanges));
                }

                foreach(var trackerThread in trackerThreads)
                {
                    trackerThread.Start();
                }

                foreach(var trackerThread in trackerThreads)
                {
                    trackerThread.Join();
                }
            }
            catch (ThreadAbortException)
            {
                foreach(var trackerThread in trackerThreads)
                {
                    trackerThread.Abort();
                }
                Logger.Debug(() => "Tracking Thread aborted");
            }
        }

        private ITracker BuildAndPrepareTracker(INotificationSetup notificationSetup)
        {
            ITracker tracker = TrackerFactory.BuildByName(
                string.IsNullOrWhiteSpace(notificationSetup.TrackingType) ? _defaultTrackingType : notificationSetup.TrackingType);

            TrackerOptions options = _defaultTrackerOptions ?? new TrackerOptions();

            tracker.Prepare(_connectionString, notificationSetup, _factory(notificationSetup.Notification), _storage, options);

            tracker.PerformanceCounter = _counter;
            return tracker;
        }
    }
}