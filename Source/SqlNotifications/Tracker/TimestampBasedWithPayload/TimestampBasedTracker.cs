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
using LandauMedia.Tracker.TimestampBased;
using LandauMedia.Wire;
using NLog;

namespace LandauMedia.Tracker.TimestampBasedWithPayload
{
    public class TimestampBasedWithPayloadTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        SqlConnection _connection;
        string _timestampField;

        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

        readonly ILookupTableWithPayload _lookupWithPayload = new HashbasedLookupWithPayload();

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

        public void Prepare(string connectionString, INotificationSetup notificationSetup, INotification notification, IVersionStorage storage, TrackerOptions options)
        {
            Logger.Debug(() => string.Format("Preparing timestampbased Notification with Options: InitOptions:{0}", options.InitializationOptions));

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
                    })
                .ToList();
            NotifyDatabaseExecution();

            if (!changedIds.Any())
                return false;

            foreach (var entry in changedIds)
            {

                if (_lookupWithPayload.Contains(entry))
                {
                    // update was found
                    Notification.OnUpdate(NotificationSetup, entry.ToString(), new AditionalNotificationInformation
                    {
                        AdditionalColumns = addionalData[entry],
                        Rowversion = ulong.Parse(addionalData[entry]["RowVersion"].ToString()),
                        ColumnOldValue = ExtractOldValues(_lookupWithPayload.GetPayload(entry))
                    });

                    // update Payload
                    UpdatePayLoadFromAddionalData(entry, addionalData[entry]);
                }
                else
                {
                    // insert was found
                    Notification.OnInsert(NotificationSetup, entry.ToString(), new AditionalNotificationInformation
                    {
                        AdditionalColumns = addionalData[entry],
                        Rowversion = ulong.Parse(addionalData[entry]["RowVersion"].ToString())
                    });

                    // update Payload
                    UpdatePayLoadFromAddionalData(entry, addionalData[entry]);
                }
            }

            _versionStorage.Store(_key, maxTimestamp);

            return true;
        }

        void UpdatePayLoadFromAddionalData(object entry, IDictionary<string, object> addionalData)
        {
            object[] payload = new object[NotificationSetup.IntrestedInUpdatedColums.Count()];

            for (int i = 0; i < NotificationSetup.IntrestedInUpdatedColums.Count(); i++)
            {
                payload[i] = addionalData[NotificationSetup.IntrestedInUpdatedColums.ElementAt(i)];
            }

            _lookupWithPayload.SetPayload(entry, payload);
        }

        IDictionary<string, object> ExtractOldValues(object[] payload)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            for (int i = 0; i < NotificationSetup.IntrestedInUpdatedColums.Count(); i++)
            {
                result.Add(NotificationSetup.IntrestedInUpdatedColums.ElementAt(i), payload[i]);
            }

            return result;
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

            string statement = string.Format("SELECT TOP {6} {0}, Convert(bigint,{3}) {7} FROM [{1}].[{2}] WHERE CONVERT(bigint, {3}) > {4} AND CONVERT(bigint, {3}) <= {5} ORDER BY {3} ASC ",
                NotificationSetup.KeyColumn,
                NotificationSetup.Schema,
                NotificationSetup.Table,
                _timestampField,
                fromTimestamp,
                toTimestamp,
                bucketSize,
                additionalColumnsStatement);

            return statement;
        }

        private ulong GetLastTimestamp()
        {
            NotifyDatabaseExecution();
            return (ulong)_connection.ExecuteSkalar<long>("SELECT CONVERT(bigint, @@dbts)");
        }

        private void InitializeHashTable(ulong intializeToRowVersion)
        {
            var addionalColumns = NotificationSetup.IntrestedInUpdatedColums.Aggregate(string.Empty, (s, s1) => s + "," + s1);

            string select = string.Format("SELECT {1} {5} FROM [{0}].[{2}] WHERE CONVERT(bigint, {3}) <= {4} ",
                NotificationSetup.Schema,
                NotificationSetup.KeyColumn,
                NotificationSetup.Table,
                _timestampField,
                intializeToRowVersion,
                addionalColumns);

            // read Data with payload
            IDictionary<object, object[]> entries = new Dictionary<object, object[]>();
            var keys = _connection.ExecuteList<object>(select, record =>
            {
                object[] payload = new object[NotificationSetup.IntrestedInUpdatedColums.Count()];

                for (int i = 0; i < NotificationSetup.IntrestedInUpdatedColums.Count(); i++)
                {
                    payload[i] = record.ReadFromReader(i + 1);
                }

                entries.Add(record.ReadFromReader(0), payload);
            }).ToList();

            _lookupWithPayload.AddRange(entries.Keys, entries);
            NotifyDatabaseExecution();
        }

        private void NotifyDatabaseExecution()
        {
            if (PerformanceCounter != null)
                PerformanceCounter.Inc("DatabaseQuery");
        }
    }
}