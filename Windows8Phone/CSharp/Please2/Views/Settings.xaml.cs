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

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please2.Models;
using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Provider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ResourceManager resx = new ResourceManager("Plexi.Resources.PlexiResources", Assembly.Load("Plexi"));

            string endpoint = resx.GetString("Authorization");

            ProviderModel provider = (sender as FrameworkElement).DataContext as ProviderModel;

            string auth = String.Format("{0}/{1}", endpoint, provider.endpointName);

            string url = String.Format(ViewModelLocator.ChildBrowserUri, auth, false);

            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }
    }
}