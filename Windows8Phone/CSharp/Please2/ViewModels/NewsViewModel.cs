using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class NewsViewModel : ListViewModel, IViewModel
    {
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }

        public NewsViewModel()
        {
            NewsItemSelection = new RelayCommand<NewsModel>(NewsItemSelected);
        }
        
        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<NewsModel>>();

            this.Scheme = ColorScheme.Information;
            this.Title = "news";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void NewsItemSelected(NewsModel e)
        {
        }
    }
}
