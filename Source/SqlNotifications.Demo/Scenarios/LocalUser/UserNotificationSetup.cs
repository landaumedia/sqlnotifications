using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.LocalUser
{
    public class UserNotificationSetup : AbstractNotificationSetup
    {
        public UserNotificationSetup()
        {
            SetTable("User");
            SetKeyColumn("Id");
            SetIdType<int>();
            SetTrackingType("changetracking");

            IntrestedInColumn("Username");
            IntrestedInColumn("Description");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}