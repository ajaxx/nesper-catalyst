///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;
using System.Xml.Linq;
using com.espertech.esper.client;
using RabbitMQ.Client;

namespace NEsper.Catalyst.Publishers
{
    class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private readonly PublicationAddress _address;

        /// <summary>
        /// Returns a URI that describes where events are published.
        /// </summary>
        /// <value>The URI.</value>
        public Uri URI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventPublisher"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="exchangePath">The exchange path.</param>
        public RabbitMqEventPublisher(ConnectionFactory connectionFactory, string exchangePath)
        {
            URI = new Uri(string.Format("rabbitmq:{0}", exchangePath));

            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(
                exchangePath,
                ExchangeType.Fanout,
                true,
                true,
                new Hashtable());

            _address = new PublicationAddress(
                ExchangeType.Fanout,
                exchangePath,
                string.Empty);
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        public void SendEvent(UpdateEventArgs eventArgs)
        {
            var eventElement = eventArgs.ToXElement();
            var eventContent = Encoding.Unicode.GetBytes(eventElement.ToString(SaveOptions.None));

            var basicProperties = _model.CreateBasicProperties();
            basicProperties.DeliveryMode = 1;

            _model.BasicPublish(_address, basicProperties, eventContent);
        }
    }
}
