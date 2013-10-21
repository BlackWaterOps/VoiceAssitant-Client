using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        /* WEIGHT */
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

        private FitbitWeightGoals goals;
        public FitbitWeightGoals Goals
        {
            get { return goals; }
            set
            {
                goals = value;
                RaisePropertyChanged("Goals");
            }
        }

        /* FOOD */
        private FitbitFoodSummary foodSummary;
        public FitbitFoodSummary FoodSummary
        {
            get { return foodSummary; }
            set
            {
                foodSummary = value;
                RaisePropertyChanged("FoodSummary");
            }
        }

        private FitbitFoodGoals foodGoals;
        public FitbitFoodGoals FoodGoals
        {
            get { return foodGoals; }
            set
            {
                foodGoals = value;
                RaisePropertyChanged("FoodGoals");
            }
        }

        private IEnumerable<FitbitFood> foods;
        public IEnumerable<FitbitFood> Foods
        {
            get { return foods; }
            set
            {
                foods = value;
                RaisePropertyChanged("Foods");
            }
        }

        private int caloriesRemaining;
        public int CaloriesRemaining 
        {
            get { return caloriesRemaining; }
            set
            {
                caloriesRemaining = value;
                RaisePropertyChanged("CaloriesRemaining");
            }
        }

        public FitbitViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
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
                    case "log-food":
                        ret = PopulateFood(item);
                        break;

                    case "activity":
                    case "log-activity":
                        ret = PopulateFitness(item);
                        break;
                }    
            }

            return ret;
        }

        private Dictionary<string, object> PopulateWeight(JObject item)
        {
            var ret = new Dictionary<string, object>();

            points = (item["timeseries"] as JArray).ToObject<IEnumerable<FitbitWeightTimeseries>>();
            goals = item["goals"].ToObject<FitbitWeightGoals>();

            ret.Add("title", "fitbit weight");

            return ret;
        }

        private Dictionary<string, object> PopulateFood(JObject item)
        {
            var ret = new Dictionary<string, object>();

            foods = (item["foods"] as JArray).ToObject<IEnumerable<FitbitFood>>();
            foodGoals = item["goals"].ToObject<FitbitFoodGoals>();
            foodSummary = item["summary"].ToObject<FitbitFoodSummary>();

            var remaining = foodGoals.calories - foodSummary.calories;

            caloriesRemaining = (remaining > 0) ? remaining : 0;

            ret.Add("title", "fitbit food");

            return ret;
        }

        private Dictionary<string, object> PopulateFitness(JObject item)
        {
            var ret = new Dictionary<string, object>();

            return ret;
        }
    }
}
