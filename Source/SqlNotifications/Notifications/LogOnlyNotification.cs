using LandauMedia.Model;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Notifications
{
    public class LogOnlyNotification : INotification
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnDelete(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            Logger.Debug(string.Format("Delete Row for Id:{0}", id));
        }

        public void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            Logger.Debug(string.Format("Insert Row for Id:{0}", id));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            Logger.Debug(string.Format("Update Row for Id:{0}", id));
        }
    }
}