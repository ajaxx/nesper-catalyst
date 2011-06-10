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

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class InjectNative
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
        /// Initializes a new instance of the <see cref="InjectSynthetic"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public InjectNative(CatalystInstance instance)
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
            var marketData = _marketDataGenerator.GetEnumerator();

            Console.Out.WriteLine("Sending events ...");
            // send the first event
            instanceRuntime.SendEvent(marketData.Advance());
            // mark the reset event so that notifications go out
            _sendIndicator.Set();
            // send the rest of the events
            for( int ii = 0 ;; ii++ ) {
                instanceRuntime.SendEvent(marketData.Advance());
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
