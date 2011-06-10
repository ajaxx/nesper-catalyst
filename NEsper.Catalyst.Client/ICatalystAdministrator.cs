using System.Collections.Generic;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public interface ICatalystAdministrator
        : EPAdministrator
    {
        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="typeMap">The type map.</param>
        void AddEventType(string eventTypeName, IDictionary<string, object> typeMap);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="typeMap">The type map.</param>
        /// <param name="superTypes">The super types.</param>
        void AddEventType(string eventTypeName, IDictionary<string, object> typeMap, params string[] superTypes);
    }
}