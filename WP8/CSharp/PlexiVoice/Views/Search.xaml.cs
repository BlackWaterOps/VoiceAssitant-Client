using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PlexiVoice.Views
{
    public partial class Search : ViewBase
    {
        public Search()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ProgressIndicator = new ProgressIndicator();

            SystemTray.ProgressIndicator.IsIndeterminate = true;

            SystemTray.ProgressIndicator.IsVisible = true;
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (SystemTray.ProgressIndicator != null)
            {
                SystemTray.ProgressIndicator.IsVisible = false;
            }
        }
    }
}