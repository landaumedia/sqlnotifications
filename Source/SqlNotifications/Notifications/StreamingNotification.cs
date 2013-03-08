using System;
using System.Reactive.Subjects;
using LandauMedia.Model;
using LandauMedia.Wire;

namespace LandauMedia.Notifications
{
    public sealed class StreamingNotification<T> : INotification
    {
        readonly Func<string, AditionalNotificationInformation, T> _convertFunction;

        public StreamingNotification(Func<string, AditionalNotificationInformation, T> convertFunction)
        {
            InsertStream = new Subject<T>();
            UpdateStream = new Subject<T>();
            DeleteStream = new Subject<T>();

            _convertFunction = convertFunction;
        }

        public Subject<T> InsertStream { get; set; }

        public Subject<T> UpdateStream { get; set; }
        
        public Subject<T> DeleteStream { get; set; }

        public void OnDelete(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            DeleteStream.OnNext(_convertFunction(id, addtionalInformation));
        }

        public void OnInsert(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            InsertStream.OnNext(_convertFunction(id, addtionalInformation));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AditionalNotificationInformation addtionalInformation)
        {
            UpdateStream.OnNext(_convertFunction(id, addtionalInformation));
        }
    }
}