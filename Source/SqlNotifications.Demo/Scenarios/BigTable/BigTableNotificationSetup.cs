using System;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.BigTable
{
    public class BigTableNotificationSetup : AbstractNotificationSetup
    {
        public BigTableNotificationSetup()
        {
            SetTable("BigTable");
            SetKeyColumn("Id");
            SetIdType<int>();
            SetTrackingType("timestamp");

        }

        public override Type Notification
        {
            get { return typeof(LoggerNotification); }
        }
    }
}