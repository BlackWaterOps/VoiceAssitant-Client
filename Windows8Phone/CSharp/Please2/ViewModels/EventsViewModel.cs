﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class EventsViewModel : ListViewModel, IViewModel
    {
        private List<EventModel> _eventResults;
        public List<EventModel> EventResults
        {
            get { return _eventResults; }
            set { _eventResults = value; }
        }

        public RelayCommand<EventModel> EventItemSelection { get; set; }
        public RelayCommand MapLoaded { get; set; }

        public EventsViewModel()
        {
            // attach event handlers
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
            MapLoaded = new RelayCommand(BuildMap);
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<EventModel>>();

            this.Scheme = ColorScheme.Commerce;
            this.Title = "events";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void EventItemSelected(EventModel model)
        {
            EventDetailsViewModel vm = ViewModelLocator.GetServiceInstance<EventDetailsViewModel>();

            vm.CurrentItem = model;

            navigationService.NavigateTo(new Uri("/Views/EventDetails.xaml", UriKind.Relative));           
        }

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().Where(x => x.Layers.Count == 0).FirstOrDefault();

            if (map != null)
            {
                List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                // create list so we can get index of elements
                List<object> itemsList = Items.ToList();

                foreach (EventModel item in itemsList)
                {
                    double lat = item.location.lat;
                    double lon = item.location.lon;

                    GeoCoordinate geo = new GeoCoordinate(lat, lon);

                    geoList.Add(geo);

                    try
                    {
                        MapLayer mapLayer = MapService.Default.CreateMapLayer(lat, lon, (itemsList.IndexOf(item) + 1));

                        map.Layers.Add(mapLayer);
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine(err.Message);
                    }
                }

                if (geoList.Count > 0)
                {
                    LocationRectangle rect = LocationRectangle.CreateBoundingRectangle(geoList);
                    map.SetView(rect);
                }
            }
        }
    }
}
