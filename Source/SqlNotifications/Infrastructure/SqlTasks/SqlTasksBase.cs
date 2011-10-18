using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LandauMedia.Infrastructure.SqlTasks
{
    internal class SqlTasksBase
    {
        readonly SqlConnection _connection;

        public SqlTasksBase(SqlConnection connection)
        {
            _connection = connection;
        }

        protected void ExecuteCommand(string statement)
        {
            _connection.EnsureIsOpen();

            using (SqlCommand command = new SqlCommand(statement, _connection))
            {
                command.ExecuteNonQuery();
            }
        } 

        public T SkalarRead<T>(string statement)
        {
            _connection.EnsureIsOpen();

            using (SqlCommand command = new SqlCommand(statement, _connection))
            {
                return (T)command.ExecuteScalar();
            }
        }

        public IEnumerable<T> ListRead<T>(string statement)
        {
            _connection.EnsureIsOpen();

            using (SqlCommand command = new SqlCommand(statement, _connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    yield return (T)ReadFromReader(reader);
                }
            }
        }

        public IEnumerable<T> ListRead<T>(string statement, Action<IDataRecord> onRead)
        {
            _connection.EnsureIsOpen();

            using (SqlCommand command = new SqlCommand(statement, _connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (onRead != null)
                        onRead(reader);
                    yield return (T)ReadFromReader(reader);
                }
            }
        }

        private static object ReadFromReader(IDataRecord reader)
        {
            Type t = reader.GetFieldType(0);

            if (t == typeof(string))
                return reader.GetString(0);

            if (t == typeof(int))
                return reader.GetInt32(0);

            if (t == typeof(Guid))
                return reader.GetGuid(0);

            if (t == typeof(long))
                return reader.GetInt64(0);

            if (t == typeof(byte))
                return reader.GetInt16(0);

            if (t == typeof(decimal))
                return reader.GetDecimal(0);

            if (t == typeof(DateTime))
                return reader.GetDateTime(0);

            if (t == typeof(double))
                return reader.GetDouble(0);

            if (t == typeof(float))
                return reader.GetFloat(0);

            throw new ArgumentOutOfRangeException();
        }
    }
}