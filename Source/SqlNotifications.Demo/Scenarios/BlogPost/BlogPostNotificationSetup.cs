using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios
{
    public class BlogPostNotificationSetup : AbstractNotificationSetup
    {
        public BlogPostNotificationSetup()
        {
            SetIdType<int>();
            SetTrackingType("timestamp");
            SetKeyColumn("Id");
            SetSchemaAndTable("t_blogPost", "Blog");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }


    }
}