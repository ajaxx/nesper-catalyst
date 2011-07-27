///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using com.espertech.esper.client;

namespace NEsper.Catalyst.Client.Consumers
{
    class RabbitMqEventConsumer : IDisposable
    {
        private string _routingKey;
        private long _active;

        /// <summary>
        /// Event handler for this consumer
        /// </summary>
        private readonly EventHandler<UpdateEventArgs> _eventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventConsumer"/> class.
        /// </summary>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="eventHandler">The event handler.</param>
        public RabbitMqEventConsumer(string routingKey, EventHandler<UpdateEventArgs> eventHandler)
        {
            _active = 1;
            _eventHandler = eventHandler;
            _routingKey = routingKey;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _active, 1, 0) == 1) {
            }
        }
    }
}
