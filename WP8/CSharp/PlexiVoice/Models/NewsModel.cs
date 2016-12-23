using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
    public class NewsModel
    {
        public string description { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string summary { get; set; }
        public string source { get; set; }
        public DateTime date { get; set; }
    }
}
