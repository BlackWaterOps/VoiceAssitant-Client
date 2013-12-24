using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Controls
{
    public enum ValidationType { None, Username, AlphaNumeric, Numeric, PhoneNumber, Email, Url };

    public partial class ValidationTextBox : UserControl
    {
        private const double unfocusedOpacity = 0.5;

        private const double focusedOpacity = 1;

        private static Dictionary<ValidationType, string> validationRegex = new Dictionary<ValidationType, string>()
        {
            {ValidationType.None, null},
            {ValidationType.Username, @"[a-zA-Z0-9_.-]"},
            {ValidationType.AlphaNumeric, @"[a-zA-Z0-9]"},
            {ValidationType.Numeric, @"[0-9]"},
            {ValidationType.PhoneNumber, @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$"},
            {ValidationType.Email, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"},
            {ValidationType.Url, @"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/"}
        };

        private static Dictionary<ValidationType, string> validationMessage = new Dictionary<ValidationType, string>()
        {
            {ValidationType.Username, @"{0} can only contain letters, numbers, underscores, dashes, and periods"},
            {ValidationType.AlphaNumeric, @"{0} can only contain letters and numbers"},
            {ValidationType.Numeric, @"{0} can only contain numbers"},
            {ValidationType.PhoneNumber, "Please enter a valid phone number"},
            {ValidationType.Email, "Please enter a valid email"},
            {ValidationType.Url, "Please enter a valid url"}
        };

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
        public new static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(
            "Template",
            typeof(ControlTemplate),
            typeof(ValidationTextBox),
            null);

        public new ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(string),
            typeof(ValidationTextBox),
            new PropertyMetadata(String.Empty));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

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

        public new static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(
            "Opacity",
            typeof(double),
            typeof(ValidationTextBox),
            new PropertyMetadata(unfocusedOpacity));

        public new double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            private set { SetValue(OpacityProperty, value); }
        }

        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.Register(
            "CaretBrush",
            typeof(Brush),
            typeof(ValidationTextBox),
            null);

        public Brush CaretBrush
        {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ValidationTextBox),
            new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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

        public static readonly DependencyProperty ValidationRuleProperty = DependencyProperty.Register(
            "ValidationRule",
            typeof(ValidationType),
            typeof(ValidationTextBox),
            new PropertyMetadata(ValidationType.None, new PropertyChangedCallback(OnValidationRuleChanged)));

        public ValidationType ValidationRule
        {
            get { return (ValidationType)GetValue(ValidationRuleProperty); }
            set { SetValue(ValidationRuleProperty, value); }
        }

        private static void OnValidationRuleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ValidationTextBox val = (ValidationTextBox)dependencyObject;

            ValidationType type = (ValidationType)eventArgs.NewValue;

            Debug.WriteLine(String.Format("update input scope for type {0}", type));

            val.UpdateInputScope(type);
        }

        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register(
            "MinLength",
            typeof(int),
            typeof(ValidationTextBox),
            new PropertyMetadata(0));

        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength",
            typeof(int),
            typeof(ValidationTextBox),
            new PropertyMetadata(0));

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(
            "InputScope",
            typeof(InputScope),
            typeof(ValidationTextBox),
            null);

        public InputScope InputScope
        {
            get { return (InputScope)GetValue(InputScopeProperty); }
            private set { SetValue(InputScopeProperty, value); }
        }

        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            private set { isValid = value; }
        }
        #endregion

        #region Event Handlers
        private void Border_KeyUp(object sender, EventArgs e)
        {
            // check validation pattern          
            string message = Validate();

            if (message != null)
            {
                ShowError(message);
            }
            else
            {
                HideError();
            }
        }

        private void Border_GotFocus(object sender, RoutedEventArgs e)
        {
            Opacity = focusedOpacity;

            string message = Validate();

            if (message != null)
            {

                ShowError(message);
            }
        }

        private void Border_LostFocus(object sender, RoutedEventArgs e)
        {
            Opacity = unfocusedOpacity;
            HideError();
        }
        #endregion

        #region helpers
        private void UpdateInputMethod(bool isPasswordVisible)
        {
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
        }

        private void UpdateInputScope(ValidationType type)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();

            switch (type)
            {
                case ValidationType.Email:
                    name.NameValue = InputScopeNameValue.EmailNameOrAddress;
                    break;

                case ValidationType.Numeric:
                    name.NameValue = InputScopeNameValue.Digits;
                    break;

                case ValidationType.PhoneNumber:
                    name.NameValue = InputScopeNameValue.TelephoneNumber;
                    break;

                case ValidationType.Url:
                    name.NameValue = InputScopeNameValue.Url;
                    break;

                default:
                    name.NameValue = InputScopeNameValue.Default;
                    break;
            }

            scope.Names.Add(name);

            InputScope = scope;
        }

        private string Validate()
        {
            string value = (IsPassword) ? ValPasswordBox.Password : ValTextBox.Text;

            IsValid = false;

            if (MinLength != 0 && value.Length < MinLength)
            {
                return String.Format("{0} must have a min length of {1} chars", Label, MinLength).Trim();
            }

            if (MaxLength != 0 && value.Length > MaxLength)
            {
                return String.Format("{0} must have a max length of {1} chars", Label, MaxLength).Trim();
            }

            string pattern = validationRegex[ValidationRule];

            if (pattern != null && !Regex.IsMatch(value, pattern))
            {
                return String.Format(validationMessage[ValidationRule], Label);
            }

            IsValid = true;

            return null;
        }

        private void ShowError(string message)
        {
            ErrorBubbleTextBlock.Text = message;

            ErrorFadeIn.Begin();
        }

        private void HideError()
        {
            ErrorFadeOut.Begin();

            ErrorFadeOut.Completed += (s, e) =>
                {
                    ErrorBubbleTextBlock.Text = String.Empty;
                };
        }
        #endregion
    }
}
