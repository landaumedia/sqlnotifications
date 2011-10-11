using System;
using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public interface INotificationSetup
    {
        string Table { get; }
        string Schema { get; }
        string KeyColumn { get; }
        Type IdType { get; }

        string TrackingType { get; }

        IEnumerable<string> IntrestedInUpdatedColums { get; }

        INotification Notification { get; }
    }

    public interface INotification
    {
        void OnInsert(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
        void OnUpdate(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
        void OnDelete(INotificationSetup notificationSetup, string id, IEnumerable<string> updatedColumns);
    }
}