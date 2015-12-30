
using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

using Stockfighter.Api;

namespace App
{
    public sealed partial class MainPage : Page
    {
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

            Chart.Simulation = new Simulation(json);
            Chart.Invalidate();
        }
    }
}
