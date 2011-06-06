namespace NEsper.Catalyst
{
    using com.espertech.esper.client;

    public interface IEventPublisherFactory
    {
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
