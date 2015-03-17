using System;
using System.Data.SqlClient;

namespace LandauMedia.Infrastructure.SqlTasks
{
    /// <summary> checks if the table is in the databaseschema </summary>
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

            var existTableStatement = @"SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA='@Schema' AND TABLE_NAME='@TableName'";

            existTableStatement = existTableStatement.Replace("@TableName", tableName);
            existTableStatement = existTableStatement.Replace("@Schema", schemaName);

            return _connection.ExecuteSkalar<int>(existTableStatement, TimeSpan.FromSeconds(15)) == 1;
        }
    }
}