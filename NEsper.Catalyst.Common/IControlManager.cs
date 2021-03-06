﻿///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

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
        [WebInvoke(UriTemplate = "/instance/create", Method = "POST")]
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
        /// Creates a statement from a prepared statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementArgs">The statement args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/create/prepared", Method = "POST")]
        StatementDescriptor CreatePrepared(string instanceId, StatementCreationArgs statementArgs);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/create/epl", Method = "POST")]
        StatementDescriptor CreateEPL(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/create/pattern", Method = "POST")]
        StatementDescriptor CreatePattern(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Compiles the statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/compile", Method = "POST")]
        EPStatementObjectModel Compile(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Creates a prepared statement based off the EPL that is presented.  The value
        /// that is returned is a unique identifier to the representation of the prepared
        /// statement on the server.  It is the prepared statement id.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/prepare/epl", Method = "POST")]
        string PrepareEPL(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Creates a prepared statement based off the pattern that is presented.  The value
        /// that is returned is a unique identifier to the representation of the prepared
        /// statement on the server.  It is the prepared statement id.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/prepare/pattern", Method = "POST")]
        string PreparePattern(string instanceId, StatementCreationArgs creationArgs);

        /// <summary>
        /// Sets a value within the prepared statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        /// <param name="preparedValueArgs">The prepared value args.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/prepared/{statementId}/set", Method = "POST")]
        void SetPreparedValue(string instanceId, string statementId, PreparedValueArgs preparedValueArgs);

        /// <summary>
        /// Destroys the pattern.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/statements/{statementId}", Method = "DELETE")]
        void DestroyPattern(string instanceId, string statementId);

        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="typeDefinition">The event type definition.</param>
        [OperationContract(Name = "RegisterType")]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/type/native", Method = "POST")]
        void RegisterType(string instanceId, NativeTypeDefinition typeDefinition);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventTypeDefinition">The event type definition.</param>
        [OperationContract(Name = "AddNativeEventType")]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/eventType/native", Method = "POST")]
        void AddEventType(string instanceId, NativeEventTypeDefinition eventTypeDefinition);

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventTypeDefinition">The event type definition.</param>
        [OperationContract(Name = "AddMapEventType")]
        [WebInvoke(UriTemplate = "/instance/{instanceId}/eventType/map", Method = "POST")]
        void AddEventType(string instanceId, MapEventTypeDefinition eventTypeDefinition);
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
}
