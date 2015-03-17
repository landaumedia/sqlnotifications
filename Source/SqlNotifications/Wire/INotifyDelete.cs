using LandauMedia.Model;

namespace LandauMedia.Wire
{
    public interface INotifyDelete
    {
        void OnDelete(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation addtionalInformation);
    }
}