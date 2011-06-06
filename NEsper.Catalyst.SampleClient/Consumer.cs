using System;

using com.espertech.esper.client;

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class Consumer
    {
        /// <summary>
        /// Catalyst instance
        /// </summary>
        private readonly CatalystInstance _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public Consumer(CatalystInstance instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            var statement = _instance.Admininstrator.CreateEPL(
                "select * from NEsper.Catalyst.SampleClient.MarketDataEvent");
            statement.Events += DisplayEvents;
        }

        static void DisplayEvents(object sender, UpdateEventArgs updateEventArgs)
        {
            Console.WriteLine("Received event");
        }
    }
}
