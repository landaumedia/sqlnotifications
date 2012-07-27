using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public interface INotifyDelete
    {
        void OnDelete(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
    }
}