using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Ioc;

using Please2.Models;

namespace Please2.ViewModels
{
    public class SingleViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private const string templateDict = "SingleTemplateDictionary";

        private Visibility titleVisibility;
        public Visibility TitleVisibility
        {
            get { return (titleVisibility == null) ? Visibility.Visible : titleVisibility; }
            set
            {
                titleVisibility = value;
                RaisePropertyChanged("TitleVisibility");
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string subtitle;
        public string SubTitle
        {
            get { return subtitle; }
            set
            {
                subtitle = value;
                RaisePropertyChanged("SubTitle");
            }
        }

        private DataTemplate contentTemplate;
        public DataTemplate ContentTemplate
        {
            get { return contentTemplate; }
            set
            {
                contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        public SingleViewModel()
        {

        }

        public void LoadDefaultTemplate(string template)
        {
            var templates = App.Current.Resources[templateDict] as ResourceDictionary;

            if (templates.Contains(template))
            {
                switch (template)
                {
                    case "weather":
                        var weather = App.GetViewModelInstance<WeatherViewModel>();
                        weather.GetDefaultForecast();

                        title = "weather";

                        var pos = Please2.Util.Location.GeoPosition;
                        /*
                        if (pos != null)
                        {
                            subtitle += pos.CivicAddress.City + ", " + pos.CivicAddress.State + ": ";
                        }
                        */
                        subtitle = DateTime.Now.ToString("dddd, MMMM d, yyyy");
                        break;

                    case "notifications":
                        var notifications = App.GetViewModelInstance<NotificationsViewModel>();

                        notifications.LoadNotifications();

                        title = "reminders & alerts";
                        subtitle = DateTime.Now.ToString("dddd, MMMM d, yyyy @ h:mm tt");

                        titleVisibility = Visibility.Collapsed;
                        break;
                }

                contentTemplate = templates[template] as DataTemplate;
            }
            else
            {
                Debug.WriteLine(template + " template could not be found");
            }    
        }

        public void RunTest(string test, DataTemplate contentTemplate)
        {
            ContentTemplate = contentTemplate;

            switch (test)
            {
                case "stock":
                    StockTest();
                    break;
            }
        }

        private void StockTest()
        {
            try
            {
                var locator = GetLocator();
              
                var vm = locator.StockViewModel;

                var data = "{\"show\":{\"simple\":{\"text\":\"Facebook stock is trading at $49.19, down 2.17%\"},\"structured\":{\"item\":{\"opening_price\":50.46,\"low_price\":49.06,\"P/E Ratio\":227.51,\"share_price\":49.19,\"stock_exchange\":\"NasdaqNM\",\"trade_volume\":\"48.20 million\",\"52_week_high\":51.6,\"average_trade_volume\":\"70.12 million\",\"pe\":0.221,\"high_price\":50.72,\"share_price_change\":-1.09,\"market_cap\":\"119.80 billion\",\"5_day_moving_average\":43.542,\"symbol\":\"FB\",\"share_price_change_percent\":2.17,\"name\":\"Facebook\",\"yield\":\"N/A\",\"52_week_low\":18.8,\"share_price_direction\":\"down\"},\"template\":\"simple:stock\"}},\"speak\":\"Facebook stock is trading at $49.19, down 2.17%\"}";

                var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<Please2.Models.ActorModel>(data);

                var show = actor.show;

                vm.StockData = ((Newtonsoft.Json.Linq.JToken)show.structured["item"]).ToObject<StockModel>();

                var direction = vm.StockData.share_price_direction;

                if (direction == "down")
                {
                    vm.DirectionSymbol = "\uf063";
                    vm.DirectionColor = "#dc143c";
                }
                else if (direction == "up")
                {
                    vm.DirectionSymbol = "\uf062";
                    vm.DirectionColor = "#008000";
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void WeatherTest()
        {
            var locator = GetLocator();
              
            var vm = locator.WeatherViewModel;

            var results = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Mostly Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(results);

            var show = actor.show;

            var weatherResults = ((Newtonsoft.Json.Linq.JToken)show.structured["item"]).ToObject<WeatherModel>();

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

            vm.MultiForecast = weatherResults.week;

            vm.CurrentCondition = weatherResults.now; 
        }

        private ViewModelLocator GetLocator()
        {
            return App.Current.Resources["Locator"] as ViewModelLocator;
        }
    }
}
