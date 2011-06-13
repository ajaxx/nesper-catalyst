///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Xml.Linq;

using com.espertech.esper.compat;

using RabbitMQ.Client;

namespace NEsper.Catalyst.Consumers
{
    class RabbitMqEventConsumerFactory : IEventConsumerFactory
    {
        /// <summary>
        /// Creates an event consumer.
        /// </summary>
        /// <param name="consumerElement">The consumer configuration.</param>
        /// <returns></returns>
        public IEventConsumer CreateConsumer(XElement consumerElement)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Address = consumerElement.RequiredAttribute("address");
            
            // optional attributes
            consumerElement.OnOptionalAttribute(
                "username", value => connectionFactory.UserName = value);
            consumerElement.OnOptionalAttribute(
                "password", value => connectionFactory.Password = value);
            consumerElement.OnOptionalAttribute(
                "ssl", value => connectionFactory.Ssl = EnumHelper.Parse<SslOption>(value));

            // configuration for the queue
            var consumer = new RabbitMqEventConsumer(
                connectionFactory,
                consumerElement.OptionalAttribute("exchange"),
                consumerElement.RequiredAttribute("queue"));

            return consumer;
        }
    }
}
