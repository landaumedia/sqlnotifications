﻿using System.Collections.Generic;
using System.Linq;
using Krowiorsch.Dojo.Wire;
using NLog;

namespace SqlNotifications.Demo.Notifications
{
    public class UserNotification : AbstractNotification
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UserNotification()
        {
            SetTable("User");
            SetKeyColumn("Id");
            SetIdType<int>();

            IntrestedInColumn("Username");
            IntrestedInColumn("Description");
        }

        public override void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("INSERT On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }

        public override void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("UPDATE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }

        public override void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            string columns = updatedColumns.Aggregate(string.Empty, (a, b) => a + b + ",");
            Logger.Info(() => string.Format("DELETE On Table '{0}' With Id '{1}' (UpdatedColumns:{2})", notification.Table, id, columns));
        }
    }
}