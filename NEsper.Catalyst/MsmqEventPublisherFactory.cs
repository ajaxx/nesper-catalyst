///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.compat;

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
        /// Initializes a new instance of the <see cref="MsmqEventPublisherFactory"/> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        public MsmqEventPublisherFactory(IDictionary<string, string> attributes)
        {
            var mqPathAttribute = attributes.Get("mqPath");
            _mqBasePath = mqPathAttribute ?? string.Format(@".\private$\esper_{0}", Guid.NewGuid());

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
