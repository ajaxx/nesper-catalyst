///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Xml.Linq;

namespace NEsper.Catalyst
{
    using com.espertech.esper.client;

    public interface IEventPublisherFactory
    {
        /// <summary>
        /// Initializes the specified publisher configuration.
        /// </summary>
        /// <param name="publisherConfiguration">The publisher configuration.</param>
        void Initialize(XElement publisherConfiguration);

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <param name="eventPublisherArgs">The event publisher args.</param>
        /// <returns></returns>
        IEventPublisher CreatePublisher(EventPublisherArgs eventPublisherArgs);
    }

    public class EventPublisherArgs
    {
        /// <summary>
        /// Gets or sets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        public EPRuntime Runtime { get; set; }

        /// <summary>
        /// Gets or sets the statement.
        /// </summary>
        /// <value>The statement.</value>
        public EPStatement Statement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherArgs"/> class.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <param name="statement">The statement.</param>
        public EventPublisherArgs(EPRuntime runtime, EPStatement statement)
        {
            Runtime = runtime;
            Statement = statement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherArgs"/> class.
        /// </summary>
        public EventPublisherArgs()
        {
        }
    }
}
