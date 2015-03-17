using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LandauMedia.Infrastructure.SqlTasks
{
    public static class Sql
    {
        public static bool HasSchema(this SqlConnection connection, string schemaName)
        {
            return new SqlObjectExistenceChecker(connection).ExistSchema(schemaName);
        }

        public static void CreateSchema(this SqlConnection connection, string schemaName)
        {
            new SqlObjectCreator(connection).CreateSchema(schemaName);
        }

        public static bool HasTable(this SqlConnection connection, string tableName, string schemaName)
        {
            return new SqlObjectExistenceChecker(connection).ExistTable(tableName, schemaName);
        }

        public static void CreateVersionTable(this SqlConnection connection, string schemaName, string tableName)
        {
            new SqlObjectCreator(connection).CreateVersionTable(tableName, schemaName);
        }

        public static void ExecuteCommand(this SqlConnection connection, string statement)
        {
            new SqlTasksBase(connection, TimeSpan.FromSeconds(15)).ExecuteCommand(statement);
        }

        public static void ExecuteCommand(this SqlConnection connection, string statement, TimeSpan commandTimeout)
        {
            new SqlTasksBase(connection, commandTimeout).ExecuteCommand(statement);
        }

        public static void EnsureIsOpen(this SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public static void TryClose(this SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public static T ExecuteSkalar<T>(this SqlConnection connection, string statement, TimeSpan commandTimeout)
        {
            return new SqlTasksBase(connection, commandTimeout).SkalarRead<T>(statement);
        }

        public static IEnumerable<T> ExecuteList<T>(this SqlConnection connection, string statement, TimeSpan commandTimeout)
        {
            return new SqlTasksBase(connection, commandTimeout).ListRead<T>(statement);
        }

        public static IEnumerable<T> ExecuteList<T>(this SqlConnection connection, string statement, Action<IDataRecord> onRead, TimeSpan commandTimeout)
        {
            return new SqlTasksBase(connection, commandTimeout).ListRead<T>(statement, onRead);
        }


        public static object ReadFromReader(this IDataRecord reader, int ordinal)
        {
            var t = reader.GetFieldType(ordinal);

            if (reader.IsDBNull(ordinal))
            {
                if (t == typeof(DateTime))
                    return null;

                return t.IsValueType ? Activator.CreateInstance(t) : null;
            }

            if (t == typeof(bool))
                return reader.GetBoolean(ordinal);

            if (t == typeof(string))
                return reader.GetString(ordinal);

            if (t == typeof(int))
                return reader.GetInt32(ordinal);

            if (t == typeof(Guid))
                return reader.GetGuid(ordinal);

            if (t == typeof(long))
                return reader.GetInt64(ordinal);

            if (t == typeof(byte))
                return reader.GetInt16(ordinal);

            if (t == typeof(decimal))
                return reader.GetDecimal(ordinal);

            if (t == typeof(DateTime))
                return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);

            if (t == typeof(double))
                return reader.GetDouble(ordinal);

            if (t == typeof(float))
                return reader.GetFloat(ordinal);

            throw new ArgumentOutOfRangeException();
        }
    }
}