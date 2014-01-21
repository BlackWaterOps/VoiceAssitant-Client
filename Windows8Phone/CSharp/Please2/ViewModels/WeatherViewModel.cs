﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Phone.Maps.Services;

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
using PlexiSDK.Models;
namespace Please2.ViewModels
{
    public class WeatherViewModel : ViewModelBase, IViewModel
    {
        public ColorScheme Scheme { get { return ColorScheme.Weather; } }
        
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
    
        public WeatherViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();            
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
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

            var data = new Dictionary<string, object>();

            data.Add("title", "weather");
            data.Add("subtitle", DateTime.Now.ToString("dddd, MMMM d, yyyy"));
            data.Add("scheme", this.Scheme);

            return data;
        }

        public void LoadDefault()
        {
            plexiService.Query("weather today");
        }
    }
}
