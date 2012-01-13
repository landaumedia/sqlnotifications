using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.LocalUserWithDatabaseStorage
{
    public class UserNotificationSetup : AbstractNotificationSetup
    {
        public UserNotificationSetup()
        {
            SetTable("User");
            SetKeyColumn("Id");
            SetIdType<int>();
            SetTrackingType("timestamp");

            IntrestedInColumn("Username");
            IntrestedInColumn("Description");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}