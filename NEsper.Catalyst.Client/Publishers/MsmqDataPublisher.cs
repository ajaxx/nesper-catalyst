///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Messaging;
using System.Xml.Linq;

namespace NEsper.Catalyst.Client.Publishers
{
    class MsmqDataPublisher : IDataPublisher
    {
        /// <summary>
        /// Message queue
        /// </summary>
        private readonly MessageQueue _messageQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqDataPublisher"/> class.
        /// </summary>
        /// <param name="mqPath">The mq path.</param>
        public MsmqDataPublisher(string mqPath)
        {
            if (!MessageQueue.Exists(mqPath)) {
                _messageQueue = MessageQueue.Create(mqPath);
            } else {
                _messageQueue = new MessageQueue(mqPath);
            }

            _messageQueue.Formatter = new BinaryMessageFormatter();
        }

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void SendEvent(XElement @event)
        {
            var eventMessage = new Message(
                @event.ToString(), new BinaryMessageFormatter());
            _messageQueue.Send(eventMessage);
        }
    }
}
