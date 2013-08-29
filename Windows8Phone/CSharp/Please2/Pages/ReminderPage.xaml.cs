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

namespace Please2.Pages
{
    public partial class ReminderPage : PhoneApplicationPage
    {
        public ReminderPage()
        {
            InitializeComponent();
        }

        protected void SaveReminder_Click(object sender, EventArgs e)
        {
            DateTime date = (DateTime)ReminderDate.Value;
            DateTime time = (DateTime)ReminderTime.Value;
            DateTime beginTime = date + time.TimeOfDay;

            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("You can not set reminders in the past");
                return;
            }

            Debug.WriteLine(beginTime.ToString());

            var reminder = new Reminder(System.Guid.NewGuid().ToString());

            reminder.BeginTime = beginTime;

            ScheduledActionService.Add(reminder);

            // need to navigate to a page to show confirmation of the saved reminder 
        }
    }
}