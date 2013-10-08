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

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Alarm : PhoneApplicationPage
    {
        Please2.Models.Alarm currentAlarm = null;

        NotificationsViewModel vm;

        public Alarm()
        {
            InitializeComponent();

            vm = (NotificationsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string alarmID;

            if (NavigationContext.QueryString.TryGetValue("id", out alarmID))
            {
                DeleteSavePanel.Visibility = Visibility.Visible;
                SavePanel.Visibility = Visibility.Collapsed;

                currentAlarm = vm.GetAlarm(alarmID);
            }
        }

        protected void SaveAlarmButton_Click(object sender, EventArgs e)
        {
            DateTime alarmTime = (DateTime)AlarmTime.Value;

            var daysOfWeek = new List<DayOfWeek>();

            // add selected days to a list which will be stored in the DB
            foreach (var day in AlarmRecurringDays.SelectedItems)
            {
                daysOfWeek.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
            }

            vm.SaveAlarm(alarmTime, daysOfWeek);
        }

        protected void UpdateAlarmButton_Click(object sender, EventArgs e)
        {
            if (currentAlarm != null)
            {
                RecurrenceInterval newInterval;

                var daysOfWeek = new List<DayOfWeek>();
                
                var currentInterval = currentAlarm.Interval;

                var alarmTime = (DateTime)AlarmTime.Value;

                // populate new list of DayOfWeek
                foreach (var day in AlarmRecurringDays.SelectedItems)
                {
                    daysOfWeek.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
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

                vm.UpdateAlarm(currentAlarm, alarmTime, currentInterval, newInterval, daysOfWeek);
            }
        }

        protected void DeleteAlarmButton_Click(object sender, EventArgs e)
        {

            if (currentAlarm != null)
                vm.DeleteAlarm(currentAlarm);
        }
    }
}