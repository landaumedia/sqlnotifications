using System.Data.SqlClient;

namespace LandauMedia.Storage.SqlTasks
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
            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}