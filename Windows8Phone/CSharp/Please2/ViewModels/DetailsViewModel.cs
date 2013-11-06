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
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using LinqToVisualTree;

using GalaSoft.MvvmLight.Command;

using Please2.Util;
using Please2.Models;

using Plexi;
namespace Please2.ViewModels
{
    public class DetailsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private Map currentMap;
        
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
        public RelayCommand<string> ShowFullMapCommand { get; set; }

        public RelayCommand EventDirectionsLoaded { get; set; }
        public RelayCommand ListingDirectionsLoaded { get; set; }
        public RelayCommand FuelDirectionsLoaded { get; set; }

        IPlexiService plexiService;

        public DetailsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.plexiService = plexiService;

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
            ShowFullMapCommand = new RelayCommand<string>(ShowFullMap);

            EventDirectionsLoaded = new RelayCommand(AddDirectionsMap);
            ListingDirectionsLoaded = new RelayCommand(AddListingDirectionsMap);
            FuelDirectionsLoaded = new RelayCommand(AddFuelDirectionsMap);
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

        private async void ShowFullMap(string e)
        {
            GeoCoordinate geo = null;

            string title = null;

            switch (e)
            {
                case "real_estate":
                    RealEstateListing listing = CurrentItem as RealEstateListing;
                    geo = new GeoCoordinate(listing.location.latitude, listing.location.longitude);
                    title = listing.title;
                    break;

                case "fuel":
                    AltFuelModel fuel = CurrentItem as AltFuelModel;
                    geo = new GeoCoordinate(fuel.latitude, fuel.longitude);
                    title = fuel.station_name;
                    break;

                case "events":
                    EventModel ev = CurrentItem as EventModel;
                    geo = new GeoCoordinate(ev.location.lat, ev.location.lon);
                    title = ev.title;
                    break;
            }

            if (geo != null && title != null)
            {
                var fullMap = new MapsDirectionsTask();

                var currPos = await plexiService.GetDeviceInfo();

                var startGeo = new GeoCoordinate((double)currPos["latitude"], (double)currPos["longitude"]);

                var startLabeledMap = new LabeledMapLocation("current location", startGeo);

                fullMap.Start = startLabeledMap;

                var endLabeledMap = new LabeledMapLocation(title, geo);

                fullMap.End = endLabeledMap;

                fullMap.Show();
            }
            else
            {
                Debug.WriteLine("show full map: unfulfilled geo or title");
            }
        }

        // Do to the lack of binding support in the maps api, we have to
        // resort to adding the map layer in code. XAML binding would be preferred.
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

        private async void AddListingDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                currentMap = maps.Single();
                
                var item = CurrentItem as RealEstateListing;
                
                /*
                List<GeoCoordinate> coords = new List<GeoCoordinate>();

                Dictionary<string, object> deviceInfo = await plexiService.GetDeviceInfo();
                
                coords.Add(new GeoCoordinate((double)deviceInfo["latitude"], (double)deviceInfo["longitude"]));
                coords.Add(new GeoCoordinate(item.location.latitude, item.location.longitude));

                RouteQuery routeQuery = new RouteQuery();
                routeQuery.Waypoints = coords;
                routeQuery.QueryCompleted += RouteQuery_Completed;
                routeQuery.QueryAsync();
                */

                var layer = CreateMapLayer(item.location.latitude, item.location.longitude);

                currentMap.Layers.Add(layer);
               
                //currentMap.Center = new GeoCoordinate((double)deviceInfo["latitude"], (double)deviceInfo["longitude"]); 
                currentMap.Center = new GeoCoordinate(item.location.latitude, item.location.longitude);
            }
        }

        private void AddFuelDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                var map = maps.Single();

                var item = CurrentItem as AltFuelModel;

                var layer = CreateMapLayer(item.latitude, item.longitude);

                map.Layers.Add(layer);
                map.Center = new GeoCoordinate(item.latitude, item.longitude);
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
        /*
        private void RouteQuery_Completed(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                
                Route route = e.Result;
                MapRoute mapRoute = new MapRoute(route);

                if (currentMap != null)
                {
                    currentMap.AddRoute(mapRoute);
                }
            }
        }
        */
    }
}
