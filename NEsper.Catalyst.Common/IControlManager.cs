///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;

using com.espertech.esper.client.soda;

namespace NEsper.Catalyst.Common
{
    [ServiceContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "ControlManager")]
    public interface IControlManager
    {
        #region Operations - Engine
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/instance/default")]
        InstanceDescriptor GetDefaultInstance();

        /// <summary>
        /// Gets the descriptor for the specified instance.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/instance/{id}")]
        InstanceDescriptor GetInstance(string id);

        /// <summary>
        /// Lists all catalyst instances.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/instance")]
        InstanceDescriptor[] ListInstances();

        /// <summary>
        /// Creates a catalyst instance.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/create", Method = "PUT")]
        InstanceDescriptor CreateInstance();

        /// <summary>
        /// Destroys a catalyst instance.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{id}", Method = "DELETE")]
        void DestroyInstance(string id);
        #endregion

        #region Operations - Instance
        /// <summary>
        /// Gets the statement description.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/instance/{instanceId}/statements/{statementId}")]
        StatementDescriptor GetStatement(string instanceId, string statementId);

        /// <summary>
        /// Gets descriptions for all statements.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        /// <value>The statements.</value>
        [OperationContract]
        [WebGet(UriTemplate = "/instance/{instanceId}/statements")]
        StatementDescriptorCollection GetStatements(string instanceId);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/create/epl", Method = "PUT")]
        StatementDescriptor CreateEPL(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/create/pattern", Method = "PUT")]
        StatementDescriptor CreatePattern(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Compiles the statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/compile", Method = "PUT")]
        EPStatementObjectModel Compile(string instanceId, StatementCreationArgs creationArgs);
        
        /// <summary>
        /// Destroys the pattern.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/statements/{statementId}", Method = "DELETE")]
        void DestroyPattern(string instanceId, string statementId);

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="event">The @event.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/event/map", Method = "PUT")]
        void SendMapEvent(string instanceId, MapEvent @event);

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="event">The @event.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/event/xml", Method = "PUT")]
        void SendXmlEvent(string instanceId, XElement @event);

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventArgs">The <see cref="NEsper.Catalyst.Common.JsonEventArgs"/> instance containing the event data.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/event/json", Method = "PUT")]
        void SendJsonEvent(string instanceId, JsonEventArgs eventArgs);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventTypeDefinition">The event type definition.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/eventType/add", Method = "PUT")]
        void AddEventType(string instanceId, EventTypeDefinition eventTypeDefinition);
        #endregion

        #region Statistics
        /// <summary>
        /// Gets statistics for the engine.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statistics")]
        EngineStatistics GetStatistics();


        /// <summary>
        /// Gets statistics for an instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statistics/{instanceId}")]
        InstanceStatistics GetInstanceStatistics(string instanceId);
        #endregion
    }

    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "EventTypeDefinition")]
    public class EventTypeDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type map.
        /// </summary>
        /// <value>The type map.</value>
        [DataMember]
        public EventTypeAtom[] TypeMap { get; set; }

        /// <summary>
        /// Gets or sets the super types.
        /// </summary>
        /// <value>The super types.</value>
        [DataMember]
        public string[] SuperTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeMap">The type map.</param>
        public EventTypeDefinition(string name, EventTypeAtom[] typeMap)
        {
            Name = name;
            TypeMap = typeMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeMap">The type map.</param>
        /// <param name="superTypes">The super types.</param>
        public EventTypeDefinition(string name, EventTypeAtom[] typeMap, string[] superTypes)
        {
            Name = name;
            TypeMap = typeMap;
            SuperTypes = superTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeDefinition"/> class.
        /// </summary>
        public EventTypeDefinition()
        {
        }
    }

    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "EventTypeAtom")]
    public class EventTypeAtom
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [DataMember]
        public string TypeName { get; set; }
        /// <summary>
        /// Gets or sets the type declaration.
        /// </summary>
        /// <value>The type decl.</value>
        [DataMember]
        public EventTypeAtom[] TypeDecl { get; set; }
    }

    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "JsonEvent")]
    public class JsonEventArgs
    {
        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        [DataMember]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        /// <value>The event data.</value>
        [DataMember]
        public string EventData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEventArgs"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        public JsonEventArgs(string eventType, string eventData)
        {
            EventType = eventType;
            EventData = eventData;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEventArgs"/> class.
        /// </summary>
        public JsonEventArgs()
        {
        }
    }
}
