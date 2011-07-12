///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Xml.Linq;
using com.espertech.esper.compat;
using NEsper.Catalyst.Consumers;

using RabbitMQ.Client;

namespace NEsper.Catalyst.Publishers
{
    class RabbitMqEventPublisherFactory: IEventPublisherFactory
    {
        private ConnectionFactory _connectionFactory;

        private IConnection _connection;
        private IModel _model;
        private PublicationAddress _address;

        private string _routingKey;
        private string _exchangePath;
        private string _exchangeAddr;

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
            _exchangeAddr = publisherConfiguration.RequiredAttribute("address");

            var connectionFactory = new ConnectionFactory();
            connectionFactory.Address = _exchangeAddr;

            // optional attributes
            publisherConfiguration.OnOptionalAttribute(
                "username", value => connectionFactory.UserName = value);
            publisherConfiguration.OnOptionalAttribute(
                "password", value => connectionFactory.Password = value);
            publisherConfiguration.OnOptionalAttribute(
                "ssl", value => connectionFactory.Ssl = EnumHelper.Parse<SslOption>(value));

            // construct the exchange name
            _exchangePath = "esper.catalyst.events";
            publisherConfiguration.OnOptionalAttribute(
                "exchangePath", value => _exchangePath = value);

            _routingKey = "anonymous.info";
            publisherConfiguration.OnOptionalAttribute(
                "routingKey", value => _routingKey = value);

            _connectionFactory = connectionFactory;

            _connection = connectionFactory.CreateConnection();

            _model = _connection.CreateModel();
            _model.ExchangeDeclare(
                _exchangePath,
                ExchangeType.Topic);
        }

        /// <summary>
        /// Creates the publisher topic.
        /// </summary>
        /// <returns></returns>
        public static string CreatePublisherTopic()
        {
            do
            {
                string temp = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                if (!temp.Contains("/"))
                {
                    return temp;
                }
            } while (true); 
        }

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <returns></returns>
        public IEventPublisher CreatePublisher(EventPublisherArgs eventPublisherArgs)
        {
            // construct the exchange name
            var publisherTopic = CreatePublisherTopic();
            // construct the publication address
            var publicationAddress = new PublicationAddress(
                ExchangeType.Topic, _exchangePath, publisherTopic);
            // construct the publisher
            var eventPublisher = new RabbitMqEventPublisher(_model, _exchangeAddr, publicationAddress);
            // connect the statement to the publisher
            eventPublisherArgs.Statement.Events += (sender, eventArgs) => eventPublisher.SendEvent(eventArgs);
            // return the publisher
            return eventPublisher;
        }
    }
}
