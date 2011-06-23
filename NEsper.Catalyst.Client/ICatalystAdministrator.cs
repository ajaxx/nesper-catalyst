using System;
using System.Collections.Generic;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public interface ICatalystAdministrator
        : EPAdministrator
    {
        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegisterType<T>();

        /// <summary>
        /// Registers a native type.  Not required, but encouraged.
        /// </summary>
        /// <param name="nativeType">Type of the native.</param>
        void RegisterType(Type nativeType);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AddEventType<T>();

        /// <summary>
        /// Adds the type of the event with a specific name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventTypeName">The name.</param>
        void AddEventType<T>(string eventTypeName);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="nativeEventType">Native event type.</param>
        void AddEventType(string eventTypeName, Type nativeEventType);

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

        /// <summary>
        /// Occurs when a native type is registered.
        /// </summary>
        event EventHandler<TypeEventArgs> TypeRegistered;

        /// <summary>
        /// Sets the value of the designated parameter using the given object.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterIndex">the first parameter is 1, the second is 2, ...</param>
        /// <param name="value">the object containing the input parameter value</param>
        /// <exception name="EPException">if the substitution parameter could not be located</exception>
        void SetObject(string statementId, int parameterIndex, Object value);
    }

    public class TypeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeEventArgs"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TypeEventArgs(Type type)
        {
            Type = type;
        }
    }
}