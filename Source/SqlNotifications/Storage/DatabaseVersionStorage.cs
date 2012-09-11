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

        readonly string _keyPrefix;

        SqlConnection _database;

        static readonly object Synclock = new object();

        public DatabaseVersionStorage(string connectionString,
            string tableName = "Versions",
            string schemaName = "Management",
            string keyPrefix = "")
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _schemaName = schemaName;
            _keyPrefix = keyPrefix;

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
            key = AddPrefixToKey(key);

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
            key = AddPrefixToKey(key);

            return ReadVersionFormKey(key);
        }

        public bool Exist(string key)
        {
            key = AddPrefixToKey(key);

            return ExistKey(key);
        }

        public void Reset()
        {
            string statement = string.Format("TRUNCATE TABLE [{0}].[{1}]", _schemaName, _tableName);
            using (SqlCommand command = new SqlCommand(statement, _database))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// check for Key in Database
        /// </summary>
        /// <param name="key">the Key that should be checked</param>
        /// <returns><c>true</c> if exist, otherwise <c>false</c></returns>
        private bool ExistKey(string key)
        {
            // checking with count
            string statement = string.Format("SELECT Count(*) FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _database))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    return (int)command.ExecuteScalar() == 1;
                }
            }
        }

        /// <summary>
        /// Updates the <paramref name="version"/> in the Database
        /// </summary>
        /// <param name="key">the Key that should be updated.</param>
        /// <param name="version">value for version </param>
        private void UpdateKey(string key, ulong version)
        {
            string statement = string.Format("UPDATE [{0}].[{1}] SET version=@version WHERE [Key]=@key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _database))
                {
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@version", (long)version);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertKey(string key, ulong version)
        {
            string statement = string.Format("INSERT INTO [{0}].[{1}]([Key], Version) VALUES(@key, @version)", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _database))
                {
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@version", (long)version);
                    command.ExecuteNonQuery();
                }
            }
        }

        private string AddPrefixToKey(string key)
        {
            if (string.IsNullOrEmpty(_keyPrefix))
                return key;

            return _keyPrefix + "_" + key;
        }

        private ulong ReadVersionFormKey(string key)
        {
            string statement = string.Format("SELECT Version FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _database))
                {
                    command.Parameters.AddWithValue("@Key", key);

                    object result = command.ExecuteScalar();
                    return result == null ? 0 : Convert.ToUInt64((long)result);
                }
            }
        }

        public void Dispose()
        {
            _database.Close();
        }
    }
}