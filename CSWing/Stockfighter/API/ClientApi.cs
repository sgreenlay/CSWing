using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSWing.Stockfighter.Api
{
    public class Client
    {
        HttpClient httpClient;

        public Client(string authToken)
        {
            httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://api.stockfighter.io/ob/api/");
            httpClient.DefaultRequestHeaders.Add("X-Stockfighter-Authorization", authToken);
        }

        class Response
        {
            public bool Ok { get; set; }
        }

        class HeartbeatResponse : Response
        {
            public string Error { get; set; }
        }

        public async Task<bool> IsOnline()
        {
            var res = await httpClient.GetAsync("heartbeat");

            if (!res.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<HeartbeatResponse>(json);

            return data.Ok;
        }

        class VenueHeartbeatResponse : Response
        {
            public string Venue { get; set; }
        }
        
        public async Task<bool> IsVenueOnline(string venue)
        {
            var res = await httpClient.GetAsync(string.Format("venues/{0}/heartbeat", venue));

            if (!res.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<VenueHeartbeatResponse>(json);

            return data.Ok;
        }

        public class Stock
        {
            public string Name { get; set; }
            public string Symbol { get; set; }
        }

        class StocklistResponse : Response
        {
            public IList<Stock> Symbols { get; set; }
        }

        public async Task<IList<Stock>> GetStockList(string venue)
        {
            var res = await httpClient.GetAsync(string.Format("venues/{0}/stocks", venue));

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<StocklistResponse>(json);

            if (!data.Ok)
            {
                return null;
            }

            return data.Symbols;
        }

        class OrderbookResponse : Response
        {
            public string Venue { get; set; }
            public string Symbol { get; set; }

            public class Bid
            {
                public int Price { get; set; }
                public int Qty { get; set; }
                public bool IsBuy { get; set; }
            }

            public IList<Bid> Bids { get; set; }
            public DateTime Ts { get; set; }
        }

        public async Task GetOrderBook(string venue, string symbol)
        {
            var res = await httpClient.GetAsync(string.Format("venues/{0}/stocks/{1}", venue, symbol));

            if (!res.IsSuccessStatusCode)
            {
                return;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OrderbookResponse>(json);

            if (!data.Ok)
            {
                return;
            }

            // TODO

            return;
        }
    }
}
