using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class NewsViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        private List<NewsModel> stories;
        public List<NewsModel> Stories
        {
            get { return stories; }
            set
            {
                stories = value;
                RaisePropertyChanged("Stories");
            }
        }

        //private IPleaseService pleaseService;
        private IPlexiService plexiService;

        public NewsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.plexiService = plexiService;
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            Stories = (structured["items"] as JArray).ToObject<List<NewsModel>>();

            var data = new Dictionary<string, object>();

            data.Add("title", "news results");
            //data.Add("subtitle", String.Format("news search on \"{0}\"", this.plexiService.OriginalQuery));
            data.Add("scheme", "information");

            return data;
        }
    }
}
