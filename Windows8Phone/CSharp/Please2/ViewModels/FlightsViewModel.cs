using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

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

        INavigationService navigationService;

        public FlightsViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var flightResults = (structured["item"] as JObject).ToObject<FlightModel>();

            Flights = flightResults.details;
            Airline = flightResults.airline;
            FlightNumber = flightResults.flight_number;

            ret.Add("title", "flights");
            ret.Add("subtitle", "flight results");

            //ret.Add("subtitle", String.Format("flight results for \"{0}\"", originalQuery));

            return ret;
        }


    }
}
