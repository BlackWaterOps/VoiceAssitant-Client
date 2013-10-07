using System;
using System.Collections;
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
    public partial class SingleResult : ViewBase
    {
        private SingleViewModel vm;

        public SingleResult()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string template;

            NavigationContext.QueryString.TryGetValue("template", out template);

            vm = GetViewModelInstance<SingleViewModel>();

            Debug.WriteLine("single page");
            if (template != null)
            {
                Debug.WriteLine("load default template: " + template);
                vm.LoadDefaultTemplate(template);
            }

            DataContext = vm;
        }

        private void RunTest(string name)
        {
            vm.RunTest(name, this.Resources[name] as DataTemplate);
        }
    }

    /*
    public partial class ResourceDictionarySingle : ResourceDictionary
    {
        public ResourceDictionarySingle()
        {
            InitializeComponent();
        }
        
        #region Reminders
        protected void ReminderButton_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/ReminderPage.xaml", UriKind.Relative));
        }

        protected void ReminderItem_Tapped(object sender, EventArgs e)
        {
            var reminder = (sender as FrameworkElement).DataContext as Reminder;

            //NavigationService.Navigate(new Uri("/Pages/ReminderPage.xaml?name=" + reminder.Name, UriKind.Relative));
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
            //NavigationService.Navigate(new Uri("/Pages/AlarmPage.xaml", UriKind.Relative));
        }

        protected void AlarmItem_Tapped(object sender, EventArgs e)
        {
            var alarm = (sender as FrameworkElement).DataContext as Please2.Models.Alarm;

            //NavigationService.Navigate(new Uri("/Pages/AlarmPage.xaml?id=" + alarm.ID, UriKind.Relative));
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
    */
}