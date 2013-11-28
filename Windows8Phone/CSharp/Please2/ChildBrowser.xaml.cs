using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string isOptional;

            NavigationContext.QueryString.TryGetValue("url", out url);
            NavigationContext.QueryString.TryGetValue("isOptional", out isOptional);

            if (url != String.Empty)
            {
                string headers = BuildHeaders();

                WebBrowser.Navigate(new Uri(url), null, headers);
            }

            if (isOptional == Convert.ToString(true))
            {
                ApplicationBar.IsVisible = true;
            }
            else
            {
                ApplicationBar.IsVisible = false;
            }
        }

        protected void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // check uri after navigation. if success, reverse off of backstack
            Debug.WriteLine(e.Uri.OriginalString);

            if (e.Uri.OriginalString == String.Format("{0}/success", resx.GetString("Authorization")))
            {
                Util.AccountHelper.Default.AddAccount();
            }
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            Util.AccountHelper.Default.CheckAccounts();
        }

        private string BuildHeaders()
        {
            resx = new ResourceManager("Plexi.Resources.PlexiResources", Assembly.Load("Plexi"));

            // Auth Token info
            string tokenHeader = resx.GetString("AuthTokenHeader");

            //string authToken = plexiService.GetAuthToken();
            string authToken = "CF08o2kLQ2qbCVguyLgsTB71p4J2FGt2A79cKVWtW1eiiMxK5zkorrDw6GAyz4zo|1385589452|c23807e8adee2d5c22501e7d795992db54b4d392585f0fe7e4c7bf35bed9610a";

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