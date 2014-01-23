using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.IO.IsolatedStorage;

using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;

using Please2.ViewModels;
using Please2.Util;

using PlexiSDK;
namespace Please2
{
    public partial class ChildBrowser : PhoneApplicationPage
    {
        private IPlexiService plexiService;

        private ResourceManager resx;

        private string url;

        private string provider;

        public ChildBrowser()
        {
            InitializeComponent();

            plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationContext.QueryString.TryGetValue("url", out url);
            NavigationContext.QueryString.TryGetValue("provider", out provider);

            if (url != String.Empty)
            {
                string headers = BuildHeaders();

                Debug.WriteLine(headers);

                WebBrowser.Navigate(new Uri(url, UriKind.Absolute), null, headers);
            }
        }

        protected void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // check uri after navigation. if success, reverse off of backstack
            Debug.WriteLine(e.Uri.OriginalString);

            string successEndpoint = String.Format("{0}/success", resx.GetString("Authorization"));

            if (e.Uri.OriginalString.Contains(successEndpoint))
            {
                Debug.WriteLine(String.Format("authorization success: {0}", plexiService.State));

                AccountType account = (AccountType)Enum.Parse(typeof(AccountType), provider);

                SettingsViewModel settings = ViewModelLocator.GetServiceInstance<SettingsViewModel>();

                settings.UpdateAccount(account, AccountStatus.Connected);

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }

                //if (plexiService.State)
                // need to check plexi and retrieve the current state

                //Util.AccountHelper.Default.AddAccount();
                //Util.AccountHelper.Default.CheckAccounts();
            }
        }

        /*
        protected void Reset_Click(object sender, EventArgs e)
        {
            Util.AccountHelper.Default.ResetAccounts();

            plexiService.LogoutUser();

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(ViewModelLocator.RegistrationUri);
            }
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            Util.AccountHelper.Default.CheckAccounts();
        }
        */

        private string BuildHeaders()
        {
            resx = new ResourceManager("PlexiSDK.Resources.PlexiResources", Assembly.Load("PlexiSDK"));

            // Auth Token info
            string tokenHeader = resx.GetString("AuthTokenHeader");

            string authToken = plexiService.GetAuthToken();

            string deviceHeader = resx.GetString("AuthDeviceHeader");

            // Device info
            byte[] duidAsBytes = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

            string duid = Convert.ToBase64String(duidAsBytes);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add(tokenHeader, authToken);
            headers.Add(deviceHeader, duid);
            //headers.Add(deviceHeader, "cRljODI+F0i6w8l72x9Kc9Ez6V8=");

            string headerString = String.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> header in headers)
            {
                sb.AppendLine(String.Format("{0}: {1}", header.Key, header.Value));
            }

            return sb.ToString();
        }
    }
}