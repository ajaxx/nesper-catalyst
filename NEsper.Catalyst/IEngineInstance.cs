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
        /// Sends an event.
        /// </summary>
        /// <param name="event">The @event.</param>
        void SendEvent(XElement @event);
    }
}
