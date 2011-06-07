///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using RabbitMQ.Client;

namespace NEsper.Catalyst.Client
{
    public class RabbitMqEventConsumerFactory
        : IEventConsumerFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventConsumerFactory"/> class.
        /// </summary>
        /// <param name="address">The rabbitmq exchange address.</param>
        public RabbitMqEventConsumerFactory(string address)
        {
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Address = address;
        }

        /// <summary>
        /// Returns true if the factory can subscribe to events from the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified URI; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Uri uri)
        {
            return uri.Scheme == "rabbitmq";
        }

        /// <summary>
        /// Creates an event consumer that begins consuming events from the specified URI.
        /// Events are delivered to the event handler.  The disposable object that is
        /// returned is used to define the lifetime associated with the subscription.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public IDisposable Subscribe(Uri uri, EventHandler<UpdateEventArgs> eventHandler)
        {
            return new RabbitMqEventConsumer(_connectionFactory, uri.LocalPath, eventHandler);
        }
    }
}
