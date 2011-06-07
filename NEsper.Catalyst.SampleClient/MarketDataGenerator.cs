///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace NEsper.Catalyst.SampleClient
{
    class MarketDataGenerator
    {
        private readonly Random _marketDataRandomizer =
            new Random();

        public MarketDataEvent NextEvent()
        {
            MarketDataEvent marketDataEvent = new MarketDataEvent(
                "GOOG",
                Math.Round(_marketDataRandomizer.NextDouble()*100.0, 2) + 500.0,
                _marketDataRandomizer.Next(1, 10)*100,
                Math.Round(_marketDataRandomizer.NextDouble()*100.0, 2) + 600.0,
                _marketDataRandomizer.Next(1, 10)*100);
            return marketDataEvent;
        }
    }
}
