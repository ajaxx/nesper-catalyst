﻿///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;
using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public interface IEventConsumerFactory
    {
        /// <summary>
        /// Initializes the specified consumer configuration.
        /// </summary>
        /// <param name="consumerConfiguration">The consumer configuration.</param>
        void Initialize(XElement consumerConfiguration);

        /// <summary>
        /// Returns true if the factory can subscribe to events from the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified URI; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandle(Uri uri);

        /// <summary>
        /// Creates an event consumer that begins consuming events from the specified URI.
        /// Events are delivered to the event handler.  The disposable object that is
        /// returned is used to define the lifetime associated with the subscription.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        IDisposable Subscribe(Uri uri, EventHandler<UpdateEventArgs> eventHandler);
    }
}
