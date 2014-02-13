using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
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
}
