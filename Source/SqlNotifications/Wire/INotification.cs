using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public interface INotification
    {
        void OnInsert(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
        void OnUpdate(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
        void OnDelete(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
    }
}