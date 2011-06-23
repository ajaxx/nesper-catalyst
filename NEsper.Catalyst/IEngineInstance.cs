///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml.Linq;

using com.espertech.esper.client;
using com.espertech.esper.client.soda;

using NEsper.Catalyst.Common;

namespace NEsper.Catalyst
{
    /// <summary>
    /// Interface that describes the behavior of a catalyst instance.
    /// </summary>
    public interface IEngineInstance : IDisposable
    {
        /// <summary>
        /// Occurs when a statement is created but before it is made available to the public.
        /// </summary>
        event EventHandler<StatementCreationEventArgs> StatementCreated;

        /// <summary>
        /// Gets the unique identifier for the service.
        /// </summary>
        /// <value>The id.</value>
        string Id { get; }

        /// <summary>
        /// Gets the public name of the instance.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        EPServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Schema fabricator for this engine.
        /// </summary>
        SchemaFabricator SchemaFabricator { get; }

        /// <summary>
        /// Gets the statement associated with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        StatementDescriptor GetStatement(string id);

        /// <summary>
        /// Gets descriptions for all statements.
        /// </summary>
        /// <value>The statements.</value>
        IEnumerable<StatementDescriptor> GetStatements();

        /// <summary>
        /// Creates a statement based on the prepared statement.
        /// </summary>
        /// <param name="statementCreationArgs">The statement creation args.</param>
        /// <returns></returns>
        StatementDescriptor CreatePrepared(StatementCreationArgs statementCreationArgs);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="creationArgs">The epl statement.</param>
        /// <returns></returns>
        StatementDescriptor CreateEPL(StatementCreationArgs creationArgs);

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="creationArgs">The on expression.</param>
        /// <returns></returns>
        StatementDescriptor CreatePattern(StatementCreationArgs creationArgs);

        /// <summary>
        /// Compiles the specified instance id.
        /// </summary>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        EPStatementObjectModel Compile(StatementCreationArgs creationArgs);

        /// <summary>
        /// Destroys the pattern.
        /// </summary>
        /// <param name="statementID">The statement ID.</param>
        void DestroyPattern(string statementID);

        /// <summary>
        /// Sends a POCO event.
        /// </summary>
        /// <param name="event">The @event.</param>
        void SendEvent(Object @event);

        /// <summary>
        /// Sends an event.
        /// </summary>
        /// <param name="event">The @event.</param>
        void SendEvent(XElement @event);

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="eventTypeName">Name of the event type.</param>
        void SendEvent(IDictionary<string, object> map, string eventTypeName);

        /// <summary>
        /// Prepares the EPL.
        /// </summary>
        /// <param name="statementText">The statement text.</param>
        string PrepareEPL(string statementText);

        /// <summary>
        /// Prepares the pattern.
        /// </summary>
        /// <param name="statementText">The statement text.</param>
        string PreparePattern(string statementText);

        /// <summary>
        /// Sets the prepared value.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterIndex">Index of the parameter.</param>
        /// <param name="value">The value.</param>
        void SetPreparedValue(string statementId, int parameterIndex, object value);
    }
}
