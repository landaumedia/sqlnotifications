﻿using System;
using System.Reflection;
using LandauMedia.Storage;
using LandauMedia.Wire;

namespace LandauMedia.Tracker
{
    public interface ITrackerSetup
    {
        ITrackerSetup Database(string connectionString);

        ITrackerSetup WithNotificationsOfAssembly(Assembly aseembly);

        ITrackerSetup UseDefaultChangeTracking();

        ITrackerSetup UseDefaultTimestampBased();

        ITrackerSetup WithVersionStorage(IVersionStorage storage);

        ITrackerSetup WithNotificationFactory(Func<Type, INotification> notificationFactory);

        NotificationTracker Build();
    }
}