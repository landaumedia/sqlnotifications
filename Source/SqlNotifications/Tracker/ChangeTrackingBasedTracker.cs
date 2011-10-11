using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker
{
    public class ChangeTrackingBasedTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string _connectionString;
        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

        ulong _lastId;

        public INotification Notification { get; internal set; }

        public void TrackingChanges()
        {
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
                    command.Parameters.AddWithValue("@lastId", _versionStorage.Load(_key));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            switch (reader.GetString(1).ToUpper())
                            {
                                case "U":
                                    Notification.OnUpdate(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                                case "I":
                                    Notification.OnInsert(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                                case "D":
                                    Notification.OnDelete(Notification, reader.GetSqlValue(0).ToString(), ParseUpdated(reader));
                                    break;
                            }
                        }
                    }
                }

                _versionStorage.Store(_key, GetLastId(connection));
            }
        }

        public void Prepare(string connectionString, INotification notification, IVersionStorage storage, TrackerOptions trackerOptions)
        {
            Logger.Info(() => "Preparing ChangeTrackingbased Notification");

            _key = notification.GetType().FullName + "_" + GetType().FullName;

            _connectionString = connectionString;
            Notification = notification;
            _options = trackerOptions;
            _versionStorage = storage;

            string updateNotifications = 
                string.Format(@"ALTER TABLE [{0}] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON);", Notification.Table);

            string isChangeTrackingEnabled =
                string.Format(@"SELECT COUNT(*) FROM sys.change_tracking_tables a 
                    INNER JOIN sys.tables b ON a.object_id = b.object_id WHERE b.name = '{0}';", Notification.Table);

            _lastId = _options.InitializeToCurrentVersion ? GetInitialId() : 0;
            _versionStorage.Store(_key, _lastId);

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

        public ulong GetInitialId()
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

        private ulong GetLastId(SqlConnection connection)
        {
            using (SqlCommand lastIdCommand = new SqlCommand("SELECT CHANGE_TRACKING_CURRENT_VERSION()", connection))
            {
                return (ulong)lastIdCommand.ExecuteScalar();
            }
        }
    }
}