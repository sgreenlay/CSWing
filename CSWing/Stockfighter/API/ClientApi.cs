using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                throw new Exception("Failed to connect to server");
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
                throw new Exception("Failed to connect to server");
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
                throw new Exception("Failed to execute command");
            }

            return data.Symbols;
        }

        public class Bid
        {
            public int Price { get; set; }
            public int Qty { get; set; }
            public bool IsBuy { get; set; }
        }

        public class Book
        {
            public IList<Bid> Bids { get; set; }
            public IList<Bid> Asks { get; set; }
        }

        class OrderbookResponse : Response
        {
            public string Venue { get; set; }
            public string Symbol { get; set; }

            public IList<Bid> Bids { get; set; }
            public IList<Bid> Asks { get; set; }

            public DateTime Ts { get; set; }
        }

        public async Task<Book> GetOrderBook(string venue, string symbol)
        {
            var res = await httpClient.GetAsync(string.Format("venues/{0}/stocks/{1}", venue, symbol));

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to connect to server");
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OrderbookResponse>(json);

            if (!data.Ok)
            {
                throw new Exception("Failed to execute command");
            }

            return new Book { Bids = data.Bids, Asks = data.Asks };
        }

        class OrderQuery
        {
            public string Account { get; set; }

            public string Venue { get; set; }
            public string Stock { get; set; }

            public int Price { get; set; }
            public int Qty { get; set; }

            public string Direction { get; set; }

            public string OrderType { get; set; }
        }

        class OrderResponse : Response
        {
            public int Id { get; set; }
            public string Account { get; set; }

            public string Venue { get; set; }
            public string Symbol { get; set; }

            public string Direction { get; set; }

            public int OriginalQty { get; set; }
            public int Qty { get; set; }

            public int Price { get; set; }
            public string Type { get; set; }

            public class Fill
            {
                public int Price { get; set; }
                public int Qty { get; set; }
                public DateTime Ts { get; set; }
            }

            public IList<Fill> Fills { get; set; }
            public DateTime Ts { get; set; }

            public int TotalFilled { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; }
            public int Outstanding { get; set; }
        }

        public async Task<Order> PlaceOrder(
            string account,
            string venue,
            string symbol,
            int price,
            int qty,
            string direction,
            string ordertype)
        {
            var query = new OrderQuery();

            query.Account = account;
            query.Venue = venue;
            query.Stock = symbol;
            query.Price = price;
            query.Qty = qty;
            query.Direction = direction;
            query.OrderType = ordertype;

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var jsonQuery = JsonConvert.SerializeObject(query, Formatting.Indented, settings);
            var content = new StringContent(jsonQuery, Encoding.UTF8, "application/json");

            var res = await httpClient.PostAsync(string.Format("venues/{0}/stocks/{1}/orders", venue, symbol), content);

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to connect to server");
            }

            var jsonResponse = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);

            if (!data.Ok)
            {
                throw new Exception("Failed to execute command");
            }

            return new Order { Id = data.Id, Price = data.Price, Quantity = data.TotalFilled, Outstanding = data.Qty };
        }

        public async Task<Order> GetOrderStatus(
            string venue,
            string symbol,
            int id)
        {
            var res = await httpClient.GetAsync(string.Format("venues/{0}/stocks/{1}/orders/{2}", venue, symbol, id));

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to connect to server");
            }

            var jsonResponse = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);

            if (!data.Ok)
            {
                throw new Exception("Failed to execute command");
            }

            return new Order { Id = data.Id, Price = data.Price, Quantity = data.TotalFilled, Outstanding = data.Qty };
        }

        public async Task<Order> CancelOrder(
            string venue,
            string symbol,
            int id)
        {
            var res = await httpClient.DeleteAsync(string.Format("venues/{0}/stocks/{1}/orders/{2}", venue, symbol, id));

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to connect to server");
            }

            var jsonResponse = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);

            if (!data.Ok)
            {
                throw new Exception("Failed to execute command");
            }

            return new Order { Id = data.Id, Price = data.Price, Quantity = data.TotalFilled, Outstanding = data.Qty };
        }
    }
}
