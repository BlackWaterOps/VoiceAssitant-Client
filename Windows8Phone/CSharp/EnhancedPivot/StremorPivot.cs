using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Phone.Controls
{
    public class StremorPivot : Microsoft.Phone.Controls.Pivot
    {
        public StremorPivot()
        {
            DefaultStyleKey = typeof(StremorPivot);
        }

        #region Header Properties
        public static readonly DependencyProperty HeaderMinWidthProperty = DependencyProperty.Register(
            "HeaderMinWidth",
            typeof(double),
            typeof(StremorPivot),
            null);

        private static void OnHeaderMinWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            StremorPivot pivot = dependencyObject as StremorPivot;
        }

        public double HeaderMinWidth
        {
            get { return (double)GetValue(HeaderMinWidthProperty); }
            set { SetValue(HeaderMinWidthProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(
           "HeaderBackground",
           typeof(Brush),
           typeof(StremorPivot),
           null);

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }
        #endregion
    }
    /*
    public class StremorPivotHeadersControl : Microsoft.Phone.Controls.Primitives.PivotHeadersControl
    {
        public static readonly DependencyProperty HeaderItemMinWidthProperty = DependencyProperty.Register(
            "HeaderItemMinWidth",
            typeof(double),
            typeof(StremorPivotHeadersControl),
            null);

        public double HeaderItemMinWidth
        {
            get { return (double)GetValue(HeaderItemMinWidthProperty); }
            set { SetValue(HeaderItemMinWidthProperty, value); }
        }
    }
    */
}
