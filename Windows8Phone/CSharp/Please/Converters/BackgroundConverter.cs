using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Please.Resources;

namespace Please.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            string brush;

            if (sender == "user")
            {
                brush = "UserBackground";
            }
            else
            {
                brush = "PleaseBackground";
            }

            return Application.Current.Resources[brush] as SolidColorBrush;  
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

