using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class EventLocationModel
    {
        public double lat { get; set; }
        public double lon {get; set; }
        public string address { get; set; }
    }

    public class PerformerModel
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
        public Dictionary<string, PerformerModel> performers { get; set; }
        public string stop_time { get; set; }
        public EventLocationModel location { get; set; }
    }
}
