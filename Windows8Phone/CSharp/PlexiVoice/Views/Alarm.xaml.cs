using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using PlexiVoice.Models;
using PlexiVoice.ViewModels;

namespace PlexiVoice.Views
{
    public partial class Alarm : PhoneApplicationPage
    {
        AlarmDetailsViewModel vm;

        private bool isSet = false;

        private int? currentAlarmID;

        private static Uri redirectUri = ViewModelLocator.MainPageUri;

        public Alarm()
        {
            InitializeComponent();

            vm = (AlarmDetailsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string alarmID;

            Debug.WriteLine("navigated to alarm page");

            if (NavigationContext.QueryString.TryGetValue("id", out alarmID))
            {
                AddDeleteSaveButtons();

                this.currentAlarmID = int.Parse(alarmID);

                vm.SetCurrentAlarm(alarmID);
            }
            else
            {
                AddSaveButton();
            }
        }

        private void AddDeleteSaveButtons()
        {
            if (!isSet)
            {
                ApplicationBarIconButton deleteButton = new ApplicationBarIconButton();

                deleteButton.Text = "delete";
                deleteButton.IconUri = new Uri("/Assets/delete.png", UriKind.Relative);
                deleteButton.Click += DeleteAlarmButton_Click;

                ApplicationBarIconButton updateButton = new ApplicationBarIconButton();

                updateButton.Text = "update";
                updateButton.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
                updateButton.Click += UpdateAlarmButton_Click;

                ApplicationBar.Buttons.Add(updateButton);
                ApplicationBar.Buttons.Add(deleteButton);

                isSet = true;
            }
        }

        private void AddSaveButton()
        {
            if (!isSet)
            {
                ApplicationBarIconButton saveButton = new ApplicationBarIconButton();

                saveButton.Text = "save";
                saveButton.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
                saveButton.Click += SaveAlarmButton_Click;

                ApplicationBar.Buttons.Add(saveButton);

                isSet = true;
            }
        }

        protected void SaveAlarmButton_Click(object sender, EventArgs e)
        {
            string alarmName = AlarmLabel.Text;

            DateTime alarmTime = (DateTime)AlarmTime.Value;
            
            // Save the alarm through the template's viewmodel so the template will be populated
            AlarmViewModel template = new AlarmViewModel();

            template.SaveAlarm(alarmName, alarmTime, (List<string>)AlarmRecurringDays.SelectedItems);

            MainViewModel vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            vm.Items.Add(template);

            NavigationService.GoBack();
        }

        protected void UpdateAlarmButton_Click(object sender, EventArgs e)
        {
            if (currentAlarmID.HasValue)
            {
                RecurrenceInterval newInterval;

                List<DayOfWeek> newDaysOfWeek = new List<DayOfWeek>();

                string newAlarmName = AlarmLabel.Text;

                DateTime newAlarmTime = (DateTime)AlarmTime.Value;

                // populate new list of DayOfWeek
                foreach (var day in AlarmRecurringDays.SelectedItems)
                {
                    newDaysOfWeek.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
                }

                // set new alarm interval
                if (AlarmRecurringDays.SelectedItems.Count == 7)
                {
                    newInterval = RecurrenceInterval.Daily;
                }
                else if (AlarmRecurringDays.SelectedItems.Count > 0)
                {
                    newInterval = RecurrenceInterval.Weekly;
                }
                else
                {
                    newInterval = RecurrenceInterval.None;
                }

                vm.UpdateAlarm(currentAlarmID.Value, newAlarmName, newAlarmTime, newInterval, newDaysOfWeek);
            }
        }

        protected void DeleteAlarmButton_Click(object sender, EventArgs e)
        {

            if (currentAlarmID.HasValue)
                vm.DeleteAlarm(currentAlarmID.Value);

            NavigationService.GoBack();
        }
    }
}