using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSWing
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync(args[0]).Wait();
        }

        static async Task RunAsync(string apiKey)
        {
            /*
            var gmApi = new Stockfighter.Api.Gm(apiKey);

            var instanceId = await gmApi.StartLevel("first_steps");

            if (instanceId == 0)
            {
                Console.WriteLine("Couldn't start a new instance");
                Console.ReadKey();

                return;
            }

            var status = await gmApi.GetLevelStatus(instanceId);

            if (!status)
            {
                Console.WriteLine("Couldn't get status of instance {0}", instanceId);
                Console.ReadKey();

                return;
            }

            var resumed = await gmApi.DoLevelOperation(instanceId, "resume");

            if (!resumed)
            {
                Console.WriteLine("Couldn't resume instance {0}", instanceId);
                Console.ReadKey();

                return;
            }

            var stopped = await gmApi.DoLevelOperation(instanceId, "stop");

            if (!stopped)
            {
                Console.WriteLine("Couldn't stop instance {0}", instanceId);
                Console.ReadKey();

                return;
            }
            */

            var api = new Stockfighter.Api.Client(apiKey);

            var isOnline = await api.IsOnline();

            if (!isOnline)
            {
                Console.WriteLine("Stockfighter is offline");
                Console.ReadKey();

                return;
            }

            var account = "EXB123456";

            var exchange = "TESTEX";
            var stock = "FOOBAR";

            var quantity = 10000;
            var price = 2000;

            var isExchangeOnline = await api.IsVenueOnline(exchange);

            if (!isExchangeOnline)
            {
                Console.WriteLine("Exchange is offline");
                Console.ReadKey();

                return;
            }

            var stockList = await api.GetStockList(exchange);

            foreach (var symbol in stockList)
            {
                Console.WriteLine("[{0}] {1}", symbol.Symbol, symbol.Name);
            }

            for (var i = 0; i < 10; ++i)
            {
                var order = await api.PlaceOrder(account, exchange, stock, price, quantity, "buy", "market");

                Console.WriteLine("{0} shares of {1} purchased for {2}", order.Quantity, stock, order.Price);
            }
        }
    }
}
