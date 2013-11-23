using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;

using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class FlightsViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
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

        public FlightsViewModel(INavigationService navigationService, IPlexiService pleaseService)
        {
            this.navigationService = navigationService;

            MapLoaded = new RelayCommand(BuildMap);
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var flightResults = (structured["item"] as JObject).ToObject<FlightModel>();

            Flights = flightResults.details;
            Airline = flightResults.airline;
            FlightNumber = flightResults.flight_number;

            var data = new Dictionary<string, object>();

            data.Add("title", "flights");
            data.Add("subtitle", "flight results");
            //data.Add("subtitle", String.Format("flight results for \"{0}\"", originalQuery));
            data.Add("scheme", "default");

            return data;
        }

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var maps = currentPage.Descendants<Map>().Cast<Map>();

            if (maps.Count() > 0)
            {
                Map map = maps.Single();

                Flight flight = Flights.FirstOrDefault();

                //TODO: avoid nested callbacks
                MapService.GeoQuery(flight.origin.city, (og) =>
                {
                    Debug.WriteLine(String.Format("{0}:{1}", og.GeoCoordinate.Latitude, og.GeoCoordinate.Longitude));
                
                    GeoCoordinate origin = new GeoCoordinate(og.GeoCoordinate.Latitude, og.GeoCoordinate.Latitude);

                    MapService.GeoQuery(flight.destination.city, (dg) =>
                        {
                            Debug.WriteLine(String.Format("{0}:{1}", dg.GeoCoordinate.Latitude, dg.GeoCoordinate.Longitude));

                            GeoCoordinate destination = new GeoCoordinate(dg.GeoCoordinate.Latitude, dg.GeoCoordinate.Longitude);

                            List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                            geoList.Add(origin);
                            geoList.Add(destination);

                            MapPolyline polyline = MapService.CreatePolyline(geoList);
                            
                            map.MapElements.Add(polyline);

                            MapLayer originLayer = MapService.CreateMapLayer(origin);
                            MapLayer destinationLayer = MapService.CreateMapLayer(destination);

                            map.Layers.Add(originLayer);
                            map.Layers.Add(destinationLayer);

                            map.Center = origin;
                        });
                });
            }
        }

        /*
         * need scope to map
        private List<MapLocation> locations = new List<MapLocation>();

        private void GeoQueryResponse(MapLocation location)
        {
            locations.Add(location);

            if (locations.Count == 2)
            {
                List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                foreach (MapLocation ml in locations)
                {
                    Debug.WriteLine(String.Format("{0}:{1}", ml.GeoCoordinate.Latitude, ml.GeoCoordinate.Longitude));

                    GeoCoordinate coordinate = new GeoCoordinate(ml.GeoCoordinate.Latitude, ml.GeoCoordinate.Longitude);
                    geoList.Add(coordinate);
                }

                MapPolyline polyline = MapService.CreatePolyline(geoList);

                map.MapElements.Add(polyline);

                //MapLayer originLayer = MapService.CreateMapLayer(origin);
                MapLayer destinationLayer = MapService.CreateMapLayer(destination);

                //map.Layers.Add(originLayer);
                map.Layers.Add(destinationLayer);

                map.Center = new GeoCoordinate(dg.Latitude, dg.Longitude);

                locations = new List<MapLocation>();
            }
        }
        */
    }
}
