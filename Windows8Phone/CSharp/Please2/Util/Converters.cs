using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

using System.Windows.Data;
using System.Windows.Media;

using Coding4Fun.Toolkit.Controls;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.ViewModels;

using Plexi.Util;

namespace Please2.Util
{
    /*
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            string brush;

            if (sender == "user")
            {
                brush = "UserDialogBackground";
            }
            else
            {
                brush = "PleaseDialogBackground";
            }

            return Application.Current.Resources[brush] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            string brush;

            if (sender == "user")
            {
                brush = "UserDialogForeground";
            }
            else
            {
                brush = "PleaseDialogForeground";
            }

            return Application.Current.Resources[brush] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class AlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            HorizontalAlignment align;

            if (sender == "user")
            {
                align = HorizontalAlignment.Right;
            }
            else
            {
                align = HorizontalAlignment.Left;
            }

            return align;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class ChatBubbleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            ChatBubbleDirection dir;

            if (sender == "user")
            {
                dir = ChatBubbleDirection.LowerRight;
            }
            else
            {
                dir = ChatBubbleDirection.UpperLeft;
            }

            return dir;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    */
    public class SchemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }

            string brush = null;

            ResourceDictionary schemes = App.Current.Resources["SchemeDictionary"] as ResourceDictionary;

            switch ((string)parameter)
            {
                case "background":
                    brush = String.Format("{0}Background", value);
                    break;

                case "item":
                case "apptitle":
                    brush = String.Format("{0}AppTitle", value);
                    break;

                case "pagetitle":
                    brush = String.Format("{0}PageTitle", value);
                    break;

                case "pagesubtitle":
                    brush = String.Format("{0}PageSubTitle", value);
                    break;
            }

            return (brush != null) ? schemes[brush] : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class PrettyDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string param = ((string)parameter != null) ? (string)parameter : null;

            string parseString = null;

            switch (param)
            {   
                case "news":
                    /*
                    DateTime storyDate = (DateTime)value;

                    parseString = storyDate.ToString("dddd d, yyyy: h:mm tt");
                     */
                    
                    parseString = Datetime.GetTimeElapsed((DateTime)value);
                    break;

                case "notifications":
                    DateTime beginDate = (DateTime)value;

                    DateTime now = DateTime.Now;

                     string date;

                    if (beginDate.Date == now.Date)
                    {
                        date = "today ";
                    }
                    else if (beginDate.Date == now.AddDays(1).Date)
                    {
                        date = "tomorrow ";
                    }
                    else
                    {
                        date = beginDate.ToString("dddd, MMMM d, yyyy ");
                    }

                    parseString = date + beginDate.ToString("@ h:mm tt");
                    break;

                default:
                    string datestring = ((string)value).ToLower();

                    parseString =  DateTime.Parse(datestring).ToString("ddd, MMM d, yyyy");
                    break;
            }

            return parseString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = (string)parameter;

            string color = null;

            switch (param)
            {
                case "stock":
                    var direction = (string)value;

                    if (direction == "down")
                    {
                        //color = "#dc143c"; // red
                        color = "#b22222";
                    }
                    else if (direction == "up")
                    {
                        color = "#006400"; // green
                    }
                    break;

                case "flights":
                    var delay = System.Convert.ToInt64(value);

                    color =  "#006400"; // green

                    if (delay > 0)
                    {
                        //color = "#dc143c"; // red
                        color = "#b22222";
                    }
                    break;
            }

            if (color == null)
            {
                return value;
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            string status = null;

            if (value == null)
            {
                // flight is on time
                status = "Arriving on-time";
            }
            else
            {
                var delay = System.Convert.ToInt64(value);

                if (delay > 0)
                {
                    // flight delayed
                    status = "Delayed by " + delay + " mins";
                }
                else if (delay < 0)
                {
                    // flight is early
                    status = "Arriving early by " + delay + " mins";
                }
            }

            return status;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var margin = new Thickness();

            var item = (MainMenuModel)value;

            var vm = ViewModelLocator.GetServiceInstance<MainMenuViewModel>();

            var items = vm.MainMenu;
            var gridMargin = vm.Margin;
            var gridCols = vm.Columns;

            var ind = items.IndexOf(item);
            var total = items.Count;
            var lastRow = total - gridCols;


            margin.Top = (ind < gridCols) ? 0 : gridMargin;
            margin.Right = (ind % 2 == 0) ? gridMargin : 0;
            margin.Left = (ind % 2 == 1) ? gridMargin : 0;
            margin.Bottom = (ind > (lastRow - 1)) ? 0 : gridMargin;

            return margin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   
            Visibility visible = Visibility.Visible;

            var param = (string)parameter;

            object val;

            Debug.WriteLine(value.GetType().GetGenericTypeDefinition());

            if (value.GetType() == typeof(JArray) && ((JArray)value).Count <= 0)
            {
                val = null;
            } 
            else if (value.GetType() == typeof(JObject) && ((JObject)value).Count <= 0)
            {
                val = null;
            }
            else if (value.GetType() == typeof(List<string>) && ((List<string>)value).Count <= 0)
            {
                val = null;
            }
            else
            {
                val = value;
            }

            if (val == null)
            {
                visible = (param != null && param == "invert") ? Visibility.Visible : Visibility.Collapsed;
            }

            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    
    public class EntityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;

            var map = new Dictionary<string, string>()
            {
                {"\n", "&#10;"},
                {"\r", "&#13;"}
            };

            foreach (var item in map)
            {
                text.Replace(item.Key, item.Value);
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    // TODO: try to merge with background converter and pass param to indicate type of conversion needed
    public class NotificationBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime beginDate = (DateTime)value;

            DateTime now = DateTime.Now;

            string background = "#daa520"; // goldenrod

            if (beginDate.Date == now.Date)
            {
                background = "#f7301e"; // red
            }
            else if (beginDate.Date == now.AddDays(1).Date)
            {
                background = "#ff4500"; // orangered
            }

            return background;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    // TODO: try to merge with PrettyDateConverter
    public class NotificationDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime beginDate = (DateTime)value;

            DateTime now = DateTime.Now;

            string date;

            if (beginDate.Date == now.Date)
            {
                date = "today ";
            }
            else if (beginDate.Date == now.AddDays(1).Date)
            {
                date = "tomorrow ";
            }
            else
            {
                date = beginDate.ToString("dddd, MMMM d, yyyy ");
            }

            return date + beginDate.ToString("@ h:mm tt");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class NotificationOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime beginDate = (DateTime)value;

            DateTime now = DateTime.Now;

            double opacity = .5;

            if (beginDate.Date == now.Date)
            {
                opacity = 1.0;
            }
            else if (beginDate.Date == now.AddDays(1).Date)
            {
                opacity = .75;
            }

            return opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class WeatherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // need to pass in the whole weatherday model so we have access to daytime and night fields  
                var weatherDay = value as WeatherDay;

                bool isNight = false;
                string currCondition;
                
                if (weatherDay.daytime != null && weatherDay.daytime.sky != null)
                {
                    currCondition = weatherDay.daytime.sky;
                }
                else
                {
                    isNight = true;
                    currCondition = weatherDay.night.sky;
                }

                currCondition = currCondition.ToLower();

                string icon = null;

                // NOTE:
                // each key in the dict cooresponds to a list of glyphs found in Assets/Fonts/forcasticons.ttf
                // the first value in the dict key list is a daytime glyph, the second (if there is one) is a nighttime glyph
                // each dict value list cooresponds to a sky condition returned from the api
                var conditions = new Dictionary<List<string>, List<string>>()
                {
                { new List<string> { "3" }, new List<string> { "overcast", "overcast with haze" } },
                { new List<string> { "a" }, new List<string> { "mostly cloudy", "mostly cloudy with haze" } },
                { new List<string> { "A" }, new List<string> { "partly cloudy", "partly cloudy with haze",  } },
                { new List<string> { "2", "6" }, new List<string> { "a few clouds", "a few clouds with haze", "mostly clear", "mostly sunny" } },
                { new List<string> { "1", "6" }, new List<string> { "fair", "clear", "fair with haze", "fair and breezy", "clear and breezy", "sunny" } },

                { new List<string> { "b" }, new List<string> { "mostly cloudy and breezy" } },
                { new List<string> { "Z", "!" }, new List<string> { "overcast and breezy", "patchy fog", "area fog" } },
                { new List<string> { "d" }, new List<string> { "mostly cloudy and windy" } },
                /*
                { "f", new List<string> { "" } },
                { "j", new List<string> { "" } },
                { "t", new List<string> { "" } },
                { "n", new List<string> { "" } },
                { "x", new List<string> { "" } },
                { "p", new List<string> { "" } },
                { "h", new List<string> { "" } },
                { "l", new List<string> { "" } },
                { "v", new List<string> { "" } },
                { "r", new List<string> { "" } },

                { "B", new List<string> {  } },
                */
                { new List<string> { "z", "!" }, new List<string> { "a few clouds and breezy", "partly cloudy and breezy", "breezy" } },
                { new List<string> { "D", "e" }, new List<string> { "windy", "fair and windy", "a few clouds and windy", "partly cloudy and windy" } },
                { new List<string> { "F", "g" }, new List<string> { "freezing rain in vicinity", "freezing drizzle in vicinity" } },
                { new List<string> { "J", "k" }, new List<string> { 
                    "rain showers in vicinity", 
                    "showers rain in vicinity", 
                    "rain showers in vicinity fog/mist", 
                    "showers rain in vicinity fog/mist",
                    "showers in vicinity",
                    "showers in vicinity fog/mist",
                    "showers in vicinity fog",
                    "showers in vicinity haze"
                } },
                /*
                { "T", new List<string> { "" } },
                { "N", new List<string> { "" } },
                */
                { new List<string> { "X", "y" }, new List<string> { 
                    "thunderstorm in vicinity fog/mist", 
                    "thunderstorm in vicinity haze", 
                    "thunderstorm haze in vicinity",
                    "thunderstorm in vicinity",
                    "thunderstorm in vicinity fog",
                    "thunderstorm in vicinity haze"
                } },
                { new List<string> { "P", "q" }, new List<string> { "thunderstorm showers in vicinity" } },
                { new List<string> { "H", "i" }, new List<string> { 
                    "ice pellets in vicinity", 
                    "showers in vicinity snow", 
                    "snow showers in vicinity", 
                    "snow showers in vicinity fog/mist",
                    "snow showers in vicinity fog",
                    "blowing snow in vicinity"
                } },
                /*
                { "L", new List<string> { "" } },
                { "V", new List<string> { "" } },\
                */
                { new List<string> { "R", "s" }, new List<string> { 
                    "thunderstorm showers in vicinity hail", 
                    "thunderstorm in vicinity hail", 
                    "thunderstorm in vicinity hail haze",
                    "thunderstorm haze in vicinity hail"
                } },
               
                //{ "C", new List<string> { "" } },
                { new List<string> { "E" }, new List<string> { "overcast and windy" } },
                { new List<string> { "G", "g" }, new List<string> { 
                    "freezing drizzle", 
                    "light freezing drizzle", 
                    "light rain showers",
                    "light showers rain",
                    "light rain showers fog/mist",
                    "freezing drizzle rain",
                    "light freezing drizzle rain",
                    "rain freezing drizzle",
                    "light rain freezing drizzle",
                    "light rain",
                    "drizzle",
                    "light drizzle",
                    "heavy drizzle",
                    "light rain fog/mist",
                    "drizzle fog/mist",
                    "light drizzle fog/mist",
                    "heavy drizzle fog/mist",
                    "light rain fog",
                    "drizzle fog",
                    "light drizzle fog",
                    "heavy drizzle fog"
                } },
                { new List<string> { "K", "k" }, new List<string> { 
                    "freezing rain", 
                    "light freezing rain", 
                    "rain ice pellets", 
                    "light rain ice pellets", 
                    "drizzle ice pellets", 
                    "light drizzle ice pellets", 
                    "ice pellets rain", 
                    "light ice pellets rain", 
                    "ice pellets drizzle", 
                    "light ice pellets drizzle",
                    "rain showers",
                    "showers rain",
                    "rain showers fog/mist",
                    "showers rain fog/mist",
                    "freezing rain rain",
                    "light freezing rain rain",
                    "rain freezing rain",
                    "light rain freezing rain",
                    "rain",
                    "rain fog/mist",
                    "rain fog"
                } },
                { new List<string> { "U", "u" }, new List<string> { 
                    "heavy freezing rain", 
                    "heavy freezing drizzle", 
                    "heavy rain ice pellets", 
                    "heavy drizzle ice pellets", 
                    "heavy ice pellets rain", 
                    "heavy ice pellets drizzle",
                    "heavy rain showers",
                    "heavy showers rain",
                    "heavy showers rain fog/mist",
                    "heavy freezing rain rain",
                    "heavy rain freezing rain",
                    "heavy freezing drizzle rain",
                    "heavy rain freezing drizzle",
                    "heavy rain",
                    "heavy rain fog/mist",
                    "heavy rain fog"
                } },
                { new List<string> { "O", "o" }, new List<string> { "light rain and breezy" } },
                { new List<string> { "Y", "y" }, new List<string> { "thunderstorm", "thunderstorm fog" } },
                { new List<string> { "Q", "q" }, new List<string> { 
                    "thunderstorm rain", 
                    "light thunderstorm rain", 
                    "heavy thunderstorm rain",
                    "thunderstorm rain fog/mist",
                    "heavy thunderstorm rain fog and windy",
                    "heavy thunderstorm rain fog/mist",
                    "light thunderstorm rain haze",
                    "heavy thunderstorm rain haze",
                    "light thunderstorm rain fog",
                    "heavy thunderstorm rain fog",
                    "thunderstorm light rain",
                    "thunderstorm heavy rain",
                    "thunderstorm rain fog/mist",
                    "thunderstorm light rain fog/mist",
                    "thunderstorm heavy rain fog/mist",
                    "thunderstorm light rain haze",
                    "thunderstorm heavy rain haze",
                    "thunderstorm light rain fog",
                    "thunderstorm heavy rain fog",
                    "thunderstorm hail",
                    "thunderstorm rain hail fog/mist",
                    "light thunderstorm rain hail fog/mist",
                    "heavy thunderstorm rain hail fog/hail",
                    "light thunderstorm rain hail haze",
                    "heavy thunderstorm rain hail haze",
                    "light thunderstorm rain hail fog",
                    "heavy thunderstorm rain hail fog",
                    "thunderstorm light rain hail",
                    "thunderstorm heavy rain hail",
                    "thunderstorm rain hail fog/mist",
                    "thunderstorm light rain hail fog/mist",
                    "thunderstorm heavy rain hail fog/mist",
                    "thunderstorm light rain hail haze",
                    "thunderstorm heavy rain hail haze",
                    "thunderstorm light rain hail fog",
                    "thunderstorm heavy rain hail fog",
                    ""
                } },
                { new List<string> { "I", "i" }, new List<string> { 
                    "ice pellets", 
                    "light ice pellets", 
                    "ice crystals", 
                    "hail", 
                    "small hail/snow pellets", 
                    "light small hail/snow pellets",
                    "light drizzle snow",
                    "snow drizzle",
                    "light snow drizzle",
                    "snow",
                    "light snow",
                    "snow showers",
                    "light snow showers",
                    "showers snow",
                    "light showers snow",
                    "snow fog/mist",
                    "light snow fog/mist",
                    "snow showers fog/mist",
                    "light snow showers fog/mist",
                    "showers snow fog/mist",
                    "light showers snow fog/mist",
                    "snow fog",
                    "light snow fog",
                    "snow showers fog",
                    "light snow showers fog",
                    "showers snow fog",
                    "light showers snow fog",
                    "low drifting snow",
                    "blowing snow",
                    "snow low drifting snow",
                    "snow blowing snow",
                    "light snow low drifting snow",
                    "light snow blowing snow",
                    "light snow blowing snow fog/mist",
                    "snow grains",
                    "light snow grains"
                } },
                { new List<string> { "M", "m" }, new List<string> { 
                    "heavy ice pellets", 
                    "heavy small hail/snow pellets",
                    "heavy drizzle snow",
                    "heavy snow",
                    "heavy snow showers",
                    "heavy showers snow",
                    "heavy snow fog/mist",
                    "heavy snow showers fog/mist",
                    "heavy showers snow fog/mist",
                    "heavy snow fog",
                    "heavy snow showers fog",
                    "heavy showers snow fog",
                    "heavy snow blowing snow",
                    "heavy snow grains",
                    "heavy blowing snow"
                } },
                { new List<string> { "W", "w" }, new List<string> { 
                    "showers ice pellets", 
                    "showers hail", 
                    "hail showers", 
                    "freezing rain snow", 
                    "light freezing rain snow", 
                    "heavy freezing rain snow", 
                    "freezing drizzle snow", 
                    "light freezing drizzle snow", 
                    "heavy freezing drizzle snow", 
                    "snow freezing rain", 
                    "light snow freezing rain", 
                    "heavy snow freezing rain", 
                    "snow freezing drizzle", 
                    "light snow freezing drizzle", 
                    "heavy snow freezing drizzle", 
                    "rain snow",
                    "light rain snow",
                    "heavy rain snow",
                    "snow rain",
                    "light snow rain",
                    "heavy snow rain"
                } },
                { new List<string> { "S", "s" }, new List<string> { 
                    "thunderstorm ice pellets", 
                    "light thunderstorm rain hail", 
                    "heavy thunderstorm rain hail", 
                    "thunderstorm hail fog",
                    "thunderstorm small hail/snow pellets",
                    "thunderstorm rain small hail/snow pellets",
                    "light thunderstorm rain small hail/snow pellets",
                    "heavy thunderstorm rain small hail/snow pellets",
                    "thunderstorm snow",
                    "light thunderstorm snow",
                    "heavy thunderstorm snow"
                } },

                { new List<string> { ":" }, new List<string> { "funnel cloud", "funnel cloud in vicinity", "tornado/water spout" } },
                { new List<string> { "/" }, new List<string> {
                    "smoke",
                    "dust",
                    "low drifting dust",
                    "blowing dust",
                    "sand",
                    "blowing sand",
                    "low drifting sand",
                    "dust/sand whirls",
                    "dust/sand whirls in vicinity",
                    "dust storm",
                    "heavy dust storm",
                    "dust storm in vicinity",
                    "sand storm",
                    "heavy sand storm",
                    "sand storm in vicinity",
                    "haze"
                } }
                };

                foreach (var conditionList in conditions)
                {
                    foreach (var condition in conditionList.Value)
                    {
                        if (currCondition == condition)
                        {
                            // set icon to appropriate glyph char
                            var t = conditionList.Key;

                            if (isNight == true && conditionList.Key.Count > 1)
                            {
                                icon = conditionList.Key[1];
                            }
                            else
                            {
                                icon = conditionList.Key[0];
                            }
                        }
                    }
                }

                if (icon == null)
                {
                    Debug.WriteLine("could not find a suitable icon for " + currCondition);
                }

                return icon;
            }
            catch (Exception err)
            {
                Debug.WriteLine("converter exception");
                Debug.WriteLine(err.Message);
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
