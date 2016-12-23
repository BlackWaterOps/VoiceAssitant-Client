using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using PlexiVoice.ViewModels;

using PlexiSDK;
namespace PlexiVoice.Views
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

        private async void ResetButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            SystemTray.ProgressIndicator.Text = "sending";
            SystemTray.ProgressIndicator.IsVisible = true;

            string inputValue = ResetPasswordTextBox.Text;

            IPlexiService plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            Dictionary<string, object> response = await plexiService.ForgotPassword(inputValue);

            SystemTray.ProgressIndicator.IsVisible = false;

            MessageBox.Show("An email with the password reset instructions has been sent to your email address.", "Password Reset", MessageBoxButton.OK);
        }
    }
}