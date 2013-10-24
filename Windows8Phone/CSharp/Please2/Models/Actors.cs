﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    /* Search */
    public class SearchModel
    {
        public string url { get; set; }
        public string title { get; set; }
        public string opengraph_image { get; set; }
        public string summary { get; set; }
    }

    /* Dictionary */
    public class DictionarySense
    {
        public string definition { get; set; }
        public string kind { get; set; }
        public List<string> examples { get; set; }
        public List<string> lemmas { get; set; }
    }

    public class DictionaryModel
    {
        public string headword { get; set; }
        public Dictionary<string, List<DictionarySense>> senses { get; set; }
        public List<string> pos { get; set; }
    }

    /* GeoPolitics */
    public class GeopoliticsStats
    {
        public string area { get; set; }
        public string leader { get; set; }
        public string population { get; set; }
    }
    
    public class GeopoliticsModel
    {
        public string country { get; set; }
        public string flag { get; set; }
        public GeopoliticsStats stats { get; set; }
        public string text { get; set; }
    }

    /* Horoscope */
    public class HoroscopeModel
    {
        public string zodiac_sign { get; set; }
        public string horoscope { get; set; }
    }

    /* Alt Fuels */
    public class AltFuelFederalAgency
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class AltFuelModel
    {
        public object access_days_time { get; set; }
        public string status_code { get; set; }
        public object intersection_directions { get; set; }
        public string updated_at { get; set; }
        public object e85_blender_pump { get; set; }
        public object hy_status_link { get; set; }
        public int id { get; set; }
        public string geocode_status { get; set; }
        public object ng_fill_type_code { get; set; }
        public object ng_vehicle_class { get; set; }
        public string date_last_confirmed { get; set; }
        public object ev_dc_fast_num { get; set; }
        public double latitude { get; set; }
        public string open_date { get; set; }
        public object ng_psi { get; set; }
        public object ev_other_evse { get; set; }
        public object cards_accepted { get; set; }
        public string groups_with_access_code { get; set; }
        public string fuel_type_code { get; set; }
        public object ev_level2_evse_num { get; set; }
        public object lpg_primary { get; set; }
        public string bd_blends { get; set; }
        public object ev_network_web { get; set; }
        public string station_name { get; set; }
        public object plus4 { get; set; }
        public object expected_date { get; set; }
        public string owner_type_code { get; set; }
        public object ev_network { get; set; }
        public double longitude { get; set; }
        public object ev_level1_evse_num { get; set; }
        public AltFuelFederalAgency federal_agency { get; set; }
        public object station_phone { get; set; }
        public string address { get; set; }
        public double distance { get; set; }
    }

    /* Stock */
    public class StockModel
    {
        public string opening_price { get; set; }
        public string low_price { get; set; }
        public double share_price { get; set; }
        public string stock_exchange { get; set; }
        public string trade_volume { get; set; }
        public string average_trade_volume { get; set; }
        public double pe { get; set; }
        public double high_price { get; set; }
        public double share_price_change { get; set; }
        public string market_cap { get; set; }
        public string symbol { get; set; }
        public double share_price_change_percent { get; set; }
        public string name { get; set; }
        public string yield { get; set; }
        public string share_price_direction { get; set; }
    }

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
        public DateTime date { get; set; }
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
        public DateTime release_date { get; set; }
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
        public DateTime? start_time { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public DateTime? created { get; set; }
        public string all_day { get; set; }
        public string venue_url { get; set; }
        public Dictionary<string, EventPerformer> performers { get; set; }
        public DateTime? stop_time { get; set; }
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
        public double longitude { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
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
        public double price { get; set; }
        public double bedrooms { get; set; }
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

    public class RealEstateListing
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
        //public RealEstateUser user { get; set; }
    }

    public class RealEstateStatsNums
    {
        public double std {get; set; }
        public double min {get; set; }
        public double max {get; set; }
        public double median {get; set; }
        public double range {get; set; }
        public double mode {get; set; }
        public double mean {get; set; }
    }
    public class RealEstateStats
    {
        public RealEstateStatsNums price { get; set; }
        public RealEstateStatsNums bedrooms { get; set; }
        public RealEstateStatsNums bathrooms { get; set; }
        public RealEstateStatsNums square_feet { get; set; }
    }

    public class RealEstateModel
    {
        public List<RealEstateListing> listings { get; set; }
        public RealEstateStats stats { get; set; }
    }

    /* Fitbit */
    public class FitbitWeightTimeseries
    {
        public double value { get; set; }
        public DateTime dateTime { get; set; }
    }

    public class FitbitWeightGoals
    {
        public int weight { get; set; }

    }

    public class FitbitWeightModel
    {
        public List<FitbitWeightTimeseries> timeseries { get; set; }
        public FitbitWeightGoals goals { get; set; }
    }

    public class FitbitFoodUnit
    {
        public string plural { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class FitbitLoggedFood
    {
        public int mealTypeId { get; set; }
        public int foodId { get; set; }
        public string locale { get; set; }
        public string brand { get; set; }
        public int calories { get; set; }
        public int amount { get; set; }
        public List<int> units { get; set; }
        public string accessLevel { get; set; }
        public FitbitFoodUnit unit { get; set; }
        public string name { get; set; }
    }

    public class FitbitNutritionalValues
    {
        public double carbs { get; set; }
        public double fiber { get; set; }
        public double sodium { get; set; }
        public int calories { get; set; }
        public double fat { get; set; }
        public double protein { get; set; }
    }

    public class FitbitFood
    {
        public int logId { get; set; }
        public FitbitLoggedFood loggedFood { get; set; }
        public FitbitNutritionalValues nutritionalValues { get; set; }
        public string logDate { get; set; }
        public bool isFavorite { get; set; }
    }

    public class FitbitFoodGoals
    {
        public int calories { get; set; }
        public int estimatedCaloriesOut { get; set; }
    }

    public class FitbitFoodSummary
    {
        public double carbs { get; set; }
        public double fiber { get; set; }
        public double sodium { get; set; }
        public int calories { get; set; }
        public double fat { get; set; }
        public int water { get; set; }
        public double protein { get; set; }
    }

    public class FitbitFoodModel
    {
        public List<FitbitFood> foods { get; set; }
        public FitbitFoodGoals goals { get; set; }
        public FitbitFoodSummary summary { get; set; }
    }

}
