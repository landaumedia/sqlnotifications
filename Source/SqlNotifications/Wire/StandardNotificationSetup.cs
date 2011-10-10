using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Krowiorsch.Dojo.Wire
{
    public class StandardNotificationSetup : INotificationSetup
    {
        string _connectionString;
        IPublishingNotifications _publishing;
        IEnumerable<INotification> _notificationTypes;

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

        public INotificationSetup PublishingTo(IPublishingNotifications publishing)
        {
            _publishing = publishing;
            return this;
        }

        public NotificationTracker Build()
        {
            return new NotificationTracker(_connectionString, _publishing, _notificationTypes);
        }
    }
}