using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LandauMedia.Infrastructure;
using LandauMedia.Model;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker.ChangeTrackingBased
{
    public class ChangeTrackingBasedTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string _connectionString;
        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

        public INotificationSetup NotificationSetup { get; internal set; }
        public INotification Notification { get; internal set; }

        public IPerformanceCounter PerformanceCounter { get; set; }

        public void TrackingChanges()
        {
            string checkChange = string.Format(@"SELECT CT.Id, CT.SYS_CHANGE_OPERATION, {3} CT.SYS_CHANGE_COLUMNS, CT.SYS_CHANGE_CONTEXT
                FROM [{0}].[{1}] AS P RIGHT OUTER JOIN CHANGETABLE(CHANGES [{0}].[{1}], @lastId) AS CT ON P.Id = CT.[{2}]",
                NotificationSetup.Schema,
                NotificationSetup.Table,
                NotificationSetup.KeyColumn,
                BuildCheckColumnsStatement());

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(checkChange, connection))
                {
                    command.Parameters.AddWithValue("@lastId", (long)_versionStorage.Load(_key));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // should be a async notification
                            switch (reader.GetString(1).ToUpper())
                            {
                                case "U":
                                    Notification.OnUpdate(NotificationSetup, reader.GetSqlValue(0).ToString(), new AditionalNotificationInformation { UpdatedColumns = ParseUpdated(reader) });
                                    break;
                                case "I":
                                    Notification.OnInsert(NotificationSetup, reader.GetSqlValue(0).ToString(), new AditionalNotificationInformation { UpdatedColumns = ParseUpdated(reader) });
                                    break;
                                case "D":
                                    Notification.OnDelete(NotificationSetup, reader.GetSqlValue(0).ToString(), new AditionalNotificationInformation { UpdatedColumns = ParseUpdated(reader) });
                                    break;
                            }
                        }
                    }
                }

                _versionStorage.Store(_key, (ulong)GetLastId(connection));
            }
        }

        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions trackerOptions)
        {
            Logger.Info(() => "Preparing ChangeTrackingbased Notification");

            _key = notificationSetup.GetType().FullName + "_" + GetType().FullName;

            _connectionString = connectionString;
            NotificationSetup = notificationSetup;
            Notification = notification;
            _options = trackerOptions;
            _versionStorage = storage;

            string updateNotifications =
                string.Format(@"ALTER TABLE [{0}].[{1}] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON);",
                NotificationSetup.Schema,
                NotificationSetup.Table);

            string isChangeTrackingEnabled =
                string.Format(@"SELECT Count(*) FROM sys.change_tracking_tables a 
                    INNER JOIN sys.tables b ON a.object_id = b.object_id 
                    INNER JOIN sys.schemas c ON b.schema_id = c.schema_id
                    WHERE b.name = '{0}' and c.name = '{1}'",
                    NotificationSetup.Table,
                    NotificationSetup.Schema);

            ulong keyToStore = 0;

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrent)
                keyToStore = GetInitialId();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = GetInitialId();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            _versionStorage.Store(_key, keyToStore);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand isEnabled = new SqlCommand(isChangeTrackingEnabled, connection))
                {
                    if ((int)isEnabled.ExecuteScalar() > 0)
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
            return from columns in NotificationSetup.IntrestedInUpdatedColums
                   let isChanged = reader.GetInt32(reader.GetOrdinal(string.Format("HasChanged{0}", columns))) == 1
                   where isChanged
                   select columns;
        }

        public ulong GetInitialId()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return (ulong)GetLastId(connection);
            }
        }


        private string BuildCheckColumnsStatement()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var colum in NotificationSetup.IntrestedInUpdatedColums)
            {
                sb.AppendLine(
                    string.Format("CHANGE_TRACKING_IS_COLUMN_IN_MASK(COLUMNPROPERTY(OBJECT_ID('[{0}].[{1}]'), '{2}', 'ColumnId'), CT.SYS_CHANGE_COLUMNS) as HasChanged{2}, "
                    , NotificationSetup.Schema
                    , NotificationSetup.Table
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