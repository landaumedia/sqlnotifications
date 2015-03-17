using System;
using System.Data.SqlClient;
using LandauMedia.Infrastructure.SqlTasks;

namespace LandauMedia.Tracker.TimestampBased
{
    internal class TableFinder
    {
        readonly SqlConnection _connection;

        public TableFinder(SqlConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");
            _connection = connection;
        }

        public bool Exist(string tableName, string schemaName)
        {
            _connection.EnsureIsOpen();

            var existTimestampField = @"SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA='@Schema' AND TABLE_NAME='@TableName'";

            existTimestampField = existTimestampField.Replace("@TableName", tableName);
            existTimestampField = existTimestampField.Replace("@Schema", schemaName);

            return _connection.ExecuteSkalar<int>(existTimestampField, TimeSpan.FromSeconds(15)) == 1;
        }
    }
}