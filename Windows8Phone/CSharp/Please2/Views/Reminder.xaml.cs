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
    public partial class Reminder : PhoneApplicationPage
    {
        Microsoft.Phone.Scheduler.Reminder currentReminder;

        NotificationsViewModel vm;

        private bool isSet = false;

        public Reminder()
        {
            InitializeComponent();

            vm = (NotificationsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string name;

            if (NavigationContext.QueryString.TryGetValue("name", out name))
            {
                AddDeleteSaveButtons();

                currentReminder = vm.GetReminder(name);
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
                deleteButton.Click += DeleteReminderButton_Click;

                ApplicationBarIconButton updateButton = new ApplicationBarIconButton();

                updateButton.Text = "update";
                updateButton.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
                updateButton.Click += UpdateReminderButton_Click;

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
                saveButton.Click += SaveReminderButton_Click;

                ApplicationBar.Buttons.Add(saveButton);

                isSet = true;
            }
        }

        private void SaveReminderButton_Click(object sender, EventArgs e)
        {
            SaveOrUpdateReminder(false);
        }

        private void DeleteReminderButton_Click(object sender, EventArgs e)
        {
            vm.DeleteReminder(currentReminder);
            
            NavigationService.GoBack();
        }

        private void UpdateReminderButton_Click(object sender, EventArgs e)
        {
            SaveOrUpdateReminder(true);
        }

        private void SaveOrUpdateReminder(bool update = false)
        {
            DateTime date = (DateTime)ReminderDate.Value;
            DateTime time = (DateTime)ReminderTime.Value;

            DateTime beginTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);

            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("You can not set reminders in the past");
                return;
            }

            string title = ReminderLabel.Text;

            if (update == true)
            {
                vm.UpdateReminder(currentReminder.Name, beginTime, title);
            }
            else
            {
                vm.CreateReminder(beginTime, title);
            }

            NavigationService.GoBack();
        }
    }
}