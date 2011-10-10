using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Krowiorsch.Dojo.Wire;

namespace Krowiorsch.Dojo
{
    public class Tracker
    {
        readonly string _connectionString;
        readonly IPublishingNotifications _publisher;

        public Tracker(string connectionString, INotification notification, IPublishingNotifications publisher)
        {
            _connectionString = connectionString;
            Notification = notification;
            _publisher = publisher;
        }

        public INotification Notification { get; internal set; }

        public long TrackOnce(long lastCheckId)
        {
            string hasChanged = BuildCheckColumnsStatement();

            string checkChange = string.Format(@"SELECT CT.Id, CT.SYS_CHANGE_OPERATION, {2} CT.SYS_CHANGE_COLUMNS, CT.SYS_CHANGE_CONTEXT
                FROM [{0}] AS P RIGHT OUTER JOIN CHANGETABLE(CHANGES [{0}], @lastId) AS CT ON P.Id = CT.[{1}]", 
                Notification.Table, 
                Notification.KeyColumn,
                BuildCheckColumnsStatement());

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(checkChange, connection))
                {
                    command.Parameters.AddWithValue("@lastId", lastCheckId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            switch (reader.GetString(1).ToUpper())
                            {
                                case "U":
                                    _publisher.OnUpdate(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                                case "I":
                                    _publisher.OnInsert(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                                case "D":
                                    _publisher.OnDelete(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                            }
                        }
                    }
                }

                return GetLastId(connection);
            }
        }

        public void Prepare()
        {
            string updateNotifications = 
                string.Format(@"ALTER TABLE [{0}] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON);", Notification.Table);

            string isChangeTrackingEnabled =
                string.Format(@"SELECT COUNT(*) FROM sys.change_tracking_tables a 
                    INNER JOIN sys.tables b ON a.object_id = b.object_id WHERE b.name = '{0}';", Notification.Table);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand isEnabled = new SqlCommand(isChangeTrackingEnabled, connection))
                {
                    if (((int)isEnabled.ExecuteScalar() > 0))
                        return;
                }

                using (SqlCommand command = new SqlCommand(updateNotifications, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<string> ParseUpdated(SqlDataReader reader)
        {
            foreach (string columns in Notification.IntrestedInUpdatedColums)
            {
                bool isChanged = reader.GetInt32(reader.GetOrdinal(string.Format("HasChanged{0}", columns))) ==  1;
                if (isChanged) yield return columns;
            }
        }

        public long GetInitialId()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return GetLastId(connection);
            }
        }


        private string BuildCheckColumnsStatement()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var colum in Notification.IntrestedInUpdatedColums)
            {
                sb.AppendLine(
                    string.Format("CHANGE_TRACKING_IS_COLUMN_IN_MASK(COLUMNPROPERTY(OBJECT_ID('{0}'), '{1}', 'ColumnId'), CT.SYS_CHANGE_COLUMNS) as HasChanged{1}, "
                    , Notification.Table
                    , colum));
            }

            return sb.ToString();
        }

        private long GetLastId(SqlConnection connection)
        {
            using (SqlCommand lastIdCommand = new SqlCommand("SELECT CHANGE_TRACKING_CURRENT_VERSION()", connection))
            {
                return (long)lastIdCommand.ExecuteScalar();
            }
        }
    }
}