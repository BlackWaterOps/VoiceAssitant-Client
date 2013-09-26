using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Coding4Fun.Toolkit.Controls;

using Please2.Models;

namespace Please2.Util
{
    public class ListResultsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = value.GetType();

            Debug.WriteLine("list results converter");
            Debug.WriteLine(type);

            /*
            if (type == typeof(ShoppingModel))
            {

            }
            */

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

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

    public class PrettyDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string datestring = ((string)value).ToLower();

            return DateTime.Parse(datestring).ToString("ddd, MMM d, yyyy");  
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
            if (value == null)
            {
                return System.Windows.Visibility.Collapsed;
            }

            string link = ((string)value).ToLower();

            System.Windows.Visibility visible;

            if (link == "" || link == null)
            {
                visible = System.Windows.Visibility.Collapsed;
            }
            else
            {
                visible = System.Windows.Visibility.Visible;
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
            string currCondition = ((string)value).ToLower();

            string icon = null;

            //var temp = new List<string>() { "mostly cloudy" };

            var conditions = new Dictionary<string, List<string>>()
            {
                {"WeatherMostlyCloudy", new List<string> {"mostly cloudy"} },
                {"WeatherClear", new List<string> {"fair", "clear"} },
                {"WeatherPartlyCloudy", new List<string> {"partly cloudy", "few clouds"} },
                {"WeatherOvercast", new List<string> {"overcast"} },
                {"WeatherFog", new List<string> {"fog"} },
                {"WeatherSmoke", new List<string> {"smoke"} },
                {"WeatherFreezingRain", new List<string> {"freezing rain", "freezing drizzle"} },
                {"WeatherHail", new List<string> {"ice pellets", "ice crystals", "hail", "snow pellets"} },
                {"WeatherRain", new List<string> {"rain showers", "light rain", "light showers", "showers rain"} },
                {"WeatherRainSnow", new List<string> {"rain snow", "snow rain", "drizzle snow", "snow drizzle"} },
                {"WeatherThunderstorm", new List<string> {"thunderstorm"} },
                {"WeatherSnow", new List<string> {"snow"} },
                {"WeatherWindy", new List<string> {"windy", "breezy"} },
                {"WeatherTornado", new List<string> {"funnel cloud", "tornado", "water spout"} },
                {"WeatherDust", new List<string> {"dust", "sand"} },
                {"WeatherHaze", new List<string> {"haze"} }
            };

            foreach (var conditionList in conditions)
            {
                foreach (var condition in conditionList.Value)
                {
                    if (culture.CompareInfo.IndexOf(currCondition, condition, System.Globalization.CompareOptions.IgnoreCase) != -1)
                    {
                        // set icon to appropriate image brush name
                        icon = conditionList.Key;

                        // there's no point to have the images defined as brushes in app.xaml
                        // make the dict keys the name of the image
                        return new Uri("/Assets/Weather/" + icon + ".png", UriKind.Relative);
                    }
                }
            }

            return Application.Current.Resources[icon] as ImageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
