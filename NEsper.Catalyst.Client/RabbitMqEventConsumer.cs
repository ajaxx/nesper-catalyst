///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;

using com.espertech.esper.client;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace NEsper.Catalyst.Client
{
    class RabbitMqEventConsumer : IDisposable
    {
        private IConnection _connection;
        private IModel _model;
        private String _queue;
        private Subscription _subscription;
        private long _active;

        /// <summary>
        /// Event handler for this consumer
        /// </summary>
        private readonly EventHandler<UpdateEventArgs> _eventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventConsumer"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="exchangePath">The exchange path.</param>
        /// <param name="eventHandler">The event handler.</param>
        public RabbitMqEventConsumer(ConnectionFactory connectionFactory, string exchangePath, EventHandler<UpdateEventArgs> eventHandler)
        {
            _active = 1;
            _eventHandler = eventHandler;
            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _queue = _model.QueueDeclare();
            _model.QueueBind(_queue, exchangePath, string.Empty);
            _subscription = new Subscription(_model, _queue, true);

            var thread = new Thread(ReceiveEvents);
            thread.IsBackground = true;
            thread.Name = "rabbitmq:consumer";
            thread.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _active, 1, 0) == 1) {
                _subscription.Close();
                _subscription = null;

                _model.Dispose();
                _model = null;

                _connection.Dispose();
                _connection = null;
            }
        }

        private void ReceiveEvents()
        {
            BasicDeliverEventArgs e;

            while (_active == 1) {
                if (_subscription.Next(1000, out e)) {
                    var latestEvent = _subscription.LatestEvent;
                    if (latestEvent != null) {
                        var elementText = Encoding.Unicode.GetString(latestEvent.Body);
                        var eventArgs = elementText.ToUpdateEventArgs();
                        // send the event(s) along
                        _eventHandler.Invoke(null, eventArgs);
                    }
                }
            }
        }
    }
}
