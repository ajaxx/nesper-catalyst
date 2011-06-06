using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using com.espertech.esper.client.soda;
using com.espertech.esper.events;
using NEsper.Catalyst.Common;

namespace NEsper.Catalyst
{
    using com.espertech.esper.client;

    class EngineInstance : IEngineInstance
    {
        /// <summary>
        /// Occurs when a statement is created but before it is made available to the public.
        /// </summary>
        public event EventHandler<StatementCreationEventArgs> StatementCreated;

        /// <summary>
        /// Gets the unique identifier for the service.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the public name of the instance.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public EPServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Event publisher factory for this instance.
        /// </summary>
        private readonly IEventPublisherFactory _eventPublisherFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineInstance"/> class.
        /// </summary>
        public EngineInstance()
        {
            Id = Guid.NewGuid().ToString();

            // create the service instance
            var serviceConfiguration = new Configuration();
            ServiceProvider = EPServiceProviderManager.GetDefaultProvider(serviceConfiguration);

            _eventPublisherFactory = new MsmqEventPublisherFactory(
                string.Format(@".\private$\esper_{0}", Id));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the statement associated with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public StatementDescriptor GetStatement(string id)
        {
            var administrator = ServiceProvider.EPAdministrator;
            var statement = administrator.GetStatement(id);
            if (statement == null) {
                return null;
            }

            return (StatementDescriptor)statement.UserObject;
        }

        /// <summary>
        /// Returns a list of statement descriptors.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StatementDescriptor> GetStatements()
        {
            var administrator = ServiceProvider.EPAdministrator;
            var result = administrator.StatementNames
                .Select(administrator.GetStatement)
                .Where(statement => statement != null)
                .Select(statement => (StatementDescriptor)statement.UserObject);
            return result;
        }

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="creationArgs">The epl statement.</param>
        /// <returns></returns>
        public StatementDescriptor CreateEPL(StatementCreationArgs creationArgs)
        {
            var administrator = ServiceProvider.EPAdministrator;
            var statementDescriptor = new StatementDescriptor();
            var statement = administrator.CreateEPL(
                creationArgs.StatementText,
                creationArgs.StatementName,
                statementDescriptor);

            var publisher = _eventPublisherFactory.CreatePublisher(
                new EventPublisherArgs(ServiceProvider.EPRuntime, statement));

            if (StatementCreated != null) {
                StatementCreated(this, new StatementCreationEventArgs(this, statement));
            }

            statementDescriptor.Id = statement.Name;
            statementDescriptor.URI = publisher.URI.ToString();
            return statementDescriptor;
        }

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="creationArgs">The on expression.</param>
        /// <returns></returns>
        public StatementDescriptor CreatePattern(StatementCreationArgs creationArgs)
        {
            var administrator = ServiceProvider.EPAdministrator;
            var statementDescriptor = new StatementDescriptor();
            var statement = administrator.CreatePattern(
                creationArgs.StatementText,
                creationArgs.StatementName,
                statementDescriptor);

            var publisher = _eventPublisherFactory.CreatePublisher(
                new EventPublisherArgs(ServiceProvider.EPRuntime, statement));

            if (StatementCreated != null) {
                StatementCreated(this, new StatementCreationEventArgs(this, statement));
            }

            statementDescriptor.Id = statement.Name;
            statementDescriptor.URI = publisher.URI.ToString();
            return statementDescriptor;
        }

        /// <summary>
        /// Compiles the specified statement text.
        /// </summary>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        public EPStatementObjectModel Compile(StatementCreationArgs creationArgs)
        {
            var administrator = ServiceProvider.EPAdministrator;
            var statementObjectModel = administrator.CompileEPL(creationArgs.StatementText);
            return statementObjectModel;
        }

        /// <summary>
        /// Destroys the pattern.
        /// </summary>
        /// <param name="statementID">The statement ID.</param>
        public void DestroyPattern(string statementID)
        {
            var administrator = ServiceProvider.EPAdministrator;
            var statement = administrator.GetStatement(statementID);
            if (statement != null)
            {
                statement.Dispose();
            }
        }

        /// <summary>
        /// Sends an event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void SendEvent(XElement @event)
        {
            var runtime = ServiceProvider.EPRuntime;

            try
            {
                runtime.SendEvent(@event);
            } catch(EventAdapterException) {
                var administrator = ServiceProvider.EPAdministrator;
                var configuration = new ConfigurationEventTypeXMLDOM();
                configuration.RootElementName = @event.Name.LocalName;
                configuration.RootElementNamespace = @event.Name.Namespace.NamespaceName;
                administrator.GetConfiguration().AddEventType(
                    @event.Name.LocalName,
                    configuration);
                runtime.SendEvent(@event);
            }
        }
    }
}
