using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
    public class FlightOrigin
    {
        public string city { get; set; }
        public string airport_code { get; set; }
        public string airport_name { get; set; }
    }

    public class FlightSchedule
    {
        public DateTime estimated_arrival { get; set; }
        public DateTime filed_departure { get; set; }
        public DateTime? actual_departure { get; set; }
        public DateTime? actual_arrival { get; set; }
    }

    public class FlightDestination
    {
        public string city { get; set; }
        public string airport_code { get; set; }
        public string airport_name { get; set; }
    }

    public class Flight
    {
        public FlightOrigin origin { get; set; }
        public string status { get; set; }
        public FlightSchedule schedule { get; set; }
        public FlightDestination destination { get; set; }
        public object delay { get; set; }
        public string identification { get; set; }
        public string duration { get; set; }
    }

    public class FlightAirline
    {
        public string code { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string callsign { get; set; }
        public string location { get; set; }
        public string shortname { get; set; }
    }

    public class FlightModel
    {
        public List<Flight> details { get; set; }
        public string flight_number { get; set; }
        public FlightAirline airline { get; set; }
    }
}
