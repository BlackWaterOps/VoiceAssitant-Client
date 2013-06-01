using System;
using System.Diagnostics;
using System.Windows.Data;

using Please.Resources;

namespace Please.Util
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string link = ((string)value).ToLower();

            double fontSize;
            
            if (link == null || link == String.Empty)
            {
                fontSize = .1;
            }
            else
            {
                fontSize = 11;
            }

            return fontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

