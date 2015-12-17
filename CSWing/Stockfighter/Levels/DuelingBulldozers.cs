using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSWing.Stockfighter.Helpers;

namespace CSWing
{
    class DuelingBulldozers
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

            var account = "HFW91235834";

            var exchange = "YPEMEX";
            var stock = "SRC";

            var isExchangeOnline = await api.IsVenueOnline(exchange);

            if (!isExchangeOnline)
            {
                Console.WriteLine("Exchange is offline");
                return;
            }

            // TODO
        }
    }
}
