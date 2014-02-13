using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    public class SearchViewModel : ListBase
    {
        public RelayCommand<SearchModel> SearchItemSelection { get; set; }

        public SearchViewModel()
        {
            SearchItemSelection = new RelayCommand<SearchModel>(SearchItemSelected);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<SearchModel>>();
        }

        public void SearchItemSelected(SearchModel result)
        {
            Debug.WriteLine(result.url);
            // pass selection to please service to process and send to auditor
            var browser = new WebBrowserTask();
            browser.Uri = new Uri(result.url, UriKind.Absolute);
            browser.Show();
        }
    }
}
