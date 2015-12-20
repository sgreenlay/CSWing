using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stockfighter.Api
{
    public class Simulation
    {
        public class Response
        {
            public bool Ok { get; set; }
        }

        public class Bid
        {
            public int Price { get; set; }
            public int Qty { get; set; }
            public bool IsBuy { get; set; }
        }

        public class OrderbookResponse : Response
        {
            public string Venue { get; set; }
            public string Symbol { get; set; }

            public IList<Bid> Bids { get; set; }
            public IList<Bid> Asks { get; set; }

            public DateTime Ts { get; set; }
        }

        public class OrderbookCache
        {
            public string Venue { get; set; }
            public string Symbol { get; set; }

            public IList<OrderbookResponse> History { get; set; }
        }

        public OrderbookCache cache { get; }

        public class TimeRange
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        public TimeRange Timeline { get; set; }

        public class OrderRange
        {
            public Bid Min { get; set; }
            public Bid Max { get; set; }
        }

        public OrderRange AskRange { get; set; }
        public OrderRange BidRange { get; set; }

        public Simulation(String json)
        {
            cache = JsonConvert.DeserializeObject<OrderbookCache>(json);

            foreach (var entry in cache.History)
            {
                if (Timeline == null)
                {
                    Timeline = new TimeRange { Start = entry.Ts, End = entry.Ts };
                }
                else
                {
                    if (entry.Ts < Timeline.Start)
                    {
                        Timeline.Start = entry.Ts;
                    }
                    else if (entry.Ts > Timeline.End)
                    {
                        Timeline.End = entry.Ts;
                    }
                }

                if (entry.Asks != null)
                {
                    foreach (var ask in entry.Asks)
                    {
                        if (AskRange == null)
                        {
                            AskRange = new OrderRange { Min = ask, Max = ask };
                        }
                        else if (ask.Price < AskRange.Min.Price)
                        {
                            AskRange.Min = ask;
                        }
                        else if (ask.Price > AskRange.Max.Price)
                        {
                            AskRange.Max = ask;
                        }
                    }
                }
                if (entry.Bids != null)
                {
                    foreach (var bid in entry.Bids)
                    {
                        if (BidRange == null)
                        {
                            BidRange = new OrderRange { Min = bid, Max = bid };
                        }
                        else if (bid.Price < BidRange.Min.Price)
                        {
                            BidRange.Min = bid;
                        }
                        if (bid.Price > BidRange.Max.Price)
                        {
                            BidRange.Max = bid;
                        }
                    }
                }
            }
        }
    }
}
