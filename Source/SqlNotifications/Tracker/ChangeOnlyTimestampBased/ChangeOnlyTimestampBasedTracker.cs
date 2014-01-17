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

namespace LandauMedia.Tracker.ChangeOnlyTimestampBased
{
    public class ChangeOnlyTimestampBasedTracker : ITracker
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        SqlConnection _connection;
        string _timestampField;

        IVersionStorage _versionStorage;
        TrackerOptions _options;
        string _key;

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
            Logger.Debug(() => "Preparing changetimestampbased Notification");

            _key = string.IsNullOrEmpty(notificationSetup.NotificationKey)
                ? notificationSetup.GetType().FullName + "_" + GetType().FullName
                : notificationSetup.NotificationKey;

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

            // check for Index on Timestamp
            if (!new SqlIndexChecker(_connection).Exists(NotificationSetup.Schema, NotificationSetup.Table, _timestampField))
                Logger.Warn("The timestamp-Field has no index - this will result in bad performance (Field:{0} Table:{1})", _timestampField, NotificationSetup.Table);

            ulong keyToStore = 0;

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrent)
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = GetLastTimestamp();

            if (_options.InitializationOptions == InitializationOptions.InitializeToCurrentIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            if (_options.InitializationOptions == InitializationOptions.InitializeToZeroIfNotSet && _versionStorage.Exist(_key))
                keyToStore = _versionStorage.Load(_key);

            if (_options.InitializationOptions == InitializationOptions.InitializeToZeroIfNotSet && !_versionStorage.Exist(_key))
                keyToStore = 0;

            _versionStorage.Store(_key, keyToStore);

            Logger.Debug(() => "Finished Prepare for timestampbased Notification");
        }

        private bool TrackChangesForOneBucket(int bucketSize)
        {
            var fromTimestamp = _versionStorage.Load(_key);
            var toTimestamp = GetLastTimestamp();

            var maxTimestamp = ulong.MinValue;

            var statement = BuildTrackerStatement(bucketSize, fromTimestamp, toTimestamp);

            IDictionary<object, IDictionary<string, object>> additionalData = new Dictionary<object, IDictionary<string, object>>();
            var changedIds = _connection.ExecuteList<object>(statement,
                reader =>
                {
                    maxTimestamp = Math.Max(maxTimestamp, Convert.ToUInt64(reader.GetInt64(1)));
                    additionalData.Add(reader.GetValue(0), ExtractAddionalData(reader, Convert.ToUInt64(reader.GetInt64(1))));
                }, 
                _options.SqlCommandTimeout).ToList();

            NotifyDatabaseExecution();

            if (!changedIds.Any())
                return false;

            changedIds.ForEach(entry =>
                Notification.OnUpdate(NotificationSetup,
                entry.ToString(),
                new AditionalNotificationInformation
                {
                    AdditionalColumns = additionalData[entry],
                    Rowversion = ulong.Parse(additionalData[entry]["RowVersion"].ToString())
                }));

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
                customWhereStatement = " AND (" + NotificationSetup.CustomWhereStatement + ")";
            }

            string statement = string.Format("SELECT TOP {6} {0}, Convert(bigint,{3}) {7} FROM [{1}].[{2}] WITH (UPDLOCK) WHERE CONVERT(bigint, {3}) > {4} AND CONVERT(bigint, {3}) <= {5} {8} ORDER BY {3} ASC ",
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
            return (ulong)_connection.ExecuteSkalar<long>("SELECT CONVERT(bigint, MIN_ACTIVE_ROWVERSION())", TimeSpan.FromSeconds(15)) - 1;
        }

        private void NotifyDatabaseExecution()
        {
            if (PerformanceCounter != null)
                PerformanceCounter.Inc("DatabaseQuery");
        }
    }
}