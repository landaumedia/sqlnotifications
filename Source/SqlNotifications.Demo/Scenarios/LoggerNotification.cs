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

        public void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "INSERT On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", information.UpdatedColumns)));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "UPDATE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", information.UpdatedColumns)));

            if (information.AdditionalColumns.Any())
            {
                Logger.Info(string.Format("AddionalInfo:{0}\n", 
                    information.AdditionalColumns.Aggregate(string.Empty, (s, pair) => s + "Feld:" + pair.Key + ":" + pair.Value)));
            }

            if (information.ColumnOldValue.Any())
            {
                Logger.Info(string.Format("ColumnOldValue:{0}\n",
                    information.ColumnOldValue.Aggregate(string.Empty, (s, pair) => s + "Feld:" + pair.Key + ":" + pair.Value)));
            }
        }

        public void OnDelete(INotificationSetup notificationSetup, string id, AditionalNotificationInformation information)
        {
            Logger.Info(string.Format(CultureInfo.InvariantCulture, "DELETE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notificationSetup.Table, id, String.Join(",", information.UpdatedColumns)));
        }
    }
}