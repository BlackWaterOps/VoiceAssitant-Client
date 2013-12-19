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

        
            /*
            string authToken = "CF08o2kLQ2qbCVguyLgsTB71p4J2FGt2A79cKVWtW1eiiMxK5zkorrDw6GAyz4zo|1385589452|c23807e8adee2d5c22501e7d795992db54b4d392585f0fe7e4c7bf35bed9610a";

            Debug.WriteLine(String.Format("original: {0}", authToken));

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            string testKey = "testencrypt";

            //settings.Remove(testKey);

            if (!settings.Contains(testKey))
            {
                Debug.WriteLine("create encrypt");
                settings.Add(testKey, Plexi.Util.Security.Encrypt(authToken));
                settings.Save();
            }

            var k = (byte[])settings[testKey];

            string decrypt = Plexi.Util.Security.Decrypt(k);

            Debug.WriteLine(String.Format("Decrypt Value: {0}", decrypt));
            */

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
                Debug.WriteLine("authorization success");
                //Util.AccountHelper.Default.AddAccount();
                //Util.AccountHelper.Default.CheckAccounts();
            }
        }

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