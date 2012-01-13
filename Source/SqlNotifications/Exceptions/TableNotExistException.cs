using System;
using System.Data;
using System.Runtime.Serialization;

namespace LandauMedia.Exceptions
{
    /// <summary>
    /// Thrown, if the requested Table not exist on Database
    /// </summary>
    public class TableNotExistException : DataException
    {
        protected TableNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TableNotExistException(string tableName, string schemaName)
        {
            SchemaName = schemaName;
            TableName = tableName;
        }

        public TableNotExistException(string s)
            : base(s)
        {
        }

        public TableNotExistException(string s, Exception innerException)
            : base(s, innerException)
        {
        }

        public TableNotExistException()
        {
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}