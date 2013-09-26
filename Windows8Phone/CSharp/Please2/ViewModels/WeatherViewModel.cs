using System;
using System.Collections.Generic;
using System.Diagnostics;

using GalaSoft.MvvmLight;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class WeatherViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public string SubTitle
        {
            get { return DateTime.Now.ToString("dddd, MMMM d, yyyy @ h:mm tt"); }
        }
        
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

        private List<WeatherModel> multiForecast;

        public List<WeatherModel> MultiForecast
        {
            get { return multiForecast; }
            set
            {
                multiForecast = value;
                RaisePropertyChanged("MultiForecast");
            }
        }

        INavigationService navigationService;

        IPleaseService pleaseService;
     
        public WeatherViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            this.pleaseService = pleaseService;
            
            WeatherTest();
            //GetDefaultForecast();
        }
      
        /*
        public WeatherViewModel()
        {
            WeatherTest();
        }
        */

        public void GetDefaultForecast()
        {
            // TODO: get city/state from device location data 
            //HandleUserInput("weather today for scottsdale arizona");
            pleaseService.HandleUserInput("weather today for scottsdale arizona");
        }

        private void WeatherTest()
        {
            var results = "{\"speak\":\"Here's our forecast for Saturday: Mostly cloudy, with a low around 62. North wind 5 to 7 mph becoming calm  after midnight.\",\"show\":{\"simple\":{\"text\":\"Here's our forecast for Saturday: Mostly cloudy, with a low around 62. North wind 5 to 7 mph becoming calm  after midnight.\"},\"structured\":{\"item\":{\"full_forecast\":\"Mostly cloudy, with a low around 62. North wind 5 to 7 mph becoming calm  after midnight.\",\"sky\":\"Mostly Cloudy\",\"temperature\":\"62\"},\"template\":\"weather:single_day\"}},\"error\":null}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(results);

            Show(actor.show, actor.speak);
        }

        protected void Show(ShowModel showModel, string speak = "")
        {
            try
            {
                // if returning weather for multiple days 
                //MultiForecast = ((Newtonsoft.Json.Linq.JToken)showModel.structured["items"]).ToObject<List<WeatherModel>>();

                SingleForecast = ((Newtonsoft.Json.Linq.JToken)showModel.structured["item"]).ToObject<WeatherModel>();

                MultiForecast = new List<WeatherModel>() { SingleForecast };
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
