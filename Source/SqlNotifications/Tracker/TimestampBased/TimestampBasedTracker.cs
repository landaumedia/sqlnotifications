using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using LandauMedia.Exceptions;
using LandauMedia.Infrastructure;
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
        string _timestampField;

        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

        readonly ILookupTable _lastseenIds = new SortedArrayLookupTable();

        public INotificationSetup NotificationSetup { get; internal set; }
        public INotification Notification { get; internal set; }

        public IPerformanceCounter PerformanceCounter { get; set; }

        public void TrackingChanges()
        {
            while (true)
            {
                try
                {
                    if (!TrackChangesForOneBucket(_options.BucketSize))
                    {
                        Thread.Sleep(_options.FetchInterval);                  // wait short time if no changed pending
                    }
                }
                catch (SqlException sqlException)
                {
                    // on error log a warning and 
                    Logger.WarnException("Error on Tracking Database", sqlException);
                }
            }
        }

        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions options)
        {
            Logger.Debug(() => "Preparing timestampbased Notification");

            _key = notificationSetup.GetType().FullName + "_" + GetType().FullName;

            NotificationSetup = notificationSetup;
            Notification = notification;
            _connection = new SqlConnection(connectionString);
            _options = options;
            _versionStorage = storage;

            if (!new TableFinder(_connection).Exist(NotificationSetup.Table, NotificationSetup.Schema))
                throw new TableNotExistException(NotificationSetup.Table, NotificationSetup.Schema);

            _timestampField = new TimestampFieldFinder(_connection).GetOrEmpty(NotificationSetup.Table, NotificationSetup.Schema);

            if (string.IsNullOrEmpty(_timestampField))
                throw new InvalidOperationException(string.Format("requested Table has no timestamp field (Table:{0} Schema:{1})", NotificationSetup.Table, NotificationSetup.Schema));


            ulong keyToStore = 0;

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrent)
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            _versionStorage.Store(_key, keyToStore);

            InitializeHashTable();

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

            var changedIds = _connection.ExecuteList<object>(statement, reader => maxTimestamp = Math.Max(maxTimestamp, Convert.ToUInt64(reader.GetInt64(1))))
                .ToList();
            NotifyDatabaseExecution();

            if (!changedIds.Any())
                return false;

            foreach (var entry in changedIds)
            {
                if (_lastseenIds.Contains(entry))
                {
                    Notification.OnUpdate(NotificationSetup, entry.ToString(), Enumerable.Empty<string>());
                }
                else
                {
                    Notification.OnInsert(NotificationSetup, entry.ToString(), Enumerable.Empty<string>());
                    _lastseenIds.Add(entry);
                }
            }

            _versionStorage.Store(_key, maxTimestamp);

            return true;
        }

        private ulong GetLastTimestamp()
        {
            NotifyDatabaseExecution();
            return (ulong)_connection.ExecuteSkalar<long>("SELECT CONVERT(bigint, @@dbts)");
        }

        private void InitializeHashTable()
        {
            string select = string.Format("SELECT {1} FROM [{0}].[{2}]", NotificationSetup.Schema, NotificationSetup.KeyColumn, NotificationSetup.Table);
            _lastseenIds.AddRange(_connection.ExecuteList<object>(select));
            NotifyDatabaseExecution();
        }

        private void NotifyDatabaseExecution()
        {
            if(PerformanceCounter != null)
                PerformanceCounter.Inc("DatabaseQuery");
        }
    }
}