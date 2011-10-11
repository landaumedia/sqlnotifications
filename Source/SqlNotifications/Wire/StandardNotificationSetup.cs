using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LandauMedia.Tracker;

namespace LandauMedia.Wire
{
    public class StandardNotificationSetup : INotificationSetup
    {
        string _connectionString;
        IEnumerable<INotification> _notificationTypes;
        IVersionStorage _storage;

        string _trackerType;

        public INotificationSetup ForDatabase(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public INotificationSetup WithNotificationsOfAssembly(Assembly aseembly)
        {
            Type n = typeof (INotification);

            _notificationTypes = aseembly.GetTypes()
                .Where(n.IsAssignableFrom)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Select(t => (INotification)Activator.CreateInstance(t))
                .ToList();

            return this;
        }

        public INotificationSetup UseDefaultChangeTracking()
        {
            _trackerType = "changetracking";
            return this;
        }

        public INotificationSetup UseDefaultTimestampBased()
        {
            _trackerType = "timestamp";
            return this;
        }

        public INotificationSetup WithVersionStorage(IVersionStorage storage)
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