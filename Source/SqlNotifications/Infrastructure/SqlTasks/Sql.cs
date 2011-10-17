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
            connection.EnsureIsOpen();

            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void EnsureIsOpen(this SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public static void TryClose(this SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public static T ExecuteSkalar<T>(this SqlConnection connection, string statement)
        {
            return new SqlTasksBase(connection).SkalarRead<T>(statement);
        }

        public static IEnumerable<T> ExecuteList<T>(this SqlConnection connection, string statement)
        {
            return new SqlTasksBase(connection).ListRead<T>(statement);
        }
        
    }
}