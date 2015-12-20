using Microsoft.Graphics.Canvas.UI.Xaml;
using Stockfighter.Api;
using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace App
{
    public sealed partial class MainPage : Page
    {
        Simulation simulation { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            LoadSimulationDataAsync();
        }

        async void LoadSimulationDataAsync()
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Resources");
            var file = await folder.GetFileAsync("lvl4 2015-12-18 09-03-44.json");

            var json = await FileIO.ReadTextAsync(file);
            simulation = new Simulation(json);

            HistoryGraph.Invalidate();
        }

        void Redraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (simulation == null)
            {
                args.DrawingSession.DrawText("Loading...", 20, 20, Colors.Black);
            }
            else
            {
                var yMin = Math.Min(simulation.AskRange.Min.Price, simulation.BidRange.Min.Price);
                var yRange = Math.Max(simulation.AskRange.Max.Price, simulation.BidRange.Max.Price) - yMin;

                var xMin = simulation.Timeline.Start.Ticks;
                var xRange = simulation.Timeline.End.Ticks - xMin;

                foreach (var entry in simulation.cache.History)
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
