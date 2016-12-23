using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Scheduler;

namespace PlexiVoice.ViewModels
{
    public class ReminderBase : NotificationsViewModel
    {
        public virtual string CreateReminder(DateTime reminderDate, string title)
        {
            string name = System.Guid.NewGuid().ToString();

            var reminder = new Microsoft.Phone.Scheduler.Reminder(name);

            reminder.RecurrenceType = RecurrenceInterval.None;

            reminder.Title = title;

            reminder.BeginTime = reminderDate;

            ScheduledActionService.Add(reminder);

            return name;
        }

        public Microsoft.Phone.Scheduler.Reminder GetReminder(string name)
        {
            return ScheduledActionService.Find(name) as Microsoft.Phone.Scheduler.Reminder;
        }

        public void UpdateReminder(string name, DateTime reminderDate, string title)
        {
            var action = ScheduledActionService.Find(name);

            if (action != null)
            {
                var oldReminder = (Microsoft.Phone.Scheduler.Reminder)action;

                oldReminder.BeginTime = reminderDate;

                oldReminder.Title = title;

                ScheduledActionService.Replace(oldReminder);
            }
        }

        public void DeleteReminder(Microsoft.Phone.Scheduler.Reminder reminder)
        {
            DeleteNotification(reminder.Name);
        }
    }
}
