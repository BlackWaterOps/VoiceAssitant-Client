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
            get
            {
                string subtitle = "";

                var pos = Please2.Util.Location.GeoPosition;
                /*
                if (pos != null)
                {
                    subtitle += pos.CivicAddress.City + ", " + pos.CivicAddress.State + ": ";
                }
                */
                return subtitle + DateTime.Now.ToString("dddd, MMMM d, yyyy");
            }
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
            //var results = "{\"show\": {\"simple\": {\"text\": \"Here's our forecast for Wednesday: Sunny, with a high near 90. South southwest wind around 7 mph.\"}, \"structured\": {\"item\": {\"week\": [{\"date\": \"2013-10-02\", \"daytime\": {\"text\": \"Sunny, with a high near 90. South southwest wind around 7 mph. \", \"sky\": \"Sunny\", \"temp\": \"90\"}, \"night\": {\"text\": \"Mostly clear, with a low around 64. North northwest wind around 5 mph becoming calm  after midnight. \", \"sky\": \"Mostly Clear\", \"temp\": \"64\"}}, {\"date\": \"2013-10-03\", \"daytime\": {\"text\": \"Sunny, with a high near 85. Light and variable wind becoming southwest 5 to 10 mph in the morning. \", \"sky\": \"Sunny\", \"temp\": \"85\"}, \"night\": {\"text\": \"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming east northeast after midnight. \", \"sky\": \"Mostly Clear\", \"temp\": \"63\"}}, {\"date\": \"2013-10-04\", \"daytime\": {\"text\": \"Sunny, with a high near 85. Light and variable wind becoming northwest around 6 mph in the afternoon. \", \"sky\": \"Sunny\", \"temp\": \"85\"}, \"night\": {\"text\": \"Mostly clear, with a low around 59. Breezy. \", \"sky\": \"Breezy\", \"temp\": \"59\"}}, {\"date\": \"2013-10-05\", \"daytime\": {\"text\": \"Sunny, with a high near 85. Breezy. \", \"sky\": \"Breezy\", \"temp\": \"85\"}, \"night\": {\"text\": \"Mostly clear, with a low around 57.\", \"sky\": \"Mostly Clear\", \"temp\": \"57\"}}, {\"date\": \"2013-10-06\", \"daytime\": {\"text\": \"Sunny, with a high near 88.\", \"sky\": \"Sunny\", \"temp\": \"88\"}, \"night\": {\"text\": \"Mostly clear, with a low around 61.\", \"sky\": \"Mostly Clear\", \"temp\": \"61\"}}, {\"date\": \"2013-10-07\", \"daytime\": {\"text\": \"Sunny, with a high near 89.\", \"sky\": \"Sunny\", \"temp\": \"89\"}, \"night\": {\"text\": \"Mostly clear, with a low around 62.\", \"sky\": \"Mostly Clear\", \"temp\": \"62\"}}], \"now\": {\"sky\": \"Sunny\", \"temp\": \"89\"}, \"location\": null}, \"template\": \"single:weather\"}}, \"speak\": \"Here's our forecast for Wednesday: Sunny, with a high near 90. South southwest wind around 7 mph.\"}";
            
            var results = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Mostly Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";
            
            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(results);

            Show(actor.show);
        }

        protected void Show(ShowModel showModel, string speak = "")
        {
            try
            {
                // if returning weather for multiple days 
                //MultiForecast = ((Newtonsoft.Json.Linq.JToken)showModel.structured["items"]).ToObject<List<WeatherModel>>();

                // SingleForecast = ((Newtonsoft.Json.Linq.JToken)showModel.structured["item"]).ToObject<WeatherModel>();
                //MultiForecast = new List<WeatherDay>() { SingleForecast };

                var weatherResults = ((Newtonsoft.Json.Linq.JToken)showModel.structured["item"]).ToObject<WeatherModel>();

                // since the api drops the daytime info part way through the afternoon, 
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
                
                MultiForecast = weatherResults.week;
                 
                CurrentCondition = weatherResults.now;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
