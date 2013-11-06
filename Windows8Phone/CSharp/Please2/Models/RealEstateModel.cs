using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
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
        public string pets_allowed { get; set; }
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
        public double std { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double median { get; set; }
        public double range { get; set; }
        public double mode { get; set; }
        public double mean { get; set; }
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
}
