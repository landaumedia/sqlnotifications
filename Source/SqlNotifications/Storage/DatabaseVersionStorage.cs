using System;
using System.Data.SqlClient;
using LandauMedia.Infrastructure.SqlTasks;

namespace LandauMedia.Storage
{
    public class DatabaseVersionStorage : IVersionStorage, IDisposable
    {
        readonly string _connectionString;
        readonly string _tableName;
        readonly string _schemaName;

        SqlConnection _database;

        public DatabaseVersionStorage(string connectionString, string tableName = "Versions", string schemaName = "Management")
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _schemaName = schemaName;

            Prepare();
        }

        void Prepare()
        {
            _database = new SqlConnection(_connectionString);

            _database.Open();

            if (!_database.HasSchema(_schemaName))
            {
                _database.CreateSchema(_schemaName);
            }

            if (!_database.HasTable(_tableName, _schemaName))
            {
                _database.CreateVersionTable(_schemaName, _tableName);
            }
        }

        public void Store(string key, ulong version)
        {
            if (ExistKey(key))
            {
                UpdateKey(key, version);
            }
            else
            {
                InsertKey(key, version);
            }
        }

        public ulong Load(string key)
        {
            return ReadVersionFormKey(key);
        }


        private bool ExistKey(string key)
        {
            string statement = string.Format("SELECT Count(*) FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            using (SqlCommand command = new SqlCommand(statement, _database))
            {
                command.Parameters.AddWithValue("@Key", key);
                return (int)command.ExecuteScalar() == 1;
            }
        }

        private void UpdateKey(string key, ulong version)
        {
            string statement = string.Format("UPDATE [{0}].[{1}] SET version=@version WHERE [Key]=@key", _schemaName, _tableName);

            using (SqlCommand command = new SqlCommand(statement, _database))
            {
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@version", (long)version);
                command.ExecuteNonQuery();
            }
        }

        private void InsertKey(string key, ulong version)
        {
            string statement = string.Format("INSERT INTO [{0}].[{1}]([Key], Version) VALUES(@key, @version)", _schemaName, _tableName);

            using (SqlCommand command = new SqlCommand(statement, _database))
            {
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@version", (long)version);
                command.ExecuteNonQuery();
            }
        }

        private ulong ReadVersionFormKey(string key)
        {
            string statement = string.Format("SELECT Version FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            using (SqlCommand command = new SqlCommand(statement, _database))
            {
                command.Parameters.AddWithValue("@Key", key);

                object result = command.ExecuteScalar();
                return result == null ? 0 : Convert.ToUInt64((long)result);
            }
        }

        public void Dispose()
        {
            _database.Close();
        }
    }
}