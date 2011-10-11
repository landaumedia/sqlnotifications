using System;
using System.Collections.Generic;

namespace LandauMedia.Wire
{
    public abstract class AbstractNotification : INotification
    {
        string _tableName;
        string _keyColum;
        string _trackingType;
        Type _id;
        readonly IList<string> _intrestedUpdateColumns = new List<string>();

        protected void SetTable(string tableName)
        {
            _tableName = tableName;
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
        }


        public string Table
        {
            get { return _tableName; }
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

        public IEnumerable<string> IntrestedInUpdatedColums
        {
            get { return _intrestedUpdateColumns; }
        }

        public abstract void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns);
        public abstract void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns);
        public abstract void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns);
    }
}