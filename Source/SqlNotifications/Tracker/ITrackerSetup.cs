using System;
using System.Collections.Generic;
using System.Reflection;
using LandauMedia.Infrastructure;
using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public interface ITrackerSetup
    {
        ITrackerSetup Database(string connectionString);

        ITrackerSetup WithNotificationsOfAssembly(Assembly aseembly);

        ITrackerSetup WithNotifications(IEnumerable<Type> types);

        ITrackerSetup UseDefaultChangeTracking();

        ITrackerSetup UseDefaultTimestampBased();

        ITrackerSetup UseDefaultTrackingTypeOf(string trackingType);

        ITrackerSetup WithVersionStorage(IVersionStorage storage);

        ITrackerSetup WithNotificationFactory(Func<Type, INotification> notificationFactory);

        ITrackerSetup WithDefaultTrackerOptions(TrackerOptions options);

        ITrackerSetup WithPerformanceCounter(IPerformanceCounter counter);

        TrackerRunner Build();
    }
}