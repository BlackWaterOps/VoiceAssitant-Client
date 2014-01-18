using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiSDK.Models
{
    public class GeoLocation
    {
        public string city { get; set; }

        public bool dst { get; set; }

        public double latitude { get; set; }
       
        public double longitude { get; set; }

        public string state { get; set; }

        public int time_offset { get; set; }

        public string zipcode { get; set; }
    }
}
