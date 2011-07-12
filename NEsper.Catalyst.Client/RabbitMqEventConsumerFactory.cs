///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.client;
using com.espertech.esper.compat;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            var pathParts = uri.Segments;
            var exchangePath = pathParts[1].TrimEnd('/');
            var routingKey = pathParts[2];

            // find the exchange path binding
            ExchangePathBinding exchangePathBinding;
            if (!_exchangePathBindingTable.TryGetValue(exchangePath, out exchangePathBinding))
            {
                exchangePathBinding = new ExchangePathBinding();
                exchangePathBinding.Connection = _connectionFactory.CreateConnection();
                exchangePathBinding.Channel = exchangePathBinding.Connection.CreateModel();
                _exchangePathBindingTable[exchangePath] = exchangePathBinding;
            }

            // find a queue for this routingKey
            RouteBinding routeBinding;
            if (!exchangePathBinding.RoutingKeyTable.TryGetValue(routingKey, out routeBinding))
            {
                routeBinding = new RouteBinding();
                routeBinding.Queue = exchangePathBinding.Channel.QueueDeclare();
                routeBinding.Consumer = new EventingBasicConsumer();
                routeBinding.Consumer.Received += routeBinding.HandleEvent;

                exchangePathBinding.RoutingKeyTable[routingKey] = routeBinding;

                exchangePathBinding.Channel.QueueBind(routeBinding.Queue, exchangePath, routingKey);
                exchangePathBinding.Channel.BasicConsume(routeBinding.Queue, true, routeBinding.Consumer);
            }

            routeBinding.Events += eventHandler;

            return new TrackedDisposable(
                () => routeBinding.Events -= eventHandler);
        }

        class RouteBinding
        {
            /// <summary>
            /// Gets or sets the queue.
            /// </summary>
            /// <value>The queue.</value>
            public string Queue { get; set; }

            /// <summary>
            /// Gets or sets the consumer.
            /// </summary>
            /// <value>The consumer.</value>
            public EventingBasicConsumer Consumer { get; set; }

            /// <summary>
            /// Occurs when routed events are consumed.
            /// </summary>
            public event EventHandler<UpdateEventArgs> Events;

            /// <summary>
            /// Handles the event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="args">The <see cref="RabbitMQ.Client.Events.BasicDeliverEventArgs"/> instance containing the event data.</param>
            public void HandleEvent(IBasicConsumer sender, BasicDeliverEventArgs args)
            {
                if (Events != null)
                {
                    var elementText = Encoding.Unicode.GetString(args.Body);
                    var eventArgs = elementText.ToUpdateEventArgs();
                    // send the event(s) along
                    Events.Invoke(null, eventArgs);
                }
            }
        }

        private readonly IDictionary<string, ExchangePathBinding> _exchangePathBindingTable =
            new Dictionary<string, ExchangePathBinding>();

        class ExchangePathBinding
        {
            /// <summary>
            /// Gets or sets the connection.
            /// </summary>
            /// <value>The connection.</value>
            public IConnection Connection { get; set; }
            /// <summary>
            /// Gets or sets the channel.
            /// </summary>
            /// <value>The channel.</value>
            public IModel Channel { get; set; }

            /// <summary>
            /// The routing key table
            /// </summary>
            public readonly IDictionary<string, RouteBinding> RoutingKeyTable =
                new Dictionary<string, RouteBinding>();
        }
    }
}
