using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios
{
    public class ForumPostNotificationSetup : AbstractNotificationSetup
    {
        public ForumPostNotificationSetup()
        {
            SetSchemaAndTable("t_forumpost", "Blog");
            SetIdType<int>();
            SetKeyColumn("Id");
            SetTrackingType("timestamp");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}