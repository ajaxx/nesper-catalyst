using System;
using System.Threading;

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class Injector
    {
        /// <summary>
        /// Catalyst instance
        /// </summary>
        private readonly CatalystInstance _instance;

        /// <summary>
        /// Generates market data events
        /// </summary>
        private readonly MarketDataGenerator _marketDataGenerator;

        /// <summary>
        /// Indicates that an event has been sent.
        /// </summary>
        private readonly ManualResetEvent _sendIndicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Injector"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public Injector(CatalystInstance instance)
        {
            _instance = instance;
            _marketDataGenerator = new MarketDataGenerator();
            _sendIndicator = new ManualResetEvent(false);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            var thread = new Thread(SendEvents) { IsBackground = false, Name = "Injector" };
            thread.Start();
        }

        /// <summary>
        /// Sends the events.
        /// </summary>
        private void SendEvents()
        {
            var instanceRuntime = _instance.Runtime;

            Console.Out.WriteLine("Sending events ...");
            // send the first event
            instanceRuntime.SendEvent(_marketDataGenerator.NextEvent());
            // mark the reset event so that notifications go out
            _sendIndicator.Set();
            // send the rest of the events
            for( int ii = 0 ;; ii++ ) {
                instanceRuntime.SendEvent(_marketDataGenerator.NextEvent());
                if (( ii % 1000 ) == 0 ) {
                    Console.Out.Write('.');
                    Console.Out.Flush();
                }
            }
            
            Console.ReadLine();
        }

        /// <summary>
        /// Waits until at least one event has been posted by the injector.
        /// </summary>
        public void WaitOne()
        {
            _sendIndicator.WaitOne();
        }
    }
}
