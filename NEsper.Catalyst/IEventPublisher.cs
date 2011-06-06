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
