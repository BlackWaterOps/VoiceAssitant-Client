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

using Please2.Controls;
using Please2.ViewModels;

using Plexi;
using Plexi.Resources;

namespace Please2.Util
{
    class AccountHelper
    {
        private bool isInitial = true;

        private Uri originalUri;

        private Tuple<string, string, bool> currentAccount;

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
            //AddVerifyPrompt();

            // check if we have an auth token.
            // if not redirect to registration view, else check account creation flags (ie google, fitbit, etc) 

            string settingsKey = resx.GetString("SettingsAuthKey");

            if (!settings.Contains(settingsKey))
            {
                Navigate(ViewModelLocator.RegistrationUri);
                return;
            }
           
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

                if (!settings.Contains(settingsKey) || (bool)settings[settingsKey] == false)
                {
                    if (this.currentAccount == null || accountName != this.currentAccount.Item1)
                    {
                        this.currentAccount = account;
                        string auth = String.Format("{0}/{1}", endpoint, accountName);

                        Debug.WriteLine("go to auth view");

                        Navigate(auth, isOptional);
                        return;
                    }
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
            if (this.currentAccount != null)
            {
                string settingsKey = this.currentAccount.Item2;

                try
                {
                    Debug.WriteLine(String.Format("set {0} to true", this.currentAccount.Item2));
                    settings[settingsKey] = true;
                }
                catch (KeyNotFoundException)
                {
                    settings.Add(settingsKey, true);
                }
                catch (ArgumentException)
                {
                    settings.Add(settingsKey, true);
                }

                settings.Save();
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

        private void BuildAccounts()
        {
            this.accounts = new List<Tuple<string, string, bool>>();

            this.accounts.Add(new Tuple<string, string, bool>("google", "GoogleAccount", false));
            this.accounts.Add(new Tuple<string, string, bool>("facebook", "FacebookAccount", false));
            this.accounts.Add(new Tuple<string, string, bool>("fitbit", "FitBitAccount", true));
        }

        public void ResetAccounts()
        {
            foreach (var account in this.accounts)
            {
                string settingsKey = account.Item2;

                Debug.WriteLine(String.Format("removing account {0} from settings", settingsKey));

                settings.Remove(settingsKey);
            }
        }

        private void AddVerifyPrompt()
        {
            string betaKey = "StremorBetaTestKey";

            // check database if we already have credentials
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(betaKey))
            {
                // show verify control if no credentials are found
                var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);
                var layoutRoot = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").Single();

                if (layoutRoot != null)
                {
                    VerifyPrompt verifyPrompt = new VerifyPrompt();

                    var deviceHeight = App.Current.Host.Content.ActualHeight;
                    var deviceWidth = App.Current.Host.Content.ActualWidth;

                    var colSpan = layoutRoot.ColumnDefinitions.Count;
                    var rowSpan = layoutRoot.RowDefinitions.Count;

                    verifyPrompt.Height = deviceHeight;
                    verifyPrompt.Width = deviceWidth;

                    if (colSpan > 0)
                    {
                        verifyPrompt.SetValue(Grid.ColumnSpanProperty, colSpan);
                    }

                    if (rowSpan > 0)
                    {
                        verifyPrompt.SetValue(Grid.RowSpanProperty, rowSpan);
                    }

                    verifyPrompt.Closed += (s, e) =>
                    {
                        // save credentials to database
                        Dictionary<string, string> beta = new Dictionary<string, string>();

                        beta.Add("email", e.email);
                        beta.Add("code", e.code);

                        settings.Add(betaKey, beta);

                        // remove prompt from page
                        layoutRoot.Children.Remove(verifyPrompt);
                    };

                    layoutRoot.Children.Add(verifyPrompt);
                }
            }

        }
    }
}
