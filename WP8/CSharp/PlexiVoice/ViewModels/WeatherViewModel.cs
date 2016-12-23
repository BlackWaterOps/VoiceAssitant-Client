using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Phone.Maps.Services;

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Models;
namespace PlexiVoice.ViewModels
{
    public class WeatherViewModel : ViewModelBase, IViewModel
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

        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged("Location");
            }
        }

        INavigationService navigationService;

        IPlexiService plexiService;
     
        public WeatherViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();            
        }

        public async void Load(string templateName, Dictionary<string, object> structured)
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

            MultiForecast = weatherResults.week;
            CurrentCondition = weatherResults.now;

            GeoLocation location = this.plexiService.QueryLocation;

            if (location != null)
            {
                if (!String.IsNullOrEmpty(location.city))
                {
                    Debug.WriteLine("set city");
                    Location = location.city;
                }
                else if (location.latitude != 0 && location.longitude != 0)
                {
                    IList<MapLocation> locations = await MapService.Default.ReverseGeoQuery(location.latitude, location.longitude);

                    if (locations.Count > 0)
                    {
                        MapLocation loc = locations[0];

                        Debug.WriteLine("reverse geoquery");
                        Newtonsoft.Json.JsonConvert.SerializeObject(loc);

                        //loc.Information.Address.City
                    }
                }
            }
        }

        /*
        public void GetDefaultForecast()
        {
            // TODO: get city/state from device location data 
            plexiService.Query("weather today for scottsdale arizona");
        }
        */
    }
}
