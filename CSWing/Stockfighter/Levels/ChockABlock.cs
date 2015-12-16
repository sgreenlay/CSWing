using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSWing
{
    class ChockABlock
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

            var account = "MFB70486931";

            var exchange = "UHOEX";
            var stock = "AREC";

            var isExchangeOnline = await api.IsVenueOnline(exchange);

            if (!isExchangeOnline)
            {
                Console.WriteLine("Exchange is offline");
                return;
            }
            
            Queue<int> runningLowestCost = new Queue<int>();

            while (runningLowestCost.Count < 60)
            {
                var book = await api.GetOrderBook(exchange, stock);

                if (book != null && book.Asks != null)
                {
                    runningLowestCost.Enqueue(book.Asks[0].Price);
                }

                Thread.Sleep(1000);
            }

            var totalQuantity = 100000;
            var acquiredStock = 0;

            while (acquiredStock < totalQuantity)
            {
                var book = await api.GetOrderBook(exchange, stock);

                if (book != null && book.Asks != null)
                {
                    runningLowestCost.Dequeue();
                    runningLowestCost.Enqueue(book.Asks[0].Price);

                    if (book.Asks[0].Price <= runningLowestCost.Min())
                    {
                        var order = await api.PlaceOrder(
                            account,
                            exchange,
                            stock,
                            book.Asks[0].Price,
                            book.Asks[0].Qty,
                            "buy",
                            "immediate-or-cancel");

                        Console.WriteLine("{0} shares of {1} purchased for {2}", order.Quantity, stock, order.Price);

                        acquiredStock += order.Quantity;

                        Console.WriteLine("{0} total shares of {1} acquired", acquiredStock, stock);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
