using System.Data;

namespace LandauMedia.Exceptions
{
    public class TableNotExistException : DataException
    {
        public TableNotExistException(string tableName, string schemaName)
        {
            SchemaName = schemaName;
            TableName = tableName;
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}