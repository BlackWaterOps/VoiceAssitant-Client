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

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class FitbitWeightViewModel : ViewModelBase, IViewModel
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

        public FitbitWeightViewModel()
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            FitbitWeightModel weightResults = ((JObject)structured["item"]).ToObject<FitbitWeightModel>();

            Points = weightResults.timeseries;
            Goals = weightResults.goals;
        }
    }
}
