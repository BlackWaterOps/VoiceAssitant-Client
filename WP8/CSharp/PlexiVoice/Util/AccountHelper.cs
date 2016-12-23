using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Info;
using Microsoft.Phone.Controls;

using LinqToVisualTree;

using PlexiVoice.Controls;
using PlexiVoice.ViewModels;

using PlexiSDK;
using PlexiSDK.Resources;
namespace PlexiVoice.Util
{
    class AccountHelper
    {
        private bool isInitial = true;

        private Uri originalUri;

        public static readonly AccountHelper Default = new AccountHelper();
        
        private IPlexiService plexiService;

        private AccountHelper()
        {
            plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
        }

        public void Launching()
        {
            //AddVerifyPrompt();

            // check if we have an auth token.
            // if not redirect to registration view, else check account creation flags (ie google, fitbit, etc) 

            try
            {
                string token = plexiService.GetAuthToken();
            }
            catch (KeyNotFoundException)
            {
                Navigate(ViewModelLocator.RegistrationUri);
            }
        }

        private void Navigate(Uri uri)
        {
            Debug.WriteLine(uri.OriginalString);

            if (this.isInitial == true)
            {
                NavigatingCancelEventHandler handler = null;

                handler = (s, e) =>
                    {
                        App.RootFrame.Navigating -= handler;

                        this.isInitial = false;
                       
                        originalUri = e.Uri;

                        if (e.Uri.OriginalString == uri.OriginalString)
                        {
                            return;
                        }

                        e.Cancel = true;

                        App.RootFrame.Dispatcher.BeginInvoke(delegate
                        {
                            App.RootFrame.Navigate(uri);
                        });
                    };

                App.RootFrame.Navigating += handler;
            }
            else
            {
                App.RootFrame.Navigate(uri);
            }
        }
    }
}
