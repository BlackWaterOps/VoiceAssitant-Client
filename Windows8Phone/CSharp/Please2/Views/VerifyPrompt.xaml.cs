using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Views
{
    public partial class VerifyPrompt : UserControl
    {
        public event EventHandler Closed;

        private bool isVisible;

        public VerifyPrompt()
        {
            InitializeComponent();

            HideBoxAnimation.Completed += HideBoxAnimationCompleted;

            var deviceHeight = App.Current.Host.Content.ActualHeight;
            var deviceWidth = App.Current.Host.Content.ActualWidth;

            Debug.WriteLine(deviceHeight + "::" + deviceWidth);
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
            HideBoxAnimation.Stop();
            Visibility = Visibility.Collapsed;

            Dispatcher.BeginInvoke(
                () =>
                {
                    if (Closed != null)
                    {
                        Closed(this, EventArgs.Empty);
                    }
                }
            );
        }

        public void Show()
        {
            if (isVisible)
            {
                return;
            }

            isVisible = true;
            Visibility = Visibility.Visible;
            HideBoxAnimation.Stop();
            ShowBoxAnimation.Begin();
        }

        public void Hide()
        {
            if (!isVisible)
            {
                return;
            }

            isVisible = false;
            ShowBoxAnimation.Stop();
            HideBoxAnimation.Begin();
        }

        private void CancelTap(object sender, GestureEventArgs e)
        {
            // exit app
            Application.Current.Terminate();
            e.Handled = true;
        }

        private void OKTap(object sender, GestureEventArgs e)
        {
            VerifyCredentials();
            e.Handled = true;
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                VerifyCredentials();
            }
        }

        private void VerifyCredentials()
        {
            string email = VerifyEmail.Text;
            string code = VerifyCode.Text;

            if (email.Equals(String.Empty))
            {
                // add error style to VerifyEmail
                VerifyEmail.Style = Resources["ErrorStyle"] as Style;
                return;
            }

            if (code.Equals(String.Empty))
            {
                // add error style to VerifyCode
                VerifyCode.Style = Resources["ErrorStyle"] as Style;
                return;
            }

            Byte[] bytes = Encoding.UTF8.GetBytes(email);

            string base64 = Convert.ToBase64String(bytes);

            

            if (base64.Substring(0, 10) == code)
            {
                // all is good so hide prompt
                Hide();
            }
            else
            {
                // display error message
                
            }

            // clear both fields
            //VerifyEmail.Text = String.Empty;
            //VerifyCode.Text = String.Empty;
        }

    }
}