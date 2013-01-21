//using System.Data.SqlClient;
//using LandauMedia.Infrastructure.SqlTasks;

//namespace LandauMedia.State
//{
//    public class DatabaseStateHolder : IHoldStates
//    {
//        readonly string _connectionString;
//        readonly string _tableName = "State";
//        readonly string _schemaName = "Management";

//        // Structure of Table (Id nvarchar(50), stateType nvarchar(100), state nvarchar(MAX)

//        public DatabaseStateHolder(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public DatabaseStateHolder(string connectionString, string tableName, string schemaName)
//        {
//            _connectionString = connectionString;
//            _tableName = tableName;
//            _schemaName = schemaName;
//        }

//        public void Put(string id, State state)
//        {
//            var putData = 

//            using (var connection = new SqlConnection(_connectionString))
//            {
//                connection.Open();
//                EnsureTableExists(connection);

//                using(SqlCommand command = new)
//                {
                    
//                }

//            }
//        }

//        public State GetForTypeOrNull(string id, string type)
//        {
//            var selectData = string.Format(@"Select Id, StateType, State FROM {0}.{1} WHERE Id = @Id AND stateType = @StateType", _schemaName, _tableName);

//            using (var connection = new SqlConnection(_connectionString))
//            using (var selectCommand = new SqlCommand(selectData))
//            {
//                connection.Open();
//                selectCommand.Parameters.AddWithValue("@Id", id);
//                selectCommand.Parameters.AddWithValue("@StateType", type);

//                using (var reader = selectCommand.ExecuteReader())
//                {

//                }
//            }
//        }

//        void EnsureTableExists(SqlConnection connection)
//        {
//            if (connection.HasTable(_tableName, _schemaName))
//                return;

//            connection.ExecuteCommand(GetTableCreationStatement(_schemaName, _tableName));
//            connection.ExecuteCommand(PrimaryKeyStatement(_schemaName, _tableName));
//        }

//        static string GetTableCreationStatement(string schema, string table)
//        {
//            return string.Format(@"CREATE TABLE {0}.{1}
//	            (Id nvarchar(50) NOT NULL,
//                StateType nvarchar(100) NOT NULL,
//                State nvarchar(MAX) NOT NULL)  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", schema, table);
//        }

//        static string PrimaryKeyStatement(string schema, string table)
//        {
//            return string.Format(@"ALTER TABLE {0}.{1} ADD CONSTRAINT
//	                PK_{1} PRIMARY KEY CLUSTERED (Id) 
//                    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]", schema, table);
//        }
//    }
//}