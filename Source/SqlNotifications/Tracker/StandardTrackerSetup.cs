using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public class StandardTrackerSetup : ITrackerSetup
    {
        string _connectionString;
        IVersionStorage _storage;

        Func<Type, INotification> _notificationFactory;

        Assembly _souceAssembly;

        string _trackerType;

        public ITrackerSetup Database(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public ITrackerSetup WithNotificationsOfAssembly(Assembly assembly)
        {
            _souceAssembly = assembly;


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

        public ITrackerSetup WithNotificationFactory(Func<Type, INotification> notificationFactory)
        {
            _notificationFactory = notificationFactory;
            return this;
        }

        public NotificationTracker Build()
        {
            if (_storage == null)
                throw new InvalidOperationException("No Storage Defined");

            Type n = typeof(INotificationSetup);

            IEnumerable<INotificationSetup>  notificationTypes = _souceAssembly.GetTypes()
                .Where(n.IsAssignableFrom)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Select(t => (INotificationSetup)Activator.CreateInstance(t))
                .ToList();

            if (_notificationFactory == null)
                _notificationFactory = t => (INotification)Activator.CreateInstance(t);

            return new NotificationTracker(_connectionString, notificationTypes, _trackerType, _storage, _notificationFactory);
        }
    }
}