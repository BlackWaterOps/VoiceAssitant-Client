using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Coding4Fun.Toolkit.Controls;

namespace Please2.Util
{
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
                align = HorizontalAlignment.Left;
            }
            else
            {
                align = HorizontalAlignment.Right;
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
                dir = ChatBubbleDirection.LowerLeft;
            }
            else
            {
                dir = ChatBubbleDirection.LowerRight;
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
    /*
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
    */
}
