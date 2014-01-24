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

            string value;

            if (e.NavigationMode == NavigationMode.Back)
            {
                Debug.WriteLine(App.RootFrame.BackStack.First());
            }

            if (e.NavigationMode != NavigationMode.Back)
            {
                if (NavigationContext.QueryString.TryGetValue("index", out value))
                {
                    int idx = int.Parse(value);

                    NotificationsPivot.SelectedIndex = idx;
                }

                if (NavigationContext.QueryString.TryGetValue("page", out value))
                {
                    switch (value)
                    {
                        case "alarm":
                            NavigationService.Navigate(ViewModelLocator.AlarmPageUri);
                            break;

                        case "reminder":
                            NavigationService.Navigate(ViewModelLocator.ReminderPageUri);
                            break;
                    }
                }
            }
        }

        private void Pivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            int idx = pivot.Items.IndexOf(e.Item);

            switch (idx)
            {
                case 0:
                    AddReminderButton();
                    break;

                case 1:
                    AddAlarmButton();
                    break;
            }
        }

        #region Reminders
        private void AddReminderButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "reminder";
            button.IconUri = new Uri("/Assets/feature.calendar.png", UriKind.Relative);
            button.Click += ReminderButton_Click;

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            ApplicationBar.Buttons.Add(button);
        }

        protected void ReminderButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(ViewModelLocator.ReminderPageUri);
        }

        protected void ReminderItem_Tapped(object sender, EventArgs e)
        {
            var reminder = (sender as FrameworkElement).DataContext as Microsoft.Phone.Scheduler.Reminder;

            vm.SetCurrentReminder(reminder, null);

            NavigationService.Navigate(new Uri(String.Format(ViewModelLocator.ReminderUri, reminder.Name), UriKind.Relative));
        }

        protected void ReminderToggle_Click(object sender, EventArgs e)
        {
            var reminder = (sender as FrameworkElement).DataContext as Microsoft.Phone.Scheduler.Reminder;
            
            //TODO: need to find a way to "disable" a reminder

        }
        #endregion

        #region Alarms
        private void AddAlarmButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "alarm";
            button.IconUri = new Uri("/Assets/feature.alarm.png", UriKind.Relative);
            button.Click += AlarmButton_Click;

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            ApplicationBar.Buttons.Add(button);
        }

        protected void AlarmButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(ViewModelLocator.AlarmPageUri);
        }

        protected void AlarmItem_Tapped(object sender, EventArgs e)
        {
            AlarmItem alarm = (sender as FrameworkElement).DataContext as AlarmItem;

            vm.SetCurrentAlarm(alarm,  null);

            NavigationService.Navigate(new Uri(String.Format(ViewModelLocator.AlarmUri, alarm.ID), UriKind.Relative));
        }

        protected void AlarmToggle_Checked(object sender, RoutedEventArgs e)
        {
            var alarm = (sender as FrameworkElement).DataContext as AlarmItem;

            alarm.IsEnabled = false;
            // might need to update the DB. ie. db.SubmitChanges();

            /*
            foreach (var name in alarm.Names)
            {
                var action = ScheduledActionService.Find(name);

                if (action != null)
                {
                    var al = (Microsoft.Phone.Scheduler.Alarm)action;

                    // TODO: need to find a way to "disable" an alarm
                    // possibly use expiration time property
                }
            }
             */
        }

        protected void AlarmToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("unchecked");
        }
        #endregion
    }
}