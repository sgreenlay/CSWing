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

            var account = "FTB64831451";

            var exchange = "LUWMEX";
            var stock = "KFI";

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

                Thread.Sleep(500);
            }

            var totalQuantity = 100000;
            var acquiredStock = 0;

            var outstandingOrder = 0;

            while (acquiredStock < totalQuantity)
            {
                var book = await api.GetOrderBook(exchange, stock);

                if (book != null && book.Asks != null)
                {
                    if (outstandingOrder != 0)
                    {
                        var order = await api.GetOrderStatus(exchange, stock, outstandingOrder);

                        if (order.Price > runningLowestCost.Min() || order.TotalFilled >= (totalQuantity / 10))
                        {
                            order = await api.CancelOrder(exchange, stock, outstandingOrder);

                            if (order.TotalFilled > 0)
                            {
                                Console.WriteLine("{0} shares of {1} purchased for {2}", order.TotalFilled, stock, order.Price);

                                acquiredStock += order.TotalFilled;

                                Console.WriteLine("{0} total shares of {1} acquired", acquiredStock, stock);
                            }

                            outstandingOrder = 0;
                        }
                    }

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

                        Console.WriteLine("{0} shares of {1} purchased for {2}", order.TotalFilled, stock, order.Price);

                        acquiredStock += order.TotalFilled;

                        Console.WriteLine("{0} total shares of {1} acquired", acquiredStock, stock);
                    }

                    runningLowestCost.Dequeue();
                    runningLowestCost.Enqueue(book.Asks[0].Price);

                    if (outstandingOrder == 0)
                    {
                        var order = await api.PlaceOrder(
                            account,
                            exchange,
                            stock,
                            runningLowestCost.Min(),
                            (totalQuantity / 10),
                            "buy",
                            "limit");

                        outstandingOrder = order.Id;
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}
