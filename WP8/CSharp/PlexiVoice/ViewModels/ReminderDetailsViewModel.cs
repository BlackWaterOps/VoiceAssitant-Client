using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Scheduler;

namespace PlexiVoice.ViewModels
{
    public class ReminderDetailsViewModel : ReminderBase
    {
        private DateTime? reminderDate;
        public DateTime? ReminderDate
        {
            get { return (!reminderDate.HasValue) ? DateTime.Now : reminderDate.Value; }
            set
            {
                reminderDate = value;
                RaisePropertyChanged("ReminderDate");
            }
        }

        private DateTime? reminderTime;
        public DateTime? ReminderTime
        {
            get { return (!reminderTime.HasValue) ? DateTime.Now : reminderTime.Value; }
            set
            {
                reminderTime = value;
                RaisePropertyChanged("ReminderTime");
            }
        }

        private string reminderSubject;
        public string ReminderSubject
        {
            get { return reminderSubject; }
            set
            {
                reminderSubject = value;
                RaisePropertyChanged("ReminderSubject");
            }
        }

        public ReminderDetailsViewModel()
        {
        }

        public void SetCurrentReminder(string guid)
        {
            Reminder reminder = base.GetReminder(guid);

            ReminderDate = reminder.BeginTime;

            ReminderTime = reminder.BeginTime;

            ReminderSubject = reminder.Name;
        }
    }
}
