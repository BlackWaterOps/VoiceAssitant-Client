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
    public class FitbitActivityViewModel : ViewModelBase, IViewModel
    { 
        public FitbitActivityViewModel()
        {
            // TODO: create pie chart 
            // 4, 4, 9 rule
            // protein, carbs, fat
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
          // load activity data   
        }
    }
}
