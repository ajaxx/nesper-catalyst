///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Configuration;

using com.espertech.esper.compat;

using RabbitMQ.Client;

namespace NEsper.Catalyst
{
    class RabbitMqEventPublisherFactory: IEventPublisherFactory
    {
        private readonly ConnectionFactory _connectionFactory;

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
        /// <param name="attributes">The attributes.</param>
        public RabbitMqEventPublisherFactory(IDictionary<string, string> attributes)
        {
            var exchangeAttribute = attributes.Get("exchange");
            if (exchangeAttribute == null) {
                throw new ConfigurationErrorsException("missing exchange attribute");
            }

            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Address = exchangeAttribute;
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
