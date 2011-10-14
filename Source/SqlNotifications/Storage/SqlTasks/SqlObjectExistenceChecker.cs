using System.Data.SqlClient;

namespace LandauMedia.Storage.SqlTasks
{
    public class SqlObjectExistenceChecker
    {
        readonly SqlConnection _connection;

        public SqlObjectExistenceChecker(SqlConnection connection)
        {
            _connection = connection;
        }

        public bool ExistSchema(string schemaName)
        {
            const string selectSchema = @"SELECT Count(*) FROM [INFORMATION_SCHEMA].[SCHEMATA]  WHERE SCHEMA_NAME = @SchemaName";

            using (SqlCommand command = new SqlCommand(selectSchema, _connection))
            {
                command.Parameters.AddWithValue("@SchemaName", schemaName);
                return (int)command.ExecuteScalar() == 1;
            }
        }

        public bool ExistTable(string tableName, string schemaName)
        {
            const string selectSchema = @"SELECT Count(*) FROM [INFORMATION_SCHEMA].[TABLES]  
                WHERE [TABLE_SCHEMA] = @schemaName and [TABLE_NAME] = @tableName";

            using (SqlCommand command = new SqlCommand(selectSchema, _connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                command.Parameters.AddWithValue("@schemaName", schemaName);
                return (int)command.ExecuteScalar() == 1;
            }
        }
    }
}