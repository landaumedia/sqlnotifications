using System;
using System.Collections.Generic;

namespace Krowiorsch.Dojo.Wire
{
    public interface INotification
    {
        string Table { get; }
        string KeyColumn { get; }
        Type IdType { get; }

        IEnumerable<string> IntrestedInUpdatedColums { get; } 
    }
}