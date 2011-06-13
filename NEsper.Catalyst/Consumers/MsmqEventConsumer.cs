///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Xml.Linq;
using com.espertech.esper.client;
using com.espertech.esper.compat.logging;

namespace NEsper.Catalyst.Consumers
{
    class MsmqEventConsumer 
        : BaseEventConsumer
    {
        /// <summary>
        /// Message queue
        /// </summary>
        private MessageQueue _messageQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqEventConsumer"/> class.
        /// </summary>
        /// <param name="mqPath">The mq path.</param>
        public MsmqEventConsumer(string mqPath)
        {
            if (!MessageQueue.Exists(mqPath)) {
                throw new ArgumentException("invalid msmq path", "mqPath");
            }

            _messageQueue = new MessageQueue(mqPath);
            _messageQueue.Formatter = new BinaryMessageFormatter();
            _messageQueue.ReceiveCompleted += FinishReceive;
            _messageQueue.BeginReceive();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            var messageQueue = Interlocked.Exchange(ref _messageQueue, null);
            if (messageQueue != null) {
                messageQueue.Close();
                messageQueue.Dispose();
            }
        }

        /// <summary>
        /// Finishes the asynchronous receive.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Messaging.ReceiveCompletedEventArgs"/> instance containing the event data.</param>
        private void FinishReceive(object sender, ReceiveCompletedEventArgs e)
        {
            if (_messageQueue == null) {
                return;
            }

            try
            {
                _messageQueue.EndReceive(e.AsyncResult);
                // receive the current message
                var message = e.Message;
                var messageBody = message.Body as string;
                if (messageBody == null)
                {
                    Log.Warn("msmq message body was invalid");
                    return;
                }

                // convert to an XML wrapper
                var messageInXml = XElement.Parse(messageBody);
                var messageData = messageInXml.Nodes().OfType<XCData>().FirstOrDefault();
                if (messageData == null)
                {
                    Log.Warn("msmq message missing content node");
                    return;
                }

                var contentType = messageInXml.Attribute("content-type");
                if (contentType == null)
                {
                    Log.Warn("msmq message missing content-type attribute");
                    return;
                }

                DecodeAndRouteEvent(
                    contentType.Value,
                    messageData.Value);
            } catch(MessageQueueException) {
            } catch(NullReferenceException) {
            } finally {
                // begin receiving the next message
                _messageQueue.BeginReceive();
            }
        }

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
