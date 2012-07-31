using System.Collections.Generic;

namespace LandauMedia.Model
{
    public class AditionalNotificationInformation
    {
        /// <summary>
        /// gibt die Spalten an, in denen Updates stattgefunden haben
        /// </summary>
        public IEnumerable<string> UpdatedColumns { get; set; }


        /// <summary>
        /// wenn im Setup zusätzliche Daten geladen werden sollten, tauchen diese hier auf
        /// </summary>
        public IDictionary<string, string> AdditionalColumns { get; set; }
    }
}