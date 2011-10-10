using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Krowiorsch.Dojo;
using Krowiorsch.Dojo.Wire;

namespace LandauMedia.Wire
{
    public class StandardNotificationSetup : INotificationSetup
    {
        string _connectionString;
        IEnumerable<INotification> _notificationTypes;

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

        public NotificationTracker Build()
        {
            return new NotificationTracker(_connectionString, _notificationTypes, _trackerType);
        }
    }
}