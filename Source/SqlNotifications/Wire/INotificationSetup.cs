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

        string[] AdditionalColumns { get; }

        IEnumerable<string> IntrestedInUpdatedColums { get; }

        Type Notification { get; }
    }
}