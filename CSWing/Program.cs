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

            var isTestExOnline = await api.IsVenueOnline("TESTEX");

            if (!isTestExOnline)
            {
                Console.WriteLine("TESTEX is offline");
                Console.ReadKey();

                return;
            }

            var testExStockList = await api.GetStockList("TESTEX");

            foreach (var symbol in testExStockList)
            {
                Console.WriteLine("[{0}] {1}", symbol.Symbol, symbol.Name);
            }

            Console.ReadKey();
        }
    }
}
