using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class GeopoliticsViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value;
                RaisePropertyChanged("Country");
            }
        }

        private string flag;
        public string Flag
        {
            get { return flag; }
            set
            {
                flag = value;
                RaisePropertyChanged("Flag");
            }
        }

        private GeopoliticsStats stats;
        public GeopoliticsStats Stats
        {
            get { return stats; }
            set
            {
                stats = value;
                RaisePropertyChanged("Stats");
            }
        }

        public GeopoliticsViewModel(INavigationService naivgationService, IPleaseService pleaseService)
        {

        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var geoResults = (structured["item"] as JObject).ToObject<GeopoliticsModel>();

            Flag = geoResults.flag;
            Country = geoResults.country;
            Stats = geoResults.stats;

            return ret;
        }
    }
}
