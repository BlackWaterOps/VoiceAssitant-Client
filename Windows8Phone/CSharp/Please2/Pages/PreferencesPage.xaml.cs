using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please2.ViewModels;

namespace Please2.Pages
{
    public partial class PreferencesPage : PhoneApplicationPage
    {
        private PreferencesViewModel preferences;
        
        public PreferencesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            preferences = new PreferencesViewModel();

            DataContext = preferences;

            HavePreferences(); 
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            // slide up transition AddPreferencePage
            NavigationService.Navigate(new Uri("/AddPreferencePage.xaml", UriKind.Relative));
        }

        // edit in place when the list item is tapped
        protected void PreferenceItem_Tapped(object sender, EventArgs e)
        {
            
        }

        //TODO: slide to delete
        protected void PreferenceItem_Delete()
        {
            HavePreferences();
        }

        private void HavePreferences()
        {
            var prefs = preferences.PreferenceList;

            if (prefs.Count() > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}