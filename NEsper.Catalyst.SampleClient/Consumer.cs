///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

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
