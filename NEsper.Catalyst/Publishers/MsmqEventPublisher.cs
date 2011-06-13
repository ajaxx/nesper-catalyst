///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Messaging;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Publishers
{
    class MsmqEventPublisher : IEventPublisher
    {
        /// <summary>
        /// Message queue
        /// </summary>
        private readonly MessageQueue _messageQueue;

        /// <summary>
        /// Returns a URI that describes where events are published.
        /// </summary>
        /// <value>The URI.</value>
        public Uri URI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqEventPublisher"/> class.
        /// </summary>
        /// <param name="mqPath">The mq path.</param>
        public MsmqEventPublisher(string mqPath)
        {
            URI = new Uri(string.Format("msmq:{0}", mqPath));
            
            if (!MessageQueue.Exists(mqPath)) {
                _messageQueue = MessageQueue.Create(mqPath);
            } else {
                _messageQueue = new MessageQueue(mqPath);
            }

            _messageQueue.Formatter = new BinaryMessageFormatter();
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        public void SendEvent(UpdateEventArgs eventArgs)
        {
            var eventElement = eventArgs.ToXElement();
            var eventMessage = new Message(eventElement.ToString(), new BinaryMessageFormatter());
            _messageQueue.Send(eventMessage);
        }
    }
}
