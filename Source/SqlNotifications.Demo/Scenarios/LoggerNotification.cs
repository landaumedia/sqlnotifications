using System;
using System.Globalization;
using System.Linq;
using LandauMedia.Model;
using LandauMedia.Wire;
using NLog;

namespace SqlNotifications.Demo.Scenarios
{
    public class LoggerNotification : INotification
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        int _countInsert = 0;
        int _countUpdate = 0;

        public void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {
            _countInsert++;

            if ((_countInsert % 1000) == 0)
                Logger.Info(string.Format("{0} Insert Events detected", _countInsert));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {
            _countUpdate++;

            if ((_countUpdate % 1000) == 0)
                Logger.Info(string.Format("{0} Update Events detected", _countUpdate));
        }

        public void OnDelete(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {

            Logger.Info(string.Format(CultureInfo.InvariantCulture, "DELETE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", information.UpdatedColumns)));
        }
    }
}