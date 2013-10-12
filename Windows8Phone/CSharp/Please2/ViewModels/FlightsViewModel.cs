using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class FlightsViewModel : GalaSoft.MvvmLight.ViewModelBase
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
    }
}
