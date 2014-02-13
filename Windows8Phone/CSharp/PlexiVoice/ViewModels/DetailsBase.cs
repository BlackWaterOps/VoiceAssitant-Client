using System;
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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using PlexiVoice.Util;
using PlexiVoice.Models;

using PlexiSDK;
using PlexiSDK.Util;

namespace PlexiVoice.ViewModels
{
    public class DetailsBase : ViewModelBase
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

        public RelayCommand PinToStartCommand { get; set; }
        public RelayCommand ShowFullMapCommand { get; set; }

        INavigationService navigationService;
        IPlexiService plexiService;

        public DetailsBase()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
        }

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
    }
}
