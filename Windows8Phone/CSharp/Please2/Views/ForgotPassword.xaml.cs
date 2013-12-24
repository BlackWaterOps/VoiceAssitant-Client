using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Views
{
    public partial class ForgotPassword : PhoneApplicationPage
    {
        public string Scheme { get { return "Settings"; } } 
        
        public ForgotPassword()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void ResetPasswordTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ApplicationBarIconButton button = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

            if (ResetPasswordTextBox.Text != String.Empty)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            SystemTray.ProgressIndicator.Text = "sending email";
            SystemTray.ProgressIndicator.IsVisible = true;

            string inputValue = ResetPasswordTextBox.Text;

            // send off to server to look up and send email
            // need error handling here if server comes back with no account found.

            // success message box w/ system tray progress indicator
            // an email with the password reset instructions has been sent to your email address
        }
    }
}