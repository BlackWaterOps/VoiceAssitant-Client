using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using Please2.Models;
using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Notifications : PhoneApplicationPage
    {
        NotificationsViewModel vm;
        
        public Notifications()
        {
            InitializeComponent();

            vm = (NotificationsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        #region Reminders
        protected void ReminderButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Reminder.xaml", UriKind.Relative));
        }

        protected void ReminderItem_Tapped(object sender, EventArgs e)
        {
            var reminder = (sender as FrameworkElement).DataContext as Reminder;

            NavigationService.Navigate(new Uri("/Views/Reminder.xaml?name=" + reminder.Name, UriKind.Relative));
        }

        protected void ReminderToggle_Click(object sender, EventArgs e)
        {
            var reminder = (sender as FrameworkElement).DataContext as Reminder;
            
            //TODO: need to find a way to "disable" a reminder

        }
        #endregion

        #region Alarms
        protected void AlarmButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Alarm.xaml", UriKind.Relative));
        }

        protected void AlarmItem_Tapped(object sender, EventArgs e)
        {
            var alarm = (sender as FrameworkElement).DataContext as Please2.Models.Alarm;

            NavigationService.Navigate(new Uri("/Views/Alarm.xaml?id=" + alarm.ID, UriKind.Relative));
        }

        protected void AlarmToggle_Click(object sender, EventArgs e)
        {
            var alarm = (sender as FrameworkElement).DataContext as Please2.Models.Alarm;

            alarm.IsEnabled = false;
            // might need to update the DB. ie. db.SubmitChanges();

            foreach (var name in alarm.Names)
            {
                var action = ScheduledActionService.Find(name);

                if (action != null)
                {
                    var al = (Microsoft.Phone.Scheduler.Alarm)action;

                    // TODO: need to find a way to "disable" an alarm
                }
            }
        }
        #endregion
    }
}