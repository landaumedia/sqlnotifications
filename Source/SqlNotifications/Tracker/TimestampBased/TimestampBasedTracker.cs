using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using LandauMedia.Exceptions;
using LandauMedia.Infrastructure.SqlTasks;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker.TimestampBased
{
    public class TimestampBasedTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        SqlConnection _connection;
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
            while (TrackChangesForOneBucket(_options.BucketSize))
            {
                
            }
        }

        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions options)
        {
            Logger.Debug(() => "Preparing timestampbased Notification");

            _key = notificationSetup.GetType().FullName + "_" + GetType().FullName;

            NotificationSetup = notificationSetup;
            Notification = notification;
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
            _options = options;
            _versionStorage = storage;

            if (!new TableFinder(_connection).Exist(NotificationSetup.Table, NotificationSetup.Schema))
                throw new TableNotExistException(NotificationSetup.Table, NotificationSetup.Schema);

            _timestampField = new TimestampFieldFinder(_connection).GetOrEmpty(NotificationSetup.Table, NotificationSetup.Schema);

            if (string.IsNullOrEmpty(_timestampField))
                throw new InvalidOperationException(string.Format("requested Table has no timestamp field (Table:{0} Schema:{1})", NotificationSetup.Table, NotificationSetup.Schema));


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

            var maxTimestamp = ulong.MinValue;

            string statement = string.Format("SELECT TOP {6} {0}, Convert(bigint,{3}) FROM [{1}].[{2}] WHERE CONVERT(bigint, {3}) > {4} AND CONVERT(bigint, {3}) <= {5} ORDER BY {3} ASC ",
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
                        maxTimestamp = Math.Max(maxTimestamp, Convert.ToUInt64(reader.GetInt64(1)));
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

            _versionStorage.Store(_key, maxTimestamp);

            return true;
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
            return (ulong)_connection.ExecuteSkalar<long>("SELECT CONVERT(bigint, @@dbts)");
        }

        private void InitializeHashTable()
        {
            Logger.Debug(() => "Intizialize HashTable");

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
    }
}