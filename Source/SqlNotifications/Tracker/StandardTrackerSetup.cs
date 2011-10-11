using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LandauMedia.Storage;
using LandauMedia.Tracker;

namespace LandauMedia.Wire
{
    public class StandardTrackerSetup : ITrackerSetup
    {
        string _connectionString;
        IEnumerable<INotificationSetup> _notificationTypes;
        IVersionStorage _storage;

        string _trackerType;

        public ITrackerSetup Database(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public ITrackerSetup WithNotificationsOfAssembly(Assembly aseembly)
        {
            Type n = typeof (INotificationSetup);

            _notificationTypes = aseembly.GetTypes()
                .Where(n.IsAssignableFrom)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Select(t => (INotificationSetup)Activator.CreateInstance(t))
                .ToList();

            return this;
        }

        public ITrackerSetup UseDefaultChangeTracking()
        {
            _trackerType = "changetracking";
            return this;
        }

        public ITrackerSetup UseDefaultTimestampBased()
        {
            _trackerType = "timestamp";
            return this;
        }

        public ITrackerSetup WithVersionStorage(IVersionStorage storage)
        {
            _storage = storage;
            return this;
        }

        public NotificationTracker Build()
        {
            if (_storage == null)
                throw new InvalidOperationException("No Storage Defined");

            return new NotificationTracker(_connectionString, _notificationTypes, _trackerType, _storage);
        }
    }
}