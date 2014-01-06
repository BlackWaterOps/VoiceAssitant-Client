﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
using Plexi.Util;

namespace Please2.ViewModels
{
    public class DetailsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        //xprivate Map currentMap;

        private ColorScheme scheme;
        public ColorScheme Scheme
        {
            get { return scheme; }
            set 
            {
                scheme = value;
                RaisePropertyChanged("Scheme");
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

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

        private ObservableCollection<object> pivotItems;
        public ObservableCollection<object> PivotItems
        {
            get { return pivotItems; }
            set
            {
                pivotItems = value;
                RaisePropertyChanged("PivotItems");
            }
        }


        public RelayCommand PinToStartCommand { get; set; }
        public RelayCommand ShowFullMapCommand { get; set; }

        //public RelayCommand EventDirectionsLoaded { get; set; }
        //public RelayCommand ListingDirectionsLoaded { get; set; }
        //public RelayCommand FuelDirectionsLoaded { get; set; }

        IPlexiService plexiService;

        public DetailsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.plexiService = plexiService;

            //AttachEventHandlers();
        }

        public void SetDetailsTemplate(string template)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            if (templates[template] != null)
            {
                ContentTemplate = templates[template] as DataTemplate;
            }

            string control = String.Format("{0}_control", template);
            if (templates[control] != null)
            {

            }
        }

        /*
        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand<object>(PinToStart);
            ShowFullMapCommand = new RelayCommand<string>(ShowFullMap);

            EventDirectionsLoaded = new RelayCommand(AddDirectionsMap);
            ListingDirectionsLoaded = new RelayCommand(AddListingDirectionsMap);
            FuelDirectionsLoaded = new RelayCommand(AddFuelDirectionsMap);
        }
        */

        protected void PinToStart(Uri uri, string title, string content, string image)
        {
            var tile = new FlipTileData();

            if (image != null)
            {
                tile.BackgroundImage = new Uri(image, UriKind.Absolute);
            }

            tile.BackContent = content;
            tile.Title = tile.BackTitle = title;
            tile.Count = 0;

            ShellTile.Create(uri, tile);
        }

        /*
        protected void PinToStart(object e)
        {
            Type t = currentItem.GetType();

            string id = null;
            string image = null;
            string title = null;
            string content = null;

            if (t == typeof(RealEstateListing))
            {
                RealEstateListing listing = CurrentItem as RealEstateListing;
                title = listing.title;
                content = listing.location.address;
                image = (listing.images as List<RealEstateImage>).ElementAt(0).src;
                id = listing.id;
            }

            if (t == typeof(AltFuelModel))
            {
                AltFuelModel fuel = CurrentItem as AltFuelModel;
                title = fuel.station_name;
                content = fuel.address;
                id = fuel.id.ToString();
            }

            if (t == typeof(EventModel))
            {
                EventModel ev = CurrentItem as EventModel;
                title = ev.title;
                content = ev.location.address;
                image = ev.image;
                id = ev.id;
            }

            Uri uri = new Uri(String.Format("/EventDetailsPage.xaml?id={0}", id));

            PinToStart(uri, title, content, image);
        }
        */

        protected async Task ShowFullMap(string title, GeoCoordinate geo)
        {
            if (geo != null && title != null)
            {
                var fullMap = new MapsDirectionsTask();

                var currPos = await plexiService.GetDeviceInfo();

                var startGeo = new GeoCoordinate(currPos.geoCoordinate.Latitude, currPos.geoCoordinate.Longitude);

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

        /*
        protected async void ShowFullMap(string e)
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

            await ShowFullMap(title, geo);
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

        private void AddListingDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                currentMap = maps.Single();
                
                var item = CurrentItem as RealEstateListing;
                
                var layer = CreateMapLayer(item.location.latitude, item.location.longitude);

                currentMap.Layers.Add(layer);
               
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
        */

        protected MapLayer CreateMapLayer(double lat, double lon)
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
