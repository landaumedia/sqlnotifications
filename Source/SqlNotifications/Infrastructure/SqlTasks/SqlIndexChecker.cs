using System.Data.SqlClient;

namespace LandauMedia.Infrastructure.SqlTasks
{
    public class SqlIndexChecker
    {
        readonly SqlConnection _connection;

        public SqlIndexChecker(SqlConnection connection)
        {
            _connection = connection;
        }

        public bool Exists(string schemaName, string tablename, string columnName)
        {
            if (string.IsNullOrEmpty(schemaName))
                schemaName = "dbo";

            using (var command = new SqlCommand(BuildStatement(), _connection))
            {
                command.Parameters.AddWithValue("@columnName", columnName);
                command.Parameters.AddWithValue("@tablename", tablename);
                command.Parameters.AddWithValue("@schemaName", schemaName);

                _connection.EnsureIsOpen();

                return ((int)command.ExecuteScalar()) > 0;
            }
        }

        static string BuildStatement()
        {
            return @"select Count(*)
            from sys.indexes i 
                join sys.objects o on i.object_id = o.object_id
                join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
                join sys.columns co on co.object_id = i.object_id and co.column_id = ic.column_id
                join sys.schemas s on s.schema_id = o.schema_id
            where i.[type] = 2 
            and i.is_primary_key = 0
            and o.[type] = 'U'
            and co.name = @columnName
            and o.name = @tableName
            and s.name = @schemaName;";
        }
    }
}