using System;
using System.Data.SqlClient;
using LandauMedia.Infrastructure.SqlTasks;

namespace LandauMedia.GenericStorage
{
    public class DatabaseGenericVersionStorage : IGenericVersionStorage, IDisposable
    {
        readonly string _connectionString;
        readonly string _tableName;
        readonly string _schemaName;
        readonly string _keyPrefix;
        SqlConnection _connection;
        static readonly object Synclock = new object();

        public DatabaseGenericVersionStorage(string connectionString,
            string schemaName = "Management",
            string tableName = "GenericVersions",
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
            _connection = new SqlConnection(_connectionString);
            _connection.Open();

            if (!_connection.HasSchema(_schemaName))
            {
                _connection.CreateSchema(_schemaName);
            }

            if (!_connection.HasTable(_tableName, _schemaName))
            {
                _connection.CreateVersionTable(_schemaName, _tableName);
            }
        }

        public void Store(string key, string version)
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

        public string Load(string key)
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
            var statement = string.Format("TRUNCATE TABLE [{0}].[{1}]", _schemaName, _tableName);
            using (var command = new SqlCommand(statement, _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// check for Key in Database
        /// </summary>
        /// <param name="key">the Key that should be checked</param>
        /// <returns><c>true</c> if exist, otherwise <c>false</c></returns>
        bool ExistKey(string key)
        {
            var statement = string.Format("SELECT Count(*) FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _connection))
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
        void UpdateKey(string key, string version)
        {
            var statement = string.Format("UPDATE [{0}].[{1}] SET version=@version WHERE [Key]=@key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _connection))
                {
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@version", version);
                    command.ExecuteNonQuery();
                }
            }
        }

        void InsertKey(string key, string version)
        {
            var statement = string.Format("INSERT INTO [{0}].[{1}]([Key], Version) VALUES(@key, @version)", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _connection))
                {
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@version", version);
                    command.ExecuteNonQuery();
                }
            }
        }

        string AddPrefixToKey(string key)
        {
            if (string.IsNullOrEmpty(_keyPrefix))
                return key;

            return _keyPrefix + "_" + key;
        }

        string ReadVersionFormKey(string key)
        {
            var statement = string.Format("SELECT Version FROM [{0}].[{1}] WHERE [Key]=@Key", _schemaName, _tableName);

            lock (Synclock)
            {
                using (SqlCommand command = new SqlCommand(statement, _connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    var result = command.ExecuteScalar();
                    return result == null ? null : result.ToString();
                }
            }
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}