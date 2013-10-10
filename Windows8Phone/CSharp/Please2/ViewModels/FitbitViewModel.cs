using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        private IEnumerable<object> points;
        public IEnumerable<object> Points
        {
            get { return points; }
            set 
            {
                points = value;
                RaisePropertyChanged("Points");
            }
        }

        private FitbitGoals goals;
        public FitbitGoals Goals
        {
            get { return goals; }
            set
            {
                goals = value;
                RaisePropertyChanged("Goals");
            }
        }

        public FitbitViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {

        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            string[] template = (structured["template"] as string).Split(':');

            if (structured.ContainsKey("item"))
            {
                var item = structured["item"] as JObject;

                switch (template.Last())
                {
                    case "weight":
                        ret = PopulateWeight(item);
                        break;

                    case "food":
                        ret = PopulateFood(item);
                        break;

                    case "fitness":
                        ret = PopulateFitness(item);
                        break;
                }    
            }

            return ret;
        }

        private Dictionary<string, object> PopulateWeight(JObject item)
        {
            var ret = new Dictionary<string, object>();

            points = (item["timeseries"] as JArray).ToObject<IEnumerable<FitbitTimeseries>>();
            goals = item["goals"].ToObject<FitbitGoals>();

            ret.Add("title", "fitbit");
            ret.Add("subtitle", "");

            return ret;
        }

        private Dictionary<string, object> PopulateFood(JObject item)
        {
            var ret = new Dictionary<string, object>();

            return ret;
        }

        private Dictionary<string, object> PopulateFitness(JObject item)
        {
            var ret = new Dictionary<string, object>();

            return ret;
        }
    }
}
