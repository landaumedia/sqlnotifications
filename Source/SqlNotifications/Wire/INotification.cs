﻿using System;
using System.Collections.Generic;

namespace Krowiorsch.Dojo.Wire
{
    public interface INotification
    {
        string Table { get; }
        string KeyColumn { get; }
        Type IdType { get; }

        IEnumerable<string> IntrestedInUpdatedColums { get; }

        void OnInsert(INotification notification, string id, IEnumerable<string> updatedColumns);
        void OnUpdate(INotification notification, string id, IEnumerable<string> updatedColumns);
        void OnDelete(INotification notification, string id, IEnumerable<string> updatedColumns);
    }
}