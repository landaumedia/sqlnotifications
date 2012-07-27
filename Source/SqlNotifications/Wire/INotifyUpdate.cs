using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public interface INotifyUpdate
    {
        void OnUpdate(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns); 
    }
}