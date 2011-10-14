using System.Data.SqlClient;

namespace LandauMedia.Storage.SqlTasks
{
    public class SqlTasksBase
    {
        readonly SqlConnection _connection;

        public SqlTasksBase(SqlConnection connection)
        {
            _connection = connection;
        }

        protected void ExecuteCommand(string statement)
        {
            using (SqlCommand command = new SqlCommand(statement, _connection))
            {
                command.ExecuteNonQuery();
            }
        } 
    }
}