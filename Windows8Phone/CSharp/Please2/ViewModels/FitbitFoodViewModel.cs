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

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class FitbitFoodViewModel : FitbitViewModel, IViewModel
    {
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

        public FitbitFoodViewModel()
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            FitbitFoodModel foodResults = ((JObject)structured["item"]).ToObject<FitbitFoodModel>();

            Foods = foodResults.foods;
            FoodGoals = foodResults.goals;
            FoodSummary = foodResults.summary;

            var remaining = FoodGoals.calories - FoodSummary.calories;

            CaloriesRemaining = (remaining > 0) ? remaining : 0;

            this.Title = "fitbit food";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }
    }
}
