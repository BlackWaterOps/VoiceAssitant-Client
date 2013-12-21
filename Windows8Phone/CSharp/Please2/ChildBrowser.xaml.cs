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

using Plexi;

namespace Please2
{
    public partial class ChildBrowser : PhoneApplicationPage
    {
        private IPlexiService plexiService;

        private ResourceManager resx;

        public ChildBrowser()
        {
            InitializeComponent();

            plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url = String.Empty;
            string isOptional = null;

            NavigationContext.QueryString.TryGetValue("url", out url);
            NavigationContext.QueryString.TryGetValue("isOptional", out isOptional);

            if (url != String.Empty)
            {
                string headers = BuildHeaders();

                WebBrowser.Navigate(new Uri(url), null, headers);
            }

            try
            {
                ApplicationBarIconButton skip = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

                skip.IsEnabled = (isOptional == Convert.ToString(true)) ? true : false;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
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
                return;

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
            resx = new ResourceManager("Plexi.Resources.PlexiResources", Assembly.Load("Plexi"));

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