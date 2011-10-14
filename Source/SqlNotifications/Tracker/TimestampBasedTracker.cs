using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker
{
    public class TimestampBasedTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string _connectionString;
        string _timestampField;

        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

        readonly Hashtable _lastseenIds = new Hashtable();

        public INotificationSetup NotificationSetup { get; internal set; }
        public INotification Notification { get; internal set; }

        public void TrackingChanges()
        {
            while (TrackChangesForOneBucket(1000))
            {
                
            }
        }


        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage stroage, TrackerOptions options)
        {
            Logger.Debug(() => "Preparing timestampbased Notification");

            _key = notificationSetup.GetType().FullName + "_" + GetType().FullName;

            NotificationSetup = notificationSetup;
            Notification = notification;
            _connectionString = connectionString;
            _options = options;
            
            _versionStorage = stroage;

            _timestampField = GetTimestampFieldOrNull();

            if (_timestampField == null)
                throw new InvalidOperationException("requested Table has no timestamp field");


            ulong timestamp;
            if (_options.InitializeToCurrentVersion)
            {
                timestamp = GetLastTimestamp();
                InitializeHashTable();
            }
            else
            {
                timestamp = 0;
            }

            _versionStorage.Store(_key, timestamp);

            Logger.Debug(() => "Finished Prepare for timestampbased Notification");
        }

        private bool TrackChangesForOneBucket(int bucketSize)
        {
            var fromTimestamp = _versionStorage.Load(_key);
            var toTimestamp = GetLastTimestamp();


            string statement = string.Format("SELECT TOP {6} {0} FROM [{1}].[{2}] WHERE CONVERT(bigint, {3}) > {4} AND CONVERT(bigint, {3}) <= {5} ORDER BY {3} ASC ",
                NotificationSetup.KeyColumn,
                NotificationSetup.Schema,
                NotificationSetup.Table,
                _timestampField,
                fromTimestamp,
                toTimestamp,
                bucketSize);

            ArrayList listOfChangedRows = new ArrayList();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return false;

                    while (reader.Read())
                    {
                        listOfChangedRows.Add(ReadFromReader(reader, NotificationSetup.IdType));
                    }
                }
            }

            foreach (var entry in listOfChangedRows)
            {
                if (_lastseenIds.Contains(entry))
                {
                    Notification.OnUpdate(NotificationSetup, entry.ToString(), Enumerable.Empty<string>());
                }
                else
                {
                    Notification.OnInsert(NotificationSetup, entry.ToString(), Enumerable.Empty<string>());
                    _lastseenIds.Add(entry, entry);
                }
            }

            _versionStorage.Store(_key, toTimestamp);

            return listOfChangedRows.Count == bucketSize;
        }

        private static object ReadFromReader(SqlDataReader reader, Type t)
        {
            if (t == typeof(string))
            {
                return reader.GetString(0);
            }

            if (t == typeof(int))
            {
                return reader.GetInt32(0);
            }

            if (t == typeof(Guid))
            {
                return reader.GetGuid(0);
            }

            throw new ArgumentOutOfRangeException();
        }

        private ulong GetLastTimestamp()
        {
            const string selectTimestamp = "SELECT CONVERT(bigint, @@dbts)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(selectTimestamp, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    return Convert.ToUInt64(reader.GetInt64(0));
                }
            }
        }

        private void InitializeHashTable()
        {
            string select = string.Format("SELECT {1} FROM [{0}].[{2}]", NotificationSetup.Schema, NotificationSetup.KeyColumn, NotificationSetup.Table);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(select, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return;

                    while (reader.Read())
                    {
                        var value = ReadFromReader(reader, NotificationSetup.IdType);
                        _lastseenIds.Add(value, value);
                    }
                }
            }
        }

        private string GetTimestampFieldOrNull()
        {
            string existTimestampField = @"SELECT Column_Name FROM INFORMATION_SCHEMA.COLUMNS
                WHERE DATA_TYPE = 'timestamp' AND TABLE_SCHEMA='@Schema' and TABLE_NAME='@TableName'";

            existTimestampField = existTimestampField.Replace("@TableName", NotificationSetup.Table);
            existTimestampField = existTimestampField.Replace("@Schema", NotificationSetup.Schema);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(existTimestampField, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;

                    reader.Read();

                    return reader.GetString(0);
                }
            }
        }
    }
}