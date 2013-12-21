using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Controls
{
    public partial class ValidationTextBox : UserControl
    {
        public ValidationTextBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        #region Error Properties
        public static readonly DependencyProperty ErrorBackgroundProperty = DependencyProperty.Register(
            "ErrorBackground",
            typeof(Brush),
            typeof(ValidationTextBox), 
            null);
        
        public Brush ErrorBackground
        {
            get { return (Brush)GetValue(ErrorBackgroundProperty); }
            set { SetValue(ErrorBackgroundProperty, value); }
        }
 
        public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register(
            "ErrorText",
            typeof(string),
            typeof(ValidationTextBox),
            null);
        
        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }

        public static readonly DependencyProperty ErrorTextWrappingProperty = DependencyProperty.Register(
            "ErrorTextWrapping",
            typeof(TextWrapping),
            typeof(ValidationTextBox),
            null);

        public TextWrapping ErrorTextWrapping
        {
            get { return (TextWrapping)GetValue(ErrorTextWrappingProperty); }
            set { SetValue(ErrorTextWrappingProperty, value); }
        }

        public static readonly DependencyProperty ErrorPaddingProperty = DependencyProperty.Register(
            "ErrorPadding",
            typeof(Thickness),
            typeof(ValidationTextBox),
            null);

        public Thickness ErrorPadding
        {
            get { return (Thickness)GetValue(ErrorPaddingProperty); }
            set { SetValue(ErrorPaddingProperty, value); }
        }
        #endregion

        #region General Properties
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(ValidationTextBox),
            null);

        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(Brush),
            typeof(ValidationTextBox),
            null);

        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding",
            typeof(Thickness),
            typeof(ValidationTextBox),
            null);

        public new Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public new static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(ValidationTextBox),
            null);

        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness",
            typeof(Thickness),
            typeof(ValidationTextBox),
            null);

        public new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
 
        public static readonly DependencyProperty IsPasswordProperty = DependencyProperty.Register(
            "IsPassword",
            typeof(bool),
            typeof(ValidationTextBox),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsPasswordPropertyChanged)));

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        private static void OnIsPasswordPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ValidationTextBox val = (ValidationTextBox)dependencyObject;

            bool oldValue = (bool)eventArgs.OldValue;
            bool newValue = (bool)eventArgs.NewValue;

            if (oldValue != newValue)
            {
                val.UpdateInputMethod(newValue);  
            }
        }

        private void UpdateInputMethod(bool isPasswordVisible)
        {
            /*
            if (isPasswordVisible)
            {
                ValTextBox.Visibility = Visibility.Collapsed;
                ValPasswordBox.Visibility = Visibility.Visible;
            }
            else
            {
                ValPasswordBox.Visibility = Visibility.Collapsed;
                ValTextBox.Visibility = Visibility.Visible;
            }
            */
        }

        public static readonly DependencyProperty ValidationPatternProperty = DependencyProperty.Register(
            "ValidationPattern",
            typeof(string),
            typeof(ValidationTextBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnValidationPatternPropertyChanged)));

        private static void OnValidationPatternPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            Debug.WriteLine(eventArgs.NewValue);
        }

        public string ValidationPattern
        {
            get { return (string)GetValue(ValidationPatternProperty); }
            set { SetValue(ValidationPatternProperty, value); }
        }
        #endregion

        #region Event Handlers
        private void Border_KeyUp(object sender, EventArgs e)
        {
            // check validation pattern
            Debug.WriteLine("border keyup");
        }

        private void Border_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("border got focus");
        }

        private void Border_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("border lost focus");
        }
        #endregion
    }
}
