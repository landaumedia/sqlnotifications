using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios
{
    public class ForumPostNotificationSetup : AbstractNotificationSetup
    {
        public ForumPostNotificationSetup()
        {
            SetSchemaAndTable("t_forumpost", "blog");
            SetIdType<int>();
            SetKeyColumn("Id");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}