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
    public partial class AddPreferencePage : PhoneApplicationPage
    {
        PreferencesViewModel pvm;
        
        public AddPreferencePage()
        {
            InitializeComponent();

            pvm = new PreferencesViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("preferenceId"))
            {
                var preference = pvm.GetPreference(NavigationContext.QueryString["preferenceId"]);

                PreferenceName.Text = preference.Name;

                DateTime datetime = DateTime.Parse(preference.Value);

                PreferenceDate.Value = datetime;
                PreferenceTime.Value = datetime;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // slide down and return to preferences list
            NavigationService.GoBack();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // update DB and navigate back to preferences list
            string name = PreferenceName.Text;
            string value = "";

            DateTime? date = PreferenceDate.Value;

            DateTime? time = PreferenceTime.Value;

            if (date.HasValue)
                value += date.Value.ToString("yyyy-MM-dd");

            if (time.HasValue)
                value += time.Value.ToString("H:mm:ss");

            pvm.AddPreference(name, value);

            NavigationService.GoBack();
        }
    }
}