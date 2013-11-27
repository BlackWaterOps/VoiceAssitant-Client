using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2
{
    public partial class ChildBrowser : PhoneApplicationPage
    {
        public ChildBrowser()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url = "";
            NavigationContext.QueryString.TryGetValue("url", out url);

            WebBrowser.Navigate(new Uri(url));
        }

        protected void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // check uri after navigation. if success, reverse off of backstack
        }
    }
}