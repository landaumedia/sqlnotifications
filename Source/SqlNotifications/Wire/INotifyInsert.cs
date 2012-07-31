using System.Collections.Generic;
using LandauMedia.Model;

namespace LandauMedia.Wire
{
    public interface INotifyInsert
    {
        void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation);
    }
}