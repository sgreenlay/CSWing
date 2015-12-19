using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWing.Helpers
{
    class Portfolio
    {
        class PriceAndQuantity
        {
            public int PurchasePrice { get; set; }
            public int Quantity { get; set; }
        }

        Dictionary<string, List<PriceAndQuantity>> positionsHeld;

        public Portfolio()
        {
            positionsHeld = new Dictionary<string, List<PriceAndQuantity>>();
        }

        public void AddPositions(
            string symbol,
            int quantity,
            int purchasePrice)
        {
            if (!positionsHeld.ContainsKey(symbol))
            {
                positionsHeld[symbol] = new List<PriceAndQuantity>();
            }

            foreach (var priceAndQuantity in positionsHeld[symbol])
            {
                if (priceAndQuantity.PurchasePrice == purchasePrice)
                {
                    priceAndQuantity.Quantity += quantity;
                    return;
                }
            }

            positionsHeld[symbol].Add(new PriceAndQuantity { PurchasePrice = purchasePrice, Quantity = quantity });
        }

        class SortCheapestToMostExpensive : IComparer<PriceAndQuantity>
        {
            int IComparer<PriceAndQuantity>.Compare(PriceAndQuantity a, PriceAndQuantity b)
            {
                if (a.PurchasePrice > b.PurchasePrice)
                    return 1;
                if (a.PurchasePrice < b.PurchasePrice)
                    return -1;
                else
                    return 0;
            }
        }

        public int CountHeldPositionsCheaperThan(
            string symbol,
            int purchasePrice)
        {
            if (!positionsHeld.ContainsKey(symbol))
            {
                return 0;
            }

            positionsHeld[symbol].Sort(new SortCheapestToMostExpensive());

            var totalCount = 0;

            foreach (var priceAndQuantity in positionsHeld[symbol])
            {
                if (priceAndQuantity.PurchasePrice >= purchasePrice)
                {
                    return totalCount;
                }

                totalCount += priceAndQuantity.Quantity;
            }

            return totalCount;
        }

        public void RemovePositions(
            string symbol,
            int quantity,
            int salePrice)
        {
            if (!positionsHeld.ContainsKey(symbol))
            {
                throw new Exception("Can't sell stocks you don't own");
            }

            positionsHeld[symbol].Sort(new SortCheapestToMostExpensive());

            while (positionsHeld[symbol].Count > 0)
            {
                var priceAndQuantity = positionsHeld[symbol][0];

                if (priceAndQuantity.Quantity > quantity)
                {
                    priceAndQuantity.Quantity -= quantity;

                    return;
                }
                else
                {
                    quantity -= priceAndQuantity.Quantity;

                    positionsHeld[symbol].Remove(priceAndQuantity);
                }
            }

            if (quantity > 0)
            {
                throw new Exception("Can't sell stocks you don't own");
            }
        }
    }
}
