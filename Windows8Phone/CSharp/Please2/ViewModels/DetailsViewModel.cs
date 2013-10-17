using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using LinqToVisualTree;

using GalaSoft.MvvmLight.Command;

using Please2.Util;
using Please2.Models;

namespace Please2.ViewModels
{
    public class DetailsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private object currentItem;
        public object CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        private DataTemplate contentTemplate;
        public DataTemplate ContentTemplate
        {
            get { return contentTemplate; }
            set
            {
                contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        public RelayCommand<object> PinToStartCommand { get; set; }
        public RelayCommand<object> ShowFullMapCommand { get; set; }

        public RelayCommand EventDirectionsLoaded { get; set; }
        public RelayCommand ListingDirectionsLoaded { get; set; }


        public DetailsViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            AttachEventHandlers();
        }

        public void SetDetailsTemplate(string template)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            if (templates[template] != null)
            {
                ContentTemplate = templates[template] as DataTemplate;
            }
        }

        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand<object>(PinToStart);
            ShowFullMapCommand = new RelayCommand<object>(ShowFullMap);

            EventDirectionsLoaded = new RelayCommand(AddDirectionsMap);
            ListingDirectionsLoaded = new RelayCommand(AddListingDirectionsMap);
        }

        private void PinToStart(object e)
        {
            var tile = new FlipTileData();

           // tile.BackgroundImage = new Uri(currentEvent.image, UriKind.Absolute);
           // tile.BackContent = currentEvent.title;
           // tile.Title = tile.BackTitle = "please event";
           // tile.Count = 0;

           // ShellTile.Create(new Uri("/EventDetailsPage.xaml?id=" + currentEvent.id), tile);
        }

        private void ShowFullMap(object e)
        {
            /*
            var fullMap = new MapsDirectionsTask();

            var currPos = Please2.Util.Location.CurrentPosition;

            var startGeo = new GeoCoordinate((double)currPos["latitude"], (double)currPos["longitude"]);

            var startLabeledMap = new LabeledMapLocation("current location", startGeo);

            fullMap.Start = startLabeledMap;

            var endGeo = new GeoCoordinate(currentEvent.location.lat, currentEvent.location.lon);

            var endLabeledMap = new LabeledMapLocation(currentEvent.title, endGeo);

            fullMap.End = endLabeledMap;

            fullMap.Show();
            */
        }

        // Do to the lack of binding support in the maps api, we have to
        // resort to adding the map layer in code. XAML binding would be prefered.
        private void AddDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                var map = maps.Single();

                var item = CurrentItem as EventModel;

                var layer = CreateMapLayer(item.location.lat, item.location.lon);

                map.Layers.Add(layer);
                map.Center = new GeoCoordinate(item.location.lat, item.location.lon);
            }
        }

        private void AddListingDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                var map = maps.Single();

                var item = CurrentItem as RealEstateListing;

                var layer = CreateMapLayer(item.location.latitude, item.location.longitude);

                map.Layers.Add(layer);
                map.Center = new GeoCoordinate(item.location.latitude, item.location.longitude);
            }
        }

        private MapLayer CreateMapLayer(double lat, double lon)
        {
            var coord = new GeoCoordinate(lat, lon);

            var EventMapPushpin = new UserLocationMarker();

            MapOverlay overlay = new MapOverlay();

            overlay.Content = EventMapPushpin;

            overlay.GeoCoordinate = coord;

            overlay.PositionOrigin = new Point(0, 0.5);

            MapLayer layer = new MapLayer();

            layer.Add(overlay);

            return layer;
        }
    }
}
