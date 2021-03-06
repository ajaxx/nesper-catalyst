﻿using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace NEsper.Catalyst.Consumers
{
    public class RabbitMqEventConsumer 
        : BaseEventConsumer
    {
        private IConnection _connection;
        private IModel _model;
        private String _queue;
        private Subscription _subscription;
        private long _active;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventConsumer"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="exchangePath">The exchange path.</param>
        /// <param name="queue">The queue.</param>
        public RabbitMqEventConsumer(ConnectionFactory connectionFactory, string exchangePath, string queue)
        {
            _active = 1;
            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _queue = _model.QueueDeclare(queue, true, false, false, new Hashtable());

            // bind the queue to an exchange if specified
            if (exchangePath != null)
            {
                _model.QueueBind(_queue, exchangePath, string.Empty);
            }

            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer();
            eventingBasicConsumer.Received += HandleEvent;

            _model.BasicConsume(_queue, true, eventingBasicConsumer);

#if false           
            _subscription = new Subscription(_model, _queue, true);
            var thread = new Thread(ReceiveEvents);
            thread.IsBackground = true;
            thread.Name = "rabbitmq:consumer";
            thread.Start();
#endif

            var uriBuilder = new UriBuilder(
                "rabbitmq",
                connectionFactory.HostName,
                connectionFactory.Port,
                queue);

            Uri = uriBuilder.Uri;

        }

        private void HandleEvent(IBasicConsumer sender, BasicDeliverEventArgs args)
        {
            var elementBody = Encoding.Unicode.GetString(args.Body);
            var element = XElement.Parse(elementBody);
            DecodeAndRouteEvent(element);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (Interlocked.CompareExchange(ref _active, 1, 0) == 1)
            {
                _subscription.Close();
                _subscription = null;

                _model.Dispose();
                _model = null;

                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// Receives events from the consumer queue.
        /// </summary>
        private void ReceiveEvents()
        {
            while (_active == 1)
            {
                BasicDeliverEventArgs e;

                var subscription = _subscription;
                if (subscription == null)
                {
                    return;
                }

                if (subscription.Next(1000, out e))
                {
                    var latestEvent = subscription.LatestEvent;
                    if (latestEvent != null)
                    {
                        var elementBody = Encoding.Unicode.GetString(latestEvent.Body);
                        var element = XElement.Parse(elementBody);
                        DecodeAndRouteEvent(element);
                    }
                }
            }
        }
    }
}
