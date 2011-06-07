///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

using com.espertech.esper.client.soda;
using com.espertech.esper.compat.logging;
using com.espertech.esper.events;

namespace NEsper.Catalyst
{
    using Common;
    using Configuration;

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
        private readonly IEnumerable<IEventPublisherFactory> _eventPublisherFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineInstance"/> class.
        /// </summary>
        public EngineInstance()
        {
            Id = Guid.NewGuid().ToString();

            var appConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var catConfiguration = appConfiguration.Sections.OfType<CatalystConfiguration>().FirstOrDefault();
            if (catConfiguration == null) {
                Log.Warn("catalyst configuration section was not found");
            }

            // create the service instance
            var serviceConfiguration = new com.espertech.esper.client.Configuration();
            ServiceProvider = EPServiceProviderManager.GetDefaultProvider(serviceConfiguration);

            if ((catConfiguration != null) && (catConfiguration.Publishers != null)) {
                _eventPublisherFactories = catConfiguration.Publishers.OfType<PublisherElement>()
                    .Select(GetEventPublisherFactory)
                    .ToList();
            } else {
                _eventPublisherFactories = new List<IEventPublisherFactory>
                    {
                        new MsmqEventPublisherFactory(string.Format(@".\private$\esper_{0}", Id))
                    };
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the event publisher.
        /// </summary>
        /// <param name="publisherElement">The publisher element.</param>
        /// <returns></returns>
        public static IEventPublisherFactory GetEventPublisherFactory(PublisherElement publisherElement)
        {
            switch (publisherElement.Type.ToLower())
            {
                case "msmq":
                    return new MsmqEventPublisherFactory(publisherElement.Attributes);
                case "rabbitmq":
                    return new RabbitMqEventPublisherFactory(publisherElement.Attributes);
            }

            throw new ConfigurationErrorsException(string.Format("invalid publisher \"{0}\"", publisherElement));
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

            var publishers = _eventPublisherFactories
                .Select(factory => factory.CreatePublisher(new EventPublisherArgs(ServiceProvider.EPRuntime, statement)));

            if (StatementCreated != null) {
                StatementCreated(this, new StatementCreationEventArgs(this, statement));
            }

            statementDescriptor.Id = statement.Name;
            statementDescriptor.URIs = publishers.Select(publisher => publisher.URI.ToString()).ToArray();
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

            var publishers = _eventPublisherFactories
                .Select(factory => factory.CreatePublisher(new EventPublisherArgs(ServiceProvider.EPRuntime, statement)));

            if (StatementCreated != null) {
                StatementCreated(this, new StatementCreationEventArgs(this, statement));
            }

            statementDescriptor.Id = statement.Name;
            statementDescriptor.URIs = publishers.Select(publisher => publisher.URI.ToString()).ToArray();
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

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
