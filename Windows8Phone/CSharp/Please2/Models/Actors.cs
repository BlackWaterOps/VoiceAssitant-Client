using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    /* Weather */
    public class WeatherDayDetails
    {
        public string text { get; set; }
        public string sky { get; set; }
        public string temp { get; set; }
    }

    public class WeatherDay
    {
        public WeatherDayDetails daytime { get; set; }
        public WeatherDayDetails night { get; set; }
        public DateTime date { get; set; }
    }

    public class WeatherModel
    {
        public List<WeatherDay> week { get; set; }
        public WeatherDayDetails now { get; set; }
        public string location { get; set; }
    }

    /* Shopping */
    public class ShoppingModel
    {
        public string title { get; set; }
        public string price { get; set; }
        public string image { get; set; }
        public string url { get; set; }
    }

    /* News */
    public class NewsModel
    {
        public string description { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string summary { get; set; }
        public string source { get; set; }
        public string date { get; set; }
    }

    /* Movies */
    public class MovieRatings
    {
        public int critics_score { get; set; }
        public int audience_score { get; set; }
        public string critics_rating { get; set; }
        public string audience_rating { get; set; }
    }

    public class MovieAbridgedCast
    {
        public string name { get; set; }
        public List<string> characters { get; set; }
        public string id { get; set; }
    }

    public class MoviesModel
    {
        public string release_date { get; set; }
        public MovieRatings ratings { get; set; }
        public string mpaa_rating { get; set; }
        public string title { get; set; }
        public string critics_consensus { get; set; }
        public string image { get; set; }
        public int runtime { get; set; }
        public int year { get; set; }
        public string id { get; set; }
        public List<MovieAbridgedCast> abridged_cast { get; set; }
    }

    /* Events */
    public class EventLocation
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string address { get; set; }
    }

    public class EventPerformer
    {
        public string name { get; set; }
        public string creator { get; set; }
        public string url { get; set; }
        public string linker { get; set; }
        public string short_bio { get; set; }
    }

    public class EventModel
    {
        public string id { get; set; }
        public string start_time { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string created { get; set; }
        public string all_day { get; set; }
        public string venue_url { get; set; }
        public Dictionary<string, EventPerformer> performers { get; set; }
        public string stop_time { get; set; }
        public EventLocation location { get; set; }
    }

    /* Flights */
    public class FlightOrigin
    {
        public string city { get; set; }
        public string airport_code { get; set; }
        public string airport_name { get; set; }
    }

    public class FlightSchedule
    {
        public string estimated_arrival { get; set; }
        public string filed_departure { get; set; }
        public string actual_departure { get; set; }
    }

    public class FlightDestination
    {
        public string city { get; set; }
        public string airport_code { get; set; }
        public string airport_name { get; set; }
    }

    public class FlightItem
    {
        public FlightOrigin origin { get; set; }
        public string status { get; set; }
        public FlightSchedule schedule { get; set; }
        public FlightDestination destination { get; set; }
        public object delay { get; set; }
        public string identification { get; set; }
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
        public List<FlightItem> items { get; set; }
        public string flight_number { get; set; }
        public FlightAirline airline { get; set; }
        public string template { get; set; }
    }

    /* Real Estate */
    public class RealEstateCategory
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RealEstateSource
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RealEstateLocation
    {
        public string citycode { get; set; }
        public string name { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string longitude { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public string latitude { get; set; }
        public string addresscode { get; set; }
    }

    public class RealEstateImage
    {
        public string src { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string num { get; set; }
        public string alt { get; set; }
        public string size { get; set; }
    }

    public class RealEstateAttributes
    {
        public string price_display { get; set; }
        public string fee { get; set; }
        public double bathrooms { get; set; }
        public string price { get; set; }
        public int bedrooms { get; set; }
        public int square_feet { get; set; }
        public string currency { get; set; }
        public string amenities { get; set; }
        public string has_photo { get; set; }
        public string user_id { get; set; }
    }

    public class RealEstateUser
    {
        public string url { get; set; }
        public string photo { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RealEstateModel
    {
        public string body { get; set; }
        public RealEstateCategory category { get; set; }
        public int revenue_score { get; set; }
        public int ctime { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string similar_url { get; set; }
        public string paid { get; set; }
        public RealEstateSource source { get; set; }
        public RealEstateLocation location { get; set; }
        public List<RealEstateImage> images { get; set; }
        public RealEstateAttributes attributes { get; set; }
        public string id { get; set; }
        public RealEstateUser user { get; set; }
    }

    /* Fitbit */
    public class FitbitTimeseries
    {
        public double value { get; set; }
        public DateTime dateTime { get; set; }
    }

    public class FitbitModel
    {
        public List<FitbitTimeseries> timeseries { get; set; }
    }
}
