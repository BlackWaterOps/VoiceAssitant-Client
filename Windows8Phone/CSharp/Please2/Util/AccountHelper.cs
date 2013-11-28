using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

using Microsoft.Phone.Info;

using Please2.ViewModels;

using Plexi;
using Plexi.Resources;

namespace Please2.Util
{
    class AccountHelper
    {
        private bool isInitial = true;

        private Uri originalUri;

        private string currentAccount;

        private List<Tuple<string, string, bool>> accounts;

        public static readonly AccountHelper Default = new AccountHelper();

        private ResourceManager resx;
        
        private IsolatedStorageSettings settings;

        private IPlexiService plexiService;

        private AccountHelper()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;

            plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            resx = new ResourceManager("Plexi.Resources.PlexiResources", Assembly.Load("Plexi"));

            BuildAccounts();
        }

        public void Launching()
        {
            // check if we have an auth token.
            // if not redirect to registration view, else check account creation flags (ie google, fitbit, etc)
            /*
            if (!settings.Contains(resx.GetString("SettingsAuthKey")))
            {
                Navigate(ViewModelLocator.RegistrationUri); 
                return;
            }
            */

            CheckAccounts();
        }

        public void CheckAccounts()
        {
            string endpoint = resx.GetString("Authorization");

            if (this.accounts.Count > 0)
            {
                Tuple<string, string, bool> account = this.accounts.ElementAt(0);

                this.accounts.RemoveAt(0);

                string accountName = account.Item1;
                string settingsKey = account.Item2;
                bool isOptional = account.Item3;

                if ((!settings.Contains(settingsKey) || (bool)settings[settingsKey] == false) && accountName != this.currentAccount)
                {
                    this.currentAccount = accountName;
                    string auth = String.Format("{0}/{1}", endpoint, accountName);

                    Debug.WriteLine("go to auth view");

                    Navigate(auth, isOptional);
                    return;
                }
            }

            if (originalUri != null)
            {
                Debug.WriteLine("go back to original view");
                Navigate(originalUri);
            }
        }

        public void AddAccount()
        {
            foreach (Tuple<string, string, bool> account in this.accounts)
            {
                string accountName = account.Item1;
                string settingsKey = account.Item2;

                if (this.currentAccount == accountName)
                {
                    settings[settingsKey] = true;
                }
            }
        }

        private void Navigate(string authEndpoint, bool isOptional)
        {
            string url = String.Format(ViewModelLocator.ChildBrowserUri, authEndpoint, isOptional);

            Uri uri = new Uri(url, UriKind.Relative);

            Navigate(uri);
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
                        /*
                        string[] uris = new string[] { ViewModelLocator.RegistrationUri.OriginalString, String.Format(ViewModelLocator.ChildBrowserUri, "", "")};

                        bool isDone = false;

                        foreach (var u in uris)
                        {
                            if (e.Uri.OriginalString.Contains(u))
                            {
                                isDone = true;
                                break;
                            }
                        }

                        if (isDone == true)
                        {
                            return;
                        }
                        */

                        originalUri = e.Uri;

                        Debug.WriteLine(e.Uri.OriginalString);
                        Debug.WriteLine(uri.OriginalString);

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

        private void BuildAccounts()
        {
            this.accounts = new List<Tuple<string, string, bool>>();

            this.accounts.Add(new Tuple<string, string, bool>("google", "GoogleAccount", true));
            this.accounts.Add(new Tuple<string, string, bool>("facebook", "FacebookAccount", true));
            this.accounts.Add(new Tuple<string, string, bool>("fitbit", "FitBitAccount", true));
        }
    }
}
