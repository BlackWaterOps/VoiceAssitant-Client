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
    public class FitbitActivityViewModel : FitbitViewModel
    { 
        public FitbitActivityViewModel()
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            // load activity data 
            return default(Dictionary<string, object>);
        }
    }
}
