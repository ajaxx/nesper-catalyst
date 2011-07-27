using System;
using System.Collections.Generic;
using NEsper.Catalyst.Client.Consumers;
using NEsper.Catalyst.Client.Publishers;

namespace NEsper.Catalyst.Client
{
    public class CatalystConfiguration
    {
        /// <summary>
        /// Gets or sets the manager URI.
        /// </summary>
        /// <value>The manager URI.</value>
        public Uri ManagerUri { get; set; }
        /// <summary>
        /// Gets or sets the consumer factories.
        /// </summary>
        /// <value>The consumer factories.</value>
        public IEnumerable<IEventConsumerFactory> ConsumerFactories { get; set; }
        /// <summary>
        /// Gets or sets the publisher factories.
        /// </summary>
        /// <value>The publisher factories.</value>
        public IEnumerable<IDataPublisherFactory> PublisherFactories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystConfiguration"/> class.
        /// </summary>
        public CatalystConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystConfiguration"/> class.
        /// </summary>
        /// <param name="managerUri">The manager URI.</param>
        public CatalystConfiguration(Uri managerUri)
        {
            ManagerUri = managerUri;
            ConsumerFactories = DefaultConsumerFactories;
            PublisherFactories = DefaultPublisherFactories;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystConfiguration"/> class.
        /// </summary>
        /// <param name="managerUri">The manager URI.</param>
        /// <param name="consumerFactories">The consumer factories.</param>
        /// <param name="publisherFactories">The publisher factories.</param>
        public CatalystConfiguration(Uri managerUri, IEnumerable<IEventConsumerFactory> consumerFactories, IEnumerable<IDataPublisherFactory> publisherFactories)
        {
            ManagerUri = managerUri;
            ConsumerFactories = consumerFactories;
            PublisherFactories = publisherFactories;
        }

        public static readonly IEventConsumerFactory[] DefaultConsumerFactories =
            new IEventConsumerFactory[]
                {
                    new RabbitMqEventConsumerFactory("localhost"),
                    new MsmqEventConsumerFactory()
                };

        public static readonly IDataPublisherFactory[] DefaultPublisherFactories =
            new IDataPublisherFactory[]
                {
                    new RabbitMqDataPublisherFactory("localhost"),
                    new MsmqDataPublisherFactory()
                };
    }
}
