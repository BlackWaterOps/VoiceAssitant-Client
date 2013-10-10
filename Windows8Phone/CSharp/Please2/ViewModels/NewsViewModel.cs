using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

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

        private IPleaseService pleaseService;

        public NewsViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.pleaseService = pleaseService;
        }
        
        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            if (structured.ContainsKey("items"))
            {
                var page = ViewModelLocator.SingleResultPageUri;

                var news = ViewModelLocator.GetViewModelInstance<NewsViewModel>();

                stories = (structured["items"] as JArray).ToObject<List<NewsModel>>();

                var singleViewModel = ViewModelLocator.GetViewModelInstance<SingleViewModel>();

                // return title, subtitle, and page back to service so we can be decoupled from singleviewmodel

                ret.Add("title", "news results");
                ret.Add("subtitle", String.Format("news search on \"{0}\"", this.pleaseService.OriginalQuery));
                ret.Add("page", ViewModelLocator.SingleResultPageUri);
            }

            return ret;
        }
    }
}
