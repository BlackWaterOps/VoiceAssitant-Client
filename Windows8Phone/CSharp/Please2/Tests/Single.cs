using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;
using Please2.ViewModels;

namespace Please2.Tests
{
    class Single
    {
        private Dictionary<string, object> titles = new Dictionary<string, object>();

        private Dictionary<string, object> FlightsTest()
        {
            var originalQuery = "status of delta flight 116";

            var locator = GetLocator();

            var vm = locator.FlightsViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"UAL1053 departed at 10:29 pm yesterday, and is set to arrive in Cleveland, OH at 12:33 am.\"},\"structured\":{\"item\":{\"flight_number\":\"1053\",\"airline\":{\"code\":\"UAL\",\"name\":\"United Air Lines Inc.\",\"url\":\"http://www.united.com/\",\"country\":\"US\",\"phone\":\"+1-800-225-5833\",\"callsign\":\"United\",\"location\":\"\",\"shortname\":\"United\"},\"details\":[{\"origin\":{\"city\":\"Newark, NJ\",\"airport_code\":\"KEWR\",\"airport_name\":\"Newark Liberty Intl\"},\"status\":\"departed\",\"schedule\":{\"estimated_arrival\":\"2013-10-12T00:33:34\",\"actual_departure\":\"2013-10-11T23:38:00\",\"filed_departure\":\"2013-10-11T22:29:00\"},\"destination\":{\"city\":\"Cleveland, OH\",\"airport_code\":\"KCLE\",\"airport_name\":\"Cleveland-Hopkins Intl\"},\"delay\":44,\"identification\":\"UAL1053\"}]},\"template\":\"single:flights\"}},\"speak\":\"UAL1053 departed at 10:29 pm yesterday, and is set to arrive in Cleveland, OH at 12:33 am.\"}";

            var actor = JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            var flightResults = (show.structured["item"] as JObject).ToObject<FlightModel>();

            vm.Flights = flightResults.details;
            vm.Airline = flightResults.airline;

            titles.Add("title", "flights");
            titles.Add("subtitle", String.Format("flight results for \"{0}\"", originalQuery));

            return titles;
        }

        private Dictionary<string, object> NewsTest()
        {
            var originalQuery = "what's up with obama";

            var locator = GetLocator();

            var vm = locator.NewsViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"link\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\"},\"structured\":{\"items\":[{\"description\":\"Washington (AFP) - President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. The move would put a woman at the ...\",\"title\":\"Obama chooses Yellen to lead Fed\",\"url\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\",\"summary\":\"President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"source\":\"Yahoo! News\",\"date\":\"2013-10-09T15:41:20Z\"},{\"url\":\"http://www.bloomberg.com/news/2013-10-09/obama-seeks-post-debt-talks-with-senate-republicans-open.html\",\"source\":\"Bloomberg\",\"date\":\"2013-10-09T14:36:54Z\",\"description\":\"Giving priority to interest payments would prevent the U.S. from defaulting on its debt while requiring the government to balance its budget ... to go home,” said Stu Shea, president and chief operating officer of Leidos. A debt-ceiling ...\",\"title\":\"Obama Seeks Post-Debt Talks With Senate Republicans Open\"},{\"description\":\"WASHINGTON — President Obama will meet with congressional caucuses from both parties in the coming days, starting Wednesday in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss ...\",\"title\":\"Obama to meet with congressional caucuses\",\"url\":\"http://www.usatoday.com/story/news/politics/2013/10/09/obama-house-senate-government-shutdown/2950557/\",\"summary\":\"WASHINGTON President Obama will meet with congressional caucuses from both parties in the coming days, starting Wed. in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss the ongoing government shutdown and the prospect of a debt ceiling default. In recent days, Obama has refused Republican requests to negotiate a new spending plan that would end the government shutdown now in its ninth day.\",\"source\":\"USA Today\",\"date\":\"2013-10-09T14:22:35Z\"}],\"template\":\"list:news\"}},\"speak\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            IEnumerable<object> stories = ((JToken)show.structured["items"]).ToObject<IEnumerable<NewsModel>>();

            vm.Stories = stories.Cast<NewsModel>().ToList<NewsModel>();

            titles.Add("title", "news results");
            titles.Add("subtitle", String.Format("news search on \"{0}\"", originalQuery));

            return titles;
        }

        private Dictionary<string, object> StockTest()
        {
            var locator = GetLocator();

            var vm = locator.StockViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"Facebook stock is trading at $49.19, down 2.17%\"},\"structured\":{\"item\":{\"opening_price\":50.46,\"low_price\":49.06,\"P/E Ratio\":227.51,\"share_price\":49.19,\"stock_exchange\":\"NasdaqNM\",\"trade_volume\":\"48.20 million\",\"52_week_high\":51.6,\"average_trade_volume\":\"70.12 million\",\"pe\":0.221,\"high_price\":50.72,\"share_price_change\":-1.09,\"market_cap\":\"119.80 billion\",\"5_day_moving_average\":43.542,\"symbol\":\"FB\",\"share_price_change_percent\":2.17,\"name\":\"Facebook\",\"yield\":\"N/A\",\"52_week_low\":18.8,\"share_price_direction\":\"down\"},\"template\":\"simple:stock\"}},\"speak\":\"Facebook stock is trading at $49.19, down 2.17%\"}";

            var actor = JsonConvert.DeserializeObject<Please2.Models.ActorModel>(data);

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

            titles.Add("title", "stock results");

            return titles;
        }

        private Dictionary<string, object> WeatherTest()
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

            titles.Add("title", "weather results");

            return titles;
        }

        private Dictionary<string, object> FitbitTest()
        {
            var locator = GetLocator();

            var vm = locator.FitbitViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"As of October 7 2013 your weight is 250.9 pounds\"},\"structured\":{\"item\":{\"goals\":{\"weight\":210},\"timeseries\":[{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-08\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-09\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-10\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-11\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-12\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-13\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-14\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-15\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-16\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-17\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-18\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-19\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-20\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-21\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-22\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-23\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-24\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-25\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-26\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-27\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-28\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-29\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-09-30\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-01\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-02\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-03\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-04\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-05\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-06\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-07\"}]},\"template\":\"single:fitbit:weight\"}},\"speak\":\"As of October 7 2013 your weight is 250.9 pounds\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            var fitbitResults = ((JToken)show.structured["item"]).ToObject<FitbitWeightModel>();

            vm.Points = fitbitResults.timeseries;
            vm.Goals = fitbitResults.goals;

            titles.Add("title", "fitbit results");

            return titles;
        }

        private Dictionary<string, object> FitbitFoodTest()
        {
            var locator = GetLocator();

            var vm = locator.FitbitViewModel;

            var data = "{\"show\":{\"simple\":{\"text\":\"I'm sorry. I cannot update your food log right now\"},\"structured\":{\"item\":{\"foods\":[{\"logId\":397779132,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397781203,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":82393,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":100,\"amount\":8,\"units\":[304,179,204,319,209,189,128,364,349,91,256,279,401,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"fl oz\",\"id\":128,\"name\":\"fl oz\"},\"name\":\"Pepsi\"},\"nutritionalValues\":{\"carbs\":27.5,\"fiber\":0,\"sodium\":25.5,\"calories\":100,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397782407,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":42111,\"locale\":\"en_US\",\"brand\":\"Johnny Rockets\",\"calories\":170,\"amount\":1,\"units\":[304],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Coke\"},\"nutritionalValues\":{\"carbs\":28,\"fiber\":0,\"sodium\":80,\"calories\":170,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397787791,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":81313,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":193,\"amount\":1,\"units\":[304,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Cheese\"},\"nutritionalValues\":{\"carbs\":32.5,\"fiber\":3,\"sodium\":490,\"calories\":193,\"fat\":8.5,\"protein\":11.5},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397796670,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":77,\"amount\":3,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"oz\",\"id\":226,\"name\":\"oz\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":17,\"fiber\":1.5,\"sodium\":1.5,\"calories\":77,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397799001,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397899468,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397903970,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397911826,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397913846,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false}],\"goals\":{\"calories\":1669,\"estimatedCaloriesOut\":2169},\"summary\":{\"carbs\":226.54649353027344,\"fiber\":15.330870628356934,\"sodium\":609.0343017578125,\"calories\":1080,\"fat\":10.555147171020508,\"water\":0,\"protein\":19.55388069152832}},\"template\":\"single:fitbit:log-food\"}},\"speak\":\"I'm sorry. I cannot update your food log right now\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(data);

            var show = actor.show;

            var fitbitResults = ((JToken)show.structured["item"]).ToObject<FitbitFoodModel>();

            //Debug.WriteLine(JsonConvert.SerializeObject(fitbitResults));


            var remaining = fitbitResults.goals.calories - fitbitResults.summary.calories;

            vm.Foods = fitbitResults.foods;
            vm.FoodGoals = fitbitResults.goals;
            vm.FoodSummary = fitbitResults.summary;
            vm.CaloriesRemaining = (remaining > 0) ? remaining : 0;

            titles.Add("title", "fitbit food results");
            titles.Add("titlevisibility", Visibility.Collapsed);

            return titles;
        }

        private ViewModelLocator GetLocator()
        {
            return App.Current.Resources["Locator"] as ViewModelLocator;
        }
    }
}
