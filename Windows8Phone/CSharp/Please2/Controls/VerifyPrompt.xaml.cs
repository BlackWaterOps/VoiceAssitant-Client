using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Controls
{
    public class VerifyEventArgs : EventArgs
    {
        public string email;

        public string code;

        public VerifyEventArgs(string email, string code)
        {
            this.email = email;

            this.code = code;
        }
    }

    public partial class VerifyPrompt : UserControl
    {
        public event EventHandler<VerifyEventArgs> Closed;

        private bool isVisible;

        private const string emailPlaceholder = "email address";

        private const string codePlaceholder = "verify code";

        private const string message = "Please enter your email address and verification code that was sent to you.";

        private string email = null;

        private string code = null;

        private PhoneApplicationFrame rootFrame = null;

        public VerifyPrompt()
        {
            InitializeComponent();

            DataContext = this;

            HidePrompt.Completed += HideBoxAnimationCompleted;
            Loaded += VerifyPrompt_Loaded;
        }

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", 
            typeof(string), 
            typeof(VerifyPrompt), 
            new PropertyMetadata(String.Empty));

        private void HideBoxAnimationCompleted(object sender, EventArgs e)
        {
            HidePrompt.Stop();
            Visibility = Visibility.Collapsed;

            Dispatcher.BeginInvoke(
                () =>
                {
                    if (Closed != null)
                    {
                        Closed(this, new VerifyEventArgs(email, code));
                    }
                }
            );
        }

        private void VerifyPrompt_Loaded(object sender, RoutedEventArgs e)
        {
            AttachBackKeyPressed();

            VerifyEmail.Text = emailPlaceholder;
            VerifyCode.Text = codePlaceholder;

            if (Message == String.Empty)
            {
                Message = message;
            }
            
            Show();
        }

        private void AttachBackKeyPressed()
        {
            if (rootFrame == null)
            {
                rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                rootFrame.BackKeyPress += VerifyPrompt_BackKeyPress;
            }
        }

        private void VerifyPrompt_BackKeyPress(object sender, CancelEventArgs e)
        {
            // override application frame back key press logic here
        }

        public void Show()
        {
            if (isVisible)
            {
                return;
            }

            isVisible = true;
            Visibility = Visibility.Visible;
            HidePrompt.Stop();
            ShowPrompt.Begin();
        }

        public void Hide()
        {
            if (!isVisible)
            {
                return;
            }

            isVisible = false;
            ShowPrompt.Stop();
            HidePrompt.Begin();
        }

        private void CancelTap(object sender, RoutedEventArgs e)
        {
            // exit app
            //Application.Current.Terminate();
            Hide();
        }

        private void OKTap(object sender, RoutedEventArgs e)
        {
            VerifyCredentials();
        }

        private string GetPlaceholder(string name)
        {
            string placeholder = null;

            switch (name)
            {
                case "VerifyEmail":
                    placeholder = emailPlaceholder;
                    break;

                case "VerifyCode":
                    placeholder = codePlaceholder;
                    break;
            }

            return placeholder;
        }


        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox field = sender as TextBox;

            string placeholder = GetPlaceholder(field.Name);

            if (field.Text == placeholder)
            {
                field.Text = String.Empty;
                field.Foreground = Application.Current.Resources["PhoneTextBoxForegroundBrush"] as SolidColorBrush;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox field = sender as TextBox;

            string placeholder = GetPlaceholder(field.Name);

            if (field.Text == String.Empty)
            {
                field.Text = placeholder;
                field.Foreground = Application.Current.Resources["PhoneTextBoxReadOnlyBrush"] as SolidColorBrush;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                VerifyCredentials();
            }
        }

        private void VerifyCredentials()
        {
            Debug.WriteLine("verify creds");

            string email = VerifyEmail.Text;
            string code = VerifyCode.Text;

            if (email.Equals(String.Empty) || email.Equals(emailPlaceholder))
            {
                // add error style to VerifyEmail
                VerifyEmail.Style = Resources["ErrorStyle"] as Style;

                Message = "please enter your email address";
                return;
            }

            if (code.Equals(String.Empty) || code.Equals(codePlaceholder))
            {
                // add error style to VerifyCode
                VerifyCode.Style = Resources["ErrorStyle"] as Style;

                Message = "please enter your verification code";
                return;
            }

            Byte[] bytes = Encoding.UTF8.GetBytes(email);

            string base64 = Convert.ToBase64String(bytes);

            if (base64.Substring(0, 10) == code)
            {
                this.email = email;

                this.code = code;

                Hide();
            }
            else
            {
                // display error message
                Message = "Your email and/or verification code did not match. Please try again";
            }
        }
    }
}