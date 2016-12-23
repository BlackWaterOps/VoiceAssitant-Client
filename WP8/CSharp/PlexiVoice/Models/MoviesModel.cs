using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
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
        public string synopsis { get; set; }
    }
}
