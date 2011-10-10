using System;
using System.Collections.Generic;
using System.Linq;
using Krowiorsch.Dojo.Wire;
using LandauMedia.Wire;

namespace Krowiorsch.Dojo
{
    public class ApplicationNotification : INotification
    {
        public string Table
        {
            get { return "Application"; }
        }

        public string KeyColumn
        {
            get { return "Id"; }
        }

        public Type IdType
        {
            get { return typeof (Guid); }
        }

        public IEnumerable<string> IntrestedInUpdatedColums
        {
            get { return Enumerable.Empty<string>(); }
        }

        public void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            Console.WriteLine("I" + id);
        }

        public void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            Console.WriteLine("U" + id);
        }

        public void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns)
        {
            Console.WriteLine("D " + id);
        }
    }
}