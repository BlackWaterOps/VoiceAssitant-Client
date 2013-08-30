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
        const string facebookEndpoint = "https://graph.facebook.com/oauth/access_token";
        const string redirectUri = "https://www.facebook.com/connect/login_success.html";

        public ChildBrowser()
        {
            InitializeComponent();
        }

        protected void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
           
        }
    }
}