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
    public partial class ReminderPage : PhoneApplicationPage
    {
        Reminder currentReminder;

        NotificationsViewModel viewModel = new NotificationsViewModel();

        public ReminderPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string name;

            if (NavigationContext.QueryString.TryGetValue("name", out name))
            {
                DeleteSavePanel.Visibility = Visibility.Visible;
                SavePanel.Visibility = Visibility.Collapsed;

                currentReminder = viewModel.GetReminder(name);
            }
        }

        protected void SaveReminderButton_Click(object sender, EventArgs e)
        {
            SaveOrUpdateReminder(false);
        }

        protected void DeleteReminderButton_Click(object sender, EventArgs e)
        {
            viewModel.DeleteReminder(currentReminder);
            GoToNotifications();
        }

        protected void UpdateReminderButton_Click(object sender, EventArgs e)
        {
            SaveOrUpdateReminder(true);
        }

        private void SaveOrUpdateReminder(bool update = false)
        {
            DateTime date = (DateTime)ReminderDate.Value;
            DateTime time = (DateTime)ReminderTime.Value;
            DateTime beginTime = date + time.TimeOfDay;

            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("You can not set reminders in the past");
                return;
            }

            string title = ReminderLabel.Text;

            if (update == true)
            {
                viewModel.UpdateReminder(currentReminder.Name, beginTime, title);
            }
            else
            {
                viewModel.CreateReminder(beginTime, title);
            }

            GoToNotifications();
        }

        private void GoToNotifications()
        {
            NavigationService.Navigate(new Uri("/Pages/NotificationsPage.xaml", UriKind.Relative));
        }
    }
}