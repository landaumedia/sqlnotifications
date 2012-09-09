using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.ArticleTable
{
    public class ArticleTableNotificationSetup : AbstractNotificationSetup
    {
        public ArticleTableNotificationSetup()
        {
            SetTable("ArticleTable");
            SetKeyColumn("ID_Article");
            SetIdType<long>();
            SetTrackingType("timestamp");
        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}