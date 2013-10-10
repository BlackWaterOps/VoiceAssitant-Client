using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Ioc;

using Newtonsoft.Json.Linq;

using Please2.Models;

namespace Please2.ViewModels
{
    public class SingleViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private const string templateDict = "SingleTemplateDictionary";

        private Visibility? titleVisibility;
        public Visibility TitleVisibility
        {
            get { return (titleVisibility.HasValue == false) ? Visibility.Visible : titleVisibility.Value; }
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

        private string subTitle;
        public string SubTitle
        {
            get { return subTitle; }
            set
            {
                subTitle = value;
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

        public void RunTest(string test, DataTemplate contentTemplate)
        {
            ContentTemplate = contentTemplate;

            title = test;
            subTitle = "";

            switch (test)
            {
                case "stock":
                    StockTest();
                    break;

                case "weather":
                    WeatherTest();
                    break;

                case "fitbit":
                    FitbitTest();
                    break;

                case "news":
                    NewsTest();
                    break;
            }
        }

        private void NewsTest()
        {
            var originalQuery = "what's up with obama";

            var locator = GetLocator();

            var vm = locator.NewsViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"link\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\"},\"structured\":{\"items\":[{\"description\":\"Washington (AFP) - President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. The move would put a woman at the ...\",\"title\":\"Obama chooses Yellen to lead Fed\",\"url\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\",\"summary\":\"President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"source\":\"Yahoo! News\",\"date\":\"2013-10-09T15:41:20Z\"},{\"url\":\"http://www.bloomberg.com/news/2013-10-09/obama-seeks-post-debt-talks-with-senate-republicans-open.html\",\"source\":\"Bloomberg\",\"date\":\"2013-10-09T14:36:54Z\",\"description\":\"Giving priority to interest payments would prevent the U.S. from defaulting on its debt while requiring the government to balance its budget ... to go home,” said Stu Shea, president and chief operating officer of Leidos. A debt-ceiling ...\",\"title\":\"Obama Seeks Post-Debt Talks With Senate Republicans Open\"},{\"description\":\"WASHINGTON — President Obama will meet with congressional caucuses from both parties in the coming days, starting Wednesday in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss ...\",\"title\":\"Obama to meet with congressional caucuses\",\"url\":\"http://www.usatoday.com/story/news/politics/2013/10/09/obama-house-senate-government-shutdown/2950557/\",\"summary\":\"WASHINGTON President Obama will meet with congressional caucuses from both parties in the coming days, starting Wed. in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss the ongoing government shutdown and the prospect of a debt ceiling default. In recent days, Obama has refused Republican requests to negotiate a new spending plan that would end the government shutdown now in its ninth day.\",\"source\":\"USA Today\",\"date\":\"2013-10-09T14:22:35Z\"}],\"template\":\"list:news\"}},\"speak\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            IEnumerable<object> stories = ((JToken)show.structured["items"]).ToObject<IEnumerable<NewsModel>>();

            vm.Stories = stories.Cast<NewsModel>().ToList<NewsModel>();

            title = "news results";
            subTitle = String.Format("news search on \"{0}\"", originalQuery);
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

                vm.StockData = ((JToken)show.structured["item"]).ToObject<StockModel>();

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

            var data = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Mostly Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            var weatherResults = ((JToken)show.structured["item"]).ToObject<WeatherModel>();

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

        private void FitbitTest()
        {
            Debug.WriteLine("fitbit test");

            var locator = GetLocator();

            var vm = locator.FitbitViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"As of October 7 2013 your weight is 250.9 pounds\"},\"structured\":{\"item\":{\"goals\":{\"weight\":210},\"timeseries\":[{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-08\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-09\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-10\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-11\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-12\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-13\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-14\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-15\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-16\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-17\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-18\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-19\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-20\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-21\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-22\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-23\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-24\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-25\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-26\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-27\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-28\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-29\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-09-30\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-01\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-02\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-03\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-04\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-05\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-06\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-07\"}]},\"template\":\"single:fitbit:weight\"}},\"speak\":\"As of October 7 2013 your weight is 250.9 pounds\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            var fitbitResults = ((JToken)show.structured["item"]).ToObject<FitbitModel>();
       
            vm.Points = fitbitResults.timeseries;
            vm.Goals = fitbitResults.goals;
        }

        private ViewModelLocator GetLocator()
        {
            return App.Current.Resources["Locator"] as ViewModelLocator;
        }
    }
}
