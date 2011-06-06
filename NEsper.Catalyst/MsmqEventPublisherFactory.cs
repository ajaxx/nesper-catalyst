using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NEsper.Catalyst
{
    class MsmqEventPublisherFactory : IEventPublisherFactory
    {
        private readonly string _mqBasePath;
        private long _mqCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqEventPublisherFactory"/> class.
        /// </summary>
        /// <param name="mqBasePath">The mq base path.</param>
        public MsmqEventPublisherFactory(string mqBasePath)
        {
            _mqBasePath = mqBasePath;
            _mqCounter = 0L;
        }

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <returns></returns>
        public IEventPublisher CreatePublisher(EventPublisherArgs eventPublisherArgs)
        {
            // construct the publisher
            var mqEventPublisher = new MsmqEventPublisher(
                string.Format("{0}_{1}", _mqBasePath, Interlocked.Increment(ref _mqCounter)));
            // connect the statement to the publisher
            eventPublisherArgs.Statement.Events += (sender, eventArgs) => mqEventPublisher.SendEvent(eventArgs);
            // return the publisher
            return mqEventPublisher;
        }
    }
}
