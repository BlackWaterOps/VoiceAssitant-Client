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

using LinqToVisualTree;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;

namespace Please2.ViewModels
{
    public class GeopoliticsViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        public ColorScheme Scheme { get { return ColorScheme.Information; } }
        
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

        public GeopoliticsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            MapLoaded = new RelayCommand(BuildMap);
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var geoResults = (structured["item"] as JObject).ToObject<GeopoliticsModel>();

            Flag = geoResults.flag;
            Country = geoResults.country;
            Stats = geoResults.stats;
            Text = geoResults.text;
        
            var data = new Dictionary<string, object>();

            data.Add("title", "Geopolitics");
            data.Add("scheme", this.Scheme);
            data.Add("margin", new Thickness(12, 24, 12, 24));

            return data;
        }

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                var map = maps.Single();

                GeoQuery(Country, (geo) =>
                    {
                        var layer = CreateMapLayer(geo.Latitude, geo.Longitude);

                        map.Layers.Add(layer);
                        map.Center = new GeoCoordinate(geo.Latitude, geo.Longitude); 
                    }
                );
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

        private void GeoQuery(string searchTerm, Action<GeoCoordinate> callback)
        {
            var query = new GeocodeQuery();
            query.GeoCoordinate = new GeoCoordinate(0, 0);
            query.SearchTerm = searchTerm;
            query.MaxResultCount = 5;

            query.QueryCompleted += (s, e) =>
            {
                // take first value for now.
                // possibly return all results and show list
                if (e.Result.Count > 0)
                {
                    var first = e.Result.FirstOrDefault();

                    var geo = first.GeoCoordinate;

                    callback(geo);
                }
            };

            query.QueryAsync();
        }
    }
}
