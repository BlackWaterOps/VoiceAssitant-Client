using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
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
}
