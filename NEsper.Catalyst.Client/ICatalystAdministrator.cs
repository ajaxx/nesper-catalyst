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
        /// <typeparam name="T"></typeparam>
        void AddEventType<T>();

        /// <summary>
        /// Adds the type of the event with a specific name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        void AddEventType<T>(string name);

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