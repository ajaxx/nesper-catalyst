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
        /// <param name="model">The model.</param>
        /// <param name="exchangeAddress">The exchange address.</param>
        /// <param name="address">The address.</param>
        public RabbitMqEventPublisher(IModel model, String exchangeAddress, PublicationAddress address)
        {
            var builder = new UriBuilder();
            builder.Scheme = "rabbitmq";
            builder.Host = exchangeAddress;
            builder.Path = string.Format("{0}/{1}", 
                Uri.EscapeUriString(address.ExchangeName), 
                Uri.EscapeUriString(address.RoutingKey));

            URI = builder.Uri;

            _model = model;
            _address = address;
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        public void SendEvent(UpdateEventArgs eventArgs)
        {
            var eventElement = Common.EventExtensions.ToXElement(eventArgs);
            var eventContent = Encoding.Unicode.GetBytes(eventElement.ToString(SaveOptions.None));

            var basicProperties = _model.CreateBasicProperties();
            basicProperties.DeliveryMode = 1;

            Console.WriteLine("basicPublisher: {0}", _address);

            _model.BasicPublish(_address, basicProperties, eventContent);
        }
    }
}
