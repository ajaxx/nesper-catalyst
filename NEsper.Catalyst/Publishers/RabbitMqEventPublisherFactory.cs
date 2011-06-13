///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;
using com.espertech.esper.compat;
using NEsper.Catalyst.Consumers;

using RabbitMQ.Client;

namespace NEsper.Catalyst.Publishers
{
    class RabbitMqEventPublisherFactory: IEventPublisherFactory
    {
        private ConnectionFactory _connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventPublisherFactory"/> class.
        /// </summary>
        /// <param name="address">The rabbitmq exchange address.</param>
        public RabbitMqEventPublisherFactory(string address)
        {
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Address = address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventPublisherFactory"/> class.
        /// </summary>
        public RabbitMqEventPublisherFactory()
        {
        }

        /// <summary>
        /// Initializes the specified publisher configuration.
        /// </summary>
        /// <param name="publisherConfiguration">The publisher configuration.</param>
        public void Initialize(XElement publisherConfiguration)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Address = publisherConfiguration.RequiredAttribute("address");

            // optional attributes
            publisherConfiguration.OnOptionalAttribute(
                "username", value => connectionFactory.UserName = value);
            publisherConfiguration.OnOptionalAttribute(
                "password", value => connectionFactory.Password = value);
            publisherConfiguration.OnOptionalAttribute(
                "ssl", value => connectionFactory.Ssl = EnumHelper.Parse<SslOption>(value));

            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <returns></returns>
        public IEventPublisher CreatePublisher(EventPublisherArgs eventPublisherArgs)
        {
            // construct the exchange name
            var exchangeName = "esper." + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            // construct the publisher
            var eventPublisher = new RabbitMqEventPublisher(_connectionFactory, exchangeName);
            // connect the statement to the publisher
            eventPublisherArgs.Statement.Events += (sender, eventArgs) => eventPublisher.SendEvent(eventArgs);
            // return the publisher
            return eventPublisher;
        }
    }
}
