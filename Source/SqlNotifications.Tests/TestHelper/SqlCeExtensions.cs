using System.Data.SqlServerCe;

namespace LandauMedia.TestHelper
{
    public static class SqlCeExtensions
    {
        public static int ExecuteCommand(this SqlCeConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteNonQuery();
        }
    }
}