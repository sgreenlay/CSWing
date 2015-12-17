using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSWing.Stockfighter.Helpers;

namespace CSWing
{
    class SellSide
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

            var account = "TPB77289763";

            var exchange = "OTUOEX";
            var stock = "RLH";

            var isExchangeOnline = await api.IsVenueOnline(exchange);

            if (!isExchangeOnline)
            {
                Console.WriteLine("Exchange is offline");
                return;
            }

            int windowSize = 120;

            Queue<int> runningBuyingPrice = new Queue<int>();

            while (runningBuyingPrice.Count < windowSize)
            {
                var book = await api.GetOrderBook(exchange, stock);

                if (book == null)
                {
                    continue;
                }

                if (book.Bids != null)
                {
                    if (runningBuyingPrice.Count >= windowSize)
                    {
                        runningBuyingPrice.Dequeue();
                    }

                    runningBuyingPrice.Enqueue(book.Bids[0].Price);
                }

                Thread.Sleep(50);
            }

            int totalCash = 0;
            int totalValue = 0;
            int desiredProfit = 1000000;

            int maxShares = 1000;
            int totalShares = 0;

            Portfolio portfolio = new Portfolio();

            do
            {
                Thread.Sleep(50);

                var quote = await api.GetStockQuote(exchange, stock);
                var book = await api.GetOrderBook(exchange, stock);

                if (book == null)
                {
                    continue;
                }

                if (book.Bids != null)
                {
                    runningBuyingPrice.Dequeue();
                    runningBuyingPrice.Enqueue(book.Bids[0].Price);

                    var cheaperSharesOwned = portfolio.CountHeldPositionsCheaperThan(stock, book.Bids[0].Price);

                    if (cheaperSharesOwned > 0)
                    {
                        var order = await api.PlaceOrder(
                            account,
                            exchange,
                            stock,
                            book.Bids[0].Price,
                            cheaperSharesOwned,
                            "sell",
                            "immediate-or-cancel");

                        Console.WriteLine("{0} shares of {1} sold @ {2:C2}",
                            order.Quantity,
                            stock,
                            (float)order.Price / 100.0f);

                        portfolio.RemovePositions(stock, order.Quantity, order.Price);

                        totalShares -= order.Quantity;
                        totalCash += order.Quantity * order.Price;

                        Console.WriteLine("{0:C2} cash, {1} @ {2:C2} = {3:C2} stock, {4:C2} total",
                            (float)totalCash / 100.0f,
                            totalShares,
                            (float)quote.Last / 100.0f,
                            (float)(quote.Last * totalShares) / 100.0f,
                            (float)(totalCash + quote.Last * totalShares) / 100.0f);
                    }
                }

                if ((book.Asks != null) && (totalShares < (maxShares / 2)))
                {
                    if (book.Asks[0].Price < runningBuyingPrice.Max())
                    {
                        var order = await api.PlaceOrder(
                           account,
                           exchange,
                           stock,
                           book.Asks[0].Price,
                           book.Asks[0].Qty,
                           "buy",
                           "immediate-or-cancel");

                        Console.WriteLine("{0} shares of {1} purchased @ {2:C2}",
                            order.Quantity,
                            stock,
                            (float)order.Price / 100.0f);

                        portfolio.AddPositions(stock, order.Quantity, order.Price);

                        totalShares += order.Quantity;
                        totalCash -= order.Quantity * order.Price;

                        Console.WriteLine("{0:C2} cash, {1} @ {2:C2} = {3:C2} stock, {4:C2} total",
                            (float)totalCash / 100.0f,
                            totalShares,
                            (float)quote.Last / 100.0f,
                            (float)(quote.Last * totalShares) / 100.0f,
                            (float)(totalCash + quote.Last * totalShares) / 100.0f);
                    }
                }

                totalValue = totalCash + quote.Last * totalShares;

            } while (totalValue < desiredProfit);
        }
    }
}
