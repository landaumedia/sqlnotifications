﻿using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using Krowiorsch.Dojo.Wire;

namespace LandauMedia.Tracker
{
    public class TimestampBasedTracker : ITracker
    {
        string _connectionString;
        string _timestampField;

        long _lastTimestamp;

        public INotification Notification { get; internal set; }

        public void TrackingChanges()
        {
            string statement = string.Format("SELECT {0} FROM [{1}] WHERE CONVERT(bigint, {2}) > {3}",
                Notification.KeyColumn,
                Notification.Table,
                _timestampField,
                _lastTimestamp);

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

            foreach(var entry in list)
            {
                Notification.OnUpdate(Notification, entry.ToString(), Enumerable.Empty<string>());
            }

            _lastTimestamp = GetLastTimestamp();
        }

        public void Prepare(string connectionString, INotification notification)
        {
            Notification = notification;
            _connectionString = connectionString;

            _timestampField = GetTimestampFieldOrNull();

            if (_timestampField == null)
                throw new InvalidOperationException("requested Table has no timestamp field");

            _lastTimestamp = GetLastTimestamp();
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

        private long GetLastTimestamp()
        {
            const string selectTimestamp = "SELECT CONVERT(bigint, @@dbts)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(selectTimestamp, connection))
            {
                connection.Open();
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    return reader.GetInt64(0);
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