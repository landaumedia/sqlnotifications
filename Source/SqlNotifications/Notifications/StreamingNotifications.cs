using System.Reactive.Subjects;
using LandauMedia.Model;
using LandauMedia.Wire;

namespace LandauMedia.Notifications
{
    public abstract class StreamingNotifications<T> : INotification
    {
        protected StreamingNotifications()
        {
            InsertStream = new Subject<T>();
            UpdateStream = new Subject<T>();
            DeleteStream = new Subject<T>();
        }

        Subject<T> InsertStream { get; set; }

        Subject<T> UpdateStream { get; set; }
        
        Subject<T> DeleteStream { get; set; }

        public void OnDelete(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            DeleteStream.OnNext(ConvertTo(id, addtionalInformation));
        }

        public void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            InsertStream.OnNext(ConvertTo(id, addtionalInformation));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            UpdateStream.OnNext(ConvertTo(id, addtionalInformation));
        }

        public abstract T ConvertTo(string id, AditionalNotificationInformation aditionalNotification);
    }
}