///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;

using com.espertech.esper.compat;
using NEsper.Catalyst.Common;
using RabbitMQ.Client;

namespace NEsper.Catalyst.Client.Publishers
{
    public class RabbitMqDataPublisherFactory: IDataPublisherFactory
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private string _exchangeAddr;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqDataPublisherFactory"/> class.
        /// </summary>
        /// <param name="address">The rabbitmq exchange address.</param>
        public RabbitMqDataPublisherFactory(string address)
        {
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Address = address;
            _connection = _connectionFactory.CreateConnection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqDataPublisherFactory"/> class.
        /// </summary>
        public RabbitMqDataPublisherFactory()
        {
        }

        /// <summary>
        /// Initializes the specified publisher configuration.
        /// </summary>
        /// <param name="publisherConfiguration">The publisher configuration.</param>
        public void Initialize(XElement publisherConfiguration)
        {
            _exchangeAddr = publisherConfiguration.RequiredAttribute("address");

            var connectionFactory = new ConnectionFactory {Address = _exchangeAddr};

            // optional attributes
            publisherConfiguration.OnOptionalAttribute(
                "username", value => connectionFactory.UserName = value);
            publisherConfiguration.OnOptionalAttribute(
                "password", value => connectionFactory.Password = value);
            publisherConfiguration.OnOptionalAttribute(
                "ssl", value => connectionFactory.Ssl = EnumHelper.Parse<SslOption>(value));

            _connectionFactory = connectionFactory;
            _connection = connectionFactory.CreateConnection();
        }

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public IDataPublisher CreatePublisher(Uri uri)
        {
            if (uri.Scheme == "rabbitmq")
            {
                var pathParts = uri.Segments;
                var queuePath = pathParts[1].TrimEnd('/');

                var model = _connection.CreateModel();

                //_model.QueueDeclare(
                //    exchangePath,
                //    true,
                //    false,
                //    false,
                //    null);

                // construct the publication address
                var publicationAddress = new PublicationAddress(
                    ExchangeType.Direct, string.Empty, queuePath);
                // construct the publisher
                var eventPublisher = new RabbitMqDataPublisher(model, publicationAddress);
                // return the publisher
                return eventPublisher;
            }

            return null;
        }
    }
}
