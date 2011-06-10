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
        /// Type name for view
        /// </summary>
        private readonly string _typename;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="typename">The typename.</param>
        public Consumer(CatalystInstance instance, string typename)
        {
            _instance = instance;
            _typename = typename;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            var statement = _instance.Administrator.CreateEPL(
                string.Format("select Symbol, Ask from {0}", _typename));
                //"select Symbol, Ask from NEsper.Catalyst.SampleClient.MarketDataEvent");
                //"select * from NEsper.Catalyst.SampleClient.MarketDataEvent");
            statement.Events += DisplayEvents;
        }

        static void DisplayEvents(object sender, UpdateEventArgs updateEventArgs)
        {
            Console.WriteLine("Received event");
        }
    }
}
