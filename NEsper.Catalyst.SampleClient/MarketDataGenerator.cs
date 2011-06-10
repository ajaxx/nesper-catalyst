///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

namespace NEsper.Catalyst.SampleClient
{
    class MarketDataGenerator : IEnumerable<MarketDataEvent>
    {
        private readonly Random _marketDataRandomizer =
            new Random();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<MarketDataEvent> GetEnumerator()
        {
            MarketDataEvent marketDataEvent = new MarketDataEvent(
                "GOOG",
                Math.Round(_marketDataRandomizer.NextDouble()*100.0, 2) + 500.0,
                _marketDataRandomizer.Next(1, 10)*100,
                Math.Round(_marketDataRandomizer.NextDouble()*100.0, 2) + 600.0,
                _marketDataRandomizer.Next(1, 10)*100);
            yield return marketDataEvent;
        }
    }
}
