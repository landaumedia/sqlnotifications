using System.Data.SqlClient;

namespace LandauMedia.Storage.SqlTasks
{
    public class SqlObjectCreator : SqlTasksBase
    {
        public SqlObjectCreator(SqlConnection connection)
            : base(connection)
        {
        }

        public void CreateSchema(string schemaName)
        {
            ExecuteCommand(string.Format("CREATE Schema [{0}]", schemaName));
        }

        public void CreateVersionTable(string tableName, string schemaName)
        {
            string commandCreateTable = string.Format(@"CREATE TABLE [{0}].[{1}] 
                ([Key] nvarchar(200) NOT NULL,	Version bigint NOT NULL)  ON [PRIMARY]", schemaName, tableName);
            string commandAddPrimaryKey = string.Format(@"ALTER TABLE [{0}].[{1}] 
                ADD CONSTRAINT PK_{0}_{1} PRIMARY KEY CLUSTERED ([Key]) ON [PRIMARY]", schemaName, tableName);

            ExecuteCommand(commandCreateTable);
            ExecuteCommand(commandAddPrimaryKey);
        }
    }
}