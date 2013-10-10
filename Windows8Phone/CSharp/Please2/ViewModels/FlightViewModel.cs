using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    class FlightViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private List<FlightItem> flights;
        public List<FlightItem> Flights
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

        INavigationService navigationService;

        public FlightViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;
        }
    }
}
