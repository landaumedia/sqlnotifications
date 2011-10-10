using System;
using System.Collections.Generic;
using System.Linq;
using Krowiorsch.Dojo.Wire;

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
    }
}