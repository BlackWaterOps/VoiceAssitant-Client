using System;
using System.Diagnostics;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;

namespace PlexiVoice.Util
{
    public class NavigationService : INavigationService
    {
        PhoneApplicationFrame mainFrame;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationFailedEventHandler NavigationFailed;

        public event NavigatedEventHandler Navigated;

        public void NavigateTo(Uri uri)
        {
            if (EnsureMainFrame())
            {
                mainFrame.Navigate(uri);
            }
        }

        public void GoBack()
        {
            if (EnsureMainFrame() && mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }

        protected bool EnsureMainFrame()
        {
            if (mainFrame != null)
            {
                return true;
            }

            mainFrame = App.Current.RootVisual as PhoneApplicationFrame;

            if (mainFrame != null)
            {
                mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                mainFrame.NavigationFailed += (s, e) =>
                {
                    if (NavigationFailed != null)
                    {
                        NavigationFailed(s, e);
                    }
                };

                mainFrame.Navigated += (s, e) =>
                {
                    if (Navigated != null)
                    {
                        Navigated(s, e);
                    }
                };

                return true;
            }

            return false;
        }
    }
}
