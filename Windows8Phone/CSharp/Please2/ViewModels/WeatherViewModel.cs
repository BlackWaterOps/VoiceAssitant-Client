using System;
using System.Collections.Generic;
using System.Diagnostics;

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class WeatherViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        private WeatherModel singleForecast;
        public WeatherModel SingleForecast
        {
            get { return singleForecast; }
            set
            {
                singleForecast = value;
                RaisePropertyChanged("SingleForecast");
            }
        }

        private List<WeatherDay> multiForecast;
        public List<WeatherDay> MultiForecast
        {
            get { return multiForecast; }
            set
            {
                multiForecast = value;
                RaisePropertyChanged("MultiForecast");
            }
        }

        private WeatherDayDetails currentCondition;
        public WeatherDayDetails CurrentCondition
        {
            get { return currentCondition; }
            set
            {
                currentCondition = value;
                RaisePropertyChanged("CurrentCondition");
            }
        }

        INavigationService navigationService;

        IPlexiService plexiService;
     
        public WeatherViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;

            this.plexiService = plexiService;            
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            if (structured.ContainsKey("item"))
            {
                var weatherResults = ((JToken)structured["item"]).ToObject<WeatherModel>();

                // since the api drops the daytime info for today part way through the afternoon, 
                // lets fill in the missing pieces with what we do have
                var today = weatherResults.week[0];

                if (today.daytime == null)
                {
                    today.daytime = new WeatherDayDetails()
                    {
                        temp = weatherResults.now.temp,
                        text = today.night.text
                    };
                }

                multiForecast = weatherResults.week;
                currentCondition = weatherResults.now;

                ret.Add("title", "weather");
                ret.Add("subtitle", DateTime.Now.ToString("dddd, MMMM d, yyyy"));
            }

            return ret;
        }

        public void GetDefaultForecast()
        {
            // TODO: get city/state from device location data 
            plexiService.Query("weather today for scottsdale arizona");
        }
    }
}
