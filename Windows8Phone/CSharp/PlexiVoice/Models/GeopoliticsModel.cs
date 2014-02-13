using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
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
}
