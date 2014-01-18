using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;

using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class FuelDetailsViewModel : DetailsViewModel
    {
        public RelayCommand FuelDirectionsLoaded { get; set; }

        public FuelDetailsViewModel(INavigationService navigationService, IPlexiService plexiService) 
            : base(navigationService, plexiService)
        {
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand(PinToStart);
            ShowFullMapCommand = new RelayCommand(ShowFullMap);

            FuelDirectionsLoaded = new RelayCommand(AddDirectionsMap);
        }

        public void PinToStart()
        {
            AltFuelModel fuel = CurrentItem as AltFuelModel;

            Uri uri = new Uri(String.Format("/Views/Details.xaml?id={0}", fuel.id));
            string title = fuel.station_name;
            string content = fuel.address;
            
            base.PinToStart(uri, title, content, null);
        }

        public void AddDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().FirstOrDefault();

            if (map != null)
            {
                var item = CurrentItem as AltFuelModel;

                var layer = base.CreateMapLayer(item.latitude, item.longitude);

                map.Layers.Add(layer);
                map.Center = new GeoCoordinate(item.latitude, item.longitude);
            }
        }

        public async void ShowFullMap()
        {
            AltFuelModel fuel = CurrentItem as AltFuelModel;
            
            GeoCoordinate geo = new GeoCoordinate(fuel.latitude, fuel.longitude);
            string title = fuel.station_name;

            await base.ShowFullMap(title, geo);
        }
    }
}
