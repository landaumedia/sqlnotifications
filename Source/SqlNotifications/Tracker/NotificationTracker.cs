using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Krowiorsch.Dojo.Wire;
using LandauMedia.Tracker;
using NLog;

namespace Krowiorsch.Dojo
{
    public class NotificationTracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        readonly string _connectionString;
        readonly IEnumerable<INotification> _notificationTypes;

        readonly string _defaultTrackingType;

        Thread _workerThread;

        IEnumerable<ChangeTrackingBasedTracker> _trackers;

        public NotificationTracker(string connectionString, IEnumerable<INotification> notificationTypes, string defaultTrackingType)
        {
            _connectionString = connectionString;
            _notificationTypes = notificationTypes;
            _defaultTrackingType = defaultTrackingType;
        }

        public IDisposable Start()
        {
            _trackers = _notificationTypes.Select(n => new ChangeTrackingBasedTracker(_connectionString, n)).ToList();

            _workerThread = new Thread(StartTracking);
            _workerThread.Start();

            return Disposable.Create(() => _workerThread.Abort());
        }

        private void StartTracking()
        {
            try
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
    }
}