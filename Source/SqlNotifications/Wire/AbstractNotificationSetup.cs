using System;
using System.Collections.Generic;
using System.Linq;

namespace LandauMedia.Wire
{
    public abstract class AbstractNotificationSetup : INotificationSetup
    {
        string _tableName;
        string _schemaName;
        string _keyColum;
        string _trackingType;

        Type _id;

        readonly IList<string>_additionalColumns = new List<string>();
        readonly IList<string> _intrestedUpdateColumns = new List<string>();

        protected AbstractNotificationSetup()
        {
            _schemaName = "dbo";
        }

        protected void SetTable(string tableName)
        {
            _tableName = tableName;
        }

        protected void SetSchemaAndTable(string tableName, string schema)
        {
            SetSchema(schema);
            SetTable(tableName);
        }

        protected void SetSchema(string schema)
        {
            _schemaName = schema;
        }

        protected void SetKeyColumn(string keyColum)
        {
            _keyColum = keyColum;
        }

        protected void SetIdType<T>()
        {
            _id = typeof (T);
        }

        protected void SetTrackingType(string trackingType)
        {
            _trackingType = trackingType;
        }

        protected void IntrestedInColumn(string columnName)
        {
            _intrestedUpdateColumns.Add(columnName);

            if (_additionalColumns.All(c => c != columnName))
                _additionalColumns.Add(columnName);

        }

        protected void SetAdditionalColumns(string[] columnNames)
        {
            foreach(var columnName in columnNames)
            {
                _additionalColumns.Add(columnName);    
            }
        }

        public string Table
        {
            get { return _tableName; }
        }

        public string Schema
        {
            get { return _schemaName; }
        }

        public string KeyColumn
        {
            get { return _keyColum; }
        }

        public Type IdType
        {
            get { return _id; }
        }

        public virtual string TrackingType
        {
            get { return _trackingType; }
        }

        public string[] AdditionalColumns
        {
            get { return _additionalColumns.ToArray(); }
        }

        public IEnumerable<string> IntrestedInUpdatedColums
        {
            get { return _intrestedUpdateColumns; }
        }

        public abstract Type Notification { get; }
    }
}