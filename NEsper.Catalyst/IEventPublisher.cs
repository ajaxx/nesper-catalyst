///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace NEsper.Catalyst
{
    using com.espertech.esper.client;

    public interface IEventPublisher
    {
        /// <summary>
        /// Returns a URI that describes where events are published.
        /// </summary>
        /// <value>The URI.</value>
        Uri URI { get; }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        void SendEvent(UpdateEventArgs eventArgs);
    }
}
