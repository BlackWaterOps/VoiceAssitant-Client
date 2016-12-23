using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;

namespace PlexiVoice.ViewModels
{
    public class GeopoliticsViewModel : ViewModelBase, IViewModel
    {        
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value;
                RaisePropertyChanged("Country");
            }
        }

        private string flag;
        public string Flag
        {
            get { return flag; }
            set
            {
                flag = value;
                RaisePropertyChanged("Flag");
            }
        }

        private GeopoliticsStats stats;
        public GeopoliticsStats Stats
        {
            get { return stats; }
            set
            {
                stats = value;
                RaisePropertyChanged("Stats");
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }

        public RelayCommand MapLoaded { get; set; }

        public GeopoliticsViewModel()
        {
            MapLoaded = new RelayCommand(BuildMap);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            var geoResults = (structured["item"] as JObject).ToObject<GeopoliticsModel>();

            Flag = geoResults.flag;
            Country = geoResults.country;
            Stats = geoResults.stats;
            Text = geoResults.text;
        }

        private async void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().Where(x => x.Layers.Count == 0).FirstOrDefault();

            if (map != null)
            {
                IList<MapLocation> response = await MapService.Default.GeoQuery(Country);

                MapLocation mapLocation = response.First();

                Debug.WriteLine(JsonConvert.SerializeObject(mapLocation.Information));

                GeoCoordinate geo = mapLocation.GeoCoordinate;

                MapLayer layer = MapService.Default.CreateMapLayer(geo.Latitude, geo.Longitude);

                map.Layers.Add(layer);

                map.SetView(mapLocation.BoundingBox);

               // map.Center = new GeoCoordinate(geo.Latitude, geo.Longitude);
            }
        }
    }
}
