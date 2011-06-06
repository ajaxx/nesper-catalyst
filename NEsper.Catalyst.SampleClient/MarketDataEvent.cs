using System.Runtime.Serialization;

namespace NEsper.Catalyst.SampleClient
{
    [DataContract]
    public class MarketDataEvent
    {
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public double Bid { get; set; }
        [DataMember]
        public int BidSize { get; set; }
        [DataMember]
        public double Ask { get; set; }
        [DataMember]
        public int AskSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataEvent"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="bid">The bid.</param>
        /// <param name="bidSize">Size of the bid.</param>
        /// <param name="ask">The ask.</param>
        /// <param name="askSize">Size of the ask.</param>
        public MarketDataEvent(string symbol, double bid, int bidSize, double ask, int askSize)
        {
            Symbol = symbol;
            Bid = bid;
            BidSize = bidSize;
            Ask = ask;
            AskSize = askSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataEvent"/> class.
        /// </summary>
        public MarketDataEvent()
        {
        }
    }
}
