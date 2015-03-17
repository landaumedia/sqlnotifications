using LandauMedia.Model;

namespace LandauMedia.Wire
{
    public interface INotifyUpdate
    {
        void OnUpdate(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation addtionalInformation);
    }
}