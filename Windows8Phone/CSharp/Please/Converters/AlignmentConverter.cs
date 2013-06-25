using System;
using System.Diagnostics;
using System.Windows.Data;

using Please.Resources;

namespace Please.Converters
{
    public class AlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sender = ((string)value).ToLower();

            System.Windows.TextAlignment alignment;

            if (sender.Equals("user"))
            {
                alignment = System.Windows.TextAlignment.Right;
            }
            else
            {
                alignment = System.Windows.TextAlignment.Left;
            }

            return alignment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

