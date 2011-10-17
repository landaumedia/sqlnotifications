using System;
using System.Data.SqlClient;
using LandauMedia.Infrastructure.SqlTasks;

namespace LandauMedia.Tracker.TimestampBased
{
    public class TimestampFieldFinder
    {
        readonly SqlConnection _connection;

        public TimestampFieldFinder(SqlConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");
            _connection = connection;
        }

        public string GetOrEmpty(string tableName, string schemaName)
        {
            _connection.EnsureIsOpen();

            string existTimestampField = @"SELECT Column_Name FROM INFORMATION_SCHEMA.COLUMNS
                WHERE DATA_TYPE = 'timestamp' AND TABLE_SCHEMA='@Schema' and TABLE_NAME='@TableName'";

            existTimestampField = existTimestampField.Replace("@TableName", tableName);
            existTimestampField = existTimestampField.Replace("@Schema", schemaName);

            return _connection.ExecuteSkalar<string>(existTimestampField);
        }
    }
}