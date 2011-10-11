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

        public INotification Notification { get; internal set; }

        public void TrackingChanges()
        {
            string statement = string.Format("SELECT {0} FROM [{4}].[{1}] WHERE CONVERT(bigint, {2}) > {3}",
                Notification.KeyColumn,
                Notification.Table,
                _timestampField,
                _versionStorage.Load(_key), 
                Notification.Schema);

            ArrayList list = new ArrayList();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return;

                    while (reader.Read())
                    {
                        list.Add(ReadFromReader(reader, Notification.IdType));
                    }
                }
            }

            foreach (var entry in list)
            {
                if (_lastseenIds.Contains(entry))
                {
                    Notification.OnUpdate(Notification, entry.ToString(), Enumerable.Empty<string>());
                }
                else
                {
                    Notification.OnInsert(Notification, entry.ToString(), Enumerable.Empty<string>());
                    _lastseenIds.Add(entry, entry);
                }
            }

            _versionStorage.Store(_key, GetLastTimestamp());
        }

        public void Prepare(string connectionString, INotification notification, IVersionStorage stroage, TrackerOptions options)
        {
            Logger.Debug(() => "Preparing timestampbased Notification");

            _key = notification.GetType().FullName + "_" + GetType().FullName;

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
            string select = string.Format("SELECT {1} FROM [{0}].[{2}]", Notification.Schema, Notification.KeyColumn, Notification.Table);

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
                        var value = ReadFromReader(reader, Notification.IdType);
                        _lastseenIds.Add(value, value);
                    }
                }
            }
        }

        private string GetTimestampFieldOrNull()
        {
            string existTimestampField = @"select col.name
                from sysobjects obj inner join syscolumns col on obj.id = col.id inner join systypes types on col.xtype = types.xtype
                where obj.name = '@TableName' and types.name='timestamp'";

            existTimestampField = existTimestampField.Replace("@TableName", Notification.Table);

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