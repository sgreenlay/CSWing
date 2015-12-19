using System;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;

namespace CSWing.Helpers
{
    class Logger
    {
        public static async Task RunAsync(string apiKey)
        {
            var api = new Stockfighter.Api.Client(apiKey);

            var isOnline = await api.IsOnline();

            if (!isOnline)
            {
                Console.WriteLine("Stockfighter is offline");
                Console.ReadKey();

                return;
            }

            var account = "YMS99170543";

            var exchange = "NTIBEX";
            var stock = "FRI";

            var isExchangeOnline = await api.IsVenueOnline(exchange);

            if (!isExchangeOnline)
            {
                Console.WriteLine("Exchange is offline");
                return;
            }

            var logpath = string.Format("logs/{0}.json", DateTime.Now.ToString("lvl4 yyyy-MM-dd hh-mm-ss"));

            var log = new StreamWriter(logpath);
            log.AutoFlush = true;

            log.Write("{");
            log.Write("\"venue\":\"{0}\",", exchange);
            log.Write("\"symbol\":\"{0}\",", stock);
            log.Write("\"history\":[");

            while (true)
            {
                var book = await api.GetOrderBook(exchange, stock);

                log.Write(JsonConvert.SerializeObject(book));
                log.Write(",");
            }

            log.Write("]}");
        }
    }
}
