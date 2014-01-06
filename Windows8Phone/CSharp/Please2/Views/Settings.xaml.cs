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

using Please2.Models;
using Please2.Util;
using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                Debug.WriteLine(err.InnerException.Message);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void Provider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ResourceManager resx = new ResourceManager("Plexi.Resources.PlexiResources", Assembly.Load("Plexi"));

            ProviderModel provider = (sender as FrameworkElement).DataContext as ProviderModel;

            string endpoint = (provider.status == AccountStatus.NotConnected) ? resx.GetString("Authorization") : resx.GetString("Deauthorization");

            string auth = String.Format("{0}/{1}", endpoint, provider.name.ToString().ToLower());

            string url = String.Format(ViewModelLocator.ChildBrowserUri, auth, provider.name);

            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }
    }
}