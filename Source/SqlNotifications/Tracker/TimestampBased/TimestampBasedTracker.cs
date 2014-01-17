using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using LandauMedia.Exceptions;
using LandauMedia.Infrastructure;
using LandauMedia.Infrastructure.SqlTasks;
using LandauMedia.Model;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;
using Logger = NLog.Logger;

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

        readonly ILookupTable _lookup = new SortedArrayLookupTable();

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

                    // set throttling for Starup and big batches
                    Thread.Sleep(_options.Throttling);
                }
                catch (SqlException sqlException)
                {
                    // on error log a warning and 
                    Logger.WarnException("Error on Tracking Database", sqlException);
                }
            }
        }

        /// <exception cref="TableNotExistException">Wird geworfen, wenn die die definierte Tabelle nicht existiert</exception>
        /// <exception cref="InvalidOperationException">Wird geworfen, wenn die Tabelle kein Timestampfeld besitzt</exception>
        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions options)
        {
            Logger.Debug(() => string.Format("Preparing timestampbased Notification with Options: InitOptions:{0}", options.InitializationOptions));

            NotificationSetup = notificationSetup;
            Notification = notification;

            _key = string.IsNullOrEmpty(NotificationSetup.NotificationKey)
                ? notificationSetup.GetType().FullName + "_" + GetType().FullName
                : NotificationSetup.NotificationKey;

            _connection = new SqlConnection(connectionString);
            _options = options;
            _versionStorage = storage;

            if (!new TableFinder(_connection).Exist(NotificationSetup.Table, NotificationSetup.Schema))
                throw new TableNotExistException(NotificationSetup.Table, NotificationSetup.Schema);

            _timestampField = new TimestampFieldFinder(_connection).GetOrEmpty(NotificationSetup.Table, NotificationSetup.Schema);

            if (string.IsNullOrEmpty(_timestampField))
                throw new InvalidOperationException(string.Format("requested Table has no timestamp field (Table:{0} Schema:{1})", NotificationSetup.Table, NotificationSetup.Schema));

            // check for Index on Timestamp
            if (!new SqlIndexChecker(_connection).Exists(NotificationSetup.Schema, NotificationSetup.Table, _timestampField))
                Logger.Warn("The timestamp-Field has no index - this result in bad performance");

            ulong keyToStore = 0;

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrent)
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            if (_options.InitializationOptions == InitializationOptions.InitializeToZeroIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = 0;

            if (_options.InitializationOptions == InitializationOptions.InitializeToZeroIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            _versionStorage.Store(_key, keyToStore);

            InitializeHashTable(keyToStore);

            Logger.Debug(() => "Finished Prepare for timestampbased Notification");
        }

        private bool TrackChangesForOneBucket(int bucketSize)
        {
            var fromTimestamp = _versionStorage.Load(_key);
            var toTimestamp = GetLastTimestamp();

            var maxTimestamp = ulong.MinValue;

            var statement = BuildTrackerStatement(bucketSize, fromTimestamp, toTimestamp);

            IDictionary<object, IDictionary<string, object>> addionalData = new Dictionary<object, IDictionary<string, object>>();

            var changedIds = _connection.ExecuteList<object>(statement,
                    reader =>
                    {
                        maxTimestamp = Math.Max(maxTimestamp, Convert.ToUInt64(reader.GetInt64(1)));
                        addionalData.Add(reader.GetValue(0), ExtractAddionalData(reader, Convert.ToUInt64(reader.GetInt64(1))));
                    }, _options.SqlCommandTimeout)
                .ToList();
            NotifyDatabaseExecution();

            if (!changedIds.Any())
                return false;

            foreach (var entry in changedIds)
            {

                if (_lookup.Contains(entry))
                {
                    // update was found
                    Notification.OnUpdate(NotificationSetup, entry.ToString(), new AditionalNotificationInformation
                    {
                        AdditionalColumns = addionalData[entry],
                        Rowversion = ulong.Parse(addionalData[entry]["RowVersion"].ToString())
                    });
                }
                else
                {
                    // insert was found
                    Notification.OnInsert(NotificationSetup, entry.ToString(), new AditionalNotificationInformation
                    {
                        AdditionalColumns = addionalData[entry],
                        Rowversion = ulong.Parse(addionalData[entry]["RowVersion"].ToString())
                    });
                    _lookup.Add(entry);
                }
            }

            _versionStorage.Store(_key, maxTimestamp);

            return true;
        }

        IDictionary<string, object> ExtractAddionalData(IDataRecord reader, ulong rowversion)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();

            foreach (var column in NotificationSetup.AdditionalColumns)
            {
                data.Add(column, reader.ReadFromReader(reader.GetOrdinal(column)));
            }

            data.Add("RowVersion", rowversion.ToString(CultureInfo.InvariantCulture));

            return data;
        }

        string BuildTrackerStatement(int bucketSize, ulong fromTimestamp, ulong toTimestamp)
        {
            string additionalColumnsStatement = string.Empty;

            if (NotificationSetup.AdditionalColumns.Any())
            {
                additionalColumnsStatement = string.Join(",", NotificationSetup.AdditionalColumns);
                additionalColumnsStatement = "," + additionalColumnsStatement;
            }

            string customWhereStatement = string.Empty;

            if (!string.IsNullOrEmpty(NotificationSetup.CustomWhereStatement))
            {
                customWhereStatement = " AND (" + NotificationSetup.CustomWhereStatement + ") ";
            }

            string statement = string.Format("SELECT TOP {6} {0}, Convert(bigint,{3}) {7} FROM [{1}].[{2}] WHERE CONVERT(bigint, {3}) > {4} AND CONVERT(bigint, {3}) <= {5} {8} ORDER BY {3} ASC ",
                NotificationSetup.KeyColumn,
                NotificationSetup.Schema,
                NotificationSetup.Table,
                _timestampField,
                fromTimestamp,
                toTimestamp,
                bucketSize,
                additionalColumnsStatement,
                customWhereStatement);

            return statement;
        }

        private ulong GetLastTimestamp()
        {
            NotifyDatabaseExecution();
            return (ulong)_connection.ExecuteSkalar<long>("SELECT CONVERT(bigint, @@dbts)", TimeSpan.FromSeconds(15));
        }

        private void InitializeHashTable(ulong intializeToRowVersion)
        {
            string select = string.Format("SELECT {1} FROM [{0}].[{2}] WHERE CONVERT(bigint, {3}) <= {4} ",
                NotificationSetup.Schema,
                NotificationSetup.KeyColumn,
                NotificationSetup.Table,
                _timestampField,
                intializeToRowVersion);

            // read Data with payload
            var keys = _connection.ExecuteList<object>(select, _options.SqlCommandTimeout);

            _lookup.AddRange(keys);

            NotifyDatabaseExecution();
        }

        private void NotifyDatabaseExecution()
        {
            if (PerformanceCounter != null)
                PerformanceCounter.Inc("DatabaseQuery");
        }
    }
}