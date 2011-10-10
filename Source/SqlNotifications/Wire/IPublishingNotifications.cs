using System.Collections.Generic;

namespace Krowiorsch.Dojo.Wire
{
    public interface IPublishingNotifications
    {
        void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns);
        void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns);
        void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns);
    }
}