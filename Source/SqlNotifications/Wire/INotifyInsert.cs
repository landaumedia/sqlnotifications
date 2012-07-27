using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public interface INotifyInsert
    {
        void OnInsert(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns); 
    }
}