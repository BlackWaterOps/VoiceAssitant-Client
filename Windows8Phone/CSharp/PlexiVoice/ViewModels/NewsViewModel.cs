using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class NewsViewModel : ListBase, IViewModel
    {
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }

        public NewsViewModel()
        {
            NewsItemSelection = new RelayCommand<NewsModel>(NewsItemSelected);
        }
        
        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<NewsModel>>();
        }

        public void NewsItemSelected(NewsModel e)
        {
        }
    }
}
