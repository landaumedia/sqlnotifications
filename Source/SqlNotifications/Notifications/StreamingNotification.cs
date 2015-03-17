using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LandauMedia.Model;
using LandauMedia.Wire;

namespace LandauMedia.Notifications
{
    public sealed class StreamingNotification<T> : INotification
    {
        readonly Func<string, AdditionalNotificationInformation, T> _convertFunction;
        readonly Subject<T> _insertSubject = new Subject<T>();
        readonly Subject<T> _updateSubject = new Subject<T>();
        readonly Subject<T> _deleteSubject = new Subject<T>();


        public StreamingNotification(Func<string, AdditionalNotificationInformation, T> convertFunction)
        {
            // buildup all stream
            Stream = InsertStream
                .Merge(UpdateStream)
                .Merge(DeleteStream);

            _convertFunction = convertFunction;
        }

        public IObservable<T> InsertStream
        {
            get { return _insertSubject; }
        }

        public IObservable<T> UpdateStream
        {
            get { return _updateSubject; }
        }

        public IObservable<T> DeleteStream
        {
            get { return _deleteSubject; }
        }

        public IObservable<T> Stream { get; set; }

        public void OnDelete(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation addtionalInformation)
        {
            _deleteSubject.OnNext(_convertFunction(id, addtionalInformation));
        }

        public void OnInsert(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation addtionalInformation)
        {
            _insertSubject.OnNext(_convertFunction(id, addtionalInformation));
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation addtionalInformation)
        {
            _updateSubject.OnNext(_convertFunction(id, addtionalInformation));
        }
    }
}