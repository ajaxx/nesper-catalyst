///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public class DispatchEventConsumerFactory : IEventConsumerFactory
    {
        /// <summary>
        /// Gets the factory list.
        /// </summary>
        /// <value>The factory list.</value>
        public IList<IEventConsumerFactory> Factories { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchEventConsumerFactory"/> class.
        /// </summary>
        public DispatchEventConsumerFactory()
        {
            Factories = new List<IEventConsumerFactory>();
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
            return Factories.Any(factory => factory.CanHandle(uri));
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
            var selectFactory = Factories.FirstOrDefault(
                factory => factory.CanHandle(uri));
            if (selectFactory == null) {
                throw new ArgumentException("unable to handle URI of form " + uri, "uri");
            }

            return selectFactory.Subscribe(uri, eventHandler);
        }

        /// <summary>
        /// Subscribes the specified uris.
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public IDisposable Subscribe(IEnumerable<string> uris, EventHandler<UpdateEventArgs> eventHandler)
        {
            foreach (var uriText in uris) {
                var uri = new Uri(uriText);
                var selectFactory = Factories.FirstOrDefault(
                    factory => factory.CanHandle(uri));
                if (selectFactory != null) {
                    return selectFactory.Subscribe(uri, eventHandler);
                }
            }

            throw new ArgumentException("unable to find URI handler to handle subscription");
        }
    }
}
