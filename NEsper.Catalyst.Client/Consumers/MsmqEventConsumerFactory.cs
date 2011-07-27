///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;
using com.espertech.esper.client;

namespace NEsper.Catalyst.Client.Consumers
{
    public class MsmqEventConsumerFactory
        : IEventConsumerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqEventConsumerFactory"/> class.
        /// </summary>
        public MsmqEventConsumerFactory()
        {
        }

        /// <summary>
        /// Initializes the specified consumer configuration.
        /// </summary>
        /// <param name="consumerConfiguration">The consumer configuration.</param>
        public void Initialize(XElement consumerConfiguration)
        {
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
            return uri.Scheme == "msmq";
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
            return new MsmqEventConsumer(uri.LocalPath, eventHandler);
        }
    }
}
