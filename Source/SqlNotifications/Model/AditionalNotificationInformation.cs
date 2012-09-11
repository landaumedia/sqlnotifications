using System.Collections.Generic;
using System.Linq;

namespace LandauMedia.Model
{
    public class AditionalNotificationInformation
    {
        public AditionalNotificationInformation()
        {
            UpdatedColumns = Enumerable.Empty<string>();
            AdditionalColumns = new Dictionary<string, object>();
            ColumnOldValue = new Dictionary<string, object>();
        }

        /// <summary>
        /// gibt die Spalten an, in denen Updates stattgefunden haben
        /// </summary>
        public IEnumerable<string> UpdatedColumns { get; set; }


        /// <summary>
        /// wenn im Setup zusätzliche Daten geladen werden sollten, tauchen diese hier auf
        /// </summary>
        public IDictionary<string, object> AdditionalColumns { get; set; }


        /// <summary>
        /// beinhaltet den alten Wert der spalte
        /// </summary>
        public IDictionary<string, object> ColumnOldValue { get; set; } 

        /// <summary>
        /// Rowversion of this Notification
        /// </summary>
        public ulong Rowversion { get; set; }
    }
}