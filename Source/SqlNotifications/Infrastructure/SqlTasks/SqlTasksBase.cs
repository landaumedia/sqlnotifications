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
                    yield return (T)reader.GetValue(0);
                }
            }
        }

    }
}