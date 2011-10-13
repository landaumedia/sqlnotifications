using System.Collections.Generic;
using LandauMedia.Wire;
using NLog;

namespace SqlNotifications.Demo.Scenarios
{
    public class LoggerNotification : INotification
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnInsert(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(() => string.Format("Insert with id:{0}", id));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(() => string.Format("Update with id:{0}", id));
        }

        public void OnDelete(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(() => string.Format("Delete with id:{0}", id));
        }
    }
}