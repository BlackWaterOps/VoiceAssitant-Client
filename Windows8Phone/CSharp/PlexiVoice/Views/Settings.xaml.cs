using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using PlexiVoice.Models;
using PlexiVoice.Util;
using PlexiVoice.ViewModels;

namespace PlexiVoice.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        SettingsViewModel vm;

        public Settings()
        {
            InitializeComponent();

            vm = DataContext as SettingsViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.Text = "retrieving accounts";
            SystemTray.ProgressIndicator.IsVisible = true;

            await vm.InitializeSettings();

            SystemTray.ProgressIndicator.IsVisible = false;
        }

        private async void Provider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ResourceManager resx = new ResourceManager("PlexiSDK.Resources.PlexiResources", Assembly.Load("PlexiSDK"));

            ProviderModel provider = (sender as FrameworkElement).DataContext as ProviderModel;

            if (provider.status == AccountStatus.NotConnected)
            {
                string endpoint = resx.GetString("Authorization");

                string auth = String.Format("{0}/{1}", endpoint, provider.name.ToString().ToLower());

                string url = String.Format(ViewModelLocator.ChildBrowserUri, auth, provider.name);

                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
            else
            {
                List<ProviderModel> accounts = await vm.GetAccounts(provider.name);

                if (accounts.Count == 1)
                {
                    vm.RemoveAccount(accounts.First());
                }
                else
                {
                    Debug.WriteLine("show list to deauth");
                    foreach (ProviderModel account in accounts)
                    {
                        vm.RemoveAccount(account);
                    }

                    // show list of accounts of this provider's type to deauth
                }

                // call deauth endpoint
                // no reason to navigate to browser so just call a plexi method
                //vm.RemoveAccount(provider);
            }
        }

        private void Temperature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}