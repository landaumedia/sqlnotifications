using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Krowiorsch.Dojo.Wire
{
    public class NLogPublisher : IPublishingNotifications
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("INSERT On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }

        public void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("UPDATE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }

        public void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("DELETE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }
    }
}