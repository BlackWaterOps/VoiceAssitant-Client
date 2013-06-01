﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Please.Util
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
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
}
