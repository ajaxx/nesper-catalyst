///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

using com.espertech.esper.compat;

namespace NEsper.Catalyst.SampleClient
{
    using Client;

    class InjectSynthetic
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
        public InjectSynthetic(CatalystInstance instance)
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
            var quoteType = SyntheticType
                .Define()
                .Declare<string>("Symbol")
                .Declare<Money>("Bid")
                .Declare<Money>("Ask");

            _instance.Administrator.AddEventType<Account>();
            _instance.Administrator.AddEventType("SyntheticEvent", quoteType);

            var thread = new Thread(SendEvents) { IsBackground = false, Name = "Injector" };
            thread.Start();
        }

        private IDictionary<string, object> MakeSynthetic(MarketDataEvent marketDataEvent)
        {
            var syntheticEvent = new Dictionary<string, object>();
            syntheticEvent["Symbol"] = marketDataEvent.Symbol;
            syntheticEvent["Bid"] = new Money(marketDataEvent.Bid, "USD");
            syntheticEvent["Ask"] = new Money(marketDataEvent.Ask, "USD");
            return syntheticEvent;
        }

        private void SendEvents()
        {
            var instanceRuntime = _instance.Runtime;
            var marketData = _marketDataGenerator.GetEnumerator();

            Console.Out.WriteLine("Sending events ...");

            // send the first event
            instanceRuntime.SendEvent(MakeSynthetic(marketData.Advance()), "SyntheticEvent");
            // mark the reset event so that notifications go out
            _sendIndicator.Set();
            // send the rest of the events
            for (int ii = 0; ; ii++) {
                instanceRuntime.SendEvent(MakeSynthetic(marketData.Advance()), "SyntheticEvent");
                if ((ii % 1000) == 0)
                {
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

    [DataContract]
    public class Money
    {
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public Currency Currency { get; set; }

        public Money(double amount, string currency)
        {
            Amount = amount;
            Currency = new Currency {Country = "test", ISOCode = currency};
        }

        public Money()
        {
        }
    }

    [DataContract]
    public class Currency
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string ISOCode { get; set; }
    }

    [DataContract]
    public class Account
    {
        [DataMember]
        public IList<Money> Money { get; set; }
    }
}
