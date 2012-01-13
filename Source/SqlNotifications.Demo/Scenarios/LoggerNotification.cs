using System;
using System.Collections.Generic;
using System.Globalization;
using LandauMedia.Wire;
using NLog;

namespace SqlNotifications.Demo.Scenarios
{
    public class LoggerNotification : INotification
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnInsert(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "INSERT On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", updatedColumns)));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "UPDATE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", updatedColumns)));
        }

        public void OnDelete(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "DELETE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", updatedColumns)));
        }
    }
}