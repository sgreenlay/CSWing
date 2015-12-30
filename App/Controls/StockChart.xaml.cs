
using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;

using Microsoft.Graphics.Canvas.UI.Xaml;

using Stockfighter.Api;

namespace App.Controls
{
    public sealed partial class StockChart : UserControl
    {
        public Simulation Simulation { get; set; }

        public StockChart()
        {
            this.InitializeComponent();
        }

        public void Invalidate()
        {
            Graph.Invalidate();
        }

        void Redraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Simulation == null)
            {
                args.DrawingSession.DrawText("Loading...", 20, 20, Colors.Black);
            }
            else
            {
                var yMin = Math.Min(Simulation.AskRange.Min.Price, Simulation.BidRange.Min.Price);
                var yRange = Math.Max(Simulation.AskRange.Max.Price, Simulation.BidRange.Max.Price) - yMin;

                var xMin = Simulation.Timeline.Start.Ticks;
                var xRange = Simulation.Timeline.End.Ticks - xMin;

                foreach (var entry in Simulation.cache.History)
                {
                    var x = ((float)(entry.Ts.Ticks - xMin) / (float)xRange) * sender.ActualWidth;

                    if (entry.Bids != null)
                    {
                        foreach (var bid in entry.Bids)
                        {
                            var y = ((float)(bid.Price - yMin) / (float)yRange) * sender.ActualHeight;

                            args.DrawingSession.FillCircle((float)x, (float)y, 2, Colors.Green);
                        }
                    }
                    if (entry.Asks != null)
                    {
                        foreach (var ask in entry.Asks)
                        {
                            var y = ((float)(ask.Price - yMin) / (float)yRange) * sender.ActualHeight;

                            args.DrawingSession.FillCircle((float)x, (float)y, 2, Colors.Red);
                        }
                    }
                }
            }
        }
    }
}
