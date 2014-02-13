using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class FlightsViewModel : ViewModelBase, IViewModel
    {
        private List<Flight> flights;
        public List<Flight> Flights
        {
            get { return flights; }
            set
            {
                flights = value;
                RaisePropertyChanged("Flights");
            }
        }

        private FlightAirline airline;
        public FlightAirline Airline
        {
            get { return airline; }
            set
            {
                airline = value;
                RaisePropertyChanged("Airline");
            }
        }

        private string flightNumber;
        public string FlightNumber
        {
            get { return flightNumber; }
            set
            {
                flightNumber = value;
                RaisePropertyChanged("FlightNumber");
            }
        }

        public RelayCommand MapLoaded { get; set; }
        
        INavigationService navigationService;

        public FlightsViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            MapLoaded = new RelayCommand(BuildMap);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            var flightResults = (structured["item"] as JObject).ToObject<FlightModel>();

            // use recent result for now
            // would be nice to disambiguate when there are multiple flights
            // Flights = flightResults.details;
            Flights = new List<Flight>();
            Flights.Add(flightResults.details[0]);

            Airline = flightResults.airline;
            FlightNumber = flightResults.flight_number;
        }

        private async void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                Map map = maps.Single();

                Flight flight = Flights.FirstOrDefault();

                // get origin coordinates
                IList<MapLocation> og = await MapService.Default.GeoQuery(flight.origin.city);

                Debug.WriteLine(String.Format("{0}:{1}", og.First().GeoCoordinate.Latitude, og.First().GeoCoordinate.Longitude));

                GeoCoordinate origin = new GeoCoordinate(og.First().GeoCoordinate.Latitude, og.First().GeoCoordinate.Latitude);

                // get destination coordinates
                IList<MapLocation> dg = await MapService.Default.GeoQuery(flight.destination.city);

                Debug.WriteLine(String.Format("{0}:{1}", dg.First().GeoCoordinate.Latitude, dg.First().GeoCoordinate.Longitude));

                GeoCoordinate destination = new GeoCoordinate(dg.First().GeoCoordinate.Latitude, dg.First().GeoCoordinate.Longitude);

                // build list of coordinates to plot a line
                List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                geoList.Add(origin);
                geoList.Add(destination);

                MapPolyline polyline = MapService.Default.CreatePolyline(geoList);

                map.MapElements.Add(polyline);

                MapLayer originLayer = MapService.Default.CreateMapLayer(origin);
                MapLayer destinationLayer = MapService.Default.CreateMapLayer(destination);

                map.Layers.Add(originLayer);
                map.Layers.Add(destinationLayer);

                map.Center = origin;
            }
        }
    }
}
