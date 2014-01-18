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

using PlexiSDK;
namespace Please2.ViewModels
{
    public class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        public ColorScheme Scheme { get { return ColorScheme.Application; } }

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

        public FitbitViewModel(INavigationService navigationService, IPlexiService pleaseService)
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            var data = new Dictionary<string, object>();

            string[] template = (structured["template"] as string).Split(':');

            if (structured.ContainsKey("item"))
            {
                var item = structured["item"] as JObject;

                switch (template.Last())
                {
                    case "weight":
                        data = PopulateWeight(item);
                        break;

                    case "food":
                    case "log-food":
                    case "calories":
                        data = PopulateFood(item);
                        break;

                    case "activity":
                    case "log-activity":
                        data = PopulateFitness(item);
                        break;
                }    
            }

            data.Add("scheme", this.Scheme);

            return data;
        }

        private Dictionary<string, object> PopulateWeight(JObject item)
        {
            Points = (item["timeseries"] as JArray).ToObject<IEnumerable<FitbitWeightTimeseries>>();
            Goals = item["goals"].ToObject<FitbitWeightGoals>();

            var data = new Dictionary<string, object>();

            data.Add("title", "fitbit weight");
            
            return data;
        }

        private Dictionary<string, object> PopulateFood(JObject item)
        {
            Foods = (item["foods"] as JArray).ToObject<IEnumerable<FitbitFood>>();
            FoodGoals = item["goals"].ToObject<FitbitFoodGoals>();
            FoodSummary = item["summary"].ToObject<FitbitFoodSummary>();

            var remaining = foodGoals.calories - foodSummary.calories;

            caloriesRemaining = (remaining > 0) ? remaining : 0;

            var data = new Dictionary<string, object>();

            data.Add("title", "fitbit food");

            return data;
        }

        private Dictionary<string, object> PopulateFitness(JObject item)
        {
            var ret = new Dictionary<string, object>();

            return ret;
        }
    }
}
