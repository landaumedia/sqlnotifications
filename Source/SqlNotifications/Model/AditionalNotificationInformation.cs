using System.Collections.Generic;
using System.Linq;

namespace LandauMedia.Model
{
    public class AditionalNotificationInformation
    {
        public AditionalNotificationInformation()
        {
            UpdatedColumns = Enumerable.Empty<string>();
            AdditionalColumns = new Dictionary<string, string>();
        }

        /// <summary>
        /// gibt die Spalten an, in denen Updates stattgefunden haben
        /// </summary>
        public IEnumerable<string> UpdatedColumns { get; set; }


        /// <summary>
        /// wenn im Setup zusätzliche Daten geladen werden sollten, tauchen diese hier auf
        /// </summary>
        public IDictionary<string, string> AdditionalColumns { get; set; }


        /// <summary>
        /// Rowversion of this Notification
        /// </summary>
        public ulong Rowversion { get; set; }
    }
}